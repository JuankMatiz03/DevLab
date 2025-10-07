import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private http = inject(HttpClient);

  /** URL base; con el proxy de Angular, `/api` apunta a tu API .NET */
  protected base = '/api';

  /**
   * Helper GET.
   * @param url Ruta del endpoint (ej.: '/products')
   * @param params Parámetros de consulta opcionales
   * @returns Observable con el tipo solicitado
   */
  get<T>(url: string, params?: any) {
    return this.http.get<T>(`${this.base}${url}`, { params });
  }

  /**
   * Helper POST.
   * @param url Ruta del endpoint
   * @param body Cuerpo de la petición
   * @returns Observable con la respuesta tipada
   */
  post<T>(url: string, body: any) {
    return this.http.post<T>(`${this.base}${url}`, body);
  }
}
