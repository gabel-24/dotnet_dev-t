import axiosClient from "./axiosClient";
import type { ApplicationStatus, JobApplication, JobApplicationSummary } from "../types/jobApplication";
import type { PagedResponse } from "../types/pagedResponse";


export const getMyApplications = async (
  pageNumber = 1,
  pageSize = 10
): Promise<PagedResponse<JobApplicationSummary>> => {
  const response = await axiosClient.get<PagedResponse<JobApplicationSummary>>(
    "/candidates/me/applications",
    { params: { pageNumber, pageSize } }
  );
  return response.data;
};

export const getApplicationsForPosting = async (
  jobPostingId: number,
  pageNumber = 1,
  pageSize = 10
): Promise<PagedResponse<JobApplicationSummary>> =>
{
  const response = await axiosClient.get<PagedResponse<JobApplicationSummary>>(`/jobpostings/${jobPostingId}/applications`, { params: {pageNumber, pageSize}});
  return response.data;
};

export const updateApplicationStatus = async (
  applicationId: number,
  status: ApplicationStatus
): Promise<void> => {
  await axiosClient.put(`/jobapplications/${applicationId}/edit`, { status });
};

export const createApplication = async (
  jobPostingId: number,
  coverLetter: string
): Promise<JobApplication> => {
  const response = await axiosClient.post<JobApplication>(
    `/jobpostings/${jobPostingId}/applications`,
    { jobPostingId, coverLetter }
  );
  return response.data;
};