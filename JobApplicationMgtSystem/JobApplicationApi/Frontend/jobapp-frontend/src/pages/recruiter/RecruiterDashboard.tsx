import { useEffect, useState } from "react";
import { getMyPostings } from "../../api/jobPostings";
import type { JobPostingSummary } from "../../types/jobPosting";

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

    if(loading) return <p className="state-message">Loading Dashboard.....</p>
    if(error) return <p className="state-message is-error">{error}</p>

    return (
        <div className="page">
            <section className="section">
            <h2>My Job Postings</h2>
            {postings.length === 0 ? (
                <div className="empty-state">You haven't posted any jobs yet.</div>
            ) : (
                <ul className="row-list">
                {postings.map((posting) => (
                    <li key={posting.id} className="row-item">
                    <div className="row-main">
                        <span className="row-title">{posting.title}</span>
                    </div>
                    <div className="row-side">
                        <span className="row-meta">{posting.applicationCount} applicant{posting.applicationCount !== 1 ? "s" : ""}</span>
                    </div>
                    </li>
                ))}
                </ul>
            )}
            </section>
        </div>
    );
}

export default RecruiterDashboard;