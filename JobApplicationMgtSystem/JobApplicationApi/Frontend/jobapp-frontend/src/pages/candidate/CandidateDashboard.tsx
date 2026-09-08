import { useEffect, useState } from "react";
import { getMyApplications} from "../../api/jobApplications";
import { getJobPostings } from "../../api/jobPostings";
import type { JobApplication } from "../../types/jobApplication";
import type { JobPosting } from "../../types/jobPosting";
import { applicationStatusLabels } from "../../types/jobApplication";

const CandidateDashboard = () => {
  const [applications, setApplications] = useState<JobApplication[]>([]);
  const [openPostings, setOpenPostings] = useState<JobPosting[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const loadDashboard = async () => {
      try {
        const [applicationsRes, postingsRes] = await Promise.all([
          getMyApplications(),
          getJobPostings(),
        ]);
        setApplications(applicationsRes.items);
        setOpenPostings(postingsRes.items.filter((p) => p.isActive));
      } catch (err) {
        setError("Failed to load dashboard data.");
      } finally {
        setLoading(false);
      }
    };

    loadDashboard();
  }, []);

  if (loading) return <p>Loading dashboard...</p>;
  if (error) return <p>{error}</p>;

  return (
    <div>
      <section>
        <h2>My Applications</h2>
        {applications.length === 0 ? (
          <p>You haven't applied to anything yet.</p>
        ) : (
          <ul>
            {applications.map((app) => (
              <li key={app.id}>
                {app.jobPosting.title} — {applicationStatusLabels[app.status]}
              </li>
            ))}
          </ul>
        )}
      </section>

      <section>
        <h2>Recommended Jobs</h2>
        {openPostings.length === 0 ? (
          <p>No open postings right now.</p>
        ) : (
          <ul>
            {openPostings.map((posting) => (
              <li key={posting.id}>
                {posting.title} — {posting.location}
              </li>
            ))}
          </ul>
        )}
      </section>
    </div>
  );
};

export default CandidateDashboard;