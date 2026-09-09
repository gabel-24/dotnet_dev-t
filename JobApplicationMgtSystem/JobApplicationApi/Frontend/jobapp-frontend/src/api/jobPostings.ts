import axiosClient from "./axiosClient";
import type { JobPosting, JobPostingSummary } from "../types/jobPosting";
import type { PagedResponse } from "../types/pagedResponse";
import type { UpdateJobPostingDto } from "../types/jobPosting";
import type { CreateJobPostingDto  } from "../types/jobPosting";


export const getJobPostings = async (
  pageNumber = 1,
  pageSize = 10,
  filters?: { location?: string; employmentType?: string; keyword?: string }
): Promise<PagedResponse<JobPosting>> => {
  const response = await axiosClient.get<PagedResponse<JobPosting>>(
    "/jobpostings",
    { params: { pageNumber, pageSize, ...filters } }
  );
  return response.data;
};

export const getMyPostings = async (
  pageNumber = 1,
  pageSize = 10
): Promise<PagedResponse<JobPostingSummary>> => {
  const response = await axiosClient.get<PagedResponse<JobPostingSummary>>(
    "/recruiters/me/postings",
    { params: { pageNumber, pageSize } }
  );
  return response.data;
};

export const updateJobPosting = async (
  id: number,
  data: UpdateJobPostingDto
): Promise<void> => {
  await axiosClient.put(`/jobpostings/${id}`, data);
};

export const deleteJobPosting = async (id: number): Promise<void> => {
  await axiosClient.delete(`/jobpostings/${id}`);
};

export const createJobPosting = async (data: CreateJobPostingDto): Promise<JobPosting> => 
{
  const response = await axiosClient.post<JobPosting>('/jobpostings', data);
  return response.data;
};

export const getJobPostingById = async (id: number): Promise<JobPosting> =>
{
  const response = await axiosClient.get<JobPosting>(`/jobpostings/${id}`);
  return response.data;
}