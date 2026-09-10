import { useEffect, useState } from "react";
import { getMyApplications} from "../../api/jobApplications";
import { getJobPostings } from "../../api/jobPostings";
import type { JobApplicationSummary} from "../../types/jobApplication";
import type { JobPosting } from "../../types/jobPosting";
import { applicationStatusLabels } from "../../types/jobApplication";

const CandidateDashboard = () => {
    const [applications, setApplications] = useState<JobApplicationSummary[]>([]);
    const [openPostings, setOpenPostings] = useState<JobPosting[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const loadDashboard = async () => {
            try {
                const [applicationsRes, postingsRes] = await Promise.all([
                    getMyApplications(),
                    getJobPostings(),
                ]);
                setApplications(applicationsRes.items);
                setOpenPostings(postingsRes.items.filter((p) => p.isActive));
            } catch (err) {
                setError("Failed to load dashboard data.");
            } finally {
                setLoading(false);
            }
        };

        loadDashboard();
    }, []);

    if (loading) return <p className="state-message">Loading dashboard...</p>;
    if (error) return <p className="state-message is-error">{error}</p>;

    return (
        <div className="page">
            <section className="section">
                <h2>My Applications</h2>
                {applications.length === 0 ? (
                    <div className="empty-state">You haven't applied to anything yet — browse open roles.</div>
                ) : (
                    <ul className="row-list">
                        {applications.map((app) => (
                            <li key={app.id} className="row-item">
                                <div className="row-main">
                                    <span className="row-title">{app.jobTitle}</span>
                                </div>
                                <div className="row-side">
                                    <span className="status-pill" data-status={app.status}>
                                        {applicationStatusLabels[app.status]}
                                    </span>
                                </div>
                            </li>
                        ))}
                    </ul>
                )}
            </section>

            <section className="section">
                <h2>Recommended Jobs</h2>
                {openPostings.length === 0 ? (
                    <div className="empty-state">No open postings right now — check back soon.</div>
                ) : (
                    <ul className="row-list">
                        {openPostings.map((posting) => (
                            <li key={posting.id} className="row-item">
                                <div className="row-main">
                                    <span className="row-title">{posting.title}</span>
                                    <span className="row-meta">{posting.location}</span>
                                </div>
                            </li>
                        ))}
                    </ul>
                )}
            </section>
        </div>
    );
};

export default CandidateDashboard;
