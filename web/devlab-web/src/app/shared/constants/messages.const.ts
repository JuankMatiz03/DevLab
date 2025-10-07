export const MESSAGES = {
  success: {
    saved: '¡Guardado con éxito!',
  },
  errors: {
    generic: 'Ocurrió un error. Intenta nuevamente.',
    fetch: 'No se pudieron obtener los datos.',
    save: 'No se pudo guardar la factura.',
    invoiceNotFound: 'No encontramos una factura con ese número.'
  },
  validation: {
    numberRequired: 'El número de factura es obligatorio.',
    customerRequired: 'El cliente es obligatorio.',
    atLeastOneItem: 'Agrega al menos un producto.',
    checkRows: 'Verifica cantidad y precio de todas las filas.',
    ckeckData: 'Selecciona un cliente o ingresa un número de factura.',
  }
} as const;
