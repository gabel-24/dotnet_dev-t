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
                        closingDate: posting.closingDate,
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
                const updateData: UpdateJobPostingDto = {... form};
                await updateJobPosting(Number(id), updateData);
            }
            else
            {
                const { isActive, ... createData} = form;
                const createDto: CreateJobPostingDto = createData;
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

    if(loading) return <p>Loading posting....</p>

    return(
        <div>
            <h2>{isEditMode ? 'Edit Posting' : 'Create New Posting'}</h2>
            {error && <p>{error}</p>}
            <form onSubmit={handleSubmit}>
                <label>
                    Title
                    <input name='title' value ={form.title} onChange={handleChange} required />
                </label>
                <br />
                <label>
                    Description
                    <textarea name = "description" value={form.description} onChange={handleChange} required />
                </label>
                <br />
                <label>
                    Location
                    <input name ='location' value={form.location} onChange={handleChange} />
                </label>
                <br />
                <label>
                    Employment Type
                    <input name='employmentType' value={form.employmentType} onChange={handleChange} />
                </label>
                <br />
                <label>
                    Salary Min
                    <input type="number" name="salaryMin" value={form.salaryMin} onChange={handleChange} />
                </label>
                <br />
                <label>
                    Salary Max
                    <input type="number" name="salaryMax" value={form.salaryMax} onChange={handleChange} />
                </label>
                <br />
                <label>
                    Closing Date
                    <input type="date" name="closingDate" value={form.closingDate} onChange={handleChange} />
                </label>
                <br />
                {isEditMode && (
                    <label>
                        Active
                        <input type="checkbox" name="isActive" checked={form.isActive} onChange={handleChange} />
                    </label>
                )}
                <br />
                <button type='submit' disabled = {saving}>
                    {saving ? 'Saving.....' : isEditMode ? 'Updating Posting' : 'Create Posting'}
                </button>
            </form>
        </div>
    );

};


export default CreateandEditPostings;