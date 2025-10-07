import { Injectable } from '@angular/core';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';
import { ToastType } from '@shared/types/common.types';

@Injectable({ providedIn: 'root' })
export class ToastService {
  private baseCfg: MatSnackBarConfig = {
    duration: 2500,
    horizontalPosition: 'right',
    verticalPosition: 'top',
  };

  constructor(private sb: MatSnackBar) {}

  /**
   * mensajes custom TOAST
   * @param message mensaje a mostrar
   * @param type tipo: ok, warn, err
   * @param cfg configuracion opcional
   */
  show(message: string, type: ToastType = 'ok', cfg?: MatSnackBarConfig) {
    this.sb.open(message, 'OK', {
      ...this.baseCfg,
      ...cfg,
      panelClass: [`snack-${type}`],
    });
  }

  /**
   * toast valido
   * @param message mensaje a mostrar
   * @param cfg configuracion extra
   */
  ok(message: string, cfg?: MatSnackBarConfig) { 
    this.show(message, 'ok',   cfg); 
  }

  /**
   * toast advertencia
   * @param message mensaje a mostrar
   * @param cfg configuracion extra
   */
  warn(message: string, cfg?: MatSnackBarConfig) {
    this.show(message, 'warn', cfg); 
  }

  /**
   * toast error
   * @param message mensaje a mostrar
   * @param cfg configuracion extra
   */
  err(message: string, cfg?: MatSnackBarConfig)  { 
    this.show(message, 'err',  cfg); 
  }

  /**
   * toast para errores http
   * @param e contenido responde
   * @param fallback mensaje
   */
  fromError(e: any, fallback = 'Ocurrió un error') {
    const msg =
      e?.error?.message ??
      e?.message ??
      (typeof e === 'string' ? e : null) ??
      fallback;
    this.err(msg);
  }
}
