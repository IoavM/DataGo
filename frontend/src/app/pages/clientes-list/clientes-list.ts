import { Component, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ClienteService } from '../../services/cliente.service';

@Component({
  selector: 'app-clientes-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './clientes-list.html',
  styleUrl: './clientes-list.css'
})
export class ClientesListComponent {
  private clienteService = inject(ClienteService);

  clientes = this.clienteService.clientes;

  terminoBusqueda = signal('');
  filtroEstado = signal<'todos' | 'activos' | 'bloqueados'>('todos');
  filtroTipoDoc = signal('todos');

  clientesFiltrados = computed(() => {
    const q = this.terminoBusqueda().toLowerCase().trim();
    const estado = this.filtroEstado();
    const tipoDoc = this.filtroTipoDoc();

    return this.clientes().filter(c => {
      const coincideTexto = !q || 
        c.nombreCompleto.toLowerCase().includes(q) ||
        c.nombreNegocio.toLowerCase().includes(q) ||
        c.codigo.toLowerCase().includes(q) ||
        c.numeroDocumento.toLowerCase().includes(q);

      const coincideEstado = 
        estado === 'todos' || 
        (estado === 'activos' && !c.bloqueado) || 
        (estado === 'bloqueados' && c.bloqueado);

      const coincideTipo = 
        tipoDoc === 'todos' || 
        c.tipoDocumento === tipoDoc;

      return coincideTexto && coincideEstado && coincideTipo;
    });
  });

  conteoTodos = computed(() => this.clientes().length);
  conteoActivos = computed(() => this.clientes().filter(c => !c.bloqueado).length);
  conteoBloqueados = computed(() => this.clientes().filter(c => c.bloqueado).length);

  mostrarPaginacion = computed(() => this.clientesFiltrados().length > 5);

  setFiltroEstado(nuevo: 'todos' | 'activos' | 'bloqueados') {
    this.filtroEstado.set(nuevo);
  }

  limpiarFiltros() {
    this.terminoBusqueda.set('');
    this.filtroEstado.set('todos');
    this.filtroTipoDoc.set('todos');
  }

  retirarCliente(id: number) {
    if (confirm('¿Está seguro de que desea retirar este cliente? Se aplicará baja lógica.')) {
      this.clienteService.retirarCliente(id);
    }
  }
}
