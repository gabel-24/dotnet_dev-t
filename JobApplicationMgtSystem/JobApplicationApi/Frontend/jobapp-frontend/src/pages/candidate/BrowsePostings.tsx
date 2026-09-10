import Pagination from "../../components/Pagination";
import { useEffect, useState } from "react"
import { getJobPostings } from "../../api/jobPostings"
import type { JobPosting } from "../../types/jobPosting"
import { useNavigate } from "react-router-dom"

const BrowsePostings = () =>
{
    const navigate = useNavigate();
    const [postings, setPostings] = useState<JobPosting[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const [page, setPage] = useState(1);
    const [totalPages, setTotalPages] = useState(0);

    const [filters, setFilters] = useState<{keyword?: string; location?: string; employmentType?: string}>({});
    const [keyword, setKeyword] = useState('');
    const [location, setLocation] = useState('');
    const [employmentType, setEmploymentType] = useState('');

    const loadPostings = async () =>
    {
        setLoading(true)
        setError('')

        try
        {
            const response = await getJobPostings(page, 10,
                filters);
            setPostings(response.items);
            setTotalPages(response.totalPages);
        }
        catch(err)
        {
            setError('Failed to load job postings.');
        }
        finally
        {
            setLoading(false);
        }
    };

    useEffect(() =>
    {
        loadPostings()
    }, [page, filters]);

    
    const handleFilterSubmit = (e: React.FormEvent) =>
    {
        e.preventDefault();
        setPage(1);
        setFilters({keyword: keyword || undefined, location: location || undefined, employmentType: employmentType || undefined});
    };

    const handleApply = (postingId: number) =>
    {
        navigate(`/candidate/postings/${postingId}/apply`);
    };

    return(
        <div>
            <h2>Browse Postings</h2>

            <form onSubmit={handleFilterSubmit}>
                <input
                placeholder="Keyword"
                value={keyword}
                onChange={(e) => setKeyword(e.target.value)}
                />
                <input
                placeholder="Location"
                value={location}
                onChange={(e) => setLocation(e.target.value)}
                />
                <input
                placeholder="Employment Type"
                value={employmentType}
                onChange={(e) => setEmploymentType(e.target.value)}
                />
                <button type="submit">Search</button>
            </form>

            {loading && <p>Loading postings...</p>}
            {error && <p>{error}</p>}
            {!loading && !error && postings.length === 0 && (
                <p>No postings match your search.</p>
            )}

            <ul>
                {postings.map((posting) => (
                <li key={posting.id}>
                    <strong>{posting.title}</strong> — {posting.recruiter?.companyName}
                    <br />
                    {posting.location} · {posting.employmentType}
                    <br />
                    <button onClick={() => handleApply(posting.id)}>Apply</button>
                </li>
                ))}
            </ul>
            <Pagination page={page} totalPages={totalPages} loading={loading} onChange={setPage} />
        </div>
    )
}


export default BrowsePostings;