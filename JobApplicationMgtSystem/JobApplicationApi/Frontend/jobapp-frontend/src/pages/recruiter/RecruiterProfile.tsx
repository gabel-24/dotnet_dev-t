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

    if(loading) return <p>Loading.....</p>
    if(!profile && error) return <p>{error}</p>

    return(
        <div>
            <h2>Recruiter Profile</h2>
            {profile && <p>Email: {profile.email}</p>}

            {error && <p>{error}</p>}
            {success && <p>Profile updated.</p>}

            <form onSubmit={handleSubmit}>
                <label>
                Username
                <input
                    name="username"
                    value={form.username}
                    onChange={handleChange}
                    required
                />
                </label>
                <br />
                <label>
                Company Name
                <input
                    name="companyName"
                    value={form.companyName}
                    onChange={handleChange}
                />
                </label>
                <br />
                <button type="submit" disabled={saving}>
                {saving ? 'Saving...' : 'Update Profile'}
                </button>
            </form>
        </div>
    );

}


export default RecruiterProfile;