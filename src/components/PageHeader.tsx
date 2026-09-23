type PageHeaderProps = {
  eyebrow: string
  title: string
  description: string
}

export function PageHeader({ eyebrow, title, description }: PageHeaderProps) {
  return (
    <header className="space-y-3">
      <p className="text-sm font-semibold uppercase tracking-[0.2em] text-orange-600">
        {eyebrow}
      </p>
      <h1 className="text-4xl font-bold tracking-tight text-slate-950">{title}</h1>
      <p className="max-w-2xl text-base leading-7 text-slate-600">{description}</p>
    </header>
  )
}
