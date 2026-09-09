import { useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { createApplication } from '../../api/jobApplications';

const CreateApplications = () =>
{
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();

    const [coverLetter, setCoverLetter] = useState('');
    const [submitting, setSubmitting] = useState(false);
    const [error, setError] = useState('');

    const handleSubmit = async (e: React.FormEvent) =>
    {
        e.preventDefault()
        if(!id) return;

        setSubmitting(true)
        setError('')

        try
        {
            await createApplication(Number(id), coverLetter)
            navigate('/candidate/applications')
        }
        catch(err)
        {
            setError('failed to submit application. ')
        }
        finally
        {
            setSubmitting(false)
        }

    }

    return (
        <div>
            <h2>Apply to Posting</h2>
            {error && <p>{error}</p>}
            <form onSubmit={handleSubmit}>
                <label>
                Cover Letter
                <textarea
                    value={coverLetter}
                    onChange={(e) => setCoverLetter(e.target.value)}
                    rows={8}
                />
                </label>
                <br />
                <button type="submit" disabled={submitting}>
                {submitting ? 'Submitting...' : 'Submit Application'}
                </button>
                <button type="button" onClick={() => navigate('/candidate/postings')} disabled={submitting}>
                Cancel
                </button>
            </form>
        </div>
    )

}

export default CreateApplications