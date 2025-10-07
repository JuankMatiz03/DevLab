import { Injectable, inject } from '@angular/core';
import { ApiService } from './api.service';
import { IProduct } from '@shared/models/product.model';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ProductsService {
  private api = inject(ApiService);

  /**
   * Trae los productos, nombre, precio, imagen
   * @returns Observable<IProduct[]>
   */
  list(): Observable<IProduct[]> {
    return this.api.get<IProduct[]>('/products');
  }
}
