import { useEffect, useState } from "react"
import { getJobPostings } from "../../api/jobPostings"
import type { JobPosting } from "../../types/jobPosting"

const BrowsePostings = () =>
{
    const [postings, setPostings] = useState<JobPosting[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');

    const [keyword, setKeyword] = useState('');
    const [location, setLocation] = useState('');
    const [employmentType, setEmploymentType] = useState('');

    const loadPostings = async () =>
    {
        setLoading(true)
        setError('')

        try
        {
            const response = await getJobPostings(1, 10,
                {
                    keyword: keyword || undefined,
                    location: location || undefined,
                    employmentType: employmentType || undefined,
                });
            setPostings(response.items);
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
    }, []);

    const handleFilterSubmit = (e: React.FormEvent) =>
    {
        e.preventDefault();
        loadPostings();
    };

    const handleApply = (postingId: number) =>
    {
        //TODO wireup once createApplications exists
        alert('Apply clicked for posting ${postingId} (not yet implemented)');
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
        </div>
    )
}


export default BrowsePostings;