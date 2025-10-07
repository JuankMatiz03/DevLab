export interface IInvoiceItemDto { 
  productId: number; 
  quantity: number; 
  unitPrice: number; 
}

export interface ICreateInvoiceRequest { 
  number: number; 
  customerId: number; 
  items: IInvoiceItemDto[]; 
}

export interface IInvoiceHeaderDto {
  id: number;
  number: number; 
  customer: string;
  subtotal: number; 
  tax: number; 
  total: number; 
  createdAt: string;
}
export interface IInvoiceDetailDto {
  productId: number; 
  name: string; 
  quantity: number;
  unitPrice: number; 
  lineTotal: number; 
  imageUrl?: string | null;
}

export interface ISearchByNumberResponse { 
  header?: IInvoiceHeaderDto | null; 
  details: IInvoiceDetailDto[]; 
}

export interface ISearchByCustomerResponse { 
  items: IInvoiceHeaderDto[]; 
}
