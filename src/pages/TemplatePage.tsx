import { PageHeader } from '../components/PageHeader'

export function TemplatePage() {
  return (
    <main className="min-h-screen bg-slate-100 px-6 py-12 text-slate-950 sm:px-10">
      <div className="mx-auto max-w-5xl space-y-10">
        <PageHeader
          eyebrow="Candidate search"
          title="Your React page template"
          description="This page is ready for your candidate search workflow. Build from here with regular React components and Tailwind className utilities."
        />

        <section className="grid gap-6 md:grid-cols-3">
          <article className="rounded-xl border border-slate-200 bg-white p-6 shadow-sm md:col-span-2">
            <h2 className="text-xl font-semibold text-slate-900">Start with a feature section</h2>
            <p className="mt-3 text-slate-600">
              Place search controls, results, and filters in this area as you build the assessment.
            </p>
          </article>

          <aside className="rounded-xl bg-orange-500 p-6 text-white shadow-sm">
            <p className="text-sm font-semibold uppercase tracking-wide text-orange-100">Ready</p>
            <p className="mt-3 text-3xl font-bold">React + Tailwind</p>
          </aside>
        </section>
      </div>
    </main>
  )
}
