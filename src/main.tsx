import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import { TemplatePage } from './pages/TemplatePage'

createRoot(document.getElementById('app')!).render(
  <StrictMode>
    <TemplatePage />
  </StrictMode>,
)
