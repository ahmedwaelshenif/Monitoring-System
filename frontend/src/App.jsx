import { useCallback, useEffect, useMemo, useRef, useState } from 'react'

const API_KEY = 'monitoring-admin-api-base'
const navItems = [
  ['overview', 'Overview', 'layout-dashboard'], ['labs', 'Labs', 'building'], ['logs', 'Entry logs', 'door-open'],
  ['visitors', 'Visitors', 'user-round'], ['accounts', 'Accounts', 'users-round'], ['requests', 'Registration requests', 'clipboard-check'], ['warnings', 'Warnings', 'triangle-alert'],
]
function readRoute() {
  const route = window.location.hash.slice(1)
  const lab = route.match(/^labs\/(\d+)$/)
  return { page: lab ? 'lab-detail' : navItems.some(([id]) => id === route) ? route : 'overview', labId: lab ? Number(lab[1]) : null }
}
function Icon({ name, size = 18, strokeWidth = 1.8, className = '' }) {
  const base = { fill: 'none', stroke: 'currentColor', strokeWidth, strokeLinecap: 'round', strokeLinejoin: 'round' }
  const shapes = {
    'layout-dashboard': <><rect x="3" y="3" width="8" height="8" rx="1.5"/><rect x="13" y="3" width="8" height="5" rx="1.5"/><rect x="13" y="10" width="8" height="11" rx="1.5"/><rect x="3" y="13" width="8" height="8" rx="1.5"/></>,
    map: <><path d="m3 6 6-3 6 3 6-3v15l-6 3-6-3-6 3z"/><path d="M9 3v15m6-12v15"/></>,
    network: <><circle cx="12" cy="12" r="3"/><circle cx="5" cy="5" r="2"/><circle cx="19" cy="5" r="2"/><circle cx="5" cy="19" r="2"/><circle cx="19" cy="19" r="2"/><path d="m10 10-3.5-3.5m7.5 3.5 3.5-3.5m-7.5 7.5L6.5 17m7.5-3 3.5 3"/></>,
    'flask-conical': <><path d="M9 3h6M10 3v6l-5.5 9.2A2 2 0 0 0 6.2 21h11.6a2 2 0 0 0 1.7-2.8L14 9V3"/><path d="M8 15h8"/></>,
    'door-open': <><path d="M4 21h16M6 21V4a1 1 0 0 1 1-1h10a1 1 0 0 1 1 1v17"/><path d="M14 12h.01"/></>,
    'user-round': <><circle cx="12" cy="8" r="4"/><path d="M5 21v-1a7 7 0 0 1 14 0v1"/></>,
    'users-round': <><path d="M16 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"/><circle cx="10" cy="7" r="4"/><path d="M20 21v-2a4 4 0 0 0-3-3.87M16 3.13a4 4 0 0 1 0 7.75"/></>,
    'clipboard-check': <><rect x="5" y="4" width="14" height="17" rx="2"/><path d="M9 4.5h6a1 1 0 0 0-1-1h-4a1 1 0 0 0-1 1ZM8 13l2.5 2.5L16 10"/></>,
    'triangle-alert': <><path d="m10.3 4.3-8 14A2 2 0 0 0 4 21h16a2 2 0 0 0 1.7-2.7l-8-14a2 2 0 0 0-3.4 0Z"/><path d="M12 9v4m0 4h.01"/></>,
    'activity': <><path d="M3 12h4l3-8 4 16 3-8h4"/></>,
    'brand-eye': <><path d="M4 8V6a2 2 0 0 1 2-2h2M16 4h2a2 2 0 0 1 2 2v2M20 16v2a2 2 0 0 1-2 2h-2M8 20H6a2 2 0 0 1-2-2v-2"/><path d="M5.5 12S8.2 7.5 12 7.5 18.5 12 18.5 12 15.8 16.5 12 16.5 5.5 12 5.5 12Z"/><circle cx="12" cy="12" r="2.2"/></>,
    'check': <><path d="m5 12 4 4L19 6"/></>,
    'building': <><rect x="4" y="3" width="16" height="18" rx="2"/><path d="M9 21v-4h6v4M8 7h2m4 0h2M8 11h2m4 0h2"/></>,
    'arrow-up-right': <><path d="M7 17 17 7M7 7h10v10"/></>,
    'arrow-right': <><path d="M5 12h14m-6-6 6 6-6 6"/></>,
    'plus': <><path d="M12 5v14M5 12h14"/></>,
    'close': <><path d="m18 6-12 12M6 6l12 12"/></>,
    'trash': <><path d="M3 6h18m-2 0-.9 14H5.9L5 6m4 0V4h6v2m-5 4v6m4-6v6"/></>,
    'edit': <><path d="M12 20h9"/><path d="M16.5 3.5a2.12 2.12 0 0 1 3 3L8 18l-4 1 1-4Z"/></>,
    'clock': <><circle cx="12" cy="12" r="9"/><path d="M12 7v5l3 2"/></>,
    'calendar-days': <><rect x="3" y="5" width="18" height="16" rx="2"/><path d="M16 3v4M8 3v4M3 10h18M8 14h.01M12 14h.01M16 14h.01M8 17h.01M12 17h.01"/></>,
    'shield-check': <><path d="M12 22s8-4 8-11V5l-8-3-8 3v6c0 7 8 11 8 11Z"/><path d="m9 12 2 2 4-4"/></>,
    'refresh': <><path d="M20 7v5h-5M4 17v-5h5"/><path d="M5.6 9A7 7 0 0 1 17 6l3 6M4 12l3 6a7 7 0 0 0 11.4-3"/></>,
    'download': <><path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4"/><path d="m7 10 5 5 5-5M12 15V3"/></>,
    instagram: <><rect x="3" y="3" width="18" height="18" rx="5"/><circle cx="12" cy="12" r="4"/><path d="M17.5 6.5h.01"/></>,
    facebook: <path fill="currentColor" stroke="none" d="M13.6 21v-8h2.7l.4-3.2h-3.1v-2c0-.9.3-1.5 1.6-1.5h1.7V3.4c-.3 0-1.4-.1-2.6-.1-2.6 0-4.4 1.6-4.4 4.5v2H7v3.2h2.9v8h3.7Z"/>,
    linkedin: <><path d="M4 3h16a1 1 0 0 1 1 1v16a1 1 0 0 1-1 1H4a1 1 0 0 1-1-1V4a1 1 0 0 1 1-1Z"/><path d="M7 10v7m0-10v.01M11 17v-7m0 3a3 3 0 0 1 6 0v4"/></>,
  }
  return <svg className={`ui-icon ${className}`} width={size} height={size} viewBox="0 0 24 24" aria-hidden="true" {...base}>{shapes[name] || shapes.activity}</svg>
}
const localDateKey = (value) => `${value.getFullYear()}-${String(value.getMonth() + 1).padStart(2, '0')}-${String(value.getDate()).padStart(2, '0')}`
const nextDateStart = (dateKey) => { const value = new Date(`${dateKey}T12:00:00`); value.setDate(value.getDate() + 1); return `${localDateKey(value)}T00:00:00` }
const dateText = (value) => value ? new Date(value).toLocaleString() : '—'
const enumName = (value, choices) => choices[value] ?? String(value ?? '—')
const alertName = value => enumName(value, ['Food or drink detected', 'Crowded'])
const getMemberId = record => record?.memberId ?? record?.MemberId ?? record?.memberID ?? record?.MemberID ?? null
function buildMemberIdsByVisitorId(activeItems = [], inactiveItems = []) {
  const byId = new Map()
  for (const visitor of [...activeItems, ...inactiveItems]) {
    const id = visitor.id ?? visitor.Id
    if (id != null) byId.set(String(id), getMemberId(visitor))
  }
  return byId
}
const resolveEntryLogMemberId = (log, memberIdsByVisitorId) => getMemberId(log) ?? memberIdsByVisitorId.get(String(log?.visitorId ?? log?.VisitorId)) ?? null

async function api(base, path, options = {}) {
  const root = base.trim().replace(/\/$/, '')
  const localAddress = /^https?:\/\/(localhost|127\.0\.0\.1)(:\d+)?$/i.test(root)
  const useDevProxy = import.meta.env.DEV && (!root || localAddress)
  const requestRoot = useDevProxy ? '' : root
  let response
  try {
    response = await fetch(`${requestRoot}${path}`, options)
  } catch (error) {
    const target = useDevProxy ? (import.meta.env.VITE_API_PROXY_TARGET || 'https://localhost:7062') : (root || window.location.origin)
    throw new Error(`Could not reach the API at ${target}. Start the backend with its HTTPS profile, or set VITE_API_PROXY_TARGET to the backend address. ${error.message}`)
  }
  if (!response.ok) {
    const detail = await response.text()
    throw new Error(`${response.status} ${response.statusText}${detail ? `: ${detail}` : ''}`)
  }
  if (response.status === 204) return null
  const body = await response.text()
  if (!body) return null
  const contentType = response.headers.get('content-type') || ''
  if (/^\s*<!doctype html|^\s*<html/i.test(body)) {
    throw new Error(`The API request to ${path} returned HTML instead of JSON. Check the API address. When using Vite locally, make sure the backend is running at ${import.meta.env.VITE_API_PROXY_TARGET || 'https://localhost:7062'}.`)
  }
  if (!contentType.includes('json')) return body
  try { return JSON.parse(body) }
  catch { throw new Error(`The API returned invalid JSON for ${path}. Check that the API address points to the backend.`) }
}

async function downloadFile(base, path, defaultFilename = 'download.xlsx') {
  const root = base.trim().replace(/\/$/, '')
  const localAddress = /^https?:\/\/(localhost|127\.0\.0\.1)(:\d+)?$/i.test(root)
  const useDevProxy = import.meta.env.DEV && (!root || localAddress)
  const requestRoot = useDevProxy ? '' : root
  const response = await fetch(`${requestRoot}${path}`)
  if (!response.ok) {
    const detail = await response.text()
    throw new Error(`${response.status} ${response.statusText}${detail ? `: ${detail}` : ''}`)
  }
  const blob = await response.blob()
  const url = window.URL.createObjectURL(blob)
  const a = document.createElement('a')
  a.href = url
  a.download = defaultFilename
  document.body.appendChild(a)
  a.click()
  a.remove()
  window.URL.revokeObjectURL(url)
}

function App() {
  const initialRoute = readRoute()
  const [page, setPage] = useState(initialRoute.page)
  const [labDetailId, setLabDetailId] = useState(initialRoute.labId)
  const apiBase = localStorage.getItem(API_KEY) || ''
  const [refresh, setRefresh] = useState(0)
  const [notice, setNotice] = useState(null)
  const [modal, setModal] = useState(null)
  const [confirmation, setConfirmation] = useState(null)
  const [selectedLab, setSelectedLab] = useState('')
  const [logsPreset, setLogsPreset] = useState('all')

  const request = useCallback((path, options) => api(apiBase, path, options), [apiBase])
  const goToPage = useCallback((nextPage) => {
    setPage(nextPage)
    setLabDetailId(null)
    if (window.location.hash !== `#${nextPage}`) window.history.pushState({ page: nextPage }, '', `#${nextPage}`)
  }, [])
  const openLabDetails = useCallback((labId) => {
    setLabDetailId(Number(labId))
    setPage('lab-detail')
    const route = `#labs/${labId}`
    if (window.location.hash !== route) window.history.pushState({ page: 'lab-detail', labId }, '', route)
  }, [])
  useEffect(() => {
    const syncPage = () => {
      const route = readRoute()
      setPage(route.page)
      setLabDetailId(route.labId)
    }
    window.addEventListener('popstate', syncPage)
    window.addEventListener('hashchange', syncPage)
    return () => { window.removeEventListener('popstate', syncPage); window.removeEventListener('hashchange', syncPage) }
  }, [])
  const notify = (message, error = false) => {
    setNotice({ message, error })
    window.setTimeout(() => setNotice(null), 5000)
  }
  const askConfirm = (options, action) => setConfirmation({ options, action })
  const reload = () => setRefresh(x => x + 1)
  return <div className="app-shell">
    <main className="main-area">
      <header className="topbar">
        <nav className="primary-nav" aria-label="Main navigation">{navItems.map(([id, label, icon]) => <button key={id} className={`nav-item ${page === id ? 'active' : ''}`} aria-current={page === id ? 'page' : undefined} onClick={() => { goToPage(id); if (id === 'logs') setLogsPreset('all') }}><Icon name={icon} className="nav-icon" /><span>{label}</span></button>)}</nav>
      </header>
      <div className="content">
        {notice && <div className={`notice ${notice.error ? 'notice-error' : ''}`}>{notice.message}<button aria-label="Dismiss" onClick={() => setNotice(null)}><Icon name="close" size={16} /></button></div>}
        {page === 'overview' && <><Overview request={request} refresh={refresh} setPage={goToPage} setLogsPreset={setLogsPreset} setSelectedLab={setSelectedLab} /><CampusPulse request={request} refresh={refresh} openLabDetails={openLabDetails} goToPage={goToPage} /></>}
        {page === 'labs' && <Labs request={request} refresh={refresh} notify={notify} askConfirm={askConfirm} setSelectedLab={setSelectedLab} goToPage={goToPage} openModal={setModal} openLabDetails={openLabDetails} />}
        {page === 'lab-detail' && <LabDetails labId={labDetailId} request={request} refresh={refresh} goBack={() => goToPage('labs')} goToPage={goToPage} setSelectedLab={setSelectedLab} />}
        {page === 'logs' && <Logs request={request} apiBase={apiBase} refresh={refresh} selectedLab={selectedLab} setSelectedLab={setSelectedLab} preset={logsPreset} setPreset={setLogsPreset} askConfirm={askConfirm} notify={notify} />}
        {page === 'visitors' && <Visitors request={request} refresh={refresh} notify={notify} askConfirm={askConfirm} />}
        {page === 'accounts' && <Accounts request={request} refresh={refresh} notify={notify} askConfirm={askConfirm} />}
        {page === 'requests' && <Requests request={request} refresh={refresh} notify={notify} askConfirm={askConfirm} />}
        {page === 'warnings' && <Warnings request={request} refresh={refresh} notify={notify} askConfirm={askConfirm} />}
      </div>
    </main>
    <footer className="site-footer">
      <div className="footer-inner">
        <div className="footer-brand"><span className="footer-mark"><Icon name="brand-eye" size={19} /></span><span><strong>MONITOR</strong><small>CONTROL CENTER</small></span></div>
        <p>Monitoring System · Operations and access management</p>
      </div>
      <div className="footer-bottom">© {new Date().getFullYear()} Monitoring System</div>
    </footer>
    {modal && <LabModal item={modal === 'new' ? null : modal} onClose={() => setModal(null)} request={request} onSaved={() => { setModal(null); reload(); notify(modal === 'new' ? 'Lab created.' : 'Lab updated.') }} onError={e => notify(e.message, true)} />}
    {confirmation && <ConfirmDialog options={confirmation.options} onClose={() => setConfirmation(null)} onConfirm={async () => { await confirmation.action(); setConfirmation(null) }} />}
  </div>
}

function useLoad(loader, deps) {
  const [state, setState] = useState({ data: null, loading: true, error: '' })
  const requestVersion = useRef(0)
  const refresh = useCallback(async () => {
    const version = ++requestVersion.current
    setState(s => ({ ...s, loading: true, error: '' }))
    try {
      const data = await loader()
      if (version === requestVersion.current)
        setState({ data, loading: false, error: '' })
    } catch (e) {
      if (version === requestVersion.current)
        setState({ data: null, loading: false, error: e.message })
    }
  // loader is stabilized by the caller and deps list describes its inputs.
  // eslint-disable-next-line react-hooks/exhaustive-deps
  }, deps)
  useEffect(() => {
    refresh()
    return () => { requestVersion.current += 1 }
  }, [refresh])
  return { ...state, refresh }
}

function PageTitle({ eyebrow, title, description, action }) {
  return <div className="page-heading"><div><div className="eyebrow">{eyebrow}</div><h1>{title}</h1><p>{description}</p></div>{action}</div>
}
function Panel({ title, subtitle, action, children, className = '' }) {
  return <section className={`panel ${className}`}><div className="panel-heading"><div><h2>{title}</h2>{subtitle && <p>{subtitle}</p>}</div>{action}</div>{children}</section>
}
function ErrorBox({ message, retry }) { return <div className="error-box"><span>{message}</span>{retry && <button className="button button-light" onClick={retry}>Try again</button>}</div> }
function Empty({ children = 'No records found.' }) { return <div className="empty-state">{children}</div> }
function Pager({ page, total, onPage }) { const pages = Math.max(1, total); return <div className="pager"><span>Page {page} of {pages}</span><div><button className="button button-light" disabled={page <= 1} onClick={() => onPage(page - 1)}>Previous</button><button className="button button-light" disabled={page >= pages} onClick={() => onPage(page + 1)}>Next</button></div></div> }
function Table({ headers, children }) { return <div className="table-wrap"><table><thead><tr>{headers.map(h => <th key={h}>{h}</th>)}</tr></thead><tbody>{children}</tbody></table></div> }
function Status({ value, kind = 'log' }) {
  const name = kind === 'request' ? enumName(value, ['Pending', 'Rejected', 'Accepted']) : enumName(value, ['Rejected', 'Accepted'])
  return <span className={`status status-${name.toLowerCase()}`}>{name}</span>
}
function FilterBar({ fields, values, onChange, onSubmit, onClear }) {
  return <form className="filters-bar" onSubmit={onSubmit}><div className="filters-grid">{fields.map(field => <label key={field.name}>{field.label}{field.options
    ? <select value={values[field.name] ?? ''} onChange={e => onChange(field.name, e.target.value)}><option value="">Any</option>{field.options.map(([value, label]) => <option key={value} value={value}>{label}</option>)}</select>
    : <input type={field.type || 'text'} min={field.min} value={values[field.name] ?? ''} placeholder={field.placeholder || ''} onChange={e => onChange(field.name, e.target.value)} />}</label>)}</div><div className="filter-actions"><button type="button" className="button button-light" onClick={onClear}>Clear</button><button type="submit" className="button button-primary">Apply filters</button></div></form>
}
function RecordDetails({ title, record, onClose, fields, images = [], actions }) {
  if (!record) return null
  return <div className="modal-backdrop" onMouseDown={e => e.target === e.currentTarget && onClose()}><section className="modal detail-modal" role="dialog" aria-modal="true" aria-label={title}><div className="modal-heading"><div><span className="eyebrow">RECORD DETAILS</span><h2>{title}</h2></div><button className="icon-button" aria-label="Close" onClick={onClose}><Icon name="close" size={16} /></button></div><div className="detail-grid">{fields.map(([label, value]) => <div className="detail-item" key={label}><small>{label}</small><strong>{value ?? '—'}</strong></div>)}</div>{images.length > 0 && <div className="detail-images">{images.map(([label, data]) => <div className="detail-image" key={label}><strong>{label}</strong>{data ? <img src={imageSrc(data)} alt={label} /> : <span className="image-empty">No image available</span>}</div>)}</div>}<div className="modal-actions">{actions}<button className="button button-light" onClick={onClose}>Close</button></div></section></div>
}
function ConfirmDialog({ options, onClose, onConfirm }) {
  const [busy, setBusy] = useState(false)
  async function confirm() { setBusy(true); try { await onConfirm() } finally { setBusy(false) } }
  return <div className="modal-backdrop confirm-backdrop" onMouseDown={e => e.target === e.currentTarget && !busy && onClose()}><section className="modal confirm-modal" role="alertdialog" aria-modal="true" aria-labelledby="confirm-title"><div className="confirm-icon">{options.icon || '!'}</div><h2 id="confirm-title">{options.title}</h2><p>{options.message}</p><div className="modal-actions"><button className="button button-light" onClick={onClose} disabled={busy}>Cancel</button><button className={`button ${options.danger ? 'button-danger' : 'button-primary'}`} onClick={confirm} disabled={busy}>{busy ? 'Please wait…' : options.confirmText || 'Confirm'}</button></div></section></div>
}
function imageSrc(data) { return typeof data === 'string' && data.startsWith('data:') ? data : `data:image/jpeg;base64,${data}` }

function Overview({ request, refresh, setPage, setLogsPreset, setSelectedLab }) {
  const labs = useLoad(() => request('/lab/count'), [request, refresh])
  const dateRange = useMemo(() => Array.from({ length: 7 }, (_, i) => { const d = new Date(); d.setDate(d.getDate() - (6 - i)); return d }), [])
  const [daily, setDaily] = useState({ data: null, loading: true, error: '' })
  useEffect(() => {
    let active = true
    setDaily({ data: null, loading: true, error: '' })
    Promise.all(dateRange.map(async date => {
      const day = localDateKey(date)
      const [accepted, rejected] = await Promise.all([
        request(`/entryLog/count/accepted?date=${day}`), request(`/entryLog/count/rejected?date=${day}`),
      ])
      return { day, accepted: Number(accepted), rejected: Number(rejected) }
    })).then(data => active && setDaily({ data, loading: false, error: '' })).catch(e => active && setDaily({ data: null, loading: false, error: e.message }))
    return () => { active = false }
  }, [request, refresh, dateRange])
  const todayCounts = daily.data?.at(-1)
  const weekTotal = daily.data?.reduce((n, x) => n + x.accepted + x.rejected, 0) ?? 0
  const todayTotal = (todayCounts?.accepted ?? 0) + (todayCounts?.rejected ?? 0)
  const acceptedRate = todayTotal ? (todayCounts.accepted / todayTotal) * 100 : 0
  return <>
    <section className="overview-hero">
      <div className="overview-copy"><h1>Your operations,<br /><em>in clear view.</em></h1></div>
      <div className="hero-art" aria-hidden="true"><div className="hero-orbit orbit-one" /><div className="hero-orbit orbit-two" /><div className="hero-orbit orbit-three" /><div className="hero-center"><Icon name="brand-eye" size={37} /></div><div className="hero-node node-one"><Icon name="shield-check" size={16} /></div><div className="hero-node node-two"><Icon name="building" size={16} /></div><div className="hero-node node-three"><Icon name="door-open" size={16} /></div><div className="hero-caption">ACCESS<br /><b>MONITORING</b></div></div>
    </section>
    {(labs.error || daily.error) && <ErrorBox message={labs.error || daily.error} />}
    <div className="stats-grid">
      <StatCard title="Total labs" value={labs.data ?? '—'} hint="Labs in the system" icon="building" />
      <StatCard title="Accepted today" value={todayCounts?.accepted ?? (daily.loading ? '…' : '—')} hint="Successful entries · view logs" icon="shield-check" tone="green" onClick={() => { setSelectedLab(''); setLogsPreset('accepted'); setPage('logs') }} />
      <StatCard title="Rejected today" value={todayCounts?.rejected ?? (daily.loading ? '…' : '—')} hint="Denied entries · view logs" icon="triangle-alert" tone="red" onClick={() => { setSelectedLab(''); setLogsPreset('rejected'); setPage('logs') }} />
      <StatCard title="Entries this week" value={daily.loading ? '…' : weekTotal} hint="Accepted and rejected" icon="activity" tone="blue" />
    </div>
    <div className="dashboard-grid">
      <Panel title="Entry activity" subtitle="Daily entry totals for the last 7 days"><WeekChart data={daily.data} loading={daily.loading} /><div className="chart-legend"><span><i className="legend-dot accepted" />Accepted</span><span><i className="legend-dot rejected" />Rejected</span><small>Hover a day to see its counts</small></div></Panel>
      <Panel title="Today at a glance" subtitle={new Date().toLocaleDateString(undefined, { weekday: 'long', month: 'long', day: 'numeric' })}><div className="today-summary"><div className="ring" style={{ background: `conic-gradient(#7436b9 ${acceptedRate}%, #ece7f1 ${acceptedRate}% 100%)` }}><div><strong>{todayTotal}</strong><small>total entries</small></div></div><div className="legend"><span><i className="legend-dot accepted" />Accepted <b>{todayCounts?.accepted ?? (daily.loading ? '…' : 0)}</b></span><span><i className="legend-dot rejected" />Rejected <b>{todayCounts?.rejected ?? (daily.loading ? '…' : 0)}</b></span></div></div></Panel>
    </div>
  </>
}
function StatCard({ title, value, hint, icon, tone = 'navy', onClick }) { const contents = <><div className="stat-top"><span>{title}</span><i><Icon name={icon} size={17} /></i></div><strong>{value}</strong><small>{hint}</small></>; return onClick ? <button className={`stat-card tone-${tone} stat-card-button`} onClick={onClick}>{contents}</button> : <div className={`stat-card tone-${tone}`}>{contents}</div> }
function WeekChart({ data, loading }) {
  if (loading) return <div className="loading chart-loading">Loading weekly activity…</div>
  if (!data) return <div className="empty-state">Activity chart is unavailable.</div>
  const max = Math.max(1, ...data.map(x => x.accepted + x.rejected))
  return <div className="chart" aria-label="Accepted and rejected entry counts over the last seven days"><div className="chart-y"><span>{max}</span><span>{Math.round(max / 2)}</span><span>0</span></div><div className="chart-bars">{data.map(row => { const a = row.accepted / max * 100, r = row.rejected / max * 100; const label = new Date(`${row.day}T12:00:00`).toLocaleDateString(undefined, { weekday: 'short', month: 'short', day: 'numeric' }); return <div className="chart-day" key={row.day}><div className="chart-hover-info"><strong>{label}</strong><span>Accepted: {row.accepted}</span><span>Rejected: {row.rejected}</span><b>Total: {row.accepted + row.rejected}</b></div><div className="bar-stack"><div className="bar rejected-bar" style={{ height: `${r}%` }} /><div className="bar accepted-bar" style={{ height: `${a}%` }} /></div><span>{label.split(',')[0]}</span></div> })}</div></div>
}

function CampusPulse({ request, refresh, openLabDetails, goToPage }) {
  const [page, setPage] = useState(1)
  const pageSize = 4
  const labs = useLoad(() => request(`/lab/Active?pageNumber=${page}&pageSize=${pageSize}`), [request, refresh, page])
  const warnings = useLoad(() => request('/warning?pageNumber=1&pageSize=100'), [request, refresh])
  const activity = useLoad(() => request('/entryLog?pageNumber=1&pageSize=6'), [request, refresh])
  const rooms = labs.data?.items ?? []
  const warningItems = warnings.data?.items ?? []
  const recentLogs = activity.data?.items ?? []
  const memberCounts = useLoad(async () => Promise.all(rooms.map(async lab => {
    try {
      const result = await request(`/visitor/${lab.id}?PageNumber=1&PageSize=1`)
      return [lab.id, Number(result?.totalCount ?? 0)]
    } catch { return [lab.id, null] }
  })), [request, refresh, labs.data, page])
  const memberCountByLab = Object.fromEntries(memberCounts.data ?? [])
  const warningsForLab = labId => warningItems.filter(item => Number(item.labId) === Number(labId)).length
  const totalMembers = Object.values(memberCountByLab).reduce((total, count) => total + (Number.isFinite(count) ? count : 0), 0)
  // Four cardinal positions leave the campus hub centered and keep every
  // connector endpoint aligned with the center of its lab card.
  const positions = [[2, 1], [3, 2], [2, 3], [1, 2]]
  const refreshAll = () => { labs.refresh(); warnings.refresh(); activity.refresh(); memberCounts.refresh() }
  return <>
    <section className="campus-pulse-section">
    <div className="campus-pulse-heading"><div><h2>Facility network</h2><p>Lab access, warnings, and recent activity in one view.</p></div><button className="button button-light" onClick={refreshAll}><Icon name="refresh" size={15} /> Refresh</button></div>
    <div className="map-stats">
      <button type="button" className="map-stat-card" onClick={() => goToPage('labs')}><span>Active labs</span><strong>{labs.data?.totalCount ?? '—'}</strong><small>Current facilities · view labs</small></button>
      <button type="button" className="map-stat-card" onClick={() => goToPage('visitors')}><span>Permitted members</span><strong>{memberCounts.loading ? '…' : totalMembers}</strong><small>Across labs on this page · view visitors</small></button>
      <button type="button" className="map-stat-card" onClick={() => goToPage('warnings')}><span>Reported warnings</span><strong className={warnings.data?.totalCount ? 'has-map-warnings' : ''}>{warnings.data?.totalCount ?? '—'}</strong><small>Latest warning records · view warnings</small></button>
    </div>
    {(labs.error || warnings.error || activity.error) && <ErrorBox message={labs.error || warnings.error || activity.error} retry={refreshAll} />}
    {labs.loading ? <div className="loading">Loading campus pulse…</div> : rooms.length ? <div className="campus-pulse-layout">
      <section className="pulse-network-card">
        <div className="campus-map-card-heading"><div><span className="eyebrow">CAMPUS NETWORK</span><h2>Lab connections</h2></div><span className="schematic-tag"><i /> Operations view</span></div>
        <div className="pulse-network-canvas">
          <svg className="pulse-connectors" viewBox="0 0 300 300" preserveAspectRatio="none" aria-hidden="true">{rooms.map((lab, index) => { const [column, row] = positions[index]; const x = (column - 1) * 100 + 50; const y = (row - 1) * 100 + 50; return <line key={lab.id} x1="150" y1="150" x2={x} y2={y} className={warningsForLab(lab.id) ? 'connector-warning' : ''} /> })}</svg>
          <div className="pulse-network-grid">
            {rooms.map((lab, index) => {
              const [column, row] = positions[index]
              const warningCount = warningsForLab(lab.id)
              const members = memberCountByLab[lab.id]
              return <button type="button" key={lab.id} style={{ gridColumn: column, gridRow: row }} className={`pulse-lab-node ${warningCount ? 'has-warning' : ''}`} onClick={() => openLabDetails(lab.id)} aria-label={`Open ${lab.name}, capacity ${lab.capacity}, ${warningCount} warnings`}>
                <span className="pulse-node-top"><span className="pulse-node-icon"><Icon name="building" size={17} /></span><span className="pulse-node-state"><i />{warningCount ? `${warningCount} warning${warningCount === 1 ? '' : 's'}` : 'Clear'}</span></span>
                <strong>{lab.name}</strong><span className="pulse-node-id">Lab #{lab.id}</span>
                <span className="pulse-node-metrics"><span><small>Capacity</small><b>{lab.capacity}</b></span><span><small>Permitted</small><b>{members == null ? '—' : members}</b></span></span>
                <span className="pulse-node-open">Open details <Icon name="arrow-up-right" size={13} /></span>
              </button>
            })}
            <div className="pulse-campus-hub"><span className="hub-orbit hub-orbit-one"/><span className="hub-orbit hub-orbit-two"/><span className="hub-icon"><Icon name="network" size={23} /></span><strong>CAMPUS</strong><small>{labs.data?.totalCount ?? rooms.length} ACTIVE LABS</small></div>
          </div>
        </div>
        {labs.data?.totalPages > 1 && <Pager page={page} total={labs.data.totalPages} onPage={setPage} />}
      </section>
      <aside className="pulse-side-column">
        <section className="pulse-side-card"><div className="pulse-side-heading"><div><span className="eyebrow">LATEST ACTIVITY</span><h2>Recent entries</h2></div><Icon name="activity" size={18} /></div>
          {activity.loading ? <div className="loading">Loading activity…</div> : recentLogs.length ? <div className="pulse-feed">{recentLogs.map(log => {
            const lab = rooms.find(item => Number(item.id) === Number(log.labId))
            return <button key={log.id} className="pulse-feed-row" onClick={() => log.labId && openLabDetails(log.labId)} disabled={!log.labId}><span className={`pulse-feed-status ${Number(log.status) === 1 ? 'is-accepted' : 'is-rejected'}`}><Icon name={Number(log.status) === 1 ? 'check' : 'close'} size={13} /></span><span className="pulse-feed-copy"><strong>{log.visitorName || (log.visitorId ? `Visitor #${log.visitorId}` : 'Unidentified visitor')}</strong><small>{lab?.name || `Lab #${log.labId}`} · {dateText(log.eventTime)}</small></span><Icon name="arrow-up-right" size={14} /></button>
          })}</div> : <Empty>No recent entry activity.</Empty>}
        </section>
        <section className="pulse-side-card"><div className="pulse-side-heading"><div><span className="eyebrow">NEEDS ATTENTION</span><h2>Warnings</h2></div><span className="map-alert-count">{warnings.data?.totalCount ?? '—'}</span></div>
          {warnings.loading ? <div className="loading">Loading warnings…</div> : warningItems.length ? <div className="pulse-feed">{warningItems.slice(0, 5).map(warning => <button key={warning.id} className="pulse-feed-row" onClick={() => warning.labId && openLabDetails(warning.labId)} disabled={!warning.labId}><span className="pulse-warning-icon"><Icon name="triangle-alert" size={15} /></span><span className="pulse-feed-copy"><strong>{alertName(warning.warning)}</strong><small>{rooms.find(lab => Number(lab.id) === Number(warning.labId))?.name || `Lab #${warning.labId}`}</small></span><Icon name="arrow-up-right" size={14} /></button>)}</div> : <Empty>No warnings reported.</Empty>}
        </section>
      </aside>
    </div> : labs.error ? null : <Empty>No active labs are available.</Empty>}
    </section>
  </>
}

function Labs({ request, refresh, notify, askConfirm, setSelectedLab, goToPage, openModal, openLabDetails }) {
  const [page, setPage] = useState(1)
  const [active, setActive] = useState(true)
  const [search, setSearch] = useState({ Id: '', Name: '' })
  const hasSearch = Boolean(search.Id || search.Name)
  const query = new URLSearchParams({ PageNumber: String(page), PageSize: '10', Active: String(active) })
  if (search.Id) query.set('Id', search.Id)
  if (search.Name) query.set('Name', search.Name)
  const load = useLoad(() => hasSearch ? request(`/lab/search?${query}`) : request(`/lab/${active ? 'Active' : 'inactive'}?pageNumber=${page}&pageSize=10`), [request, refresh, page, active, hasSearch, query.toString()])
  const remove = lab => askConfirm({ title: 'Deactivate this lab?', message: `${lab.name} will be archived and can be restored later.`, confirmText: 'Deactivate lab', danger: true }, async () => { try { await request(`/lab/${lab.id}`, { method: 'DELETE' }); notify('Lab deactivated.'); load.refresh() } catch (e) { notify(e.message, true) } })
  const restore = async lab => { try { await request(`/lab/${lab.id}/restore`, { method: 'PUT' }); notify('Lab restored.'); load.refresh() } catch (e) { notify(e.message, true) } }
  const permanentDelete = lab => askConfirm({ title: 'Permanently delete this lab?', message: `This permanently removes ${lab.name}. This cannot be undone.`, confirmText: 'Permanently delete', danger: true }, async () => { try { await request(`/lab/${lab.id}/permanent`, { method: 'DELETE' }); notify('Lab permanently deleted.'); load.refresh() } catch (e) { notify(e.message, true) } })
  return <>
    <PageTitle eyebrow="FACILITIES" title="Labs" description="Create and manage labs, then inspect the entry activity for each one." action={<button className="button button-primary" onClick={() => openModal('new')}><Icon name="plus" size={15} /> Add lab</button>} />
    <Panel title={active ? 'Active labs' : 'Inactive labs'} subtitle={load.data ? `${load.data.totalCount} labs in the system` : 'Manage lab details and activity'} action={<div className="panel-controls"><input className="select-control" aria-label="Search lab name" placeholder="Search lab name" value={search.Name} onChange={e => { setSearch(s => ({ ...s, Name: e.target.value })); setPage(1) }} /><input className="select-control" aria-label="Search lab ID" placeholder="Lab ID" type="number" value={search.Id} onChange={e => { setSearch(s => ({ ...s, Id: e.target.value })); setPage(1) }} /><button className="button button-light" onClick={() => { setActive(x => !x); setPage(1) }}>{active ? 'Show inactive' : 'Show active'}</button></div>}>
      {load.error ? <ErrorBox message={load.error} retry={load.refresh} /> : load.loading ? <div className="loading">Loading labs…</div> : load.data?.items?.length ? <><Table headers={['Lab', 'Capacity', 'ID', 'Actions']}>
        {load.data.items.map(lab => <tr key={lab.id} className="clickable-row lab-row" tabIndex="0" aria-label={`Open ${lab.name} details`} onClick={() => openLabDetails(lab.id)} onKeyDown={e => e.key === 'Enter' && openLabDetails(lab.id)}><td><div className="lab-cell"><span className="lab-avatar"><Icon name="building" size={17} /></span><strong>{lab.name}</strong></div></td><td>{lab.capacity} people</td><td className="muted">#{lab.id}</td><td><div className="row-actions">{active && <><button className="button button-light" onClick={e => { e.stopPropagation(); setSelectedLab(String(lab.id)); goToPage('logs') }}>View logs</button><button className="icon-button" title="Edit lab" onClick={e => { e.stopPropagation(); openModal(lab) }}><Icon name="edit" size={16} /></button><button className="icon-button danger" title="Deactivate lab" onClick={e => { e.stopPropagation(); remove(lab) }}><Icon name="trash" size={16} /></button></>}{!active && <><button className="button button-light" onClick={e => { e.stopPropagation(); restore(lab) }}>Restore</button><button className="button button-danger small" onClick={e => { e.stopPropagation(); permanentDelete(lab) }}>Delete permanently</button></>}</div></td></tr>)}
      </Table><Pager page={page} total={load.data.totalPages} onPage={setPage} /></> : <Empty>No labs yet. Add a lab to begin.</Empty>}
    </Panel>
  </>
}

function LabDetails({ labId, request, refresh, goBack, goToPage, setSelectedLab }) {
  const [visitorId, setVisitorId] = useState('')
  const [memberPage, setMemberPage] = useState(1)
  const [busy, setBusy] = useState('')
  const [feedback, setFeedback] = useState(null)
  const [selectedLog, setSelectedLog] = useState(null)
  const lab = useLoad(async () => {
    if (!labId) throw new Error('No lab was selected.')
    for (const active of [true, false]) {
      const result = await request(`/lab/search?Id=${labId}&PageNumber=1&PageSize=1&Active=${active}`)
      if (result?.items?.[0]) return result.items[0]
    }
    throw new Error(`Lab #${labId} could not be found.`)
  }, [request, refresh, labId])
  const members = useLoad(() => labId ? request(`/visitor/${labId}?PageNumber=${memberPage}&PageSize=10`) : Promise.resolve({ items: [] }), [request, refresh, labId, memberPage])
  const visitors = useLoad(() => request('/visitor/Active?pageNumber=1&pageSize=1000'), [request, refresh])
  const inactiveVisitors = useLoad(() => request('/visitor/inactive?pageNumber=1&pageSize=1000'), [request, refresh])
  const memberIdsByVisitorId = useMemo(
    () => buildMemberIdsByVisitorId(visitors.data?.items, inactiveVisitors.data?.items),
    [visitors.data, inactiveVisitors.data]
  )
  const resolveLogMemberId = log => resolveEntryLogMemberId(log, memberIdsByVisitorId)
  const logs = useLoad(() => labId ? request(`/entryLog/ByLab?LabId=${labId}&PageNumber=1&PageSize=5`) : Promise.resolve({ items: [] }), [request, refresh, labId])

  async function updateMembership(action, rawId) {
    const id = Number(rawId)
    if (!Number.isInteger(id) || id < 1) {
      setFeedback({ error: true, message: 'Enter a valid visitor ID.' })
      return
    }
    setBusy(action)
    setFeedback(null)
    try {
      const method = action === 'add' ? 'POST' : 'DELETE'
      await request(`/lab/${labId}/visitors/${id}`, { method })
      setFeedback({ message: action === 'add' ? `Visitor #${id} added to this lab.` : `Visitor #${id} removed from this lab.` })
      if (action === 'add') {
        setVisitorId('')
        if (memberPage !== 1) setMemberPage(1)
        else members.refresh()
      } else if (members.data?.items?.length === 1 && memberPage > 1) setMemberPage(page => page - 1)
      else members.refresh()
    } catch (error) {
      setFeedback({ error: true, message: error.message })
    } finally {
      setBusy('')
    }
  }

  const record = lab.data
  return <>
    <div className="lab-detail-back"><button className="button button-light" onClick={goBack}><Icon name="arrow-right" size={15} className="back-arrow" /> All labs</button></div>
    {lab.error ? <ErrorBox message={lab.error} retry={lab.refresh} /> : lab.loading || !record ? <div className="loading">Loading lab details…</div> : <>
      <section className="lab-detail-hero"><div className="lab-detail-icon"><Icon name="building" size={25} /></div><div><span className="eyebrow">LABORATORY · #{record.id}</span><h1>{record.name}</h1><p>{record.active ? 'Active facility' : 'Inactive facility'} · Capacity {record.capacity}</p></div><span className={`access-state ${record.active ? 'is-active' : 'is-inactive'}`}><i />{record.active ? 'Active' : 'Inactive'}</span></section>
      <div className="lab-detail-metrics"><article className="lab-metric"><span>Maximum capacity</span><strong>{record.capacity}</strong><small>Registered seats for this lab</small></article><article className="lab-metric capacity-placeholder"><span>Current capacity</span><strong>— <small>/ {record.capacity}</small></strong><small>Live occupancy is not available from the current API</small></article><article className="lab-metric"><span>Recent activity</span><strong>{logs.data?.items?.length ?? '—'}</strong><small>Latest records shown below</small></article></div>
      {feedback && <div className={`notice ${feedback.error ? 'notice-error' : ''}`}>{feedback.message}<button aria-label="Dismiss" onClick={() => setFeedback(null)}><Icon name="close" size={15} /></button></div>}
      <Panel title="Lab members" subtitle={members.data ? `${members.data.totalCount} permitted members` : 'Members assigned to this lab'}>
        <div className="membership-actions">
          <form className="membership-form" onSubmit={e => { e.preventDefault(); updateMembership('add', visitorId) }}><label htmlFor="add-visitor-id">Add member by visitor ID</label><div><input id="add-visitor-id" type="number" min="1" required value={visitorId} onChange={e => setVisitorId(e.target.value)} placeholder="Visitor ID" /><button className="button button-primary" disabled={busy !== ''}><Icon name="plus" size={15} />{busy === 'add' ? 'Adding…' : 'Add member'}</button></div></form>
        </div>
        {members.error ? <ErrorBox message={members.error} retry={members.refresh} /> : members.loading ? <div className="loading">Loading permitted members…</div> : members.data?.items?.length ? <><Table headers={['Visitor', 'Email', 'Year', 'Visitor ID', 'Actions']}>
          {members.data.items.map(member => <tr key={member.id}><td><div className="person-cell">{avatar(member.faceImage, member.fullName)}<strong>{member.fullName || 'Unnamed visitor'}</strong></div></td><td>{member.email || '—'}</td><td>{member.academicYear ? `Year ${member.academicYear}` : '—'}</td><td className="muted">#{member.id}</td><td><button className="button button-light small" disabled={busy !== ''} onClick={() => updateMembership('remove', member.id)}><Icon name="trash" size={14} />{busy === 'remove' ? 'Removing…' : 'Remove'}</button></td></tr>)}
        </Table><Pager page={memberPage} total={members.data.totalPages} onPage={setMemberPage} /></> : <Empty>No visitors are currently assigned to this lab.</Empty>}
      </Panel>
      <Panel title="Recent logs" subtitle="Latest entry activity recorded for this lab" action={<button className="text-button" onClick={() => { setSelectedLab(String(labId)); goToPage('logs') }}>View all logs <Icon name="arrow-right" size={14} /></button>}>
        {logs.error ? <ErrorBox message={logs.error} retry={logs.refresh} /> : logs.loading ? <div className="loading">Loading recent logs…</div> : logs.data?.items?.length ? <Table headers={['Time', 'Member', 'Member ID', 'Face image', 'Lab', 'Decision']}>
          {logs.data.items.map(item => { const memberId = resolveLogMemberId(item); return <tr key={item.id} className="clickable-row" tabIndex="0" onClick={() => setSelectedLog(item)} onKeyDown={e => e.key === 'Enter' && setSelectedLog(item)}><td>{dateText(item.eventTime)}</td><td>{item.visitorName || (item.visitorId ? `#${item.visitorId}` : 'Unidentified')}</td><td>{memberId != null && memberId !== '' ? `#${memberId}` : '—'}</td><td>{item.faceImage ? avatar(item.faceImage, item.visitorName) : <span className="muted">—</span>}</td><td>{item.labName || record.name || `Lab #${item.labId}`}</td><td><Status value={item.status} /></td></tr> })}
        </Table> : <Empty>No entry logs have been recorded for this lab.</Empty>}
      </Panel>
      <RecordDetails title={`Entry log #${selectedLog?.id ?? ''}`} record={selectedLog} onClose={() => setSelectedLog(null)} fields={selectedLog ? [['Log ID', `#${selectedLog.id}`], ['Event time', dateText(selectedLog.eventTime)], ['Decision', enumName(selectedLog.status, ['Rejected', 'Accepted'])], ['Visitor', selectedLog.visitorName || 'Unidentified'], ['Visitor ID', selectedLog.visitorId ? `#${selectedLog.visitorId}` : '—'], ['Member ID', resolveLogMemberId(selectedLog) != null && resolveLogMemberId(selectedLog) !== '' ? `#${resolveLogMemberId(selectedLog)}` : '—'], ['Lab', selectedLog.labName || record.name || `Lab #${selectedLog.labId}`]] : []} images={selectedLog ? [['Face image', selectedLog.faceImage]] : []} />
    </>}
  </>
}

function LabModal({ item, onClose, request, onSaved, onError }) {
  const [busy, setBusy] = useState(false)
  async function submit(e) {
    e.preventDefault(); setBusy(true)
    const form = new FormData(e.currentTarget)
    const body = { name: form.get('name'), capacity: Number(form.get('capacity')) }
    try {
      await request(item ? `/lab/${item.id}` : '/lab/create', { method: item ? 'PATCH' : 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(body) })
      onSaved()
    } catch (error) { onError(error) } finally { setBusy(false) }
  }
  return <div className="modal-backdrop" onMouseDown={e => e.target === e.currentTarget && onClose()}><section className="modal"><div className="modal-heading"><div><span className="eyebrow">LAB DETAILS</span><h2>{item ? 'Edit lab' : 'Add a lab'}</h2></div><button className="icon-button" onClick={onClose}><Icon name="close" size={16} /></button></div><form onSubmit={submit} className="form-stack"><label>Lab name<input name="name" required defaultValue={item?.name || ''} placeholder="e.g. Computer Lab 01" /></label><label>Capacity<input name="capacity" type="number" required min="1" defaultValue={item?.capacity ?? ''} placeholder="Maximum people" /></label><div className="modal-actions"><button type="button" className="button button-light" onClick={onClose}>Cancel</button><button className="button button-primary" disabled={busy}>{busy ? 'Saving…' : item ? 'Save changes' : 'Create lab'}</button></div></form></section></div>
}

function Logs({ request, apiBase, refresh, selectedLab, setSelectedLab, preset, setPreset, askConfirm, notify }) {
  const [page, setPage] = useState(1)
  const [record, setRecord] = useState(null)
  const [createOpen, setCreateOpen] = useState(false)
  const [exportOpen, setExportOpen] = useState(false)
  const [rotationOpen, setRotationOpen] = useState(false)
  const [cleanupOpen, setCleanupOpen] = useState(false)
  const [range, setRange] = useState({ from: '', to: '', status: '' })
  const [visitorFilters, setVisitorFilters] = useState({ memberId: '', fullName: '', email: '', academicYear: '' })
  const [appliedVisitorFilters, setAppliedVisitorFilters] = useState({ memberId: '', fullName: '', email: '', academicYear: '' })
  const labs = useLoad(() => request('/lab/Active?pageNumber=1&pageSize=1000'), [request, refresh])
  const visitors = useLoad(() => request('/visitor/Active?pageNumber=1&pageSize=1000'), [request, refresh])
  const inactiveVisitors = useLoad(() => request('/visitor/inactive?pageNumber=1&pageSize=1000'), [request, refresh])
  const memberIdsByVisitorId = useMemo(
    () => buildMemberIdsByVisitorId(visitors.data?.items, inactiveVisitors.data?.items),
    [visitors.data, inactiveVisitors.data]
  )
  const resolveLogMemberId = log => resolveEntryLogMemberId(log, memberIdsByVisitorId)
  const load = useLoad(() => {
    const hasDateFilter = Boolean(range.from || range.to)
    const hasVisitorFilter = Boolean(appliedVisitorFilters.memberId || appliedVisitorFilters.fullName.trim() || appliedVisitorFilters.email.trim() || appliedVisitorFilters.academicYear)
    if (range.from && range.to && range.to < range.from) throw new Error('The end date must be on or after the start date.')
    if (range.from && range.to && !selectedLab && range.status === '' && !hasVisitorFilter) {
      return request(`/entryLog/date-range?fromDate=${encodeURIComponent(`${range.from}T00:00:00`)}&toDate=${encodeURIComponent(nextDateStart(range.to))}&pageNumber=${page}&pageSize=10`)
    }
    if (selectedLab && !hasDateFilter && range.status === '' && preset === 'all' && !hasVisitorFilter) return request(`/entryLog/ByLab?LabId=${selectedLab}&PageNumber=${page}&PageSize=10`)
    if (selectedLab || hasDateFilter || range.status !== '' || hasVisitorFilter) {
      const query = new URLSearchParams({ pageNumber: String(page), pageSize: '10' })
      if (selectedLab) query.set('LabId', selectedLab)
      if (range.from) query.set('StartDate', `${range.from}T00:00:00`)
      if (range.to) query.set('EndDate', nextDateStart(range.to))
      if (range.status !== '') query.set('Status', range.status)
      if (appliedVisitorFilters.memberId) query.set('MemberId', appliedVisitorFilters.memberId)
      if (appliedVisitorFilters.fullName.trim()) query.set('FullName', appliedVisitorFilters.fullName.trim())
      if (appliedVisitorFilters.email.trim()) query.set('Email', appliedVisitorFilters.email.trim())
      if (appliedVisitorFilters.academicYear) query.set('AcademicYear', appliedVisitorFilters.academicYear)
      return request(`/entryLog/search?${query}`)
    }
    if (preset === 'accepted' || preset === 'rejected') {
      const now = new Date()
      const from = `${localDateKey(now)}T00:00:00`
      const tomorrow = new Date(now.getFullYear(), now.getMonth(), now.getDate() + 1)
      const to = `${localDateKey(tomorrow)}T00:00:00`
      const status = preset === 'accepted' ? 1 : 0
      return request(`/entryLog/date-range?fromDate=${encodeURIComponent(from)}&toDate=${encodeURIComponent(to)}&status=${status}&pageNumber=${page}&pageSize=10`)
    }
    return request(`/entryLog?pageNumber=${page}&pageSize=10`)
  }, [request, refresh, selectedLab, preset, page, range.from, range.to, range.status, appliedVisitorFilters.memberId, appliedVisitorFilters.fullName, appliedVisitorFilters.email, appliedVisitorFilters.academicYear])
  const filterLabel = preset === 'accepted' ? 'Accepted today' : preset === 'rejected' ? 'Rejected today' : selectedLab ? `Lab ${selectedLab}` : 'All labs'
  const deleteLog = log => askConfirm({ title: 'Delete this entry log?', message: `Entry log #${log.id} will be permanently deleted.`, confirmText: 'Delete log', danger: true }, async () => { try { await request(`/entryLog/${log.id}`, { method: 'DELETE' }); notify('Entry log deleted.'); load.refresh() } catch (e) { notify(e.message, true) } })
  function openLog(log) { setRecord(log) }
  return <><PageTitle eyebrow="ACTIVITY" title="Entry logs" description="Review entry decisions across all labs or focus on a single location." action={<div className="panel-controls"><button className="button button-light" onClick={() => setExportOpen(true)}><Icon name="download" size={15} /> Export Excel</button><button className="button button-light" onClick={() => setRotationOpen(true)}><Icon name="refresh" size={15} /> Log rotation</button><button className="button button-light" onClick={() => setCleanupOpen(true)}><Icon name="trash" size={15} /> Cleanup</button><button className="button button-primary" onClick={() => setCreateOpen(true)}><Icon name="plus" size={15} /> Add log</button></div>} />
    <Panel title="Entry history" subtitle={load.data ? `${load.data.totalCount} records · ${filterLabel}` : 'Most recent records appear first'}>
      <form className="logs-filters" onSubmit={e => { e.preventDefault(); setAppliedVisitorFilters({ ...visitorFilters }); setPage(1) }}>
        <label className="logs-filter-field"><span>Lab</span><select value={selectedLab} onChange={e => { setSelectedLab(e.target.value); setPreset('all'); setPage(1) }}><option value="">All labs</option>{labs.data?.items?.map(l => <option key={l.id} value={l.id}>{l.name}</option>)}</select></label>
        <label className="logs-filter-field"><span>Member ID</span><input type="number" min="1" inputMode="numeric" placeholder="Search member ID" value={visitorFilters.memberId} onChange={e => setVisitorFilters(s => ({ ...s, memberId: e.target.value }))} /></label>
        <label className="logs-filter-field"><span>From date</span><div className="date-input-wrap"><Icon name="calendar-days" size={16} /><input type="date" aria-label="From date" value={range.from} onChange={e => { setRange(s => ({ ...s, from: e.target.value })); setPage(1) }} /></div></label>
        <label className="logs-filter-field"><span>To date</span><div className="date-input-wrap"><Icon name="calendar-days" size={16} /><input type="date" aria-label="To date" value={range.to} onChange={e => { setRange(s => ({ ...s, to: e.target.value })); setPage(1) }} /></div></label>
        <label className="logs-filter-field"><span>Decision</span><select value={range.status} onChange={e => { setRange(s => ({ ...s, status: e.target.value })); setPage(1) }}><option value="">Any decision</option><option value="0">Rejected</option><option value="1">Accepted</option></select></label>
        <label className="logs-filter-field"><span>Visitor name</span><input type="search" placeholder="Search visitor name" value={visitorFilters.fullName} onChange={e => setVisitorFilters(s => ({ ...s, fullName: e.target.value }))} /></label>
        <label className="logs-filter-field"><span>Visitor email</span><input type="search" placeholder="Search visitor email" value={visitorFilters.email} onChange={e => setVisitorFilters(s => ({ ...s, email: e.target.value }))} /></label>
        <label className="logs-filter-field"><span>Academic year</span><select value={visitorFilters.academicYear} onChange={e => setVisitorFilters(s => ({ ...s, academicYear: e.target.value }))}><option value="">Any year</option>{[1, 2, 3, 4].map(year => <option key={year} value={year}>Year {year}</option>)}</select></label>
        <div className="logs-filter-actions"><button type="submit" className="button button-primary">Search</button><button type="button" className="button button-light logs-clear-filters" onClick={() => { const empty = { memberId: '', fullName: '', email: '', academicYear: '' }; setSelectedLab(''); setRange({ from: '', to: '', status: '' }); setVisitorFilters(empty); setAppliedVisitorFilters(empty); setPreset('all'); setPage(1) }}>Clear all filters</button></div>
      </form>
      {load.error ? <ErrorBox message={load.error} retry={load.refresh} /> : load.loading ? <div className="loading">Loading entry logs…</div> : load.data?.items?.length ? <><Table headers={['Time', 'Member', 'Member ID', 'Face image', 'Lab', 'Decision', 'Actions']}>
        {load.data.items.map(x => { const memberId = resolveLogMemberId(x); return <tr key={x.id} className="clickable-row" tabIndex="0" onClick={() => openLog(x)} onKeyDown={e => e.key === 'Enter' && openLog(x)}><td>{dateText(x.eventTime)}</td><td>{x.visitorName || (x.visitorId ? `#${x.visitorId}` : 'Unidentified')}</td><td>{memberId != null && memberId !== '' ? `#${memberId}` : '—'}</td><td>{x.faceImage ? avatar(x.faceImage, x.visitorName) : <span className="muted">—</span>}</td><td>{x.labName || `Lab #${x.labId}`}</td><td><Status value={x.status} /></td><td><button className="icon-button danger" title="Delete entry log" onClick={e => { e.stopPropagation(); deleteLog(x) }}><Icon name="trash" size={16} /></button></td></tr> })}
      </Table><Pager page={page} total={load.data.totalPages} onPage={setPage} /></> : <Empty>No logs match this selection.</Empty>}
    </Panel>
    <RecordDetails title={`Entry log #${record?.id ?? ''}`} record={record} onClose={() => setRecord(null)} fields={record ? [['Log ID', `#${record.id}`], ['Event time', dateText(record.eventTime)], ['Decision', enumName(record.status, ['Rejected', 'Accepted'])], ['Visitor', record.visitorName || 'Unidentified'], ['Visitor ID', record.visitorId ? `#${record.visitorId}` : '—'], ['Member ID', resolveLogMemberId(record) != null && resolveLogMemberId(record) !== '' ? `#${resolveLogMemberId(record)}` : '—'], ['Lab', record.labName || `Lab #${record.labId}`]] : []} images={record ? [['Face image', record.faceImage]] : []} />
    {createOpen && <EntryLogModal labs={labs.data?.items || []} visitors={visitors.data?.items || []} request={request} onClose={() => setCreateOpen(false)} onSaved={() => { setCreateOpen(false); load.refresh(); notify('Entry log created.') }} onError={e => notify(e.message, true)} />}
    {exportOpen && <ExportExcelModal labs={labs.data?.items || []} apiBase={apiBase} onClose={() => setExportOpen(false)} notify={notify} onError={e => notify(e.message, true)} />}
    {rotationOpen && <LogRotationModal request={request} onClose={() => setRotationOpen(false)} notify={notify} onError={e => notify(e.message, true)} />}
    {cleanupOpen && <CleanupLogsModal request={request} onClose={() => setCleanupOpen(false)} onCleaned={() => load.refresh()} askConfirm={askConfirm} notify={notify} onError={e => notify(e.message, true)} />}
  </>
}

function EntryLogModal({ labs, visitors, request, onClose, onSaved, onError }) {
  const [busy, setBusy] = useState(false)
  async function submit(e) {
    e.preventDefault(); setBusy(true)
    const form = new FormData(e.currentTarget)
    const visitorId = form.get('visitorId')
    const body = { labId: Number(form.get('labId')), visitorId: visitorId ? Number(visitorId) : null, eventTime: new Date(form.get('eventTime')).toISOString(), status: Number(form.get('status')) }
    try { await request('/entryLog', { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(body) }); onSaved() }
    catch (error) { onError(error) } finally { setBusy(false) }
  }
  return <div className="modal-backdrop" onMouseDown={e => e.target === e.currentTarget && onClose()}><section className="modal"><div className="modal-heading"><h2>Add entry log</h2><button className="icon-button" onClick={onClose}><Icon name="close" size={16} /></button></div><form className="form-stack" onSubmit={submit}><label>Lab<select name="labId" required>{labs.map(l => <option key={l.id} value={l.id}>{l.name}</option>)}</select></label><label>Visitor (optional)<select name="visitorId"><option value="">Unidentified visitor</option>{visitors.map(v => <option key={v.id} value={v.id}>{v.fullName} · #{v.memberId}</option>)}</select></label><label>Event time<input name="eventTime" type="datetime-local" required defaultValue={localDateTimeValue()} /></label><label>Decision<select name="status"><option value="1">Accepted</option><option value="0">Rejected</option></select></label><div className="modal-actions"><button type="button" className="button button-light" onClick={onClose}>Cancel</button><button className="button button-primary" disabled={busy || labs.length === 0}>{busy ? 'Saving…' : 'Create log'}</button></div></form></section></div>
}
function localDateTimeValue() { const now = new Date(); return `${localDateKey(now)}T${String(now.getHours()).padStart(2, '0')}:${String(now.getMinutes()).padStart(2, '0')}` }

function ExportExcelModal({ labs, apiBase, onClose, onError, notify }) {
  const [busy, setBusy] = useState(false)
  const now = new Date()
  const todayStart = `${localDateKey(now)}T00:00`
  const todayEnd = `${localDateKey(now)}T23:59`
  const [from, setFrom] = useState(todayStart)
  const [to, setTo] = useState(todayEnd)
  const [labId, setLabId] = useState('')

  async function submit(e) {
    e.preventDefault()
    setBusy(true)
    try {
      const fromIso = new Date(from).toISOString()
      const toIso = new Date(to).toISOString()
      const query = new URLSearchParams({ fromDate: fromIso, toDate: toIso })
      if (labId) query.set('labId', labId)
      await downloadFile(apiBase, `/entryLog/export/excel?${query}`, `Attendance_${from.replace(/[:T]/g, '-')}_to_${to.replace(/[:T]/g, '-')}.xlsx`)
      notify('Attendance sheet downloaded.')
      onClose()
    } catch (error) {
      onError(error)
    } finally {
      setBusy(false)
    }
  }

  return (
    <div className="modal-backdrop" onMouseDown={e => e.target === e.currentTarget && onClose()}>
      <section className="modal">
        <div className="modal-heading">
          <div>
            <span className="eyebrow">REPORTS</span>
            <h2>Export attendance to Excel</h2>
          </div>
          <button className="icon-button" onClick={onClose}><Icon name="close" size={16} /></button>
        </div>
        <form className="form-stack" onSubmit={submit}>
          <label>From date & time
            <input type="datetime-local" required value={from} onChange={e => setFrom(e.target.value)} />
          </label>
          <label>To date & time
            <input type="datetime-local" required value={to} onChange={e => setTo(e.target.value)} />
          </label>
          <label>Lab (optional)
            <select value={labId} onChange={e => setLabId(e.target.value)}>
              <option value="">All labs</option>
              {labs.map(l => <option key={l.id} value={l.id}>{l.name}</option>)}
            </select>
          </label>
          <div className="modal-actions">
            <button type="button" className="button button-light" onClick={onClose}>Cancel</button>
            <button className="button button-primary" disabled={busy}>
              {busy ? 'Exporting…' : 'Download .xlsx'}
            </button>
          </div>
        </form>
      </section>
    </div>
  )
}

function LogRotationModal({ request, onClose, notify, onError }) {
  const [busy, setBusy] = useState(false)
  const [config, setConfig] = useState({ isEnabled: false, retentionDays: 90, runAtHourUtc: 2, lastRunAt: null })
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    let active = true
    request('/entryLog/rotation-config')
      .then(res => { if (active && res) setConfig(res) })
      .catch(e => { if (active) onError(e) })
      .finally(() => { if (active) setLoading(false) })
    return () => { active = false }
  }, [request, onError])

  async function submit(e) {
    e.preventDefault()
    setBusy(true)
    try {
      await request('/entryLog/rotation-config', {
        method: 'PATCH',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          isEnabled: config.isEnabled,
          retentionDays: Number(config.retentionDays),
          runAtHourUtc: Number(config.runAtHourUtc)
        })
      })
      notify('Automated log rotation settings updated.')
      onClose()
    } catch (error) {
      onError(error)
    } finally {
      setBusy(false)
    }
  }

  return (
    <div className="modal-backdrop" onMouseDown={e => e.target === e.currentTarget && onClose()}>
      <section className="modal">
        <div className="modal-heading">
          <div>
            <span className="eyebrow">AUTOMATION</span>
            <h2>Automated log rotation</h2>
          </div>
          <button className="icon-button" onClick={onClose}><Icon name="close" size={16} /></button>
        </div>
        {loading ? <div className="loading">Loading configuration…</div> : (
          <form className="form-stack" onSubmit={submit}>
            <label style={{ display: 'flex', alignItems: 'center', gap: 10, cursor: 'pointer' }}>
              <input
                type="checkbox"
                checked={config.isEnabled}
                onChange={e => setConfig(s => ({ ...s, isEnabled: e.target.checked }))}
                style={{ width: 18, height: 18 }}
              />
              <span>Enable automated log rotation</span>
            </label>
            <label>Retention length (days to keep)
              <input
                type="number"
                min="1"
                required
                value={config.retentionDays}
                onChange={e => setConfig(s => ({ ...s, retentionDays: e.target.value }))}
                placeholder="e.g. 30, 60, 90"
              />
            </label>
            <label>Execution time (UTC hour)
              <select
                value={config.runAtHourUtc}
                onChange={e => setConfig(s => ({ ...s, runAtHourUtc: Number(e.target.value) }))}
              >
                {Array.from({ length: 24 }, (_, i) => (
                  <option key={i} value={i}>
                    {String(i).padStart(2, '0')}:00 UTC ({i === 0 ? '12:00 AM' : i < 12 ? `${i}:00 AM` : i === 12 ? '12:00 PM' : `${i - 12}:00 PM`})
                  </option>
                ))}
              </select>
            </label>
            <div className="notice" style={{ margin: '4px 0', fontSize: 11 }}>
              Last executed: <strong>{config.lastRunAt ? dateText(config.lastRunAt) : 'Never'}</strong>
            </div>
            <div className="modal-actions">
              <button type="button" className="button button-light" onClick={onClose}>Cancel</button>
              <button className="button button-primary" disabled={busy}>
                {busy ? 'Saving…' : 'Save settings'}
              </button>
            </div>
          </form>
        )}
      </section>
    </div>
  )
}

function CleanupLogsModal({ request, onClose, onCleaned, askConfirm, notify, onError }) {
  const [tab, setTab] = useState('oldest')
  const [cnt, setCnt] = useState(50)
  const [range, setRange] = useState({ start: '', end: '' })
  const [busy, setBusy] = useState(false)

  async function deleteOldest(e) {
    e.preventDefault()
    if (!cnt || cnt <= 0) return
    askConfirm({
      title: `Delete oldest ${cnt} logs?`,
      message: `The oldest ${cnt} entry logs in the system will be permanently deleted. This cannot be undone.`,
      confirmText: `Delete ${cnt} oldest logs`,
      danger: true
    }, async () => {
      setBusy(true)
      try {
        await request(`/entryLog/Oldest?Cnt=${cnt}`, { method: 'DELETE' })
        notify(`Deleted oldest ${cnt} logs.`)
        onCleaned()
        onClose()
      } catch (err) {
        onError(err)
      } finally {
        setBusy(false)
      }
    })
  }

  async function deleteRange(e) {
    e.preventDefault()
    if (!range.start || !range.end) return
    const startIso = new Date(range.start).toISOString()
    const endIso = new Date(range.end).toISOString()
    askConfirm({
      title: 'Delete logs in date range?',
      message: `All entry logs between ${range.start} and ${range.end} will be permanently deleted. This cannot be undone.`,
      confirmText: 'Delete logs in date range',
      danger: true
    }, async () => {
      setBusy(true)
      try {
        await request(`/entryLog/Range?StartDate=${encodeURIComponent(startIso)}&EndDate=${encodeURIComponent(endIso)}`, { method: 'DELETE' })
        notify('Deleted logs within date range.')
        onCleaned()
        onClose()
      } catch (err) {
        onError(err)
      } finally {
        setBusy(false)
      }
    })
  }

  function handleTabKeyDown(event) {
    if (!['ArrowLeft', 'ArrowRight', 'Home', 'End'].includes(event.key)) return
    event.preventDefault()
    const nextTab = event.key === 'Home' ? 'oldest'
      : event.key === 'End' ? 'range'
      : tab === 'oldest' ? 'range' : 'oldest'
    setTab(nextTab)
    document.getElementById(`cleanup-tab-${nextTab}`)?.focus()
  }

  return (
    <div className="modal-backdrop" onMouseDown={e => e.target === e.currentTarget && onClose()}>
      <section className="modal">
        <div className="modal-heading">
          <div>
            <span className="eyebrow">CLEANUP</span>
            <h2>Manual log deletion</h2>
          </div>
          <button className="icon-button" onClick={onClose}><Icon name="close" size={16} /></button>
        </div>
        <div className="cleanup-tabs" role="tablist" aria-label="Log deletion method">
          <button
            type="button"
            id="cleanup-tab-oldest"
            role="tab"
            aria-selected={tab === 'oldest'}
            aria-controls="cleanup-panel-oldest"
            tabIndex={tab === 'oldest' ? 0 : -1}
            className={`cleanup-tab ${tab === 'oldest' ? 'active' : ''}`}
            onClick={() => setTab('oldest')}
            onKeyDown={handleTabKeyDown}
          >
            By number of oldest logs
          </button>
          <button
            type="button"
            id="cleanup-tab-range"
            role="tab"
            aria-selected={tab === 'range'}
            aria-controls="cleanup-panel-range"
            tabIndex={tab === 'range' ? 0 : -1}
            className={`cleanup-tab ${tab === 'range' ? 'active' : ''}`}
            onClick={() => setTab('range')}
            onKeyDown={handleTabKeyDown}
          >
            By date range
          </button>
        </div>

        {tab === 'oldest' ? (
          <div id="cleanup-panel-oldest" role="tabpanel" aria-labelledby="cleanup-tab-oldest">
          <form className="form-stack" onSubmit={deleteOldest}>
            <label>How many oldest logs should be deleted?
              <input
                type="number"
                min="1"
                required
                value={cnt}
                onChange={e => setCnt(Number(e.target.value))}
                placeholder="e.g. 50, 100, 500"
              />
            </label>
            <p style={{ fontSize: 11, color: '#8898aa', margin: 0 }}>
              This finds and deletes the earliest records recorded by the cameras.
            </p>
            <div className="modal-actions">
              <button type="button" className="button button-light" onClick={onClose}>Cancel</button>
              <button className="button button-danger" disabled={busy}>
                {busy ? 'Deleting…' : `Delete ${cnt || 0} oldest logs`}
              </button>
            </div>
          </form>
          </div>
        ) : (
          <div id="cleanup-panel-range" role="tabpanel" aria-labelledby="cleanup-tab-range">
          <form className="form-stack" onSubmit={deleteRange}>
            <label>Start date & time
              <input
                type="datetime-local"
                required
                value={range.start}
                onChange={e => setRange(s => ({ ...s, start: e.target.value }))}
              />
            </label>
            <label>End date & time
              <input
                type="datetime-local"
                required
                value={range.end}
                onChange={e => setRange(s => ({ ...s, end: e.target.value }))}
              />
            </label>
            <div className="modal-actions">
              <button type="button" className="button button-light" onClick={onClose}>Cancel</button>
              <button className="button button-danger" disabled={busy}>
                {busy ? 'Deleting…' : 'Delete logs in date range'}
              </button>
            </div>
          </form>
          </div>
        )}
      </section>
    </div>
  )
}

function Visitors({ request, refresh, notify, askConfirm }) {
  const [page, setPage] = useState(1)
  const [labAssignmentVisitor, setLabAssignmentVisitor] = useState(null)
  const [draft, setDraft] = useState({ MemberId: '', FullName: '', Email: '', AcademicYear: '', Active: 'true' })
  const [filters, setFilters] = useState({})
  const [record, setRecord] = useState(null)
  const params = new URLSearchParams({ PageNumber: String(page), PageSize: '10' })
  Object.entries(filters).forEach(([key, value]) => { if (value !== '' && value != null) params.set(key, String(value).trim()) })
  const query = params.toString()
  const hasCriteria = Object.entries(filters).some(([key, value]) => key !== 'Active' && value !== '')
  const selectedActive = filters.Active === 'false' ? false : true
  const load = useLoad(() => hasCriteria ? request(`/visitor/search?${query}`) : request(`/visitor/${selectedActive ? 'Active' : 'inactive'}?pageNumber=${page}&pageSize=10`), [request, refresh, query, hasCriteria, selectedActive])
  const remove = visitor => askConfirm({ title: 'Deactivate this visitor?', message: `${visitor.fullName}'s access record will be archived and can be restored later.`, confirmText: 'Deactivate visitor', danger: true }, async () => { try { await request(`/visitor/${visitor.id}`, { method: 'DELETE' }); notify('Visitor deactivated.'); load.refresh() } catch (e) { notify(e.message, true) } })
  const restore = async visitor => { try { await request(`/visitor/${visitor.id}/restore`, { method: 'PUT' }); notify('Visitor restored.'); load.refresh() } catch (e) { notify(e.message, true) } }
  const permanentDelete = visitor => askConfirm({ title: 'Permanently delete this visitor?', message: `This permanently removes ${visitor.fullName} and cannot be undone.`, confirmText: 'Permanently delete', danger: true }, async () => { try { await request(`/visitor/${visitor.id}/permanent`, { method: 'DELETE' }); notify('Visitor permanently deleted.'); load.refresh() } catch (e) { notify(e.message, true) } })
  return <><PageTitle eyebrow="DIRECTORY" title="Visitors" description="Browse people with monitoring access and review their profile details." />
    <Panel title="All visitors" subtitle={load.data ? `${load.data.totalCount} matching visitors` : 'Registered visitor access'}>
      <FilterBar fields={[{ name: 'MemberId', label: 'Member ID', type: 'number', min: 1 }, { name: 'FullName', label: 'Full name', placeholder: 'Search by name' }, { name: 'Email', label: 'Email', placeholder: 'Search by email' }, { name: 'AcademicYear', label: 'Academic year', options: [['1', 'Year 1'], ['2', 'Year 2'], ['3', 'Year 3'], ['4', 'Year 4']] }, { name: 'Active', label: 'Status', options: [['true', 'Active'], ['false', 'Inactive']] }]} values={draft} onChange={(key, value) => setDraft(s => ({ ...s, [key]: value }))} onSubmit={e => { e.preventDefault(); setPage(1); setFilters({ ...draft, MemberId: draft.MemberId.trim() }) }} onClear={() => { const cleared = { MemberId: '', FullName: '', Email: '', AcademicYear: '', Active: 'true' }; setDraft(cleared); setFilters(cleared); setPage(1) }} />
      {load.error ? <ErrorBox message={load.error} retry={load.refresh} /> : load.loading ? <div className="loading">Loading visitors…</div> : load.data?.items?.length ? <><Table headers={['Visitor', 'Email', 'Academic year', 'Member ID', 'Actions']}>
        {load.data.items.map(m => <tr key={m.id} className="clickable-row" tabIndex="0" onClick={() => setRecord(m)} onKeyDown={e => e.key === 'Enter' && setRecord(m)}><td><div className="person-cell">{avatar(m.faceImage, m.fullName)}<strong>{m.fullName}</strong></div></td><td>{m.email}</td><td>Year {m.academicYear}</td><td className="muted">{m.memberId != null ? `#${m.memberId}` : '—'}</td><td>{m.active ? <div className="row-actions"><button className="button button-light small" onClick={e => { e.stopPropagation(); setLabAssignmentVisitor(m) }}><Icon name="building" size={14} /> Add lab</button><button className="icon-button danger" title="Deactivate visitor" onClick={e => { e.stopPropagation(); remove(m) }}><Icon name="trash" size={16} /></button></div> : <div className="row-actions"><button className="button button-light" onClick={e => { e.stopPropagation(); restore(m) }}>Restore</button><button className="button button-danger small" onClick={e => { e.stopPropagation(); permanentDelete(m) }}>Delete permanently</button></div>}</td></tr>)}
      </Table><Pager page={page} total={load.data.totalPages} onPage={setPage} /></> : <Empty>No visitors found.</Empty>}
    </Panel>
    <RecordDetails title={record?.fullName || 'Visitor'} record={record} onClose={() => setRecord(null)} fields={record ? [['Visitor ID', `#${record.id}`], ['Member ID', record.memberId != null ? `#${record.memberId}` : '—'], ['Full name', record.fullName], ['Email', record.email], ['Academic year', `Year ${record.academicYear}`], ['Access status', record.active ? 'Active' : 'Inactive']] : []} images={record ? [['Face image', record.faceImage], ['ID image', record.idImage]] : []} />
    {labAssignmentVisitor && <VisitorLabModal visitor={labAssignmentVisitor} request={request} onClose={() => setLabAssignmentVisitor(null)} onAdded={labId => { setLabAssignmentVisitor(null); notify(`Lab #${labId} added for ${labAssignmentVisitor.fullName}.`) }} onError={error => notify(error.message, true)} />}
  </>
}

function VisitorLabModal({ visitor, request, onClose, onAdded, onError }) {
  const [busy, setBusy] = useState(false)
  async function submit(event) {
    event.preventDefault()
    const form = new FormData(event.currentTarget)
    const labId = Number(form.get('labId'))
    if (!Number.isInteger(labId) || labId < 1) return
    setBusy(true)
    try {
      await request(`/lab/${labId}/visitors/${visitor.id}`, { method: 'POST' })
      onAdded(labId)
    } catch (error) { onError(error) }
    finally { setBusy(false) }
  }
  return <div className="modal-backdrop" onMouseDown={e => e.target === e.currentTarget && !busy && onClose()}><section className="modal" role="dialog" aria-modal="true" aria-label={`Add lab for ${visitor.fullName}`}><div className="modal-heading"><div><span className="eyebrow">VISITOR ACCESS</span><h2>Add a lab</h2></div><button className="icon-button" aria-label="Close" onClick={onClose} disabled={busy}><Icon name="close" size={16} /></button></div><p className="modal-copy">Assign a lab to <strong>{visitor.fullName}</strong> (Visitor #{visitor.id}).</p><form className="form-stack" onSubmit={submit}><label>Lab ID<input name="labId" type="number" min="1" required autoFocus placeholder="Enter lab ID" /></label><div className="modal-actions"><button type="button" className="button button-light" onClick={onClose} disabled={busy}>Cancel</button><button className="button button-primary" disabled={busy}><Icon name="building" size={15} />{busy ? 'Adding…' : 'Add lab'}</button></div></form></section></div>
}
function avatar(bytes, name) { return <span className="person-avatar">{bytes ? <img src={`data:image/jpeg;base64,${bytes}`} alt="" /> : name?.slice(0, 1)?.toUpperCase()}</span> }

function Accounts({ request, refresh, notify, askConfirm }) {
  const [page, setPage] = useState(1)
  const [editing, setEditing] = useState(null)
  const [create, setCreate] = useState(false)
  const [draft, setDraft] = useState({ Name: '', Email: '', role: '', IsActive: '' })
  const [filters, setFilters] = useState({})
  const params = new URLSearchParams({ PageNumber: String(page), PageSize: '10' })
  if (filters.Name) params.set('Name', filters.Name)
  if (filters.Email) params.set('Email', filters.Email)
  if (filters.role) params.set('role', filters.role)
  if (filters.IsActive) params.set('IsActive', filters.IsActive)
  const query = params.toString()
  const hasFilters = Boolean(filters.Name || filters.Email || filters.role || filters.IsActive)
  const load = useLoad(() => request(hasFilters ? `/dashboardAccount/search?${query}` : `/dashboardAccount?pageNumber=${page}&pageSize=10`), [request, refresh, query, hasFilters, page])
  const update = async (account, fields) => { try { await request(`/dashboardAccount/${account.id}`, { method: 'PATCH', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(fields) }); notify('Account updated.'); load.refresh() } catch (e) { notify(e.message, true) } }
  const toggle = account => { const action = account.isActive ? 'deactivate' : 'activate'; askConfirm({ title: `${account.isActive ? 'Deactivate' : 'Activate'} this account?`, message: `${account.fullName} will ${account.isActive ? 'lose' : 'regain'} dashboard access.`, confirmText: `${account.isActive ? 'Deactivate' : 'Activate'} account`, danger: account.isActive }, async () => { try { await request(`/dashboardAccount/${action}/${account.id}`, { method: 'PATCH' }); notify(`Account ${action}d.`); load.refresh() } catch (e) { notify(e.message, true) } }) }
  const remove = account => askConfirm({ title: 'Delete this account?', message: `The dashboard account for ${account.fullName} will be permanently removed.`, confirmText: 'Delete account', danger: true }, async () => { try { await request(`/dashboardAccount/${account.id}`, { method: 'DELETE' }); notify('Account deleted.'); load.refresh() } catch (e) { notify(e.message, true) } })
  return <><PageTitle eyebrow="ACCESS" title="Dashboard accounts" description="Create accounts and manage names, roles, and access status." action={<button className="button button-primary" onClick={() => setCreate(true)}><Icon name="plus" size={15} /> Add account</button>} />
    <Panel title="All accounts" subtitle={load.data ? `${load.data.totalCount} matching dashboard accounts` : 'Account access and roles'}>
      <FilterBar fields={[{ name: 'Name', label: 'Full name', placeholder: 'Search by name' }, { name: 'Email', label: 'Email', placeholder: 'Search by email' }, { name: 'role', label: 'Role', options: [['1', 'Admin'], ['2', 'User']] }, { name: 'IsActive', label: 'Access status', options: [['true', 'Active'], ['false', 'Inactive']] }]} values={draft} onChange={(key, value) => setDraft(s => ({ ...s, [key]: value }))} onSubmit={e => { e.preventDefault(); setPage(1); setFilters({ ...draft }) }} onClear={() => { setDraft({ Name: '', Email: '', role: '', IsActive: '' }); setFilters({}); setPage(1) }} />
      {load.error ? <ErrorBox message={load.error} retry={load.refresh} /> : load.loading ? <div className="loading">Loading accounts…</div> : load.data?.items?.length ? <><Table headers={['Account', 'Email', 'Role', 'Status', 'Actions']}>
        {load.data.items.map(a => <tr key={a.id}><td><div className="account-cell"><span className="initial-avatar">{a.fullName?.slice(0, 1)?.toUpperCase()}</span><strong>{a.fullName}</strong></div></td><td>{a.email}</td><td><span className="role-pill">{enumName(a.role, { 1: 'Admin', 2: 'User' })}</span></td><td><span className={`access-state ${a.isActive ? 'is-active' : 'is-inactive'}`}><i />{a.isActive ? 'Active' : 'Inactive'}</span></td><td><div className="row-actions"><button className="button button-light" onClick={() => setEditing(a)}>Edit</button>{a.role !== 1 && <><button className="button button-light" onClick={() => toggle(a)}>{a.isActive ? 'Deactivate' : 'Activate'}</button><button className="icon-button danger" title="Delete account" onClick={() => remove(a)}><Icon name="trash" size={16} /></button></>}</div></td></tr>)}
      </Table><Pager page={page} total={load.data.totalPages} onPage={setPage} /></> : <Empty>No dashboard accounts found.</Empty>}
    </Panel>
    {(create || editing) && <AccountModal item={editing} onClose={() => { setCreate(false); setEditing(null) }} request={request} onSaved={() => { setCreate(false); setEditing(null); notify(editing ? 'Account updated.' : 'Account created.'); load.refresh() }} onError={e => notify(e.message, true)} />}
  </>
}
function AccountModal({ item, onClose, request, onSaved, onError }) {
  const [busy, setBusy] = useState(false)
  const [role, setRole] = useState(Number(item?.role ?? 2))
  const [selectedLabIds, setSelectedLabIds] = useState([])
  const labs = useLoad(() => request('/lab/Active?pageNumber=1&pageSize=1000'), [request])
  const isEditingAdmin = Boolean(item && Number(item.role) === 1)
  const isCreatingAdmin = !item && role === 1
  const activeLabIds = useMemo(() => (labs.data?.items ?? []).map(lab => lab.id), [labs.data])
  const effectiveLabIds = isCreatingAdmin ? activeLabIds : selectedLabIds

  async function submit(e) {
    e.preventDefault(); setBusy(true); const data = new FormData(e.currentTarget)
    const fields = { fullName: data.get('fullName'), email: data.get('email'), role: isEditingAdmin ? Number(item.role) : role }
    if (!item) fields.labIds = effectiveLabIds
    if (data.get('password')) fields.password = data.get('password')
    if (!item && effectiveLabIds.length === 0) { setBusy(false); onError(new Error('Select at least one lab for this account.')); return }
    try {
      await request(item ? `/dashboardAccount/${item.id}` : '/dashboardAccount/create', { method: item ? 'PATCH' : 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(fields) })
      onSaved()
    } catch (error) { onError(error) } finally { setBusy(false) }
  }
  return <div className="modal-backdrop" onMouseDown={e => e.target === e.currentTarget && onClose()}><section className="modal"><div className="modal-heading"><div><span className="eyebrow">ACCOUNT ACCESS</span><h2>{item ? 'Edit account' : 'Add an account'}</h2></div><button className="icon-button" onClick={onClose}><Icon name="close" size={16} /></button></div><form onSubmit={submit} className="form-stack"><label>Full name<input name="fullName" required defaultValue={item?.fullName || ''} /></label><label>Email address<input name="email" type="email" required defaultValue={item?.email || ''} /></label><label>{item ? 'New password (optional)' : 'Password'}<input name="password" type="password" required={!item} minLength="8" /></label>{!isEditingAdmin && <label>Role<select name="role" value={role} onChange={e => setRole(Number(e.target.value))}><option value="1">Admin</option><option value="2">User</option></select></label>}{!item && <div className="form-field"><strong>Assigned labs</strong><small>{isCreatingAdmin ? 'All active labs are automatically assigned and locked for Admin accounts.' : 'Select at least one lab. The backend requires a lab assignment when creating an account.'}</small>{labs.loading ? <div className="loading">Loading active labs…</div> : labs.error ? <ErrorBox message={labs.error} retry={labs.refresh} /> : labs.data?.items?.length ? <div className="check-list">{labs.data.items.map(lab => <label className={`check-row ${isCreatingAdmin ? 'is-locked' : ''}`} key={lab.id}><input type="checkbox" checked={isCreatingAdmin ? true : selectedLabIds.includes(lab.id)} disabled={isCreatingAdmin} onChange={e => { if (isCreatingAdmin) return; setSelectedLabIds(ids => e.target.checked ? [...ids, lab.id] : ids.filter(id => id !== lab.id)) }} /><span><strong>{lab.name}</strong><small>{lab.capacity} seats · Lab #{lab.id}</small></span></label>)}</div> : <Empty>No active labs available. Add or restore a lab before creating an account.</Empty>}</div>}<div className="modal-actions"><button type="button" className="button button-light" onClick={onClose}>Cancel</button><button className="button button-primary" disabled={busy || (!item && (labs.loading || !labs.data?.items?.length || effectiveLabIds.length === 0))}>{busy ? 'Saving…' : item ? 'Save changes' : 'Create account'}</button></div></form></section></div>
}

function Requests({ request, refresh, notify, askConfirm }) {
  const [page, setPage] = useState(1)
  const [draft, setDraft] = useState({ MemberId: '', Id: '', FullName: '', Email: '', AcademicYear: '', Status: '' })
  const [filters, setFilters] = useState({})
  const [record, setRecord] = useState(null)
  const [createOpen, setCreateOpen] = useState(false)
  const [approval, setApproval] = useState(null)
  const labs = useLoad(() => request('/lab/Active?pageNumber=1&pageSize=1000'), [request, refresh])
  const params = new URLSearchParams({ PageNumber: String(page), PageSize: '10' })
  Object.entries(filters).forEach(([key, value]) => { if (value !== '') params.set(key, value) })
  const query = params.toString()
  const hasFilters = Object.values(filters).some(value => value !== '')
  const load = useLoad(() => request(hasFilters ? `/registrationRequest/search?${query}` : `/registrationRequest?pageNumber=${page}&pageSize=10`), [request, refresh, query, hasFilters, page])
  function decide(item, action, onDone = () => {}) {
    const approving = action === 'approve'
    if (approving) { setApproval({ item, onDone }); return }
    askConfirm({ title: `${approving ? 'Approve' : 'Reject'} this request?`, message: `${item.fullName}'s registration request will be marked ${approving ? 'accepted' : 'rejected'}.`, confirmText: approving ? 'Approve request' : 'Reject request', danger: !approving }, async () => {
      try { await request(`/registrationRequest/${action}/${item.id}`, { method: 'PATCH' }); notify(`Request ${approving ? 'approved' : 'rejected'}.`); load.refresh(); onDone() }
      catch (e) { notify(e.message, true) }
    })
  }
  async function openRequest(item) { try { setRecord(await request(`/registrationRequest/${item.id}`)) } catch (e) { notify(e.message, true) } }
  return <><PageTitle eyebrow="ENROLLMENT" title="Registration requests" description="Review new member applications and approve or reject each request." action={<button className="button button-primary" onClick={() => setCreateOpen(true)}><Icon name="plus" size={15} /> Submit registration</button>} />
    <Panel title="Incoming requests" subtitle={load.data ? `${load.data.totalCount} matching requests` : 'Review submissions'}>
      <FilterBar fields={[{ name: 'MemberId', label: 'Student ID', type: 'number', min: 1 }, { name: 'Id', label: 'Request ID', type: 'number', min: 1 }, { name: 'FullName', label: 'Full name', placeholder: 'Search by name' }, { name: 'Email', label: 'Email', placeholder: 'Search by email' }, { name: 'AcademicYear', label: 'Academic year', options: [['1', 'Year 1'], ['2', 'Year 2'], ['3', 'Year 3'], ['4', 'Year 4']] }, { name: 'Status', label: 'Status', options: [['0', 'Pending'], ['1', 'Rejected'], ['2', 'Accepted']] }]} values={draft} onChange={(key, value) => setDraft(s => ({ ...s, [key]: value }))} onSubmit={e => { e.preventDefault(); setPage(1); setFilters({ ...draft }) }} onClear={() => { setDraft({ MemberId: '', Id: '', FullName: '', Email: '', AcademicYear: '', Status: '' }); setFilters({}); setPage(1) }} />
      {load.error ? <ErrorBox message={load.error} retry={load.refresh} /> : load.loading ? <div className="loading">Loading requests…</div> : load.data?.items?.length ? <><Table headers={['Applicant', 'Email', 'Member ID', 'Year', 'Submitted', 'Status', 'Actions']}>
        {load.data.items.map(x => <tr key={x.id} className="clickable-row" tabIndex="0" onClick={() => openRequest(x)} onKeyDown={e => e.key === 'Enter' && openRequest(x)}><td><div className="person-cell">{avatar(x.faceImage, x.fullName)}<strong>{x.fullName}</strong></div></td><td>{x.email}</td><td>#{x.memberId}</td><td>Year {x.academicYear}</td><td>{dateText(x.submittedAt)}</td><td><Status kind="request" value={x.status} /></td><td>{x.status === 0 ? <div className="row-actions"><button className="button button-primary small" onClick={e => { e.stopPropagation(); decide(x, 'approve') }}>Approve</button><button className="button button-light small" onClick={e => { e.stopPropagation(); decide(x, 'reject') }}>Reject</button></div> : <span className="muted">Processed</span>}</td></tr>)}
      </Table><Pager page={page} total={load.data.totalPages} onPage={setPage} /></> : <Empty>No registration requests.</Empty>}
    </Panel>
    <RecordDetails title={record?.fullName || 'Registration request'} record={record} onClose={() => setRecord(null)} fields={record ? [['Request ID', `#${record.id}`], ['Member ID', `#${record.memberId}`], ['Full name', record.fullName], ['Email', record.email], ['Academic year', `Year ${record.academicYear}`], ['Submitted', dateText(record.submittedAt)], ['Status', <Status kind="request" value={record.status} />]] : []} images={record ? [['Face image', record.faceImage], ['ID image', record.idImage]] : []} actions={record?.status === 0 && <><button className="button button-light small" onClick={() => decide(record, 'reject', () => setRecord(null))}>Reject</button><button className="button button-primary small" onClick={() => decide(record, 'approve', () => setRecord(null))}>Approve</button></>} />
    {createOpen && <RegistrationModal request={request} onClose={() => setCreateOpen(false)} onSaved={() => { setCreateOpen(false); load.refresh(); notify('Registration request submitted.') }} onError={e => notify(e.message, true)} />}
    {approval && <ApproveRequestModal item={approval.item} labs={labs.data?.items || []} loading={labs.loading} error={labs.error} retry={labs.refresh} request={request} onClose={() => setApproval(null)} onError={e => notify(e.message, true)} onSaved={() => { approval.onDone(); setApproval(null); setRecord(null); load.refresh(); notify('Request approved and lab access assigned.') }} />}
  </>
}
function ApproveRequestModal({ item, labs, loading, error, retry, request, onClose, onSaved, onError }) {
  const [selected, setSelected] = useState([])
  const [busy, setBusy] = useState(false)
  async function submit(e) {
    e.preventDefault()
    if (!selected.length) { onError(new Error('Select at least one active lab.')); return }
    setBusy(true)
    try { await request(`/registrationRequest/approve/${item.id}`, { method: 'PATCH', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(selected) }); onSaved() }
    catch (error) { onError(error) }
    finally { setBusy(false) }
  }
  return <div className="modal-backdrop" onMouseDown={e => e.target === e.currentTarget && onClose()}><section className="modal"><div className="modal-heading"><div><span className="eyebrow">ASSIGN ACCESS</span><h2>Approve {item.fullName}</h2></div><button className="icon-button" onClick={onClose}><Icon name="close" size={16} /></button></div><p className="modal-copy">Choose the active labs this visitor can enter. At least one lab is required.</p>{loading ? <div className="loading">Loading active labs…</div> : error ? <ErrorBox message={error} retry={retry} /> : labs.length ? <form className="form-stack" onSubmit={submit}><div className="check-list">{labs.map(lab => <label className="check-row" key={lab.id}><input type="checkbox" checked={selected.includes(lab.id)} onChange={e => setSelected(current => e.target.checked ? [...current, lab.id] : current.filter(id => id !== lab.id))} /><span><strong>{lab.name}</strong><small>{lab.capacity} seats · Lab #{lab.id}</small></span></label>)}</div><div className="modal-actions"><button type="button" className="button button-light" onClick={onClose}>Cancel</button><button className="button button-primary" disabled={busy || selected.length === 0}>{busy ? 'Approving…' : `Approve and assign ${selected.length} lab${selected.length === 1 ? '' : 's'}`}</button></div></form> : <><Empty>No active labs available. Activate a lab before approving this request.</Empty><div className="modal-actions"><button className="button button-light" onClick={onClose}>Close</button></div></>}</section></div>
}
function RegistrationModal({ request, onClose, onSaved, onError }) {
  const [busy, setBusy] = useState(false)
  async function submit(e) { e.preventDefault(); setBusy(true); try { await request('/registrationRequest', { method: 'POST', body: new FormData(e.currentTarget) }); onSaved() } catch (error) { onError(error) } finally { setBusy(false) } }
  return <div className="modal-backdrop" onMouseDown={e => e.target === e.currentTarget && onClose()}><section className="modal"><div className="modal-heading"><h2>Submit registration</h2><button className="icon-button" onClick={onClose}><Icon name="close" size={16} /></button></div><form className="form-stack" onSubmit={submit}><label>Member ID<input name="MemberId" type="number" min="1" required /></label><label>Full name<input name="FullName" required /></label><label>Email<input name="Email" type="email" required /></label><label>Academic year<select name="AcademicYear" required><option value="">Choose year</option>{[1,2,3,4].map(x => <option key={x}>{x}</option>)}</select></label><label>Face image<input name="FaceImage" type="file" accept="image/*" required /></label><label>ID image<input name="IdImage" type="file" accept="image/*" required /></label><div className="modal-actions"><button type="button" className="button button-light" onClick={onClose}>Cancel</button><button className="button button-primary" disabled={busy}>{busy ? 'Submitting…' : 'Submit request'}</button></div></form></section></div>
}
function Warnings({ request, refresh, notify, askConfirm }) {
  const [page, setPage] = useState(1)
  const [filterType, setFilterType] = useState('all')
  const [filterValue, setFilterValue] = useState('')
  const [createOpen, setCreateOpen] = useState(false)
  const [advanced, setAdvanced] = useState({ LabId: '', LabName: '', Warning: '' })
  const [record, setRecord] = useState(null)
  const labs = useLoad(() => request('/lab/Active?pageNumber=1&pageSize=1000'), [request, refresh])
  const load = useLoad(() => {
    if (filterType === 'alert' && filterValue !== '') return request(`/warning/alert/${filterValue}?pageNumber=${page}&pageSize=10`)
    if (filterType === 'lab' && filterValue !== '') return request(`/warning/lab/${filterValue}?pageNumber=${page}&pageSize=10`)
    if (filterType === 'labName' && filterValue !== '') return request(`/warning/lab/name/${encodeURIComponent(filterValue)}?pageNumber=${page}&pageSize=10`)
    if (filterType === 'advanced') {
      const q = new URLSearchParams({ pageNumber: String(page), pageSize: '10' })
      if (advanced.LabId) q.set('LabId', advanced.LabId)
      if (advanced.LabName) q.set('LabName', advanced.LabName)
      if (advanced.Warning !== '') q.set('Warning', advanced.Warning)
      return request(`/warning/search?${q}`)
    }
    return request(`/warning?pageNumber=${page}&pageSize=10`)
  }, [request, refresh, page, filterType, filterValue, advanced])
  const deleteWarning = warning => askConfirm({ title: 'Delete this warning?', message: `Warning #${warning.id} for ${warning.lab?.name || `lab #${warning.labId}`} will be permanently removed.`, confirmText: 'Delete warning', danger: true }, async () => {
    try { await request(`/warning/${warning.id}`, { method: 'DELETE' }); notify('Warning deleted.'); load.refresh() }
    catch (e) { notify(e.message, true) }
  })
  const openWarning = async warning => {
    setRecord(warning)
  }
  const detailLab = record ? record.lab?.name || labs.data?.items?.find(l => l.id === record.labId)?.name || `Lab #${record.labId}` : ''
  return <>
    <PageTitle eyebrow="MONITORING" title="Warnings" description="Review alerts raised across your labs." action={<button className="button button-primary" onClick={() => setCreateOpen(true)}><Icon name="plus" size={15} /> Add warning</button>} />
    <Panel title="Warnings" subtitle={load.data ? `${load.data.totalCount} records` : 'Lab alerts and conditions'} action={<select className="select-control" value={filterType} onChange={e => { setFilterType(e.target.value); setFilterValue(''); setPage(1) }}><option value="all">All warnings</option><option value="alert">Filter by alert</option><option value="lab">Filter by lab ID</option><option value="labName">Filter by lab name</option><option value="advanced">Advanced search</option></select>}>
      {filterType === 'alert' && <div className="warning-filter-value"><label>Alert type<select className="select-control" value={filterValue} onChange={e => { setFilterValue(e.target.value); setPage(1) }}><option value="">Choose alert</option><option value="Food_Or_Drink_Detected">Food or drink detected</option><option value="Crowded">Crowded</option></select></label></div>}
      {filterType === 'lab' && <div className="warning-filter-value"><label>Lab<select className="select-control" value={filterValue} onChange={e => { setFilterValue(e.target.value); setPage(1) }}><option value="">Choose lab</option>{labs.data?.items?.map(l => <option value={l.id} key={l.id}>{l.name}</option>)}</select></label></div>}
      {filterType === 'labName' && <div className="warning-filter-value"><label>Lab name<input className="select-control" placeholder="Enter exact lab name" value={filterValue} onChange={e => { setFilterValue(e.target.value); setPage(1) }} /></label></div>}
      {filterType === 'advanced' && <div className="panel-controls"><input className="select-control" placeholder="Lab ID" type="number" value={advanced.LabId} onChange={e => setAdvanced(s => ({ ...s, LabId: e.target.value }))} /><input className="select-control" placeholder="Lab name" value={advanced.LabName} onChange={e => setAdvanced(s => ({ ...s, LabName: e.target.value }))} /><select className="select-control" value={advanced.Warning} onChange={e => setAdvanced(s => ({ ...s, Warning: e.target.value }))}><option value="">Any alert</option><option value="0">Food or drink detected</option><option value="1">Crowded</option></select></div>}
      {load.error ? <ErrorBox message={load.error} retry={load.refresh} /> : load.loading ? <div className="loading">Loading warnings…</div> : load.data?.items?.length ? <><Table headers={['Alert', 'Lab', 'Lab ID', 'Actions']}>
        {load.data.items.map(w => <tr key={w.id} className="clickable-row" tabIndex="0" onClick={() => openWarning(w)} onKeyDown={e => e.key === 'Enter' && openWarning(w)}><td><span className="warning-name"><Icon name="triangle-alert" size={16} />{alertName(w.warning)}</span></td><td>{w.lab?.name || `Lab #${w.labId}`}</td><td className="muted">#{w.labId}</td><td><button className="icon-button danger" title="Delete warning" onClick={e => { e.stopPropagation(); deleteWarning(w) }}><Icon name="trash" size={16} /></button></td></tr>)}
      </Table><Pager page={page} total={load.data.totalPages} onPage={setPage} /></> : <Empty>No warnings found.</Empty>}
    </Panel>
    <RecordDetails title={`Warning #${record?.id ?? ''}`} record={record} onClose={() => setRecord(null)} fields={record ? [['Warning ID', `#${record.id}`], ['Alert', alertName(record.warning)], ['Lab', detailLab]] : []} />
    {createOpen && <WarningModal labs={labs.data?.items || []} request={request} onClose={() => setCreateOpen(false)} onSaved={() => { setCreateOpen(false); load.refresh(); notify('Warning created.') }} onError={e => notify(e.message, true)} />}
  </>
}
function WarningModal({ labs, request, onClose, onSaved, onError }) {
  const [busy, setBusy] = useState(false)
  async function submit(e) { e.preventDefault(); setBusy(true); const form = new FormData(e.currentTarget); const body = { labId: Number(form.get('labId')), warning: Number(form.get('warning')) }; try { await request('/warning/create', { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(body) }); onSaved() } catch (error) { onError(error) } finally { setBusy(false) } }
  return <div className="modal-backdrop" onMouseDown={e => e.target === e.currentTarget && onClose()}><section className="modal"><div className="modal-heading"><h2>Create warning</h2><button className="icon-button" onClick={onClose}><Icon name="close" size={16} /></button></div>{labs.length ? <form className="form-stack" onSubmit={submit}><label>Lab<select name="labId" required>{labs.map(l => <option key={l.id} value={l.id}>{l.name}</option>)}</select></label><label>Alert type<select name="warning"><option value="0">Food or drink detected</option><option value="1">Crowded</option></select></label><div className="modal-actions"><button type="button" className="button button-light" onClick={onClose}>Cancel</button><button className="button button-primary" disabled={busy}>{busy ? 'Saving…' : 'Create warning'}</button></div></form> : <><Empty>There are no active labs. Add or restore a lab before creating a warning.</Empty><div className="modal-actions"><button className="button button-light" onClick={onClose}>Close</button></div></>}</section></div>
}

export default App
