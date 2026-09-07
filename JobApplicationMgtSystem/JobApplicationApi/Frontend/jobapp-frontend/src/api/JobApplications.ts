import axiosClient from "./axiosClient";
import type { JobApplication } from "../types/jobApplication";
import type { PagedResponse } from "../types/pagedResponse";

export const getMyApplications = async (
  pageNumber = 1,
  pageSize = 10
): Promise<PagedResponse<JobApplication>> => {
  const response = await axiosClient.get<PagedResponse<JobApplication>>(
    "/candidates/me/applications",
    { params: { pageNumber, pageSize } }
  );
  return response.data;
};