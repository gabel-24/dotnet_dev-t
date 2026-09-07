import type { CandidateSummary } from "./candidate";
import type { JobPostingSummary } from "./jobPosting";
import type { InterviewStage } from "./interviewStage";

export interface JobApplication
{
    id: number;
    status: ApplicationStatus;
    appliedAt: string; // DateOnly → "YYYY-MM-DD"
    coverLetter: string;
    resumeSnapshotUrl: string;
    candidate: CandidateSummary;
    jobPosting: JobPostingSummary;
    interviewStages: InterviewStage[];
}

export const ApplicationStatus =
{
    Submitted: 0,
    Interview: 1,
    Offer: 2,
    Rejected: 3,
    Withdrawn: 4,
} as const;

export type ApplicationStatus = typeof ApplicationStatus[keyof typeof ApplicationStatus];

export const applicationStatusLabels: Record<ApplicationStatus, string> = {
  [ApplicationStatus.Submitted]: "Submitted",
  [ApplicationStatus.Interview]: "Interview",
  [ApplicationStatus.Offer]: "Offer",
  [ApplicationStatus.Rejected]: "Rejected",
  [ApplicationStatus.Withdrawn]: "Withdrawn",
};