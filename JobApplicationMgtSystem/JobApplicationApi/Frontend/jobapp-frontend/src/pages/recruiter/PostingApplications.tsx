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

    const loadApplications = async () =>
    {
        if(!id) return;

        setLoading(true)
        setError('')

        try
        {
            const response = await getApplicationsForPosting(Number(id),1,10)
            setApplications(response.items)
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
    }, [id]);

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
        <div>
            <h2>Applicants{applications[0] ? ` for ${applications[0].jobTitle}` : ''}</h2>

            {loading && <p>Loading applications...</p>}
            {error && <p>{error}</p>}
            {!loading && !error && applications.length === 0 && (
                <p>No applications yet for this posting.</p>
            )}

            <ul>
                {applications.map((app) => (
                <li key={app.id}>
                    <strong>{app.candidateName}</strong> — applied {app.appliedAt}
                    <br />
                    <select
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
                </li>
                ))}
            </ul>
        </div>
    );

};

export default PostingApplications;