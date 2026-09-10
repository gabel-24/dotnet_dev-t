import Pagination from "../../components/Pagination";
import { useEffect, useState } from 'react';
import { getMyApplications } from '../../api/jobApplications';
import type { JobApplicationSummary } from '../../types/jobApplication';
import { applicationStatusLabels } from '../../types/jobApplication';

const MyApplications = () =>
{
    const [applications, setApplications] = useState<JobApplicationSummary[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const [page, setPage] = useState(1);
    const [totalPages, setTotalPages] = useState(0);

    useEffect(() =>
    {
        const loadApplications = async () =>
        {
            setLoading(true);
            setError('')
            try
            {
                const response = await getMyApplications(page,10);
                setApplications(response.items);
                setTotalPages(response.totalPages);
            }
            catch(err)
            {
                setError('Failed to load applications. ');
            }
            finally
            {
                setLoading(false);
            }
        };

        loadApplications();
    }, [page]);

    return(
        <div className="page">
            <div className="page-header">
                <h2>My Applications</h2>
            </div>

            {loading && <p className="state-message">Loading applications...</p>}
            {error && <p className="state-message is-error">{error}</p>}
            {!loading && !error && applications.length === 0 && (
                <div className="empty-state">You haven't applied to anything yet — browse open roles.</div>
            )}

            <ul className="row-list">
                {applications.map((app) => (
                    <li key={app.id} className="row-item">
                        <div className="row-main">
                            <span className="row-title">{app.jobTitle}</span>
                            <span className="row-meta">Applied {app.appliedAt}</span>
                        </div>
                        <div className="row-side">
                            <span className="status-pill" data-status={app.status}>
                                {applicationStatusLabels[app.status]}
                            </span>
                        </div>
                    </li>
                ))}
            </ul>
            <Pagination page={page} totalPages={totalPages} loading={loading} onChange={setPage} />
        </div>
    );
};


export default MyApplications;
