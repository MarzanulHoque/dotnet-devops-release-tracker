export interface DatabaseHealth {
  provider: string;
  connected: boolean;
  message: string;
}

export interface HealthResponse {
  status: string; // 'Healthy' | 'Degraded' | 'Unhealthy'
  timestamp: string;
  service: string;
  version: string;
  environment: string;
  uptime: string;
  hostname: string;
  database: DatabaseHealth;
  responseTimeMs?: number;
}
