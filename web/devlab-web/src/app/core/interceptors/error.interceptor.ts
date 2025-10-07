import { HttpInterceptorFn } from '@angular/common/http';

/**
 * manejo de errores globales
 * @param req 
 * @param next 
 * @returns 
 */
export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  return next(req).pipe();
};
