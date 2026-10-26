import { Injectable, computed, inject, signal } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, tap, catchError, throwError } from 'rxjs';
import { Deployment, DeploymentDetail, CreateDeploymentDto, UpdateDeploymentDto, DeploymentLog } from '../models/deployment.model';
import { DashboardMetrics } from '../models/dashboard-stats.model';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class DeploymentService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/deployments`;

  // Reactive State via Signals
  readonly deployments = signal<Deployment[]>([]);
  readonly loading = signal<boolean>(false);
  readonly error = signal<string | null>(null);

  // Computed Telemetry Metrics
  readonly metrics = computed<DashboardMetrics>(() => {
    const list = this.deployments();
    const total = list.length;
    if (total === 0) {
      return {
        totalDeployments: 0,
        successRate: 0,
        successfulCount: 0,
        failedCount: 0,
        inProgressCount: 0,
        productionCount: 0,
        stagingCount: 0,
        devCount: 0,
        avgDurationSeconds: 0
      };
    }

    const successful = list.filter(d => d.status.toLowerCase() === 'successful' || d.status.toLowerCase() === 'success').length;
    const failed = list.filter(d => d.status.toLowerCase() === 'failed' || d.status.toLowerCase() === 'error').length;
    const inProgress = list.filter(d => d.status.toLowerCase() === 'in-progress' || d.status.toLowerCase() === 'inprogress' || d.status.toLowerCase() === 'running').length;

    const production = list.filter(d => d.environment.toLowerCase() === 'production').length;
    const staging = list.filter(d => d.environment.toLowerCase() === 'staging').length;
    const dev = list.filter(d => d.environment.toLowerCase() === 'development' || d.environment.toLowerCase() === 'dev').length;

    const totalDuration = list.reduce((acc, curr) => acc + (curr.executionDurationSeconds || 0), 0);
    const avgDuration = Math.round(totalDuration / total);
    const rate = Math.round((successful / total) * 100);

    return {
      totalDeployments: total,
      successRate: rate,
      successfulCount: successful,
      failedCount: failed,
      inProgressCount: inProgress,
      productionCount: production,
      stagingCount: staging,
      devCount: dev,
      avgDurationSeconds: avgDuration
    };
  });

  // Recent releases (Top 5)
  readonly recentDeployments = computed<Deployment[]>(() => {
    return [...this.deployments()]
      .sort((a, b) => new Date(b.deployedAt).getTime() - new Date(a.deployedAt).getTime())
      .slice(0, 5);
  });

  loadDeployments(filters?: { environment?: string; status?: string; projectId?: number; search?: string }): Observable<Deployment[]> {
    this.loading.set(true);
    this.error.set(null);

    let params = new HttpParams();
    if (filters?.environment) params = params.set('environment', filters.environment);
    if (filters?.status) params = params.set('status', filters.status);
    if (filters?.projectId) params = params.set('projectId', filters.projectId.toString());
    if (filters?.search) params = params.set('search', filters.search);

    return this.http.get<Deployment[]>(this.baseUrl, { params }).pipe(
      tap({
        next: (data) => {
          this.deployments.set(data);
          this.loading.set(false);
        },
        error: (err) => {
          this.error.set(err.message || 'Failed to load deployments');
          this.loading.set(false);
        }
      }),
      catchError((err) => throwError(() => err))
    );
  }

  getDeploymentById(id: number): Observable<DeploymentDetail> {
    return this.http.get<DeploymentDetail>(`${this.baseUrl}/${id}`);
  }

  createDeployment(dto: CreateDeploymentDto): Observable<Deployment> {
    return this.http.post<Deployment>(this.baseUrl, dto).pipe(
      tap((newRecord) => {
        this.deployments.update((current) => [newRecord, ...current]);
      })
    );
  }

  updateDeployment(id: number, dto: UpdateDeploymentDto): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, dto).pipe(
      tap(() => {
        this.deployments.update((current) =>
          current.map((item) =>
            item.id === id
              ? {
                  ...item,
                  status: dto.status,
                  appName: dto.appName ?? item.appName,
                  version: dto.version ?? item.version,
                  environment: dto.environment ?? item.environment,
                  deployedBy: dto.deployedBy ?? item.deployedBy,
                  commitHash: dto.commitHash ?? item.commitHash,
                  notes: dto.notes !== undefined ? dto.notes : item.notes,
                  executionDurationSeconds: dto.executionDurationSeconds ?? item.executionDurationSeconds
                }
              : item
          )
        );
      })
    );
  }

  deleteDeployment(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`).pipe(
      tap(() => {
        this.deployments.update((current) => current.filter((item) => item.id !== id));
      })
    );
  }

  addLog(deploymentId: number, logDto: { stepName: string; logLevel: string; message: string }): Observable<DeploymentLog> {
    return this.http.post<DeploymentLog>(`${this.baseUrl}/${deploymentId}/logs`, logDto);
  }
}
