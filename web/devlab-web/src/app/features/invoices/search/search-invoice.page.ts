import { Component, signal, effect, ViewChild, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatSelectModule } from '@angular/material/select';
import { CustomersService } from '@core/services/customers.service';
import { InvoicesService }  from '@core/services/invoices.service';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';
import { ICustomer } from '@shared/models/customer.model';
import { IInvoiceHeaderDto } from '@shared/models/invoice.model';
import { MatCardModule } from '@angular/material/card';
import { MatRadioModule } from '@angular/material/radio';
import { MatInputModule } from '@angular/material/input';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { ToastService } from '@core/services/toast.service';
import { MESSAGES } from '@shared/constants';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { EmptyStateComponent } from '@shared/components/empty-state/empty-state.component';

@Component({
  standalone: true,
  selector: 'app-search-invoice',
  imports: [
    CommonModule,
    FormsModule,
    MatCardModule, 
    MatRadioModule, 
    MatSelectModule,
    MatInputModule, 
    MatFormFieldModule, 
    MatButtonModule,
    MatIconModule, 
    MatDividerModule,
    MatTableModule, 
    MatPaginatorModule, 
    MatSortModule,
    EmptyStateComponent
  ],
  templateUrl: './search-invoice.page.html',
  styleUrl: './search-invoice.page.css'
})
export class SearchInvoicePage implements AfterViewInit {
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  mode = signal<'customer'|'number'>('customer');
  customers = signal<ICustomer[]>([]);
  selectedCustomerId = signal<number|null>(null);
  number = signal<number|null>(null);
  list   = signal<IInvoiceHeaderDto[]>([]);
  detail = signal<{ name:string; quantity:number; unitPrice:number; lineTotal:number }[]>([]);
  header = signal<any>(null);
  messsageError = signal<string>(MESSAGES.errors.invoiceNotFound)
  hasSearched = signal(false);
  displayedColumns: (keyof IInvoiceHeaderDto | 'createdAt')[] =
    ['number','customer','subtotal','tax','total','createdAt'];
  dataSource = new MatTableDataSource<IInvoiceHeaderDto>([]);

  constructor(
    private custSvc: CustomersService,
    private invSvc: InvoicesService, 
    private toast: ToastService
  ) {
    //carga inicial de clientes
    this.custSvc.list().subscribe(v => this.customers.set(v));

    /**
     * sincroniza el MatTableDataSource cuando cambie list()
     */
    effect(() => {
      const rows = this.list();
      this.dataSource.data = rows ?? [];
      this.dataSource.filterPredicate = (data, filter) => {
        const f = (filter || '').trim().toLowerCase();
        return (
          data.customer?.toLowerCase().includes(f) ||
          String(data.number).includes(f) ||
          new Date(data.createdAt).toLocaleString().toLowerCase().includes(f)
        );
      };
    });
  }


  /**
    * se ejecuta cuando la vista y los @ViewChild ya estan listos
    * conecta el MatTableDataSource con MatPaginator y MatSort para
    * habilitar paginación y ordenamiento
    */
  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  /**
   * aplica el short filtrado
   * @param ev input filtro
   */
  applyFilter(ev: Event) {
    const v = (ev.target as HTMLInputElement).value ?? '';
    this.dataSource.filter = v.trim().toLowerCase();
    this.dataSource.paginator?.firstPage();
  }

  /**
   * limpia el filtro
   */
  clearFilter() {
    this.dataSource.filter = '';
    this.dataSource.paginator?.firstPage();
  }

  /**
   * se limpia los datos de la tabla y busqueda
   */
  resetSearch() {
    this.hasSearched.set(false);
    this.list.set([]);
    this.header.set(null);
    this.detail.set([]);
    this.dataSource.data = []; 
  }

  /**
   * se usa en el ngfor de la tabla devuelve una clave estable por fila para evitar renders
   * al reutilizar los elementos del DOM
   * @param _ index
   * @param r fila
   * @returns clave unica 
   */
  trackRow = (_: number, r: IInvoiceHeaderDto) => `${r.number}-${r.createdAt}`;

  /**
   * busqueda de la factura especificada por cliente o numero
   * @returns datos encontrados y mensajes toast de validacion
   */
  search() {
    this.list.set([]); 
    this.detail.set([]); 
    this.header.set(null);
    
    if (this.mode()==='customer' && this.selectedCustomerId()) {
      this.invSvc.byCustomer(this.selectedCustomerId()!).subscribe({
        next: res => {
          this.list.set(res.items);
          this.dataSource.data = res.items;
          this.hasSearched.set(true);
        },
        error: _ =>{
          this.toast.err(MESSAGES.errors.invoiceNotFound);
        }
      });
      return;
    }

    if (this.mode()==='number' && this.number()) {
      this.invSvc.byNumber(this.number()!).subscribe({
          next: res => {
            this.header.set(res.header); 
            this.detail.set(res.details);
            this.hasSearched.set(true);
          }, 
          error: _ => {
            this.toast.err(MESSAGES.errors.invoiceNotFound);
          }
        });
      return;
    }

    this.toast.warn(MESSAGES.validation.ckeckData);
    this.hasSearched.set(false);
  }
}
