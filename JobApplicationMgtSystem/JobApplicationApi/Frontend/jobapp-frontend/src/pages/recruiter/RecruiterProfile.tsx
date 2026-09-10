import { useEffect, useState } from "react";
import { getMyRecruiterProfile,updateRecruiterProfile } from "../../api/recruters";
import type { RecruiterDto, UpdateRecruiterDto } from "../../types/recruiter";

const emptyForm: UpdateRecruiterDto =
{
    username: '',
    companyName: '',
}

const RecruiterProfile = () =>
{
    const [profile, setProfile] = useState<RecruiterDto | null>(null);
    const [form, setForm] = useState<UpdateRecruiterDto>(emptyForm);
    const [loading, setLoading] = useState(true);
    const [saving, setSaving] = useState(false);
    const [error, setError] = useState('');
    const [success, setSuccess] = useState(false);

    useEffect(() =>
    {
        const loadProfile = async () =>
        {
            setLoading(true);
            setError('');

            try
            {
                const data = await getMyRecruiterProfile();
                setProfile(data);

                setForm(
                    {
                        username: data.username,
                        companyName: data.companyName,
                    }
                );
            }
            catch(err)
            {
                setError('failed to load profile. ')
            }
            finally
            {
                setLoading(false)
            }
        };

        loadProfile();
    }, []);

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) =>
    {
        const {name, value} = e.target;

        setForm((prev) => ({...prev, [name]: value}));
    }

    const handleSubmit = async (e: React.FormEvent) =>
    {
        e.preventDefault()
        setSaving(true)
        setError('')
        setSuccess(false)

        try
        {
            await updateRecruiterProfile(form)
            setSuccess(true)
        }
        catch(err)
        {
            setError('failed to update profile. ')
        }
        finally
        {
            setSaving(false)
        }
    }

    if(loading) return <p className="state-message">Loading.....</p>
    if(!profile && error) return <p className="state-message is-error">{error}</p>

    return(
        <div className="page page-narrow">
            <h2>Recruiter Profile</h2>
            {profile && <p className="muted">Email: {profile.email}</p>}

            {error && <p className="state-message is-error">{error}</p>}
            {success && <p className="form-note is-success">Profile updated.</p>}

            <form onSubmit={handleSubmit} className="auth-form">
                <div className="field">
                    <label htmlFor="recruiter-username">Username</label>
                    <input id="recruiter-username"
                    name="username"
                    value={form.username}
                    onChange={handleChange}
                    required
                />
                </div>
                <div className="field">
                    <label htmlFor="recruiter-companyName">Company Name</label>
                    <input id="recruiter-companyName"
                    name="companyName"
                    value={form.companyName}
                    onChange={handleChange}
                />
                </div>
                <button className="btn btn-primary" type="submit" disabled={saving}>
                {saving ? 'Saving...' : 'Update Profile'}
                </button>
            </form>
        </div>
    );

}


export default RecruiterProfile;