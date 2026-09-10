import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { createJobPosting, getJobPostingById, updateJobPosting } from '../../api/jobPostings';
import type { CreateJobPostingDto, UpdateJobPostingDto } from '../../types/jobPosting';

const emptyForm: CreateJobPostingDto & { isActive: boolean} = 
{
    title: '',
    description: '',
    location: '',
    employmentType: '',
    salaryMin: 0,
    salaryMax: 0,
    closingDate: '',
    isActive: true,
};

const CreateandEditPostings = () =>
{
    const {id} = useParams<{id: string}>();
    const isEditMode = Boolean(id);
    const navigate = useNavigate();

    const [form, setForm] = useState(emptyForm);
    const [loading, setLoading] = useState(isEditMode);
    const [saving, setSaving] = useState(false);
    const [error, setError] = useState('');

    useEffect(() =>
    {
        if(!isEditMode || !id) return;

        const loadPosting = async () =>
        {
            setLoading(true);
            setError('');

            try
            {
                const posting = await getJobPostingById(Number(id));

                setForm(
                    {
                        title: posting.title,
                        description: posting.description,
                        location: posting.location,
                        employmentType: posting.employmentType,
                        salaryMin: posting.salaryMin,
                        salaryMax: posting.salaryMax,
                        closingDate: posting.closingDate ?? '',
                        isActive: posting.isActive,
                    }
                );
            }
            catch(err)
            {
                setError('failed to load posting. ')
            }
            finally
            {
                setLoading(false);
            }
        };

        loadPosting();

    },[id, isEditMode]);

    const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) =>
    {
        const {name, value, type} = e.target;

        setForm((prev) =>(
            {
                ...prev,
                [name]:
                type === 'number'
                ? Number(value)
                : type === 'checkbox'
                ? (e.target as HTMLInputElement).checked
                : value,
            }
        ));
    };

    const handleSubmit = async(e: React.FormEvent) =>
    {
        e.preventDefault();
        setSaving(true);
        setError('')

        try
        {
            if(isEditMode && id)
            {
                const updateData: UpdateJobPostingDto = {...form, closingDate: form.closingDate || null};
                await updateJobPosting(Number(id), updateData);
            }
            else
            {
                const { isActive, ... createData} = form;
                const createDto: CreateJobPostingDto = {...createData, closingDate: createData.closingDate || null};
                await createJobPosting(createDto);
            }

            navigate('/recruiter/postings');
        }
        catch(err)
        {
            setError('failed to save posting. ');
        }
        finally
        {
            setSaving(false);
        }
    };

    if(loading) return <p className="state-message">Loading posting....</p>

    return(
        <div className="page page-narrow">
            <h2>{isEditMode ? 'Edit Posting' : 'Create New Posting'}</h2>
            {error && <p className="state-message is-error">{error}</p>}
            <form onSubmit={handleSubmit} className="auth-form">
                <div className="field">
                    <label htmlFor="recruiter-title">Title</label>
                    <input id="recruiter-title" name='title' value ={form.title} onChange={handleChange} required />
                </div>
                <div className="field">
                    <label htmlFor="recruiter-description">Description</label>
                    <textarea id="recruiter-description" name = "description" value={form.description} onChange={handleChange} required />
                </div>
                <div className="field">
                    <label htmlFor="recruiter-location">Location</label>
                    <input id="recruiter-location" name ='location' value={form.location} onChange={handleChange} />
                </div>
                <div className="field">
                    <label htmlFor="recruiter-employmentType">Employment Type</label>
                    <input id="recruiter-employmentType" name='employmentType' value={form.employmentType} onChange={handleChange} />
                </div>
                <div className="field">
                    <label htmlFor="recruiter-salaryMin">Salary Min</label>
                    <input id="recruiter-salaryMin" type="number" name="salaryMin" value={form.salaryMin} onChange={handleChange} />
                </div>
                <div className="field">
                    <label htmlFor="recruiter-salaryMax">Salary Max</label>
                    <input id="recruiter-salaryMax" type="number" name="salaryMax" value={form.salaryMax} onChange={handleChange} />
                </div>
                <div className="field">
                    <label htmlFor="recruiter-closingDate">Closing Date</label>
                    <input id="recruiter-closingDate" type="date" name="closingDate" value={form.closingDate ?? ""} onChange={handleChange} />
                </div>
                {isEditMode && (
                    <div className="field field-checkbox">
                        <input id="recruiter-isActive" type="checkbox" name="isActive" checked={form.isActive} onChange={handleChange} />
                        <label htmlFor="recruiter-isActive">Active</label>
                    </div>
                )}
                <button className="btn btn-primary" type='submit' disabled = {saving}>
                    {saving ? 'Saving.....' : isEditMode ? 'Updating Posting' : 'Create Posting'}
                </button>
            </form>
        </div>
    );

};


export default CreateandEditPostings;