/**
 * redondeo a 2 decimales
 * @param n numero a redondear
 * @returns numero convertido
 */
export function round2(n: number): number {
  return Math.round((n + Number.EPSILON) * 100) / 100;
}
