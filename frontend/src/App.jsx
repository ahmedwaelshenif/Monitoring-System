import { useCallback, useEffect, useMemo, useState } from 'react'

const API_KEY = 'monitoring-admin-api-base'
const navItems = [
  ['overview', 'Overview', '◫'], ['labs', 'Labs', '▦'], ['logs', 'Entry logs', '≋'],
  ['members', 'Members', '♙'], ['accounts', 'Accounts', '♧'], ['requests', 'Registration requests', '▤'], ['warnings', 'Warnings', '⚑'],
]
const localDateKey = (value) => `${value.getFullYear()}-${String(value.getMonth() + 1).padStart(2, '0')}-${String(value.getDate()).padStart(2, '0')}`
const dateText = (value) => value ? new Date(value).toLocaleString() : '—'
const enumName = (value, choices) => choices[value] ?? String(value ?? '—')

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
  if (!contentType.includes('json') || /^\s*<!doctype html|^\s*<html/i.test(body)) {
    throw new Error(`The API request to ${path} returned HTML instead of JSON. Check the API address. When using Vite locally, make sure the backend is running at ${import.meta.env.VITE_API_PROXY_TARGET || 'https://localhost:7062'}.`)
  }
  try { return JSON.parse(body) }
  catch { throw new Error(`The API returned invalid JSON for ${path}. Check that the API address points to the backend.`) }
}

function App() {
  const [page, setPage] = useState('overview')
  const [apiBase, setApiBase] = useState(localStorage.getItem(API_KEY) || '')
  const [draftBase, setDraftBase] = useState(localStorage.getItem(API_KEY) || '')
  const [refresh, setRefresh] = useState(0)
  const [notice, setNotice] = useState(null)
  const [modal, setModal] = useState(null)
  const [confirmation, setConfirmation] = useState(null)
  const [selectedLab, setSelectedLab] = useState('')
  const [logsPreset, setLogsPreset] = useState('all')

  const request = useCallback((path, options) => api(apiBase, path, options), [apiBase])
  const notify = (message, error = false) => {
    setNotice({ message, error })
    window.setTimeout(() => setNotice(null), 5000)
  }
  const askConfirm = (options, action) => setConfirmation({ options, action })
  const reload = () => setRefresh(x => x + 1)
  const saveBase = (event) => {
    event.preventDefault()
    const value = draftBase.trim().replace(/\/$/, '')
    localStorage.setItem(API_KEY, value)
    setApiBase(value)
    notify(value ? 'API address saved.' : 'Using the current page origin for the API.')
  }

  return <div className="app-shell">
    <aside className="sidebar">
      <div className="side-label">WORKSPACE</div>
      <nav>{navItems.map(([id, label, icon]) => <button key={id} className={`nav-item ${page === id ? 'active' : ''}`} onClick={() => { setPage(id); if (id === 'logs') setLogsPreset('all') }}><span className="nav-icon">{icon}</span><span>{label}</span>{id === 'requests' && <i className="nav-dot" />}</button>)}</nav>
      <div className="sidebar-bottom"><div className="connection-dot" />API connection <span className="connection-text">{apiBase ? 'Custom' : 'Same origin'}</span></div>
    </aside>
    <main className="main-area">
      <header className="topbar">
        <div className="breadcrumb">Workspace <span>/</span> <strong>{navItems.find(x => x[0] === page)?.[1]}</strong></div>
        <form className="api-config" onSubmit={saveBase}><label htmlFor="api-base">API address</label><input id="api-base" value={draftBase} onChange={e => setDraftBase(e.target.value)} placeholder="Same origin" /><button className="button button-light" type="submit">Save</button></form>
      </header>
      <div className="content">
        {notice && <div className={`notice ${notice.error ? 'notice-error' : ''}`}>{notice.message}<button aria-label="Dismiss" onClick={() => setNotice(null)}>×</button></div>}
        {page === 'overview' && <Overview request={request} refresh={refresh} setPage={setPage} setLogsPreset={setLogsPreset} setSelectedLab={setSelectedLab} />}
        {page === 'labs' && <Labs request={request} refresh={refresh} notify={notify} askConfirm={askConfirm} setSelectedLab={setSelectedLab} goToPage={setPage} openModal={setModal} />}
        {page === 'logs' && <Logs request={request} refresh={refresh} selectedLab={selectedLab} setSelectedLab={setSelectedLab} preset={logsPreset} setPreset={setLogsPreset} />}
        {page === 'members' && <Members request={request} refresh={refresh} notify={notify} askConfirm={askConfirm} />}
        {page === 'accounts' && <Accounts request={request} refresh={refresh} notify={notify} askConfirm={askConfirm} />}
        {page === 'requests' && <Requests request={request} refresh={refresh} notify={notify} askConfirm={askConfirm} />}
        {page === 'warnings' && <Warnings request={request} refresh={refresh} notify={notify} askConfirm={askConfirm} />}
      </div>
    </main>
    {modal && <LabModal item={modal === 'new' ? null : modal} onClose={() => setModal(null)} request={request} onSaved={() => { setModal(null); reload(); notify(modal === 'new' ? 'Lab created.' : 'Lab updated.') }} onError={e => notify(e.message, true)} />}
    {confirmation && <ConfirmDialog options={confirmation.options} onClose={() => setConfirmation(null)} onConfirm={async () => { await confirmation.action(); setConfirmation(null) }} />}
  </div>
}

function useLoad(loader, deps) {
  const [state, setState] = useState({ data: null, loading: true, error: '' })
  const refresh = useCallback(async () => {
    setState(s => ({ ...s, loading: true, error: '' }))
    try { setState({ data: await loader(), loading: false, error: '' }) }
    catch (e) { setState({ data: null, loading: false, error: e.message }) }
  // loader is stabilized by the caller and deps list describes its inputs.
  // eslint-disable-next-line react-hooks/exhaustive-deps
  }, deps)
  useEffect(() => { refresh() }, [refresh])
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
  return <div className="modal-backdrop" onMouseDown={e => e.target === e.currentTarget && onClose()}><section className="modal detail-modal" role="dialog" aria-modal="true" aria-label={title}><div className="modal-heading"><div><span className="eyebrow">RECORD DETAILS</span><h2>{title}</h2></div><button className="icon-button" aria-label="Close" onClick={onClose}>×</button></div><div className="detail-grid">{fields.map(([label, value]) => <div className="detail-item" key={label}><small>{label}</small><strong>{value ?? '—'}</strong></div>)}</div>{images.length > 0 && <div className="detail-images">{images.map(([label, data]) => <div className="detail-image" key={label}><strong>{label}</strong>{data ? <img src={imageSrc(data)} alt={label} /> : <span className="image-empty">No image available</span>}</div>)}</div>}<div className="modal-actions">{actions}<button className="button button-light" onClick={onClose}>Close</button></div></section></div>
}
function ConfirmDialog({ options, onClose, onConfirm }) {
  const [busy, setBusy] = useState(false)
  async function confirm() { setBusy(true); try { await onConfirm() } finally { setBusy(false) } }
  return <div className="modal-backdrop confirm-backdrop" onMouseDown={e => e.target === e.currentTarget && !busy && onClose()}><section className="modal confirm-modal" role="alertdialog" aria-modal="true" aria-labelledby="confirm-title"><div className="confirm-icon">{options.icon || '!'}</div><h2 id="confirm-title">{options.title}</h2><p>{options.message}</p><div className="modal-actions"><button className="button button-light" onClick={onClose} disabled={busy}>Cancel</button><button className={`button ${options.danger ? 'button-danger' : 'button-primary'}`} onClick={confirm} disabled={busy}>{busy ? 'Please wait…' : options.confirmText || 'Confirm'}</button></div></section></div>
}
function imageSrc(data) { return typeof data === 'string' && data.startsWith('data:') ? data : `data:image/jpeg;base64,${data}` }

function Overview({ request, refresh, setPage, setLogsPreset, setSelectedLab }) {
  const labs = useLoad(() => request('/lab/count'), [request, refresh])
  const logs = useLoad(() => request('/entryLog?pageNumber=1&pageSize=10'), [request, refresh])
  const dateRange = useMemo(() => Array.from({ length: 7 }, (_, i) => { const d = new Date(); d.setDate(d.getDate() - (6 - i)); return d }), [])
  const [daily, setDaily] = useState({ data: null, loading: true, error: '' })
  useEffect(() => {
    let active = true
    setDaily({ data: null, loading: true, error: '' })
    Promise.all(dateRange.map(async date => {
      const day = localDateKey(date)
      const [accepted, rejected] = await Promise.all([
        request(`/entryLog/count/accepted/today?date=${day}`), request(`/entryLog/count/rejected/today?date=${day}`),
      ])
      return { day, accepted: Number(accepted), rejected: Number(rejected) }
    })).then(data => active && setDaily({ data, loading: false, error: '' })).catch(e => active && setDaily({ data: null, loading: false, error: e.message }))
    return () => { active = false }
  }, [request, refresh, dateRange])
  const todayCounts = daily.data?.at(-1)
  const weekTotal = daily.data?.reduce((n, x) => n + x.accepted + x.rejected, 0) ?? 0
  const recentLogs = logs.data?.items?.slice(0, 6) ?? []
  const todayTotal = (todayCounts?.accepted ?? 0) + (todayCounts?.rejected ?? 0)
  const acceptedRate = todayTotal ? (todayCounts.accepted / todayTotal) * 100 : 0
  return <>
    <PageTitle eyebrow="ADMINISTRATION" title="Overview" description="A clear view of activity across your monitoring system." action={<button className="button button-primary" onClick={() => setPage('labs')}>Manage labs <span>→</span></button>} />
    {(labs.error || logs.error || daily.error) && <ErrorBox message={labs.error || logs.error || daily.error} />}
    <div className="stats-grid">
      <StatCard title="Total labs" value={labs.data ?? '—'} hint="Active monitoring locations" icon="▦" />
      <StatCard title="Accepted today" value={todayCounts?.accepted ?? (daily.loading ? '…' : '—')} hint="Successful entries · view logs" icon="✓" tone="green" onClick={() => { setSelectedLab(''); setLogsPreset('accepted'); setPage('logs') }} />
      <StatCard title="Rejected today" value={todayCounts?.rejected ?? (daily.loading ? '…' : '—')} hint="Denied entries · view logs" icon="×" tone="red" onClick={() => { setSelectedLab(''); setLogsPreset('rejected'); setPage('logs') }} />
      <StatCard title="Entries this week" value={daily.loading ? '…' : weekTotal} hint="Accepted and rejected" icon="↗" tone="blue" />
    </div>
    <div className="dashboard-grid">
      <Panel title="Entry activity" subtitle="Daily entry totals for the last 7 days"><WeekChart data={daily.data} loading={daily.loading} /><div className="chart-legend"><span><i className="legend-dot accepted" />Accepted</span><span><i className="legend-dot rejected" />Rejected</span><small>Hover a day to see its counts</small></div></Panel>
      <Panel title="Today at a glance" subtitle={new Date().toLocaleDateString(undefined, { weekday: 'long', month: 'long', day: 'numeric' })}><div className="today-summary"><div className="ring" style={{ background: `conic-gradient(#4382eb ${acceptedRate}%, #e7eef8 ${acceptedRate}% 100%)` }}><div><strong>{todayTotal}</strong><small>total entries</small></div></div><div className="legend"><span><i className="legend-dot accepted" />Accepted <b>{todayCounts?.accepted ?? (daily.loading ? '…' : 0)}</b></span><span><i className="legend-dot rejected" />Rejected <b>{todayCounts?.rejected ?? (daily.loading ? '…' : 0)}</b></span></div></div></Panel>
    </div>
    <Panel title="Recent entry logs" subtitle="Latest activity recorded by the system" action={<button className="text-button" onClick={() => setPage('logs')}>View all logs <span>→</span></button>}>
      {logs.loading ? <div className="loading">Loading recent activity…</div> : recentLogs.length ? <Table headers={['Time', 'Member', 'Lab', 'Result']}>
        {recentLogs.map(x => <tr key={x.id}><td>{dateText(x.eventTime)}</td><td>{x.member?.fullName || (x.memberId ? `#${x.memberId}` : 'Unidentified')}</td><td>{x.lab?.name || `Lab #${x.labId}`}</td><td><Status value={x.status} /></td></tr>)}
      </Table> : <Empty>No entry logs are available.</Empty>}
    </Panel>
  </>
}
function StatCard({ title, value, hint, icon, tone = 'navy', onClick }) { const contents = <><div className="stat-top"><span>{title}</span><i>{icon}</i></div><strong>{value}</strong><small>{hint}</small></>; return onClick ? <button className={`stat-card tone-${tone} stat-card-button`} onClick={onClick}>{contents}</button> : <div className={`stat-card tone-${tone}`}>{contents}</div> }
function WeekChart({ data, loading }) {
  if (loading) return <div className="loading chart-loading">Loading weekly activity…</div>
  if (!data) return <div className="empty-state">Activity chart is unavailable.</div>
  const max = Math.max(1, ...data.map(x => x.accepted + x.rejected))
  return <div className="chart" aria-label="Accepted and rejected entry counts over the last seven days"><div className="chart-y"><span>{max}</span><span>{Math.round(max / 2)}</span><span>0</span></div><div className="chart-bars">{data.map(row => { const a = row.accepted / max * 100, r = row.rejected / max * 100; const label = new Date(`${row.day}T12:00:00`).toLocaleDateString(undefined, { weekday: 'short', month: 'short', day: 'numeric' }); return <div className="chart-day" key={row.day}><div className="chart-hover-info"><strong>{label}</strong><span>Accepted: {row.accepted}</span><span>Rejected: {row.rejected}</span><b>Total: {row.accepted + row.rejected}</b></div><div className="bar-stack"><div className="bar rejected-bar" style={{ height: `${r}%` }} /><div className="bar accepted-bar" style={{ height: `${a}%` }} /></div><span>{label.split(',')[0]}</span></div> })}</div></div>
}

function Labs({ request, refresh, notify, askConfirm, setSelectedLab, goToPage, openModal }) {
  const [page, setPage] = useState(1)
  const load = useLoad(() => request(`/lab?pageNumber=${page}&pageSize=10`), [request, refresh, page])
  const remove = lab => askConfirm({ title: 'Delete this lab?', message: `${lab.name} will be deleted. This may also affect records linked to the lab.`, confirmText: 'Delete lab', danger: true }, async () => { try { await request(`/lab/${lab.id}`, { method: 'DELETE' }); notify('Lab deleted.'); load.refresh() } catch (e) { notify(e.message, true) } })
  return <>
    <PageTitle eyebrow="FACILITIES" title="Labs" description="Create and manage labs, then inspect the entry activity for each one." action={<button className="button button-primary" onClick={() => openModal('new')}>＋ Add lab</button>} />
    <Panel title="All labs" subtitle={load.data ? `${load.data.totalCount} labs in the system` : 'Manage lab details and activity'}>
      {load.error ? <ErrorBox message={load.error} retry={load.refresh} /> : load.loading ? <div className="loading">Loading labs…</div> : load.data?.items?.length ? <><Table headers={['Lab', 'Capacity', 'ID', 'Actions']}>
        {load.data.items.map(lab => <tr key={lab.id}><td><div className="lab-cell"><span className="lab-avatar">{lab.name?.slice(0, 1)?.toUpperCase() || 'L'}</span><strong>{lab.name}</strong></div></td><td>{lab.capacity} people</td><td className="muted">#{lab.id}</td><td><div className="row-actions"><button className="button button-light" onClick={() => { setSelectedLab(String(lab.id)); goToPage('logs') }}>View logs</button><button className="icon-button" title="Edit lab" onClick={() => openModal(lab)}>✎</button><button className="icon-button danger" title="Delete lab" onClick={() => remove(lab)}>⌫</button></div></td></tr>)}
      </Table><Pager page={page} total={load.data.totalPages} onPage={setPage} /></> : <Empty>No labs yet. Add a lab to begin.</Empty>}
    </Panel>
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
  return <div className="modal-backdrop" onMouseDown={e => e.target === e.currentTarget && onClose()}><section className="modal"><div className="modal-heading"><div><span className="eyebrow">LAB DETAILS</span><h2>{item ? 'Edit lab' : 'Add a lab'}</h2></div><button className="icon-button" onClick={onClose}>×</button></div><form onSubmit={submit} className="form-stack"><label>Lab name<input name="name" required defaultValue={item?.name || ''} placeholder="e.g. Computer Lab 01" /></label><label>Capacity<input name="capacity" type="number" required min="1" defaultValue={item?.capacity ?? ''} placeholder="Maximum people" /></label><div className="modal-actions"><button type="button" className="button button-light" onClick={onClose}>Cancel</button><button className="button button-primary" disabled={busy}>{busy ? 'Saving…' : item ? 'Save changes' : 'Create lab'}</button></div></form></section></div>
}

function Logs({ request, refresh, selectedLab, setSelectedLab, preset, setPreset }) {
  const [page, setPage] = useState(1)
  const [record, setRecord] = useState(null)
  const labs = useLoad(() => request('/lab?pageNumber=1&pageSize=1000'), [request, refresh])
  const load = useLoad(() => {
    if (preset === 'accepted') return request(`/entryLog/AcceptedToday?PageNumber=${page}&PageSize=10`)
    if (preset === 'rejected') return request(`/entryLog/RejectedToday?PageNumber=${page}&PageSize=10`)
    if (selectedLab) return request(`/entryLog/ByLab?LabId=${selectedLab}&PageNumber=${page}&PageSize=10`)
    return request(`/entryLog?pageNumber=${page}&pageSize=10`)
  }, [request, refresh, selectedLab, preset, page])
  const filterLabel = preset === 'accepted' ? 'Accepted today' : preset === 'rejected' ? 'Rejected today' : selectedLab ? `Lab ${selectedLab}` : 'All labs'
  return <><PageTitle eyebrow="ACTIVITY" title="Entry logs" description="Review entry decisions across all labs or focus on a single location." />
    <Panel title="Entry history" subtitle={load.data ? `${load.data.totalCount} records · ${filterLabel}` : 'Most recent records appear first'} action={<div className="panel-controls">{preset !== 'all' && <button className="text-button" onClick={() => { setPreset('all'); setPage(1) }}>Clear status filter</button>}<select className="select-control" value={selectedLab} onChange={e => { setSelectedLab(e.target.value); setPreset('all'); setPage(1) }}><option value="">All labs</option>{labs.data?.items?.map(l => <option key={l.id} value={l.id}>{l.name}</option>)}</select></div>}>
      {load.error ? <ErrorBox message={load.error} retry={load.refresh} /> : load.loading ? <div className="loading">Loading entry logs…</div> : load.data?.items?.length ? <><Table headers={['Time', 'Member', 'Face image', 'Lab', 'Decision']}>
        {load.data.items.map(x => <tr key={x.id} className="clickable-row" tabIndex="0" onClick={() => setRecord(x)} onKeyDown={e => e.key === 'Enter' && setRecord(x)}><td>{dateText(x.eventTime)}</td><td>{x.member?.fullName || (x.memberId ? `#${x.memberId}` : 'Unidentified')}</td><td>{x.faceImage || x.member?.faceImage ? avatar(x.faceImage || x.member?.faceImage, x.member?.fullName) : <span className="muted">—</span>}</td><td>{x.lab?.name || `Lab #${x.labId}`}</td><td><Status value={x.status} /></td></tr>)}
      </Table><Pager page={page} total={load.data.totalPages} onPage={setPage} /></> : <Empty>No logs match this selection.</Empty>}
    </Panel>
    <RecordDetails title={`Entry log #${record?.id ?? ''}`} record={record} onClose={() => setRecord(null)} fields={record ? [['Log ID', `#${record.id}`], ['Event time', dateText(record.eventTime)], ['Decision', enumName(record.status, ['Rejected', 'Accepted'])], ['Member', record.member?.fullName || 'Unidentified'], ['Member ID', record.memberId ? `#${record.memberId}` : '—'], ['Email', record.member?.email || '—'], ['Lab', record.lab?.name || `Lab #${record.labId}`]] : []} images={record ? [['Face image', record.faceImage || record.member?.faceImage]] : []} />
  </>
}

function Members({ request, refresh, notify, askConfirm }) {
  const [page, setPage] = useState(1)
  const [draft, setDraft] = useState({ Id: '', FullName: '', Email: '', AcademicYear: '' })
  const [filters, setFilters] = useState({})
  const [record, setRecord] = useState(null)
  const params = new URLSearchParams({ PageNumber: String(page), PageSize: '10' })
  Object.entries(filters).forEach(([key, value]) => { if (value !== '') params.set(key, value) })
  const query = params.toString()
  const load = useLoad(() => request(`/member/search?${query}`), [request, refresh, query])
  const remove = member => askConfirm({ title: 'Delete this member?', message: `The member record for ${member.fullName} will be permanently removed.`, confirmText: 'Delete member', danger: true }, async () => { try { await request(`/member/${member.id}`, { method: 'DELETE' }); notify('Member deleted.'); load.refresh() } catch (e) { notify(e.message, true) } })
  return <><PageTitle eyebrow="DIRECTORY" title="Members" description="Browse registered members and review their profile details." />
    <Panel title="All members" subtitle={load.data ? `${load.data.totalCount} matching members` : 'Member records'}>
      <FilterBar fields={[{ name: 'Id', label: 'Member ID', type: 'number', min: 1 }, { name: 'FullName', label: 'Full name', placeholder: 'Search by name' }, { name: 'Email', label: 'Email', placeholder: 'Search by email' }, { name: 'AcademicYear', label: 'Academic year', options: [['1', 'Year 1'], ['2', 'Year 2'], ['3', 'Year 3'], ['4', 'Year 4']] }]} values={draft} onChange={(key, value) => setDraft(s => ({ ...s, [key]: value }))} onSubmit={e => { e.preventDefault(); setPage(1); setFilters({ ...draft }) }} onClear={() => { setDraft({ Id: '', FullName: '', Email: '', AcademicYear: '' }); setFilters({}); setPage(1) }} />
      {load.error ? <ErrorBox message={load.error} retry={load.refresh} /> : load.loading ? <div className="loading">Loading members…</div> : load.data?.items?.length ? <><Table headers={['Member', 'Email', 'Academic year', 'ID', 'Actions']}>
        {load.data.items.map(m => <tr key={m.id} className="clickable-row" tabIndex="0" onClick={() => setRecord(m)} onKeyDown={e => e.key === 'Enter' && setRecord(m)}><td><div className="person-cell">{avatar(m.faceImage, m.fullName)}<strong>{m.fullName}</strong></div></td><td>{m.email}</td><td>Year {m.academicYear}</td><td className="muted">#{m.id}</td><td><button className="icon-button danger" title="Delete member" onClick={e => { e.stopPropagation(); remove(m) }}>⌫</button></td></tr>)}
      </Table><Pager page={page} total={load.data.totalPages} onPage={setPage} /></> : <Empty>No members found.</Empty>}
    </Panel>
    <RecordDetails title={record?.fullName || 'Member'} record={record} onClose={() => setRecord(null)} fields={record ? [['Member ID', `#${record.id}`], ['Full name', record.fullName], ['Email', record.email], ['Academic year', `Year ${record.academicYear}`]] : []} images={record ? [['Face image', record.faceImage], ['ID image', record.idImage]] : []} />
  </>
}
function avatar(bytes, name) { return <span className="person-avatar">{bytes ? <img src={`data:image/jpeg;base64,${bytes}`} alt="" /> : name?.slice(0, 1)?.toUpperCase()}</span> }

function Accounts({ request, refresh, notify, askConfirm }) {
  const [page, setPage] = useState(1)
  const [editing, setEditing] = useState(null)
  const [create, setCreate] = useState(false)
  const [draft, setDraft] = useState({ Name: '', Email: '', role: '', IsActive: '' })
  const [filters, setFilters] = useState({})
  const params = new URLSearchParams({ PageNumber: String(page), PageSize: '10' })
  // The current backend service swaps Name and Email when building its search specification.
  // Send the inverse query keys so each UI field still filters the intended account property.
  if (filters.Name) params.set('Email', filters.Name)
  if (filters.Email) params.set('Name', filters.Email)
  if (filters.role) params.set('role', filters.role)
  if (filters.IsActive) params.set('IsActive', filters.IsActive)
  const query = params.toString()
  const load = useLoad(() => request(`/dashboardAccount/search?${query}`), [request, refresh, query])
  const update = async (account, fields) => { try { await request(`/dashboardAccount/${account.id}`, { method: 'PATCH', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(fields) }); notify('Account updated.'); load.refresh() } catch (e) { notify(e.message, true) } }
  const toggle = account => { const action = account.isActive ? 'deactivate' : 'activate'; askConfirm({ title: `${account.isActive ? 'Deactivate' : 'Activate'} this account?`, message: `${account.fullName} will ${account.isActive ? 'lose' : 'regain'} dashboard access.`, confirmText: `${account.isActive ? 'Deactivate' : 'Activate'} account`, danger: account.isActive }, async () => { try { await request(`/dashboardAccount/${action}/${account.id}`, { method: 'PATCH' }); notify(`Account ${action}d.`); load.refresh() } catch (e) { notify(e.message, true) } }) }
  const remove = account => askConfirm({ title: 'Delete this account?', message: `The dashboard account for ${account.fullName} will be permanently removed.`, confirmText: 'Delete account', danger: true }, async () => { try { await request(`/dashboardAccount/${account.id}`, { method: 'DELETE' }); notify('Account deleted.'); load.refresh() } catch (e) { notify(e.message, true) } })
  return <><PageTitle eyebrow="ACCESS" title="Dashboard accounts" description="Create accounts and manage names, roles, and access status." action={<button className="button button-primary" onClick={() => setCreate(true)}>＋ Add account</button>} />
    <Panel title="All accounts" subtitle={load.data ? `${load.data.totalCount} matching dashboard accounts` : 'Account access and roles'}>
      <FilterBar fields={[{ name: 'Name', label: 'Full name', placeholder: 'Search by name' }, { name: 'Email', label: 'Email', placeholder: 'Search by email' }, { name: 'role', label: 'Role', options: [['1', 'Admin'], ['2', 'User']] }, { name: 'IsActive', label: 'Access status', options: [['true', 'Active'], ['false', 'Inactive']] }]} values={draft} onChange={(key, value) => setDraft(s => ({ ...s, [key]: value }))} onSubmit={e => { e.preventDefault(); setPage(1); setFilters({ ...draft }) }} onClear={() => { setDraft({ Name: '', Email: '', role: '', IsActive: '' }); setFilters({}); setPage(1) }} />
      {load.error ? <ErrorBox message={load.error} retry={load.refresh} /> : load.loading ? <div className="loading">Loading accounts…</div> : load.data?.items?.length ? <><Table headers={['Account', 'Email', 'Role', 'Status', 'Actions']}>
        {load.data.items.map(a => <tr key={a.id}><td><div className="account-cell"><span className="initial-avatar">{a.fullName?.slice(0, 1)?.toUpperCase()}</span><strong>{a.fullName}</strong></div></td><td>{a.email}</td><td><span className="role-pill">{enumName(a.role, { 1: 'Admin', 2: 'User' })}</span></td><td><span className={`access-state ${a.isActive ? 'is-active' : 'is-inactive'}`}><i />{a.isActive ? 'Active' : 'Inactive'}</span></td><td><div className="row-actions"><button className="button button-light" onClick={() => setEditing(a)}>Edit</button><button className="button button-light" onClick={() => toggle(a)}>{a.isActive ? 'Deactivate' : 'Activate'}</button><button className="icon-button danger" title="Delete account" onClick={() => remove(a)}>⌫</button></div></td></tr>)}
      </Table><Pager page={page} total={load.data.totalPages} onPage={setPage} /></> : <Empty>No dashboard accounts found.</Empty>}
    </Panel>
    {(create || editing) && <AccountModal item={editing} onClose={() => { setCreate(false); setEditing(null) }} request={request} onSaved={() => { setCreate(false); setEditing(null); notify(editing ? 'Account updated.' : 'Account created.'); load.refresh() }} onError={e => notify(e.message, true)} />}
  </>
}
function AccountModal({ item, onClose, request, onSaved, onError }) {
  const [busy, setBusy] = useState(false)
  async function submit(e) {
    e.preventDefault(); setBusy(true); const data = new FormData(e.currentTarget)
    const fields = { fullName: data.get('fullName'), email: data.get('email'), role: Number(data.get('role')) }
    if (data.get('password')) fields.password = data.get('password')
    try {
      await request(item ? `/dashboardAccount/${item.id}` : '/dashboardAccount/create', { method: item ? 'PATCH' : 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(fields) })
      onSaved()
    } catch (error) { onError(error) } finally { setBusy(false) }
  }
  return <div className="modal-backdrop" onMouseDown={e => e.target === e.currentTarget && onClose()}><section className="modal"><div className="modal-heading"><div><span className="eyebrow">ACCOUNT ACCESS</span><h2>{item ? 'Edit account' : 'Add an account'}</h2></div><button className="icon-button" onClick={onClose}>×</button></div><form onSubmit={submit} className="form-stack"><label>Full name<input name="fullName" required defaultValue={item?.fullName || ''} /></label><label>Email address<input name="email" type="email" required defaultValue={item?.email || ''} /></label><label>{item ? 'New password (optional)' : 'Password'}<input name="password" type="password" required={!item} minLength="8" /></label><label>Role<select name="role" defaultValue={item?.role ?? 2}><option value="1">Admin</option><option value="2">User</option></select></label><div className="modal-actions"><button type="button" className="button button-light" onClick={onClose}>Cancel</button><button className="button button-primary" disabled={busy}>{busy ? 'Saving…' : item ? 'Save changes' : 'Create account'}</button></div></form></section></div>
}

function Requests({ request, refresh, notify, askConfirm }) {
  const [page, setPage] = useState(1)
  const [draft, setDraft] = useState({ Id: '', FullName: '', Email: '', AcademicYear: '', Status: '' })
  const [filters, setFilters] = useState({})
  const [record, setRecord] = useState(null)
  const params = new URLSearchParams({ PageNumber: String(page), PageSize: '10' })
  Object.entries(filters).forEach(([key, value]) => { if (value !== '') params.set(key, value) })
  const query = params.toString()
  const load = useLoad(() => request(`/registrationRequest/search?${query}`), [request, refresh, query])
  function decide(item, action, onDone = () => {}) {
    const approving = action === 'approve'
    askConfirm({ title: `${approving ? 'Approve' : 'Reject'} this request?`, message: `${item.fullName}'s registration request will be marked ${approving ? 'accepted' : 'rejected'}.`, confirmText: approving ? 'Approve request' : 'Reject request', danger: !approving }, async () => {
      try { await request(`/registrationRequest/${action}/${item.id}`, { method: 'PATCH' }); notify(`Request ${approving ? 'approved' : 'rejected'}.`); load.refresh(); onDone() }
      catch (e) { notify(e.message, true) }
    })
  }
  return <><PageTitle eyebrow="ENROLLMENT" title="Registration requests" description="Review new member applications and approve or reject each request." />
    <Panel title="Incoming requests" subtitle={load.data ? `${load.data.totalCount} matching requests` : 'Review submissions'}>
      <FilterBar fields={[{ name: 'Id', label: 'Request ID', type: 'number', min: 1 }, { name: 'FullName', label: 'Full name', placeholder: 'Search by name' }, { name: 'Email', label: 'Email', placeholder: 'Search by email' }, { name: 'AcademicYear', label: 'Academic year', options: [['1', 'Year 1'], ['2', 'Year 2'], ['3', 'Year 3'], ['4', 'Year 4']] }, { name: 'Status', label: 'Status', options: [['0', 'Pending'], ['1', 'Rejected'], ['2', 'Accepted']] }]} values={draft} onChange={(key, value) => setDraft(s => ({ ...s, [key]: value }))} onSubmit={e => { e.preventDefault(); setPage(1); setFilters({ ...draft }) }} onClear={() => { setDraft({ Id: '', FullName: '', Email: '', AcademicYear: '', Status: '' }); setFilters({}); setPage(1) }} />
      {load.error ? <ErrorBox message={load.error} retry={load.refresh} /> : load.loading ? <div className="loading">Loading requests…</div> : load.data?.items?.length ? <><Table headers={['Applicant', 'Email', 'Member ID', 'Year', 'Submitted', 'Status', 'Actions']}>
        {load.data.items.map(x => <tr key={x.id} className="clickable-row" tabIndex="0" onClick={() => setRecord(x)} onKeyDown={e => e.key === 'Enter' && setRecord(x)}><td><div className="person-cell">{avatar(x.faceImage, x.fullName)}<strong>{x.fullName}</strong></div></td><td>{x.email}</td><td>#{x.memberId}</td><td>Year {x.academicYear}</td><td>{dateText(x.submittedAt)}</td><td><Status kind="request" value={x.status} /></td><td>{x.status === 0 ? <div className="row-actions"><button className="button button-primary small" onClick={e => { e.stopPropagation(); decide(x, 'approve') }}>Approve</button><button className="button button-light small" onClick={e => { e.stopPropagation(); decide(x, 'reject') }}>Reject</button></div> : <span className="muted">Processed</span>}</td></tr>)}
      </Table><Pager page={page} total={load.data.totalPages} onPage={setPage} /></> : <Empty>No registration requests.</Empty>}
    </Panel>
    <RecordDetails title={record?.fullName || 'Registration request'} record={record} onClose={() => setRecord(null)} fields={record ? [['Request ID', `#${record.id}`], ['Member ID', `#${record.memberId}`], ['Full name', record.fullName], ['Email', record.email], ['Academic year', `Year ${record.academicYear}`], ['Submitted', dateText(record.submittedAt)], ['Status', <Status kind="request" value={record.status} />]] : []} images={record ? [['Face image', record.faceImage], ['ID image', record.idImage]] : []} actions={record?.status === 0 && <><button className="button button-light small" onClick={() => decide(record, 'reject', () => setRecord(null))}>Reject</button><button className="button button-primary small" onClick={() => decide(record, 'approve', () => setRecord(null))}>Approve</button></>} />
  </>
}
function Warnings({ request, refresh, notify, askConfirm }) {
  const [page, setPage] = useState(1)
  const [filterType, setFilterType] = useState('all')
  const [filterValue, setFilterValue] = useState('')
  const [record, setRecord] = useState(null)
  const labs = useLoad(() => request('/lab?pageNumber=1&pageSize=1000'), [request, refresh])
  const load = useLoad(() => {
    if (filterType === 'alert' && filterValue !== '') return request(`/warning/alert/${filterValue}?pageNumber=${page}&pageSize=10`)
    if (filterType === 'lab' && filterValue !== '') return request(`/warning/lab/${filterValue}?pageNumber=${page}&pageSize=10`)
    return request(`/warning?pageNumber=${page}&pageSize=10`)
  }, [request, refresh, page, filterType, filterValue])
  const alertName = value => enumName(value, ['Food or drink detected', 'Crowded'])
  const deleteWarning = warning => askConfirm({ title: 'Delete this warning?', message: `Warning #${warning.id} for ${warning.lab?.name || `lab #${warning.labId}`} will be permanently removed.`, confirmText: 'Delete warning', danger: true }, async () => {
    try { await request(`/warning/${warning.id}`, { method: 'DELETE' }); notify('Warning deleted.'); load.refresh() }
    catch (e) { notify(e.message, true) }
  })
  const openWarning = async warning => {
    try { setRecord(await request(`/warning/${warning.id}`)) }
    catch (e) { notify(e.message, true) }
  }
  const detailLab = record ? record.lab?.name || labs.data?.items?.find(l => l.id === record.labId)?.name || `Lab #${record.labId}` : ''
  return <>
    <PageTitle eyebrow="MONITORING" title="Warnings" description="Review alerts raised across your labs." />
    <Panel title="Warnings" subtitle={load.data ? `${load.data.totalCount} records` : 'Lab alerts and conditions'} action={<select className="select-control" value={filterType} onChange={e => { setFilterType(e.target.value); setFilterValue(''); setPage(1) }}><option value="all">All warnings</option><option value="alert">Filter by alert</option><option value="lab">Filter by lab</option></select>}>
      {filterType !== 'all' && <div className="warning-filter-value"><label>{filterType === 'alert' ? 'Alert type' : 'Lab'}{filterType === 'alert' ? <select className="select-control" value={filterValue} onChange={e => { setFilterValue(e.target.value); setPage(1) }}><option value="">Choose alert</option><option value="Food_Or_Drink_Detected">Food or drink detected</option><option value="Crowded">Crowded</option></select> : <select className="select-control" value={filterValue} onChange={e => { setFilterValue(e.target.value); setPage(1) }}><option value="">Choose lab</option>{labs.data?.items?.map(l => <option value={l.id} key={l.id}>{l.name}</option>)}</select>}</label></div>}
      {load.error ? <ErrorBox message={load.error} retry={load.refresh} /> : load.loading ? <div className="loading">Loading warnings…</div> : load.data?.items?.length ? <><Table headers={['Alert', 'Lab', 'Lab ID', 'Actions']}>
        {load.data.items.map(w => <tr key={w.id} className="clickable-row" tabIndex="0" onClick={() => openWarning(w)} onKeyDown={e => e.key === 'Enter' && openWarning(w)}><td><span className="warning-name"><i>!</i>{alertName(w.warning)}</span></td><td>{w.lab?.name || `Lab #${w.labId}`}</td><td className="muted">#{w.labId}</td><td><button className="icon-button danger" title="Delete warning" onClick={e => { e.stopPropagation(); deleteWarning(w) }}>⌫</button></td></tr>)}
      </Table><Pager page={page} total={load.data.totalPages} onPage={setPage} /></> : <Empty>No warnings found.</Empty>}
    </Panel>
    <RecordDetails title={`Warning #${record?.id ?? ''}`} record={record} onClose={() => setRecord(null)} fields={record ? [['Warning ID', `#${record.id}`], ['Alert', alertName(record.warning)], ['Lab', detailLab]] : []} />
  </>
}

export default App
