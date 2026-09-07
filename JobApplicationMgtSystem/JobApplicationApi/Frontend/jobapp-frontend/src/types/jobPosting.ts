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
  closingDate: string; // DateOnly → "YYYY-MM-DD"
  isActive: boolean;
  recruiter: RecruiterSummary;
  applicationCount: number;
}

export interface JobPostingSummary
{
    title: string;
    location: string;
    employmentType: string;
    salaryMin: number;
    salaryMax: number;
    closingDate: string; // DateOnly → "YYYY-MM-DD"
    isActive: boolean;
}