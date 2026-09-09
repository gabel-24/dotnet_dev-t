import axiosClient from "./axiosClient";
import type { RecruiterDto, UpdateRecruiterDto } from "../types/recruiter";

export const getMyRecruiterProfile = async (): Promise <RecruiterDto> =>
{
    const response = await axiosClient.get<RecruiterDto>('/recruiters/me');
    return response.data;
}

export const updateRecruiterProfile = async (data: UpdateRecruiterDto): Promise<void> =>
{
    await axiosClient.put('recruiters/me', data);
}