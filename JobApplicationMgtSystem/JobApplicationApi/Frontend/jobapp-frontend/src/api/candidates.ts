import axiosClient from "./axiosClient";
import type { Candidate } from "../types/candidate";
import type { UpdateCandidateDto } from "../types/candidate";


export const getMyProfile = async (): Promise<Candidate> =>
{
    const response = await axiosClient.get<Candidate>('/candidates/me');
    return response.data;
};

export const updateMyProfile = async (data: UpdateCandidateDto): Promise<void> =>
{
    await axiosClient.put('/candidates/me', data);
};