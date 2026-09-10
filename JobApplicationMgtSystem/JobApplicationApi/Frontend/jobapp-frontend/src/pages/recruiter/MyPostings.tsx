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
        <div>
            <h2>My Job Postings</h2>
            <Link to="/recruiter/postings/new">Create New Posting</Link>

            {loading && <p>Loading postings...</p>}
            {error && <p>{error}</p>}
            {!loading && !error && postings.length === 0 && (
                <p>You haven't posted any jobs yet.</p>
            )}

            <ul>
                {postings.map((posting) => (
                <li key={posting.id}>
                    <strong>{posting.title}</strong> — {posting.applicationCount} applicant
                    {posting.applicationCount !== 1 ? 's' : ''}
                    <br />
                    <Link to={`/recruiter/postings/${posting.id}/applications`}>View Applicants</Link>
                    {' | '}
                    <Link to={`/recruiter/postings/${posting.id}/edit`}>Edit</Link>
                    {' | '}
                    <button onClick={() => handleDelete(posting.id)}>Delete</button>
                </li>
                ))}
            </ul>
            <Pagination page={page} totalPages={totalPages} loading={loading} onChange={setPage} />
        </div>
    );

};

export default MyPostings;