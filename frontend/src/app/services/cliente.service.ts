import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Cliente } from '../models/cliente.model';

@Injectable({
  providedIn: 'root'
})
export class ClienteService {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5181/api';

  private clientesSignal = signal<Cliente[]>([]);
  readonly clientes = this.clientesSignal.asReadonly();
  readonly canalModernoDocs = ['12345678', '900111222', '800123456'];

  constructor() {
    this.cargarClientes();
  }

  cargarClientes() {
    this.http.get<Cliente[]>(`${this.apiUrl}/clientes`).subscribe({
      next: (data) => {
        this.clientesSignal.set(data);
      },
      error: (err) => {
        console.warn('API no disponible o error de conexión. Usando respaldo local.', err);
      }
    });
  }

  agregarCliente(payload: any, onSuccess?: () => void, onError?: (err: any) => void) {
    this.http.post<Cliente>(`${this.apiUrl}/clientes`, payload).subscribe({
      next: () => {
        this.cargarClientes();
        if (onSuccess) onSuccess();
      },
      error: (err) => {
        console.error('Error creando cliente:', err);
        if (onError) onError(err);
      }
    });
  }

  retirarCliente(id: number) {
    this.http.delete(`${this.apiUrl}/clientes/${id}`).subscribe({
      next: () => {
        this.cargarClientes();
      },
      error: (err) => console.error('Error retirando cliente:', err)
    });
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
    return (residuo === 0 || residuo === 1) ? residuo : 11 - residuo;
  }

  // Separación de nombres y apellidos
  separarNombresApellidos(nombreLegal: string): { nombres: string; apellidos: string } {
    const palabras = nombreLegal.trim().split(/\s+/).filter(p => p.length > 0);
    if (palabras.length === 0) return { nombres: '', apellidos: '' };
    if (palabras.length === 1) return { nombres: palabras[0], apellidos: '' };
    if (palabras.length === 2) return { nombres: palabras[0], apellidos: palabras[1] };
    if (palabras.length === 3) return { nombres: palabras[0], apellidos: `${palabras[1]}, ${palabras[2]}` };

    const nombres = palabras.slice(0, palabras.length - 2).join(', ');
    const apellidos = palabras.slice(palabras.length - 2).join(', ');
    return { nombres, apellidos };
  }
}
