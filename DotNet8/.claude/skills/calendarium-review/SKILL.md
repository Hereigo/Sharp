---
name: calendarium-review
description: Review C#, Razor, CSS and JS code in the Calendarium ASP.NET Core MVC project and report concrete problems with file:line references. Use this whenever the user asks to review, check, audit or "look over" code in this project — uncommitted changes, a specific file or controller, a folder, or the whole app — even if they just say "review this" or "anything wrong here?".
---

# Calendarium code review

A lightweight review for this project: find real problems, explain why they matter, suggest a fix. It is a read-only skill — don't edit files unless the user asks for fixes afterwards.

## 1. Decide what to review

- User named files/folders → review those.
- Otherwise → the uncommitted changes: `git diff HEAD -- .` plus untracked files from `git status --porcelain .` (the git root is the parent `Sharp` repo, so keep the `.` pathspec to stay inside this project).
- No changes and nothing named → ask the user what to review rather than reviewing the whole repo.

Skip generated or vendored code: `bin/`, `obj/`, `Migrations/*.Designer.cs`, `ApplicationDbContextModelSnapshot.cs`, `wwwroot/lib/`, `*.min.css`, `*.min.js`.

Read the full file around each change, not just the diff hunk — most real bugs come from how the change interacts with surrounding code.

## 2. What to look for

Focus on things that would actually break or hurt, roughly in this order:

1. **Correctness** — wrong logic, null dereferences (the project has `<Nullable>enable</Nullable>`; take its warnings seriously), off-by-one in date math, unhandled exceptions, async methods not awaited, `.Result`/`.Wait()` deadlock risks.
2. **Security** — missing `[Authorize]` or a stray `[AllowAnonymous]`, no `[ValidateAntiForgeryToken]` on POST actions, over-posting (binding EF entities directly without `[Bind]` or a view model), raw SQL with string concatenation, `Html.Raw` on user data, secrets in `appsettings*.json`. The `BacsCallback` controllers accept arbitrary JSON from outside — check they validate input and don't log sensitive data.
3. **Data / EF Core** — queries inside loops (N+1), missing `AsNoTracking()` on read-only queries, `SaveChanges` called too often, model changes without a migration. Remember `Database.CreateDbIfNotExists` uses `EnsureCreated()`, which ignores migrations — flag changes that only work on one of the two paths.
4. **Project-specific traps**
   - Dates/times: the calendar uses the `"FLE Standard Time"` zone. Flag `DateTime.Now`/`DateTime.Today` where the Helsinki-local date is intended, and mixing UTC with local times.
   - Repeating events (`CalEventRepeat`: Once/Yearly/Monthly/EveryXdays) — check edge cases: Feb 29, the 31st in short months, month/year boundaries, weeks starting Monday in the 6×7 grid.
   - `ViewBag` keys: a renamed or missing key fails silently at runtime in the view. Cross-check controller and `.cshtml`.
   - CSS edited in `all-site.css`/`calend.css` but `site.min.css` not regenerated (the layout links the `.min` file) — or the reverse, `.min` edited by hand.
   - `Program.cs` routing differs between Development and Production; changes should work in both.
5. **Maintainability** — only when it's clearly worth it: duplicated logic, dead code, misleading names. Don't nitpick formatting or style preferences.

If something looks wrong but you're not sure, say so and mark it as a question rather than stating it as a bug.

## 3. Optional build check

If C# files changed, run `dotnet build` and include any new errors or warnings in changed files. Skip it if only views/CSS/JS changed.

## 4. Report format

Use this structure, keep it short, and order findings by severity:

```
## Review: <what was reviewed>

### 🔴 Must fix
- [File.cs:42](Controllers/File.cs:42) — <problem>. <why it matters>. **Fix:** <concrete suggestion>

### 🟡 Should fix
- ...

### 🔵 Consider / questions
- ...

**Summary:** <one or two sentences: overall state, and whether it's OK to commit>
```

Leave out empty sections. If there are no real problems, say so plainly — don't invent findings to fill the template.
