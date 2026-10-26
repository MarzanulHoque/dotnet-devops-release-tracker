import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { DeploymentService } from '../../../core/services/deployment.service';
import { ProjectService } from '../../../core/services/project.service';
import { CreateDeploymentDto, UpdateDeploymentDto } from '../../../core/models/deployment.model';

@Component({
  selector: 'app-deployment-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './deployment-form.component.html'
})
export class DeploymentFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  protected readonly deploymentService = inject(DeploymentService);
  protected readonly projectService = inject(ProjectService);

  readonly isEditMode = signal<boolean>(false);
  readonly deploymentId = signal<number | null>(null);
  readonly submitting = signal<boolean>(false);
  readonly errorMessage = signal<string | null>(null);
  readonly successMessage = signal<string | null>(null);

  form: FormGroup = this.fb.group({
    projectId: [null],
    appName: ['', [Validators.required, Validators.maxLength(100)]],
    version: ['', [Validators.required, Validators.maxLength(50)]],
    environment: ['Production', [Validators.required]],
    status: ['Successful', [Validators.required]],
    deployedBy: ['Platform Engineer', [Validators.maxLength(100)]],
    commitHash: ['HEAD', [Validators.maxLength(40)]],
    executionDurationSeconds: [45, [Validators.required, Validators.min(1)]],
    notes: ['', [Validators.maxLength(500)]]
  });

  ngOnInit(): void {
    this.projectService.loadProjects().subscribe();

    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      const id = parseInt(idParam, 10);
      if (!isNaN(id)) {
        this.isEditMode.set(true);
        this.deploymentId.set(id);
        this.loadExisting(id);
      }
    }
  }

  onProjectSelected(event: Event): void {
    const select = event.target as HTMLSelectElement;
    const projId = select.value ? parseInt(select.value, 10) : null;
    if (projId) {
      const project = this.projectService.projects().find((p) => p.id === projId);
      if (project && !this.form.get('appName')?.value) {
        this.form.patchValue({ appName: project.name });
      }
    }
  }

  loadExisting(id: number): void {
    this.deploymentService.getDeploymentById(id).subscribe({
      next: (data) => {
        this.form.patchValue({
          projectId: data.projectId,
          appName: data.appName,
          version: data.version,
          environment: data.environment,
          status: data.status,
          deployedBy: data.deployedBy,
          commitHash: data.commitHash,
          executionDurationSeconds: data.executionDurationSeconds,
          notes: data.notes
        });
      },
      error: (err) => {
        this.errorMessage.set(err.message || 'Failed to load deployment details.');
      }
    });
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.submitting.set(true);
    this.errorMessage.set(null);
    const val = this.form.value;

    if (this.isEditMode() && this.deploymentId() !== null) {
      const updateDto: UpdateDeploymentDto = {
        appName: val.appName,
        version: val.version,
        environment: val.environment,
        status: val.status,
        deployedBy: val.deployedBy,
        commitHash: val.commitHash,
        executionDurationSeconds: val.executionDurationSeconds,
        notes: val.notes
      };

      this.deploymentService.updateDeployment(this.deploymentId()!, updateDto).subscribe({
        next: () => {
          this.submitting.set(false);
          this.successMessage.set('Deployment record updated successfully.');
          setTimeout(() => this.router.navigate(['/deployments']), 800);
        },
        error: (err) => {
          this.submitting.set(false);
          this.errorMessage.set(err.error?.message || err.message || 'Update failed.');
        }
      });
    } else {
      const createDto: CreateDeploymentDto = {
        projectId: val.projectId ? parseInt(val.projectId, 10) : null,
        appName: val.appName,
        version: val.version,
        environment: val.environment,
        status: val.status,
        deployedBy: val.deployedBy,
        commitHash: val.commitHash,
        executionDurationSeconds: val.executionDurationSeconds,
        notes: val.notes
      };

      this.deploymentService.createDeployment(createDto).subscribe({
        next: () => {
          this.submitting.set(false);
          this.successMessage.set('New deployment record published successfully.');
          setTimeout(() => this.router.navigate(['/deployments']), 800);
        },
        error: (err) => {
          this.submitting.set(false);
          this.errorMessage.set(err.error?.message || err.message || 'Creation failed.');
        }
      });
    }
  }
}
