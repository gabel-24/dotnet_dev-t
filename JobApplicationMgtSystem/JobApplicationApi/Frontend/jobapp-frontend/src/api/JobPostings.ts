import axiosClient from "./axiosClient";
import type { JobPosting } from "../types/jobPosting";
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