import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { DeploymentService } from '../../core/services/deployment.service';
import { HealthService } from '../../core/services/health.service';
import { ProjectService } from '../../core/services/project.service';
import { StatusBadgeComponent } from '../../shared/components/status-badge/status-badge.component';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule, StatusBadgeComponent],
  templateUrl: './dashboard.component.html'
})
export class DashboardComponent implements OnInit {
  protected readonly deploymentService = inject(DeploymentService);
  protected readonly healthService = inject(HealthService);
  protected readonly projectService = inject(ProjectService);

  ngOnInit(): void {
    this.deploymentService.loadDeployments().subscribe();
    this.projectService.loadProjects().subscribe();
    this.healthService.checkHealth().subscribe();
  }
}
