import { useEffect, useState } from 'react';
import { getMyProfile, updateMyProfile } from '../../api/candidates';
import type { Candidate } from '../../types/candidate';

const CandidateProfile = () =>
{
    const [profile, setProfile] = useState<Candidate | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const [saving, setSaving] = useState(false);
    const [saveMessage, setSaveMessage] = useState('');
    const [saveSucceeded, setSaveSucceeded] = useState(false);

    const [username, setUsername] = useState('');
    const [headline, setHeadline] = useState('');
    const [resumeUrl, setResumeUrl] = useState('');
    const [skillsInput, setSkillsInput] = useState('');

    useEffect(() =>
    {
        const loadProfile = async () =>
        {
            setLoading(true)
            setError('')

            try
            {
                const data = await getMyProfile();
                setProfile(data);
                setUsername(data.username);
                setHeadline(data.headline);
                setResumeUrl(data.resumeUrl);
                setSkillsInput(data.skills.join(', '));
            }
            catch(err)
            {
                setError('failed to load profile. ');
            }
            finally
            {
                setLoading(false);
            }
        };

        loadProfile();

    }, []);

    const handleSubmit = async (e: React.FormEvent) =>
    {
        e.preventDefault();
        setSaving(true);
        setSaveMessage('');

        try
        {
            const skills = skillsInput
                .split(',')
                .map((s) => s.trim())
                .filter((s) => s.length > 0);

            await updateMyProfile({username, headline, resumeUrl, skills});
            setSaveSucceeded(true);
            setSaveMessage('profile updated successfully. ');
        }
        catch(err)
        {
            setSaveSucceeded(false);
            setSaveMessage('failed to update profile. ');
        }
        finally
        {
            setSaving(false);
        }
    };

    if (loading) return <p className="state-message">Loading profile...</p>;
    if (error) return <p className="state-message is-error">{error}</p>;
    if (!profile) return null;

    return (
        <div className="page page-narrow">
            <h2>My Profile</h2>
            {/* <p>Email: {profile.email}</p> */}

            <form onSubmit={handleSubmit} className="auth-form">
                <div className="field">
                    <label>Username</label>
                    <input value={username} onChange={(e) => setUsername(e.target.value)} />
                </div>

                <div className="field">
                    <label>Headline</label>
                    <input value={headline} onChange={(e) => setHeadline(e.target.value)} />
                </div>

                <div className="field">
                    <label>Resume URL</label>
                    <input value={resumeUrl} onChange={(e) => setResumeUrl(e.target.value)} />
                </div>

                <div className="field">
                    <label>Skills (comma-separated)</label>
                    <input value={skillsInput} onChange={(e) => setSkillsInput(e.target.value)} />
                </div>

                <button type="submit" className="btn btn-primary" disabled={saving}>
                    {saving ? 'Saving...' : 'Save Changes'}
                </button>

                {saveMessage && (
                    <p className={`form-note ${saveSucceeded ? 'is-success' : 'is-error'}`}>{saveMessage}</p>
                )}
            </form>
        </div>
    );

};


export default CandidateProfile;
