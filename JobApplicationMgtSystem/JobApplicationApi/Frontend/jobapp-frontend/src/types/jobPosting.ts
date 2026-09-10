import type { RecruiterSummary } from "./recruiter";

export interface JobPosting 
{
  id: number;
  title: string;
  description: string;
  location: string;
  employmentType: string;
  salaryMin: number;
  salaryMax: number;
  closingDate: string | null; // DateOnly → "YYYY-MM-DD"
  isActive: boolean;
  recruiter: RecruiterSummary;
  applicationCount: number;
}

export interface JobPostingSummary
{
    id: number;
    title: string;
    location: string;
    employmentType: string;
    salaryMin: number;
    salaryMax: number;
    closingDate: string | null; // DateOnly → "YYYY-MM-DD"
    isActive: boolean;
    applicationCount: number;
}

export interface UpdateJobPostingDto 
{
  title: string;
  description: string;
  location: string;
  employmentType: string;
  salaryMin: number;
  salaryMax: number;
  closingDate: string | null; // DateOnly → "YYYY-MM-DD"
  isActive: boolean;
}

export interface CreateJobPostingDto 
{
  title: string;
  description: string;
  location: string;
  employmentType: string;
  salaryMin: number;
  salaryMax: number;
  closingDate: string | null; // ISO date string, e.g. "2026-12-31"
}