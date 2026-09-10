import Pagination from "../../components/Pagination";
import { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { getApplicationsForPosting, updateApplicationStatus } from '../../api/jobApplications';
import { ApplicationStatus, applicationStatusLabels } from '../../types/jobApplication';
import type { JobApplicationSummary } from '../../types/jobApplication';

const PostingApplications = () =>
{
    const { id } = useParams<{ id: string }>();
    const [applications, setApplications] = useState<JobApplicationSummary[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const [page, setPage] = useState(1);
    const [totalPages, setTotalPages] = useState(0);

    const loadApplications = async () =>
    {
        if(!id) return;

        setLoading(true)
        setError('')

        try
        {
            const response = await getApplicationsForPosting(Number(id),page,10)
            setApplications(response.items);
            setTotalPages(response.totalPages);
        }
        catch(err)
        {
            setError('failed to load applications. ')
        }
        finally
        {
            setLoading(false)
        }
    };

    useEffect(()=>
    {
        loadApplications();
    }, [id, page]);

    const handleStatusChange = async (applicationId: number, newStatus: ApplicationStatus) =>
    {
        try
        {
            await updateApplicationStatus(applicationId, newStatus);

            setApplications((prev) =>
                prev.map((app) =>
                app.id == applicationId ? {...app, status: newStatus} : app)
            );
        }
        catch(err)
        {
            alert('failed to update status. ');
        }
    };

    return(
        <div className="page">
            <div className="page-header"><h2>Applicants{applications[0] ? ` for ${applications[0].jobTitle}` : ''}</h2></div>

            {loading && <p className="state-message">Loading applications...</p>}
            {error && <p className="state-message is-error">{error}</p>}
            {!loading && !error && applications.length === 0 && (
                <div className="empty-state">No applications yet for this posting.</div>
            )}

            <ul className="row-list">
                {applications.map((app) => (
                <li key={app.id} className="row-item recruiter-row">
                    <div className="row-main">
                        <span className="row-title">{app.candidateName}</span>
                        <span className="row-meta">Applied {app.appliedAt}</span>
                    </div>
                    <div className="row-side">
                    <div className="field">
                    <label htmlFor={`application-status-${app.id}`}>Status</label>
                    <select id={`application-status-${app.id}`}
                    value={app.status}
                    onChange={(e) =>
                        handleStatusChange(app.id, Number(e.target.value) as ApplicationStatus)
                    }
                    >
                    {Object.entries(applicationStatusLabels).map(([value, label]) => (
                        <option key={value} value={value}>
                        {label}
                        </option>
                    ))}
                    </select>
                    </div>
                    </div>
                </li>
                ))}
            </ul>
            <Pagination page={page} totalPages={totalPages} loading={loading} onChange={setPage} />
        </div>
    );

};

export default PostingApplications;