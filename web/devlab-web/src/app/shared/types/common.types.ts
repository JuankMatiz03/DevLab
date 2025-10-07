/**
 * tipos del row
 */
export type Row = {
    productId: number;
    price: number;
    qty: number;
    imageUrl?: string | null;
    lineTotal: number;
};

/**
 * tipos del toast
 */
export type ToastType = 'ok' | 'warn' | 'err';