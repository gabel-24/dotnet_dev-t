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
        <div className="page page-narrow">
            <h2>Apply to Posting</h2>
            {error && <p className="state-message is-error">{error}</p>}
            <form onSubmit={handleSubmit} className="auth-form">
                <div className="field">
                    <label>Cover Letter</label>
                    <textarea
                        value={coverLetter}
                        onChange={(e) => setCoverLetter(e.target.value)}
                        rows={8}
                    />
                </div>
                <div className="form-actions">
                    <button type="submit" className="btn btn-primary" disabled={submitting}>
                        {submitting ? 'Submitting...' : 'Submit Application'}
                    </button>
                    <button type="button" className="btn btn-secondary" onClick={() => navigate('/candidate/postings')} disabled={submitting}>
                        Cancel
                    </button>
                </div>
            </form>
        </div>
    )

}

export default CreateApplications
