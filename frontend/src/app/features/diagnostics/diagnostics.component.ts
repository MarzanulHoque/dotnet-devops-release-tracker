import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { HealthService } from '../../core/services/health.service';

@Component({
  selector: 'app-diagnostics',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './diagnostics.component.html'
})
export class DiagnosticsComponent implements OnInit {
  protected readonly healthService = inject(HealthService);

  ngOnInit(): void {
    this.refreshProbe();
  }

  refreshProbe(): void {
    this.healthService.checkHealth().subscribe();
  }
}
