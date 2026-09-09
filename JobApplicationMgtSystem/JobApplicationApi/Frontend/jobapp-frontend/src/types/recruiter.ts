export interface RecruiterDto 
{
  id: number;
  username: string;
  email: string;
  companyName: string;
  department: string;
}

export interface RecruiterSummary 
{
  username: string;
  companyName: string;
  department: string;
}

export interface UpdateRecruiterDto 
{
  username: string;
  companyName: string;
  department: string;
}