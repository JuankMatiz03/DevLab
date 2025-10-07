import { Routes } from '@angular/router';

export const routes: Routes = [
    { path: '', redirectTo: 'search', pathMatch: 'full' },
    { 
        path: 'create', 
        loadComponent: () => import(
            '@features/invoices/create/create-invoice.page'
        ).then(m => m.CreateInvoicePage) 
    },
    { 
        path: 'search', 
        loadComponent: () => import(
            '@features/invoices/search/search-invoice.page'
        ).then(m => m.SearchInvoicePage) 
    },
    { path: '**', redirectTo: 'search' }
];
