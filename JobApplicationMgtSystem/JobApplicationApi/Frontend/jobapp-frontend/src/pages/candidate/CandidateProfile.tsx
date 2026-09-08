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
            setSaveMessage('profile updated successfully. ');
        }
        catch(err)
        {
            setSaveMessage('failed to update profile. ');
        }
        finally
        {
            setSaving(false);
        }
    };

    if (loading) return <p>Loading profile...</p>;
    if (error) return <p>{error}</p>;
    if (!profile) return null;

    return (
        <div>
            <h2>My Profile</h2>
            {/* <p>Email: {profile.email}</p> */}

            <form onSubmit={handleSubmit}>
                <div>
                <label>Username</label>
                <input value={username} onChange={(e) => setUsername(e.target.value)} />
                </div>

                <div>
                <label>Headline</label>
                <input value={headline} onChange={(e) => setHeadline(e.target.value)} />
                </div>

                <div>
                <label>Resume URL</label>
                <input value={resumeUrl} onChange={(e) => setResumeUrl(e.target.value)} />
                </div>

                <div>
                <label>Skills (comma-separated)</label>
                <input value={skillsInput} onChange={(e) => setSkillsInput(e.target.value)} />
                </div>

                <button type="submit" disabled={saving}>
                {saving ? 'Saving...' : 'Save Changes'}
                </button>

                {saveMessage && <p>{saveMessage}</p>}
            </form>
        </div>
    );

};


export default CandidateProfile;