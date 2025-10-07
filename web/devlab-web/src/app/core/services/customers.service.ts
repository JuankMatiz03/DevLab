import { Injectable, inject } from '@angular/core';
import { ApiService } from './api.service';
import { ICustomer } from '@shared/models/customer.model';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class CustomersService {
  private api = inject(ApiService);

  /**
   * trae los clientes activos.
   * @returns Observable<Customer[]>
   */
  list(): Observable<ICustomer[]> {
    return this.api.get<ICustomer[]>('/customers');
  }
}
