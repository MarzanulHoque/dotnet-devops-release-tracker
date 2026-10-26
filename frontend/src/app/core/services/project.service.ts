import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { Project } from '../models/project.model';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ProjectService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/projects`;

  readonly projects = signal<Project[]>([]);
  readonly loading = signal<boolean>(false);

  loadProjects(): Observable<Project[]> {
    this.loading.set(true);
    return this.http.get<Project[]>(this.baseUrl).pipe(
      tap({
        next: (list) => {
          this.projects.set(list);
          this.loading.set(false);
        },
        error: () => {
          this.loading.set(false);
        }
      })
    );
  }

  getProjectById(id: number): Observable<Project> {
    return this.http.get<Project>(`${this.baseUrl}/${id}`);
  }
}
