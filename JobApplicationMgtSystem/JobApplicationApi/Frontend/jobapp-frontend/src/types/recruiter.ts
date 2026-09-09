export interface RecruiterDto 
{
  id: number;
  username: string;
  email: string;
  companyName: string;
}

export interface RecruiterSummary 
{
  username: string;
  companyName: string;
}

export interface UpdateRecruiterDto 
{
  username: string;
  companyName: string;
}