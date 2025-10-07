import { Injectable, inject } from '@angular/core';
import { ApiService } from './api.service';
import { 
  ICreateInvoiceRequest, 
  ISearchByCustomerResponse, 
  ISearchByNumberResponse 
} from '@shared/models/invoice.model';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class InvoicesService {
  private api = inject(ApiService);

  /**
   * crea una factura llamando al endpoint del backend
   * @param req datos de factura, numero, cliente, items
   * @returns observable con el id creado
   */
  create(req: ICreateInvoiceRequest): Observable<{ id: number }> {
    return this.api.post<{ id: number }>('/invoices', req);
  }

  /**
   * busca facturas por cliente 
   * @param customerId id del cliente
   * @returns Observable<SearchByCustomerResponse>
   */
  byCustomer(customerId: number) {
    return this.api.get<ISearchByCustomerResponse>('/invoices/search', { customerId });
  }

  /**
   * busca una factura por numero, encabezado + detalle
   * @param number numero de factura
   * @returns Observable<SearchByNumberResponse>
   */
  byNumber(number: number) {
    return this.api.get<ISearchByNumberResponse>('/invoices/search', { number });
  }
}
