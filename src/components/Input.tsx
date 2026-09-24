import type { InputHTMLAttributes } from 'react'

type InputProps = { label: string } & InputHTMLAttributes<HTMLInputElement>

export function Input({ label, ...props }: InputProps) {
  return (
    <label className="field">
      <span>{label}</span>
      <input {...props} />
    </label>
  )
}
