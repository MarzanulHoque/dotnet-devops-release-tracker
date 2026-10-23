import { Routes } from '@angular/router';
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { DeploymentListComponent } from './features/deployments/deployment-list/deployment-list.component';
import { DeploymentFormComponent } from './features/deployments/deployment-form/deployment-form.component';
import { DeploymentDetailComponent } from './features/deployments/deployment-detail/deployment-detail.component';
import { DiagnosticsComponent } from './features/diagnostics/diagnostics.component';

export const routes: Routes = [
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
  { path: 'dashboard', component: DashboardComponent },
  { path: 'deployments', component: DeploymentListComponent },
  { path: 'deployments/new', component: DeploymentFormComponent },
  { path: 'deployments/:id', component: DeploymentDetailComponent },
  { path: 'deployments/:id/edit', component: DeploymentFormComponent },
  { path: 'diagnostics', component: DiagnosticsComponent }
];
