interface Props {
  page: number;
  totalPages: number;
  loading: boolean;
  onChange: (page: number) => void;
}

export default function Pagination({page, totalPages, loading, onChange}: Props) {
  if (totalPages < 2 && page === 1) return null;
  return <nav aria-label="Pagination">
    <button disabled={loading || page <= 1} onClick={() => onChange(page - 1)}>Previous</button>
    <span> Page {page} of {Math.max(page, totalPages)} </span>
    <button disabled={loading || page >= totalPages} onClick={() => onChange(page + 1)}>Next</button>
  </nav>;
}
