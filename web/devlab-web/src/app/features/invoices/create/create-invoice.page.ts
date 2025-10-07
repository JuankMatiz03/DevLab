import { Component, DestroyRef, effect, inject, OnInit, signal, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CustomersService } from '@core/services/customers.service';
import { ProductsService }  from '@core/services/products.service';
import { InvoicesService }  from '@core/services/invoices.service';
import { ICustomer } from '@shared/models/customer.model';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { IProduct }  from '@shared/models/product.model';
import { ICreateInvoiceRequest, IInvoiceItemDto } from '@shared/models/invoice.model';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { MatDividerModule } from '@angular/material/divider';
import { ViewChild, AfterViewInit } from '@angular/core';
import { ToastService } from '@core/services/toast.service';
import { round2 } from '@shared/utils';
import { Row } from '@shared/types/common.types';
import { MESSAGES } from '@shared/constants';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { forkJoin, Subject, takeUntil } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  standalone: true,
  selector: 'app-create-invoice',
  imports: [
    CommonModule, FormsModule,
    MatCardModule, MatFormFieldModule, MatInputModule, MatSelectModule,
    MatButtonModule, MatIconModule, MatTableModule, MatDividerModule,
    MatPaginatorModule, MatSortModule
  ],
  templateUrl: './create-invoice.page.html',
  styleUrl: './create-invoice.page.css'
})
export class CreateInvoicePage implements AfterViewInit, OnInit, OnDestroy {
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild(MatPaginator) set matPaginator(p: MatPaginator) {
    if (p) { 
      this.paginator = p; 
      this.dataSource.paginator = p; 
    }
  }
  @ViewChild(MatSort) set matSort(s: MatSort) {
    if (s) { 
      this.sort = s; 
      this.dataSource.sort = s; 
    }
  }

  customers = signal<ICustomer[]>([]);
  products  = signal<IProduct[]>([]);
  number = signal<number | null>(null);
  customerId = signal<number | null>(null);
  rows = signal<Row[]>([]);
  dataSource = new MatTableDataSource<Row>([]);
  subtotal = signal(0);
  tax = signal(0);
  total = signal(0);
  displayedColumns: string[] = ['product','price','qty','image','lineTotal','actions'];
  private destroy$ = new Subject<void>();

  constructor(
    private customersSvc: CustomersService,
    private productsSvc:  ProductsService,
    private invoicesSvc:  InvoicesService,
    private toast: ToastService
  ) {
    /**
     * sincroniza el data source con rows()
     */
    effect(() => {
      this.dataSource.data = this.rows();
    });

    /**
     * filtro por nombre de producto y numericoos basicos
     */
    this.dataSource.filterPredicate = (data, filter) => {
      const f = (filter || '').trim().toLowerCase();
      const name = this.products().find(p => p.id === data.productId)?.name?.toLowerCase() ?? '';
      return (
        name.includes(f) ||
        String(data.price).toLowerCase().includes(f) ||
        String(data.qty).toLowerCase().includes(f) ||
        String(data.lineTotal).toLowerCase().includes(f)
      );
    };

    /**
     * recalcula totales al cambiar filas  
     */ 
    effect(() => {
      const st = this.rows().reduce((s, r) => s + r.lineTotal, 0);
      const subtotal = round2(st);
      const iva = round2(subtotal * 0.19);
      this.subtotal.set(subtotal);
      this.tax.set(iva);
      this.total.set(round2(subtotal + iva));
    });
  }

  /**
   * carga inicial de clientes y productos
   */
  ngOnInit(): void {
    forkJoin({
      customers: this.customersSvc.list(),
      products:  this.productsSvc.list()
    })
    .pipe(takeUntil(this.destroy$))
    .subscribe({
      next: ({ customers, products }) => {
        this.customers.set(customers);
        this.products.set(products);
      },
      error: () => this.toast.err(MESSAGES.errors.fetch)
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
   * proceso para agregar la nueva factura, limpia todos los campos para iniciar el proceso
   */
  newInvoice() {
    this.number.set(null);
    this.customerId.set(null);
    this.rows.set([]);
    // Limpia también el filtro
    this.clearFilter();
  }

  /**
   * añade el producto a la tabla
   */
  addRow() {
    const first = this.products()[0];
    const price = first?.price ?? 0;
    this.rows.update(rs => [
      ...rs,
      {
        productId: first?.id ?? 0,
        price,
        qty: 1,
        imageUrl: first?.imageUrl ?? null,
        lineTotal: round2(price)
      }
    ]);

    this.paginator?.firstPage();
  }

  /**
   * limpia la tabla 
   */
  resetSearch() {
    this.dataSource.data = []; 
  }

  onProductChange(row: Row) {
    const p = this.products().find(x => x.id === row.productId);
    row.price = p?.price ?? 0;
    row.imageUrl = p?.imageUrl ?? null;
    this.recalc(row);
  }

  recalc(row: Row) {
    row.lineTotal = round2((row.qty || 0) * (row.price || 0));
    this.rows.update(rs => [...rs]); 
  }

  /**
   * elimina los productos agregados a la factura
   * @param i index de la tabla, row
   */
  removeRow(i: number) {
    const rs = [...this.rows()];
    rs.splice(i, 1);
    this.rows.set(rs);
  }

  /**
   * inicialmente se valida que se cumplan con las validaciones, en caso de que 
   * todo sea correcto se crear la factura
   * @returns 
   */
  save() {
    if (!this.number()) { 
      this.toast.warn(MESSAGES.validation.numberRequired);
      return; 
    }
    if (!this.customerId()) { 
      this.toast.warn(MESSAGES.validation.customerRequired); 
      return; 
    }
    if (!this.rows().length) { 
      this.toast.warn(MESSAGES.validation.atLeastOneItem); 
      return; 
    }
    if (!this.rows().every(r => r.productId && r.qty > 0 && r.price > 0)) {
      this.toast.warn(MESSAGES.validation.checkRows); return;
    }

    const items: IInvoiceItemDto[] = this.rows().map(r => ({
      productId: r.productId,
      quantity:  r.qty,
      unitPrice: r.price
    }));

    const body: ICreateInvoiceRequest = {
      number: this.number()!,
      customerId: this.customerId()!,
      items
    };

    this.invoicesSvc.create(body).subscribe({
      next: (_) => { 
        this.toast.ok(MESSAGES.success.saved)
        this.newInvoice();
      },
      error: (e) => { 
        console.error(e); 
        this.toast.err(MESSAGES.errors.save);
      }
    });
  }

  /**
   * aplica el short filtrado
   * @param ev input filtro
   */
  applyFilter(evt: Event) {
    const value = (evt.target as HTMLInputElement).value ?? '';
    this.dataSource.filter = value.trim().toLowerCase();
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
   * evita las fugas de memoria 
   */
  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
  
}
