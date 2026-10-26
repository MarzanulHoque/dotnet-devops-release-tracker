import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { HealthService } from '../../../core/services/health.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './navbar.component.html'
})
export class NavbarComponent implements OnInit {
  protected readonly healthService = inject(HealthService);

  ngOnInit(): void {
    this.healthService.checkHealth().subscribe();
    // Heartbeat poll every 30 seconds
    setInterval(() => {
      this.healthService.checkHealth().subscribe();
    }, 30000);
  }
}
