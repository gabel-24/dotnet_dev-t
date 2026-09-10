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
        <div>
            <h2>My Applications</h2>

            {loading && <p>Loading applications...</p>}
            {error && <p>{error}</p>}
            {!loading && !error && applications.length === 0 && (
                <p>You haven't applied to anything yet.</p>
            )}

            <ul>
                {applications.map((app) => (
                <li key={app.id}>
                    <strong>{app.jobTitle}</strong>
                    <br />
                    Status: {applicationStatusLabels[app.status]}
                    <br />
                    Applied: {app.appliedAt}
                </li>
                ))}
            </ul>
            <Pagination page={page} totalPages={totalPages} loading={loading} onChange={setPage} />
        </div>
    );
};


export default MyApplications;