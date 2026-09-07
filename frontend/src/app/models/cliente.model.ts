export interface Cliente {
  id: number;
  codigo: string;
  nombreCompleto: string;
  nombreNegocio: string;
  tipoDocumento: string;
  numeroDocumento: string;
  telefono: string;
  email: string;
  municipio: string;
  barrio: string;
  estrato: number;
  bloqueado: boolean;
}
