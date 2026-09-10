import Pagination from "../../components/Pagination";
import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { getMyPostings } from '../../api/jobPostings';
import { deleteJobPosting } from '../../api/jobPostings';
import type { JobPostingSummary } from '../../types/jobPosting';

const MyPostings = () =>
{
    const [postings, setPostings] = useState<JobPostingSummary[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const [page, setPage] = useState(1);
    const [totalPages, setTotalPages] = useState(0);

    const loadPostings = async () =>
    {
        setLoading(true);
        setError('');

        try
        {
            const response = await getMyPostings(page,10);
            setPostings(response.items);
            setTotalPages(response.totalPages);
        }
        catch(err)
        {
            setError('failed to load postings');
        }
        finally
        {
            setLoading(false);
        }
    };

    useEffect(() =>
    {
        loadPostings();
    }, [page]);

    const handleDelete = async (id: number) =>
    {
        const confirmed = window.confirm('Delete this job posting? This cannot be undone.');
        if (!confirmed) return;

        try
        {
            await deleteJobPosting(id);
            if (postings.length === 1 && page > 1) setPage(page - 1);
            else await loadPostings();
        }
        catch(err)
        {
            alert('failed to delete job posting. ');
        }
    };

    return(
        <div className="page">
            <div className="page-header recruiter-page-header">
                <h2>My Job Postings</h2>
                <Link className="btn btn-primary" to="/recruiter/postings/new">Create New Posting</Link>
            </div>

            {loading && <p className="state-message">Loading postings...</p>}
            {error && <p className="state-message is-error">{error}</p>}
            {!loading && !error && postings.length === 0 && (
                <div className="empty-state">You haven't posted any jobs yet.</div>
            )}

            <ul className="row-list">
                {postings.map((posting) => (
                <li key={posting.id} className="row-item recruiter-row">
                    <div className="row-main">
                        <span className="row-title">{posting.title}</span>
                        <span className="row-meta">{posting.applicationCount} applicant{posting.applicationCount !== 1 ? 's' : ''}</span>
                    </div>
                    <div className="row-side recruiter-actions">
                        <Link className="btn btn-secondary" to={`/recruiter/postings/${posting.id}/applications`}>View Applicants</Link>
                        <Link className="btn btn-secondary" to={`/recruiter/postings/${posting.id}/edit`}>Edit</Link>
                        <button className="btn btn-secondary btn-danger" onClick={() => handleDelete(posting.id)}>Delete</button>
                    </div>
                </li>
                ))}
            </ul>
            <Pagination page={page} totalPages={totalPages} loading={loading} onChange={setPage} />
        </div>
    );

};

export default MyPostings;