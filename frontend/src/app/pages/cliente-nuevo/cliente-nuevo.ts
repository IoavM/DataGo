import { Component, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink, ActivatedRoute } from '@angular/router';
import { OnInit } from '@angular/core';
import { ClienteService } from '../../services/cliente.service';

@Component({
  selector: 'app-cliente-nuevo',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './cliente-nuevo.html',
  styleUrl: './cliente-nuevo.css'
})
export class ClienteNuevoComponent implements OnInit {
  private route = inject(ActivatedRoute);
  esEdicion = signal(false);
  clienteId = signal<number | null>(null);
  ngOnInit() {
    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      const id = Number(idParam);
      this.esEdicion.set(true);
      this.clienteId.set(id);
      this.clienteService.obtenerPorId(id).subscribe({
        next: (c: any) => {
          if (c.bloqueado) {
            alert('Atención: Este cliente se encuentra retirado/bloqueado y según las reglas de negocio no puede modificarse.');
            this.router.navigate(['/clientes']);
            return;
          }
          this.tratamiento.set(c.tratamiento || 'Sr/Sra');
          this.nombreNegocio.set(c.nombreNegocio || '');
          this.razonSocial.set(c.razonSocialExtendida || c.nombreCompleto || '');
          this.tipoDocumento.set(c.tipoDocumento || 'C.C.');
          this.numeroDocumento.set(c.numeroDocumento || '');
          this.telefonoFijo.set(c.telefono || '');
          this.celular.set(c.celular || '');
          this.email.set(c.email || '');
          this.barrioSeleccionado.set(c.barrio || 'Laureles');
          this.municipio.set(c.municipio || 'Medellín');
          this.departamento.set(c.departamento || 'Antioquia');
          this.zonaTransporte.set(c.zonaTransporte || 'Zona Centro-Occidente');
          this.estrato.set(c.estrato || 3);
          this.esRural.set(c.esRural || false);
          this.direccionRural.set(c.direccionRural || '');
          this.viaTipo.set(c.viaTipo || 'Calle');
          this.viaNumero.set(c.viaNumero || '');
          this.viaLetra.set(c.viaLetra || '');
          this.viaCardinalidad.set(c.viaCardinalidad || '');
          this.cruceNumero.set(c.cruceNumero || '');
          this.placaNumero.set(c.placaNumero || '');
          this.centro.set(c.centro || 'Sede Principal Medellín');
        },
        error: (err: any) => {
          alert('Error al cargar datos del cliente: ' + (err.error?.mensaje || 'No encontrado'));
          this.router.navigate(['/clientes']);
        }
      });
    }
  }
  private clienteService = inject(ClienteService);
  private router = inject(Router);

  // PASO 1: Datos Generales
  tratamiento = signal<'Sr/Sra' | 'Empresa'>('Sr/Sra');
  nombreNegocio = signal('Distribuciones JC');
  razonSocial = signal('Juan Carlos Pérez Gómez');

  // Cálculos reactivos de nombres
  nombresSeparados = computed(() => {
    if (this.tratamiento() === 'Empresa') {
      const nombre = this.razonSocial();
      return { nombres: nombre, apellidos: nombre, nombreCompleto: nombre };
    }
    const sep = this.clienteService.separarNombresApellidos(this.razonSocial());
    return {
      nombres: sep.nombres,
      apellidos: sep.apellidos,
      nombreCompleto: this.razonSocial()
    };
  });

  // PASO 2: Identificación Oficial
  tipoDocumento = signal('C.C.');
  numeroDocumento = signal('1020485921');

  // D.V. reactivo (Algoritmo DIAN Módulo 11) - Solo aplica si es NIT
  dvCalculado = computed(() => {
    if (this.tipoDocumento() !== 'NIT') return null;
    return this.clienteService.calcularDV(this.numeroDocumento());
  });

  // Validación si el documento pertenece al Canal Moderno
  esCanalModerno = computed(() => {
    const doc = this.numeroDocumento().replace(/\D/g, '');
    return this.clienteService.canalModernoDocs.includes(doc);
  });

  // Validación real del número de documento
  documentoValido = computed(() => {
    const doc = this.numeroDocumento().replace(/\D/g, '');
    if (!doc) return false;
    if (this.esCanalModerno()) return false;
    // Dummies: no permitir secuencias o dígitos repetidos
    if (/^(\d)\1+$/.test(doc) || doc === '123456789' || doc === '1234567') return false;

    if (this.tipoDocumento() === 'NIT') {
      return doc.length >= 8 && doc.length <= 10 && this.dvCalculado() !== null;
    }
    return doc.length >= 6 && doc.length <= 10;
  });

  // PASO 3: Contacto
  telefonoFijo = signal('(604) 448 9200');
  celular = signal('+57 312 849 2011');
  email = signal('jc.distribuciones@gmail.com');

  tieneTelefono = computed(() => {
    return this.telefonoFijo().trim().length > 0 || this.celular().trim().length > 0;
  });

  esDummyEmail = computed(() => {
    const em = this.email().toLowerCase();
    return em.includes('test@') || em.includes('fake@') || em.includes('example@') || em === 'a@a.com';
  });

  // PASO 4: Dirección
  barrioSeleccionado = signal('Laureles');
  municipio = signal('Medellín');
  departamento = signal('Antioquia');
  pais = signal('Colombia');
  zonaTransporte = signal('Zona Centro-Occidente');
  esRural = signal(false);
  direccionRural = signal('');

  // Nomenclatura urbana
  viaTipo = signal('Calle');
  viaNumero = signal('10');
  viaLetra = signal('A');
  viaCardinalidad = signal('Sur');
  cruceNumero = signal('5');
  placaNumero = signal('30');

  direccionCompleta = computed(() => {
    if (this.esRural()) {
      return this.direccionRural() || 'Dirección rural pendiente';
    }
    const via = `${this.viaTipo()} ${this.viaNumero()}${this.viaLetra()} ${this.viaCardinalidad()}`.trim();
    const alimentadora = `# ${this.cruceNumero()}-${this.placaNumero()}`.trim();
    return `${via} ${alimentadora}, Barrio ${this.barrioSeleccionado()}, ${this.municipio()}, ${this.departamento()}`;
  });

  // PASO 5: Información Adicional
  estrato = signal(5);
  centro = signal('Sede Principal Medellín');

  claseImpuesto = computed(() => {
    return this.tratamiento() === 'Sr/Sra' ? 'Persona Natural' : 'Persona Jurídica';
  });

  condicionPago = '0010 Contado';

  setTratamiento(tipo: 'Sr/Sra' | 'Empresa') {
    this.tratamiento.set(tipo);
    if (tipo === 'Empresa') {
      this.tipoDocumento.set('NIT');
      this.razonSocial.set('Inversiones JC S.A.S.');
      this.nombreNegocio.set('JC Comercial');
    } else {
      this.tipoDocumento.set('C.C.');
      this.razonSocial.set('Juan Carlos Pérez Gómez');
      this.nombreNegocio.set('Distribuciones JC');
    }
  }

  onBarrioChange(event: any) {
    const b = event.target.value;
    this.barrioSeleccionado.set(b);
    if (b === 'Laureles' || b === 'El Poblado' || b === 'La Floresta') {
      this.municipio.set('Medellín');
      this.departamento.set('Antioquia');
      this.zonaTransporte.set('Zona Valle de Aburrá');
    } else if (b === 'Zúñiga') {
      this.municipio.set('Envigado');
      this.departamento.set('Antioquia');
      this.zonaTransporte.set('Zona Sur Aburrá');
    } else if (b === 'Chapinero') {
      this.municipio.set('Bogotá D.C.');
      this.departamento.set('Cundinamarca');
      this.zonaTransporte.set('Zona Capital');
    }
  }

  guardarCliente() {
    if (this.esCanalModerno()) {
      alert('Error: Este cliente debe crearse mediante el flujo "Creación clientes canal moderno".');
      return;
    }

    if (!this.tieneTelefono()) {
      alert('Error: Debe registrar al menos un teléfono (fijo o celular).');
      return;
    }

    const payload = {
      tratamiento: this.tratamiento(),
      nombreNegocio: this.nombreNegocio(),
      razonSocialExtendida: this.razonSocial(),
      tipoDocumento: this.tipoDocumento(),
      numeroDocumento: this.numeroDocumento(),
      telefono: this.telefonoFijo(),
      celular: this.celular(),
      email: this.email(),
      esRural: this.esRural(),
      direccionRural: this.direccionRural(),
      viaTipo: this.viaTipo(),
      viaNumero: this.viaNumero(),
      viaLetra: this.viaLetra(),
      viaCardinalidad: this.viaCardinalidad(),
      cruceNumero: this.cruceNumero(),
      placaNumero: this.placaNumero(),
      barrio: this.barrioSeleccionado(),
      centro: this.centro(),
      estrato: this.estrato()
    };

    if (this.esEdicion() && this.clienteId()) {
      const updatePayload = {
        nombreNegocio: this.nombreNegocio(),
        razonSocialExtendida: this.razonSocial(),
        telefono: this.telefonoFijo(),
        celular: this.celular(),
        email: this.email(),
        esRural: this.esRural(),
        direccionRural: this.direccionRural(),
        viaTipo: this.viaTipo(),
        viaNumero: this.viaNumero(),
        viaLetra: this.viaLetra(),
        viaCardinalidad: this.viaCardinalidad(),
        cruceNumero: this.cruceNumero(),
        placaNumero: this.placaNumero(),
        barrio: this.barrioSeleccionado(),
        centro: this.centro(),
        estrato: this.estrato()
      };
      this.clienteService.modificarCliente(this.clienteId()!, updatePayload, () => {
        alert('Cliente modificado exitosamente.');
        this.router.navigate(['/clientes']);
      }, (err: any) => {
        alert(err.error?.mensaje || 'Error al actualizar cliente');
      });
    } else {
      this.clienteService.agregarCliente(payload, () => {
        this.router.navigate(['/clientes']);
      }, (err: any) => {
        alert(err.error?.mensaje || 'Error al registrar cliente');
      });
    }
  }

  scrollTo(id: string) {
    const el = document.getElementById(id);
    if (el) {
      el.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
  }
}



