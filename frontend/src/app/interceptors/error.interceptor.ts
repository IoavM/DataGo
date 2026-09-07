import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      let mensajeAmigable = 'Ocurrió un error inesperado.';

      if (error.status === 0) {
        mensajeAmigable = 'No se pudo conectar con el servidor API. Verifique su conexión a Internet o el estado del backend.';
      } else if (error.status === 400) {
        mensajeAmigable = error.error?.detalle || error.error?.mensaje || 'Datos de solicitud inválidos.';
      } else if (error.status === 404) {
        mensajeAmigable = error.error?.detalle || error.error?.mensaje || 'El registro o recurso solicitado no existe.';
      } else if (error.status >= 500) {
        mensajeAmigable = 'Error interno en el servidor. Intente nuevamente en unos instantes.';
      }

      console.error(`[HTTP Error ${error.status}] URL: ${req.url} | Mensaje: ${mensajeAmigable}`, error);

      // Mutar o adjuntar el mensaje normalizado al error
      return throwError(() => ({
        status: error.status,
        mensaje: mensajeAmigable,
        original: error
      }));
    })
  );
};
