import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { NgClass } from '@angular/common';

@Component({
  standalone: true,
  selector: 'app-empty-state',
  imports: [MatIconModule, MatButtonModule, MatCardModule, NgClass],
  templateUrl: './empty-state.component.html',
  styleUrl: './empty-state.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class EmptyStateComponent {
  @Input() icon: string = 'shopping_cart';
  @Input() title: string = 'Sin datos';
  @Input() subtitle: string = 'No hay elementos para mostrar.';
  @Input() actionLabel?: string;
  @Input() color: 'primary'|'accent'|'warn' = 'primary';
  @Input() layout: 'row'|'column' = 'row';
  @Input() dashed = true;
  @Input() dense = false;
  @Output() action = new EventEmitter<void>();
}
