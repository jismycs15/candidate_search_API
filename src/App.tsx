import { useState } from 'react'
import { LoginPage } from './pages/LoginPage'
import { SearchPage } from './pages/SearchPage'
import { isLoggedIn } from './services/api'

export function App() {
  const [loggedIn, setLoggedIn] = useState(isLoggedIn)

  if (!loggedIn) {
    return <LoginPage onLoggedIn={() => setLoggedIn(true)} />
  }

  return <SearchPage onUnauthorized={() => setLoggedIn(false)} />
}
