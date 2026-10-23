import { Injectable, computed, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap, catchError, of } from 'rxjs';
import { HealthResponse } from '../models/health.model';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class HealthService {
  private readonly http = inject(HttpClient);
  private readonly healthUrl = `${environment.apiBaseUrl}/health`;

  readonly health = signal<HealthResponse | null>(null);
  readonly checking = signal<boolean>(false);
  readonly lastChecked = signal<Date | null>(null);

  readonly isApiOnline = computed<boolean>(() => {
    const h = this.health();
    return h !== null && (h.status === 'Healthy' || h.status === 'Degraded');
  });

  readonly isDatabaseConnected = computed<boolean>(() => {
    return this.health()?.database.connected ?? false;
  });

  checkHealth(): Observable<HealthResponse | null> {
    this.checking.set(true);
    const start = performance.now();

    return this.http.get<HealthResponse>(this.healthUrl).pipe(
      tap((res) => {
        const duration = Math.round(performance.now() - start);
        this.health.set({ ...res, responseTimeMs: duration });
        this.lastChecked.set(new Date());
        this.checking.set(false);
      }),
      catchError((err) => {
        const duration = Math.round(performance.now() - start);
        this.health.set({
          status: 'Unhealthy',
          timestamp: new Date().toISOString(),
          service: 'DotnetProject.Api',
          version: 'Offline',
          environment: 'N/A',
          uptime: '0s',
          hostname: 'unreachable',
          database: {
            provider: 'Unknown',
            connected: false,
            message: err.message || 'API is unreachable'
          },
          responseTimeMs: duration
        });
        this.lastChecked.set(new Date());
        this.checking.set(false);
        return of(null);
      })
    );
  }
}
