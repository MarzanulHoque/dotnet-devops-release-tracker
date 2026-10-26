import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { DeploymentService } from '../../../core/services/deployment.service';
import { ProjectService } from '../../../core/services/project.service';
import { StatusBadgeComponent } from '../../../shared/components/status-badge/status-badge.component';
import { Deployment } from '../../../core/models/deployment.model';

@Component({
  selector: 'app-deployment-list',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, StatusBadgeComponent],
  templateUrl: './deployment-list.component.html'
})
export class DeploymentListComponent implements OnInit {
  protected readonly Math = Math;
  protected readonly deploymentService = inject(DeploymentService);
  protected readonly projectService = inject(ProjectService);

  // Filter signals
  readonly searchTerm = signal<string>('');
  readonly selectedEnvironment = signal<string>('All');
  readonly selectedStatus = signal<string>('All');
  readonly selectedProjectId = signal<number | null>(null);

  // Pagination & Sorting
  readonly currentPage = signal<number>(1);
  readonly pageSize = signal<number>(10);
  readonly sortBy = signal<'deployedAt' | 'version' | 'executionDurationSeconds'>('deployedAt');
  readonly sortAsc = signal<boolean>(false);

  // Delete modal state
  readonly pendingDelete = signal<Deployment | null>(null);
  readonly deleting = signal<boolean>(false);

  // Filtered & Sorted computed list
  readonly filteredDeployments = computed(() => {
    let list = this.deploymentService.deployments();
    const search = this.searchTerm().toLowerCase().trim();
    const env = this.selectedEnvironment();
    const status = this.selectedStatus();
    const projId = this.selectedProjectId();

    if (search) {
      list = list.filter(
        (d) =>
          d.appName.toLowerCase().includes(search) ||
          d.version.toLowerCase().includes(search) ||
          d.commitHash.toLowerCase().includes(search) ||
          d.deployedBy.toLowerCase().includes(search)
      );
    }

    if (env !== 'All') {
      list = list.filter((d) => d.environment.toLowerCase() === env.toLowerCase());
    }

    if (status !== 'All') {
      list = list.filter((d) => d.status.toLowerCase() === status.toLowerCase());
    }

    if (projId !== null) {
      list = list.filter((d) => d.projectId === projId);
    }

    // Sort
    const field = this.sortBy();
    const asc = this.sortAsc();
    return [...list].sort((a, b) => {
      let cmp = 0;
      if (field === 'deployedAt') {
        cmp = new Date(a.deployedAt).getTime() - new Date(b.deployedAt).getTime();
      } else if (field === 'version') {
        cmp = a.version.localeCompare(b.version);
      } else if (field === 'executionDurationSeconds') {
        cmp = a.executionDurationSeconds - b.executionDurationSeconds;
      }
      return asc ? cmp : -cmp;
    });
  });

  // Paginated list
  readonly paginatedDeployments = computed(() => {
    const list = this.filteredDeployments();
    const start = (this.currentPage() - 1) * this.pageSize();
    return list.slice(start, start + this.pageSize());
  });

  readonly totalPages = computed(() => {
    const total = this.filteredDeployments().length;
    return Math.max(1, Math.ceil(total / this.pageSize()));
  });

  ngOnInit(): void {
    this.deploymentService.loadDeployments().subscribe();
    this.projectService.loadProjects().subscribe();
  }

  setEnvironmentFilter(env: string): void {
    this.selectedEnvironment.set(env);
    this.currentPage.set(1);
  }

  setStatusFilter(status: string): void {
    this.selectedStatus.set(status);
    this.currentPage.set(1);
  }

  onSearchChange(term: string): void {
    this.searchTerm.set(term);
    this.currentPage.set(1);
  }

  toggleSort(field: 'deployedAt' | 'version' | 'executionDurationSeconds'): void {
    if (this.sortBy() === field) {
      this.sortAsc.update((asc) => !asc);
    } else {
      this.sortBy.set(field);
      this.sortAsc.set(false);
    }
  }

  setPage(page: number): void {
    if (page >= 1 && page <= this.totalPages()) {
      this.currentPage.set(page);
    }
  }

  refresh(): void {
    this.deploymentService.loadDeployments().subscribe();
  }

  confirmDelete(deployment: Deployment): void {
    this.pendingDelete.set(deployment);
  }

  cancelDelete(): void {
    this.pendingDelete.set(null);
  }

  executeDelete(): void {
    const target = this.pendingDelete();
    if (!target) return;

    this.deleting.set(true);
    this.deploymentService.deleteDeployment(target.id).subscribe({
      next: () => {
        this.deleting.set(false);
        this.pendingDelete.set(null);
      },
      error: () => {
        this.deleting.set(false);
        this.pendingDelete.set(null);
      }
    });
  }
}
