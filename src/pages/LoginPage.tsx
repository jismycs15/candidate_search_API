import { useState, type FormEvent } from 'react'
import { Input } from '../components/Input'
import { login } from '../services/api'

export function LoginPage({ onLoggedIn }: { onLoggedIn: () => void }) {
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')

  async function submit(event: FormEvent) {
    event.preventDefault()
    setLoading(true)
    setError('')
    try {
      await login(email, password)
      onLoggedIn()
    } catch (reason) {
      setError(reason instanceof Error ? reason.message : 'Unable to log in.')
    } finally {
      setLoading(false)
    }
  }

  return (
    <main className="login-shell">
      <section className="login-art">
        <div className="logo-mark">T</div>
        <p className="kicker">Talent intelligence</p>
        <h1>Find the people who move work forward.</h1>
        <p>Search your candidate universe with clarity, context, and less busywork.</p>
        <div className="art-rule" />
        <span>Curated for modern recruiting teams</span>
      </section>
      <section className="login-panel">
        <div className="login-form">
          <p className="kicker orange">Welcome back</p>
          <h2>Sign in to TalentGrid</h2>
          <p className="muted">Your candidate workspace is ready.</p>
          <form onSubmit={submit}>
            <Input
              label="Email address"
              type="email"
              autoComplete="email"
              value={email}
              onChange={(event) => setEmail(event.target.value)}
              required
            />
            <Input
              label="Password"
              type="password"
              autoComplete="current-password"
              value={password}
              onChange={(event) => setPassword(event.target.value)}
              required
            />
            {error && <p className="alert error">{error}</p>}
            <button className="primary-button" disabled={loading}>
              {loading ? 'Signing in...' : 'Sign in'} <span>&rarr;</span>
            </button>
          </form>
        </div>
      </section>
    </main>
  )
}
