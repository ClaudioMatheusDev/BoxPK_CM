export function MetricCard({ label, value, detail, tone }) {
  return (
    <article className={`metric${tone ? ` metric-${tone}` : ''}`}>
      <span className="metric-label">{label}</span>
      <strong className="metric-value">{value}</strong>
      {detail ? <small className="metric-detail">{detail}</small> : null}
    </article>
  )
}
