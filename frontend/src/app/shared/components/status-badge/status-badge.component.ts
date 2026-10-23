import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-status-badge',
  standalone: true,
  imports: [CommonModule],
  template: `
    <span [ngClass]="getBadgeClass()" class="badge px-2.5 py-1.5 rounded-pill d-inline-flex align-items-center gap-1.5 font-mono">
      <i [ngClass]="getIconClass()"></i>
      <span>{{ status || 'Unknown' }}</span>
    </span>
  `
})
export class StatusBadgeComponent {
  @Input({ required: true }) status: string = '';

  getBadgeClass(): string {
    const s = (this.status || '').toLowerCase();
    if (s === 'successful' || s === 'success') {
      return 'badge-soft-success';
    } else if (s === 'failed' || s === 'error') {
      return 'badge-soft-danger';
    } else if (s === 'in-progress' || s === 'inprogress' || s === 'running') {
      return 'badge-soft-primary';
    } else {
      return 'badge-soft-warning';
    }
  }

  getIconClass(): string {
    const s = (this.status || '').toLowerCase();
    if (s === 'successful' || s === 'success') {
      return 'bi bi-check-circle-fill';
    } else if (s === 'failed' || s === 'error') {
      return 'bi bi-x-circle-fill';
    } else if (s === 'in-progress' || s === 'inprogress' || s === 'running') {
      return 'bi bi-arrow-repeat';
    } else {
      return 'bi bi-clock-history';
    }
  }
}
