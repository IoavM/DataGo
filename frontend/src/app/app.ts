import { Component, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Cliente } from './models/cliente.model';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  // Lista inicial con los mismos datos del mockup
  clientes = signal<Cliente[]>([
    {
      id: 1,
      codigo: 'CLI-00482',
      nombreCompleto: 'Juan Carlos Pérez Gómez',
      nombreNegocio: 'Distribuciones JC',
      tipoDocumento: 'C.C.',
      numeroDocumento: '1.020.485.921',
      telefono: '312 456 7890',
      email: 'jc.perez@distribucionesjc.co',
      municipio: 'Medellín',
      barrio: 'Laureles',
      estrato: 5,
      bloqueado: false
    },
    {
      id: 2,
      codigo: 'CLI-00319',
      nombreCompleto: 'María Helena Restrepo',
      nombreNegocio: 'Residencial Los Álamos',
      tipoDocumento: 'C.C.',
      numeroDocumento: '43.892.115',
      telefono: '300 289 1144',
      email: 'mrestrepo@alamosres.org',
      municipio: 'Envigado',
      barrio: 'Zúñiga',
      estrato: 4,
      bloqueado: false
    },
    {
      id: 3,
      codigo: 'CLI-00754',
      nombreCompleto: 'Andrés Felipe Morales',
      nombreNegocio: 'Inversiones Morales S.A.S.',
      tipoDocumento: 'NIT',
      numeroDocumento: '901.442.809-3',
      telefono: '317 890 2311',
      email: 'gerencia@morales.co',
      municipio: 'Sabaneta',
      barrio: 'Aves María',
      estrato: 3,
      bloqueado: false
    },
    {
      id: 4,
      codigo: 'CLI-00108',
      nombreCompleto: 'Distribuidora La Floresta',
      nombreNegocio: 'Comercial e Inmuebles',
      tipoDocumento: 'NIT',
      numeroDocumento: '890.104.551-0',
      telefono: '310 500 9920',
      email: 'admon@lafloresta.com.co',
      municipio: 'Medellín',
      barrio: 'La Floresta',
      estrato: 4,
      bloqueado: false
    },
    {
      id: 5,
      codigo: 'CLI-00891',
      nombreCompleto: 'Camila Echeverri Salazar',
      nombreNegocio: 'Urbanización Portal Real',
      tipoDocumento: 'C.C.',
      numeroDocumento: '1.017.332.901',
      telefono: '315 771 0088',
      email: 'camilareal@urbanportal.co',
      municipio: 'Bello',
      barrio: 'Cabañas',
      estrato: 3,
      bloqueado: true
    }
  ]);

  // Filtros de búsqueda
  terminoBusqueda = signal('');
  filtroEstado = signal<'todos' | 'activos' | 'bloqueados'>('todos');
  filtroTipoDoc = signal('todos');

  // Modal de cliente (creación o edición)
  modalAbierto = signal(false);

  // Clientes filtrados computados en tiempo real
  clientesFiltrados = computed(() => {
    const q = this.terminoBusqueda().toLowerCase().trim();
    const estado = this.filtroEstado();
    const tipoDoc = this.filtroTipoDoc();

    return this.clientes().filter(c => {
      // Filtro por texto
      const coincideTexto = !q || 
        c.nombreCompleto.toLowerCase().includes(q) ||
        c.nombreNegocio.toLowerCase().includes(q) ||
        c.codigo.toLowerCase().includes(q) ||
        c.numeroDocumento.toLowerCase().includes(q);

      // Filtro por estado
      const coincideEstado = 
        estado === 'todos' || 
        (estado === 'activos' && !c.bloqueado) || 
        (estado === 'bloqueados' && c.bloqueado);

      // Filtro por tipo doc
      const coincideTipo = 
        tipoDoc === 'todos' || 
        c.tipoDocumento === tipoDoc;

      return coincideTexto && coincideEstado && coincideTipo;
    });
  });

  // Conteo de estados
  conteoTodos = computed(() => this.clientes().length);
  conteoActivos = computed(() => this.clientes().filter(c => !c.bloqueado).length);
  conteoBloqueados = computed(() => this.clientes().filter(c => c.bloqueado).length);

  // Cambiar pestaña de estado
  setFiltroEstado(nuevo: 'todos' | 'activos' | 'bloqueados') {
    this.filtroEstado.set(nuevo);
  }

  // Limpiar todos los filtros
  limpiarFiltros() {
    this.terminoBusqueda.set('');
    this.filtroEstado.set('todos');
    this.filtroTipoDoc.set('todos');
  }

  // Abrir modal de nuevo cliente
  abrirModalNuevo() {
    this.modalAbierto.set(true);
  }

  cerrarModal() {
    this.modalAbierto.set(false);
  }
}
