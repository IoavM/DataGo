import { Injectable, signal, computed } from '@angular/core';
import { Cliente } from '../models/cliente.model';

@Injectable({
  providedIn: 'root'
})
export class ClienteService {
  // Lista central de clientes (compartida entre listado y nuevo)
  private clientesSignal = signal<Cliente[]>([
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

  // Exponer lista como solo lectura
  readonly clientes = this.clientesSignal.asReadonly();

  // Documentos en canal moderno de prueba (para la regla del PDF)
  readonly canalModernoDocs = ['12345678', '900111222', '800123456'];

  // Agregar nuevo cliente
  agregarCliente(cliente: Omit<Cliente, 'id' | 'codigo'>) {
    const actual = this.clientesSignal();
    const nuevoId = actual.length > 0 ? Math.max(...actual.map(c => c.id)) + 1 : 1;
    const nuevoCodigo = `CLI-${String(nuevoId).padStart(5, '0')}`;
    
    const nuevoRegistro: Cliente = {
      ...cliente,
      id: nuevoId,
      codigo: nuevoCodigo
    };

    this.clientesSignal.update(lista => [nuevoRegistro, ...lista]);
  }

  // Algoritmo oficial de DIAN para Dígito de Verificación (Módulo 11)
  calcularDV(nit: string): number | null {
    const limpio = nit.replace(/\D/g, '');
    if (!limpio || limpio.length < 5) return null;

    const primos = [71, 67, 59, 53, 47, 43, 41, 37, 29, 23, 19, 17, 13, 7, 3];
    const longitud = limpio.length;
    let suma = 0;

    for (let i = 0; i < longitud; i++) {
      const digito = parseInt(limpio[longitud - 1 - i], 10);
      suma += digito * primos[15 - 1 - i];
    }

    const residuo = suma % 11;
    if (residuo === 0 || residuo === 1) {
      return residuo;
    }
    return 11 - residuo;
  }

  // Algoritmo de separación de nombres y apellidos
  separarNombresApellidos(nombreLegal: string): { nombres: string; apellidos: string } {
    const palabras = nombreLegal.trim().split(/\s+/).filter(p => p.length > 0);
    if (palabras.length === 0) return { nombres: '', apellidos: '' };
    if (palabras.length === 1) return { nombres: palabras[0], apellidos: '' };
    if (palabras.length === 2) return { nombres: palabras[0], apellidos: palabras[1] };
    if (palabras.length === 3) return { nombres: palabras[0], apellidos: `${palabras[1]}, ${palabras[2]}` };
    
    // Si tiene 4 o más palabras: primeras 2 nombres, siguientes apellidos
    const nombres = palabras.slice(0, palabras.length - 2).join(', ');
    const apellidos = palabras.slice(palabras.length - 2).join(', ');
    return { nombres, apellidos };
  }
}
