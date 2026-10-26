export interface DeploymentLog {
  id: number;
  deploymentRecordId: number;
  stepName: string;
  logLevel: string;
  message: string;
  timestamp: string;
}

export interface Deployment {
  id: number;
  projectId?: number | null;
  projectName?: string | null;
  appName: string;
  version: string;
  environment: string; // 'Production' | 'Staging' | 'Development'
  status: string; // 'Successful' | 'Failed' | 'In-Progress'
  deployedBy: string;
  commitHash: string;
  deployedAt: string;
  executionDurationSeconds: number;
  notes?: string | null;
  logCount?: number;
}

export interface DeploymentDetail extends Deployment {
  logs: DeploymentLog[];
}

export interface CreateDeploymentDto {
  projectId?: number | null;
  appName: string;
  version: string;
  environment: string;
  status: string;
  deployedBy?: string;
  commitHash?: string;
  executionDurationSeconds: number;
  notes?: string | null;
}

export interface UpdateDeploymentDto {
  status: string;
  appName?: string;
  version?: string;
  environment?: string;
  deployedBy?: string;
  commitHash?: string;
  executionDurationSeconds?: number;
  notes?: string | null;
}
