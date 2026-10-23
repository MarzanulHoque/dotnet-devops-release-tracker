import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { DeploymentService } from './deployment.service';
import { Deployment } from '../models/deployment.model';
import { environment } from '../../../environments/environment';

describe('DeploymentService', () => {
  let service: DeploymentService;
  let httpTesting: HttpTestingController;

  const mockDeployments: Deployment[] = [
    {
      id: 1,
      appName: 'Payment Gateway',
      version: 'v2.4.0',
      environment: 'Production',
      status: 'Successful',
      deployedBy: 'Platform Engineer',
      commitHash: 'a1b2c3d',
      deployedAt: '2026-10-23T10:00:00Z',
      executionDurationSeconds: 45
    },
    {
      id: 2,
      appName: 'Order Processing',
      version: 'v1.8.2',
      environment: 'Staging',
      status: 'Failed',
      deployedBy: 'CI/CD Bot',
      commitHash: 'e5f6g7h',
      deployedAt: '2026-10-23T11:00:00Z',
      executionDurationSeconds: 30
    }
  ];

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        DeploymentService,
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    });
    service = TestBed.inject(DeploymentService);
    httpTesting = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpTesting.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should load deployments and compute accurate metrics', () => {
    service.loadDeployments().subscribe((data) => {
      expect(data.length).toBe(2);
    });

    const req = httpTesting.expectOne(`${environment.apiBaseUrl}/deployments`);
    expect(req.request.method).toBe('GET');
    req.flush(mockDeployments);

    expect(service.deployments().length).toBe(2);

    const metrics = service.metrics();
    expect(metrics.totalDeployments).toBe(2);
    expect(metrics.successfulCount).toBe(1);
    expect(metrics.failedCount).toBe(1);
    expect(metrics.successRate).toBe(50);
    expect(metrics.productionCount).toBe(1);
    expect(metrics.stagingCount).toBe(1);
  });

  it('should correctly identify recent deployments sorted by date', () => {
    service.deployments.set(mockDeployments);
    const recent = service.recentDeployments();
    expect(recent.length).toBe(2);
    expect(recent[0].id).toBe(2); // DeployedAt 11:00 is newer than 10:00
  });
});
