export interface DashboardMetrics {
  totalDeployments: number;
  successRate: number;
  successfulCount: number;
  failedCount: number;
  inProgressCount: number;
  productionCount: number;
  stagingCount: number;
  devCount: number;
  avgDurationSeconds: number;
}
