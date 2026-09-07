export interface InterviewStage 
{
  id: number;
  stageName: string;
  type: InterviewType;
  location: string;
  interviewer: string;
  status: InterviewStatus;
  scheduledAt: string; // DateOnly → "YYYY-MM-DD"
  notes: string;
  isComplete: boolean;
}

export const InterviewType =
{
  Phone: 0,
  Video: 1,
  OnSite: 2,
  Technical: 3,
  Behavioral: 4,
  Final: 5,
} as const;

export type InterviewType = typeof InterviewType[keyof typeof InterviewType];

export const interviewTypeLabels: Record<InterviewType, string> = 
{
    [InterviewType.Phone]: "Phone",
    [InterviewType.Video]: "Video",
    [InterviewType.OnSite]: "On-Site",
    [InterviewType.Technical]: "Technical",
    [InterviewType.Behavioral]: "Behavioral",
    [InterviewType.Final]: "Final",
}



export const InterviewStatus = 
{
  Scheduled: 0,
  Completed: 1,
  Cancelled: 2,
  Rescheduled: 3,
  Passed: 4,
  Failed: 5,
} as const;

export type InterviewStatus = typeof InterviewStatus[keyof typeof InterviewStatus];

export const interviewStatusLabels: Record<InterviewStatus, string> = 
{
    [InterviewStatus.Scheduled]: "Scheduled",
    [InterviewStatus.Completed]: "Completed",
    [InterviewStatus.Cancelled]: "Cancelled",
    [InterviewStatus.Rescheduled]: "Rescheduled",
    [InterviewStatus.Passed]: "Passed",
    [InterviewStatus.Failed]: "Failed",
}