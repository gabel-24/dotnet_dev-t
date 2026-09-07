import { useEffect, useState } from "react";
import { getMyPostings } from "../api/JobPostings";
import type { JobPostingSummary } from "../types/jobPosting";

const RecruiterDashboard = () =>
{
    const [postings, setPostings] = useState<JobPostingSummary[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() =>
    {
        const loadPostings = async () =>
        {
            try
            {
                const result = await getMyPostings();
                setPostings(result.items);
            }
            catch(err)
            {
                setError("Failed to load job postings.");
            }
            finally
            {
                setLoading(false);
            }
        };

        loadPostings();
    }, []);

    if(loading) return <p>Loading Dashboard.....</p>
    if(error) return <p>{error}</p>

    return (
        <div>
            <h2>My Job Postings</h2>
            {postings.length === 0 ? (
                <p>You haven't posted any jobs yet.</p>
            ) : (
                <ul>
                {postings.map((posting) => (
                    <li key={posting.id}>
                    {posting.title} — {posting.applicationCount} applicant
                    {posting.applicationCount !== 1 ? "s" : ""}
                    </li>
                ))}
                </ul>
            )}
        </div>
    );
}

export default RecruiterDashboard;