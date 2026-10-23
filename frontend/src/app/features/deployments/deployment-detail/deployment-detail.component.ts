import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { DeploymentService } from '../../../core/services/deployment.service';
import { DeploymentDetail } from '../../../core/models/deployment.model';
import { StatusBadgeComponent } from '../../../shared/components/status-badge/status-badge.component';

@Component({
  selector: 'app-deployment-detail',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, StatusBadgeComponent],
  templateUrl: './deployment-detail.component.html'
})
export class DeploymentDetailComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  protected readonly deploymentService = inject(DeploymentService);

  readonly deployment = signal<DeploymentDetail | null>(null);
  readonly loading = signal<boolean>(true);
  readonly error = signal<string | null>(null);

  // New log entry state
  readonly newStepName = signal<string>('Health Check');
  readonly newLogLevel = signal<string>('Info');
  readonly newMessage = signal<string>('');
  readonly addingLog = signal<boolean>(false);

  // Delete modal state
  readonly showDeleteModal = signal<boolean>(false);
  readonly deleting = signal<boolean>(false);

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      const id = parseInt(idParam, 10);
      if (!isNaN(id)) {
        this.loadDetails(id);
      }
    }
  }

  loadDetails(id: number): void {
    this.loading.set(true);
    this.deploymentService.getDeploymentById(id).subscribe({
      next: (data) => {
        this.deployment.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.message || err.message || 'Deployment record not found.');
        this.loading.set(false);
      }
    });
  }

  appendLog(): void {
    const dep = this.deployment();
    if (!dep || !this.newMessage().trim()) return;

    this.addingLog.set(true);
    const dto = {
      stepName: this.newStepName().trim(),
      logLevel: this.newLogLevel(),
      message: this.newMessage().trim()
    };

    this.deploymentService.addLog(dep.id, dto).subscribe({
      next: (createdLog) => {
        this.deployment.update((curr) => {
          if (!curr) return null;
          return {
            ...curr,
            logs: [...(curr.logs || []), createdLog]
          };
        });
        this.newMessage.set('');
        this.addingLog.set(false);
      },
      error: (err) => {
        alert(err.error?.message || 'Failed to append log.');
        this.addingLog.set(false);
      }
    });
  }

  confirmDelete(): void {
    this.showDeleteModal.set(true);
  }

  cancelDelete(): void {
    this.showDeleteModal.set(false);
  }

  executeDelete(): void {
    const dep = this.deployment();
    if (!dep) return;

    this.deleting.set(true);
    this.deploymentService.deleteDeployment(dep.id).subscribe({
      next: () => {
        this.deleting.set(false);
        this.router.navigate(['/deployments']);
      },
      error: () => {
        this.deleting.set(false);
      }
    });
  }
}
