export interface Project {
  id: number;
  name: string;
  slug: string;
  repositoryUrl?: string | null;
  owner: string;
  techStack: string;
  createdAt: string;
  totalDeployments: number;
}
