import { useEffect, useMemo, useRef, useState, type FormEvent } from 'react'
import { Input } from '../components/Input'
import { useDebouncedValue } from '../hooks/useDebouncedValue'
import {
  emptyFilters,
  filtersToSearchParams,
  logout,
  searchCandidates,
  searchParamsToFilters,
  UnauthorizedError,
  type CandidateFilters,
  type SearchResponse,
} from '../services/api'

const EDUCATION_OPTIONS = ['B.Tech', 'M.Tech', 'MBA', 'MCA', 'PhD']
const PAGE_SIZE = 10
const DEBOUNCE_MS = 450

function formatDate(value: string | null): string {
  if (!value) return '—'
  return new Date(value).toLocaleDateString('en-IN', { day: '2-digit', month: 'short', year: 'numeric' })
}

export function SearchPage({ onUnauthorized }: { onUnauthorized: () => void }) {
  const initial = useMemo(() => searchParamsToFilters(new URLSearchParams(window.location.search)), [])
  const [filters, setFilters] = useState<CandidateFilters>(initial.filters)
  const [page, setPage] = useState(initial.page)
  const [results, setResults] = useState<SearchResponse | null>(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  const debouncedFilters = useDebouncedValue(filters, DEBOUNCE_MS)
  const abortRef = useRef<AbortController | null>(null)

  async function runSearch(searchFilters: CandidateFilters, searchPage: number) {
    abortRef.current?.abort()
    const controller = new AbortController()
    abortRef.current = controller

    setLoading(true)
    setError('')
    try {
      const data = await searchCandidates(searchFilters, searchPage, PAGE_SIZE, controller.signal)
      setResults(data)
      setPage(searchPage)
    } catch (reason) {
      if (controller.signal.aborted) return // superseded by a newer request
      if (reason instanceof UnauthorizedError) {
        onUnauthorized()
        return
      }
      setError(reason instanceof Error ? reason.message : 'Search failed. Please try again.')
    } finally {
      if (!controller.signal.aborted) setLoading(false)
    }
  }

  // Debounced search-as-you-type: fires whenever the (debounced) filters or the page changes.
  // eslint-disable-next-line react-hooks/exhaustive-deps
  useEffect(() => {
    void runSearch(debouncedFilters, page)
  }, [debouncedFilters, page])

  // Keep the URL's query string in sync with the live (non-debounced) filter state so a
  // reload or shared link restores the same search.
  useEffect(() => {
    const params = filtersToSearchParams(filters, page, PAGE_SIZE)
    const query = params.toString()
    const url = query ? `${window.location.pathname}?${query}` : window.location.pathname
    window.history.replaceState(null, '', url)
  }, [filters, page])

  function setFilter<K extends keyof CandidateFilters>(key: K, value: CandidateFilters[K]) {
    setFilters((current) => ({ ...current, [key]: value }))
    setPage(1)
  }

  function submit(event: FormEvent) {
    event.preventDefault()
    void runSearch(filters, 1)
  }

  function reset() {
    setFilters(emptyFilters)
    void runSearch(emptyFilters, 1)
  }

  async function handleLogout() {
    await logout()
    onUnauthorized()
  }

  const totalPages = results ? Math.max(1, Math.ceil(results.totalCount / PAGE_SIZE)) : 1

  return (
    <main className="app-shell">
      <header className="topbar">
        <div className="brand">
          <span className="logo-mark small">T</span>
          <span>TalentGrid</span>
        </div>
        <div className="topbar-right">
          <button className="logout-button" onClick={() => void handleLogout()}>
            Log out
          </button>
        </div>
      </header>
      <div className="content">
        <div className="page-intro">
          <div>
            <p className="kicker orange">Candidate directory</p>
            <h1>Find your next great hire</h1>
            <p className="muted">Search, filter, and compare candidates across your talent pipeline.</p>
          </div>
          <div className="intro-stat">
            <strong>{results?.totalCount ?? '--'}</strong>
            <span>matching candidates</span>
          </div>
        </div>

        <form className="filter-panel" onSubmit={submit}>
          <div className="filter-heading">
            <div>
              <h2>Refine your search</h2>
              <p>Combine filters to narrow the candidate pool. Results update as you type.</p>
            </div>
            <button type="button" className="reset-button" onClick={reset}>
              Reset filters
            </button>
          </div>
          <div className="filter-grid">
            <div className="age-fields">
              <Input
                label="Age range"
                type="number"
                min={0}
                max={120}
                value={filters.minAge}
                onChange={(event) => setFilter('minAge', event.target.value === '' ? '' : Number(event.target.value))}
              />
              <span className="dash">to</span>
              <Input
                label=" "
                type="number"
                min={0}
                max={120}
                value={filters.maxAge}
                onChange={(event) => setFilter('maxAge', event.target.value === '' ? '' : Number(event.target.value))}
              />
            </div>

            <label className="field">
              <span>Gender</span>
              <select value={filters.gender} onChange={(event) => setFilter('gender', event.target.value)}>
                <option value="">Any gender</option>
                <option>Female</option>
                <option>Male</option>
                <option>Other</option>
              </select>
            </label>

            <Input
              label="Location"
              placeholder="e.g. Kochi"
              value={filters.location}
              onChange={(event) => setFilter('location', event.target.value)}
            />

            <fieldset className="education-field">
              <legend>Education</legend>
              <div className="checkboxes">
                {EDUCATION_OPTIONS.map((option) => (
                  <label key={option}>
                    <input
                      type="checkbox"
                      checked={filters.education.includes(option)}
                      onChange={(event) =>
                        setFilter(
                          'education',
                          event.target.checked
                            ? [...filters.education, option]
                            : filters.education.filter((item) => item !== option),
                        )
                      }
                    />{' '}
                    {option}
                  </label>
                ))}
              </div>
            </fieldset>

            <div className="date-group">
              <span className="date-label">Created date</span>
              <Input label="From" type="date" value={filters.createdFrom} onChange={(event) => setFilter('createdFrom', event.target.value)} />
              <Input label="To" type="date" value={filters.createdTo} onChange={(event) => setFilter('createdTo', event.target.value)} />
            </div>

            <div className="date-group">
              <span className="date-label">Last login</span>
              <Input label="From" type="date" value={filters.lastLoginFrom} onChange={(event) => setFilter('lastLoginFrom', event.target.value)} />
              <Input label="To" type="date" value={filters.lastLoginTo} onChange={(event) => setFilter('lastLoginTo', event.target.value)} />
            </div>
          </div>
          <button className="primary-button search-button" disabled={loading}>
            {loading ? 'Searching...' : 'Search candidates'} <span>&rarr;</span>
          </button>
        </form>

        <section className="results-section">
          <div className="results-heading">
            <div>
              <h2>Candidate results</h2>
              <p>{results ? `${results.totalCount} profiles in your current search` : 'Loading your candidate directory'}</p>
            </div>
            {loading && <span className="loading-label">Loading results...</span>}
          </div>

          {error && <p className="alert error">{error}</p>}

          {!loading && results?.totalCount === 0 && (
            <div className="empty-state">
              <span className="empty-icon">⌕</span>
              <h3>No candidates match your filters</h3>
              <p>Try widening your age range or removing a filter.</p>
            </div>
          )}

          {results && results.totalCount > 0 && (
            <>
              <div className="table-wrap">
                <table>
                  <thead>
                    <tr>
                      <th>Name</th>
                      <th>Age</th>
                      <th>Education</th>
                      <th>Location</th>
                      <th>Created</th>
                      <th>Last login</th>
                    </tr>
                  </thead>
                  <tbody>
                    {results.items.map((candidate) => (
                      <tr key={candidate.id}>
                        <td>
                          <strong>{candidate.name || '—'}</strong>
                        </td>
                        <td>{candidate.age}</td>
                        <td>
                          <span className="education-pill">{candidate.education || '—'}</span>
                        </td>
                        <td>{candidate.location || '—'}</td>
                        <td>{formatDate(candidate.createdAt)}</td>
                        <td>{formatDate(candidate.lastLoginAt)}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
              <div className="pagination">
                <span>
                  Showing {(page - 1) * PAGE_SIZE + 1}-{Math.min(page * PAGE_SIZE, results.totalCount)} of {results.totalCount}
                </span>
                <div>
                  <button onClick={() => void runSearch(filters, page - 1)} disabled={page === 1 || loading}>
                    &larr; Previous
                  </button>
                  <span className="page-number">
                    {page} / {totalPages}
                  </span>
                  <button onClick={() => void runSearch(filters, page + 1)} disabled={page === totalPages || loading}>
                    Next &rarr;
                  </button>
                </div>
              </div>
            </>
          )}
        </section>
      </div>
    </main>
  )
}
