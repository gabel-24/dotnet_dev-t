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
        <div className="page">
            <div className="page-header">
                <h2>Browse Postings</h2>
            </div>

            <form onSubmit={handleFilterSubmit} className="filter-bar">
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
                <button type="submit" className="btn btn-primary">Search</button>
            </form>

            {loading && <p className="state-message">Loading postings...</p>}
            {error && <p className="state-message is-error">{error}</p>}
            {!loading && !error && postings.length === 0 && (
                <div className="empty-state">No postings match your search — try different filters.</div>
            )}

            <ul className="row-list">
                {postings.map((posting) => (
                    <li key={posting.id} className="row-item">
                        <div className="row-main">
                            <span className="row-title">{posting.title}</span>
                            <span className="row-meta">
                                {posting.recruiter?.companyName}
                                <span className="sep">·</span>
                                {posting.location}
                                <span className="sep">·</span>
                                {posting.employmentType}
                            </span>
                        </div>
                        <div className="row-side">
                            <button onClick={() => handleApply(posting.id)} className="btn btn-secondary">Apply</button>
                        </div>
                    </li>
                ))}
            </ul>
            <Pagination page={page} totalPages={totalPages} loading={loading} onChange={setPage} />
        </div>
    )
}


export default BrowsePostings;
