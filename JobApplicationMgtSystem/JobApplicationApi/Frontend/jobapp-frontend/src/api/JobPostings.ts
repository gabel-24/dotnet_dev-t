import axiosClient from "./axiosClient";
import type { JobPosting, JobPostingSummary } from "../types/jobPosting";
import type { PagedResponse } from "../types/pagedResponse";

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