export interface Candidate 
{
  id: number;
  username: string;
  email: string;
  headline: string;
  resumeUrl: string;
  skills: string[];
}

export interface CandidateSummary
{
  id: number;
  username: string;
  headline: string;
}

export interface UpdateCandidateDto
{
  username: string;
  headline: string;
  resumeUrl: string;
  skills: string[];
}