# Calendarium: ASP.NET Core MVC → Minimal API + React

Branch: `feature/minimal-api-react`

| Phase | Status |
|---|---|
| 0. Prep (services, DTOs) | ✅ Done |
| 1. Minimal API next to MVC | ✅ Done |
| 2. React app scaffold | ⏳ Not started |
| 3. Port pages to React | ⏳ Not started |
| 4. Auth switch to Identity API | ⏳ Not started |
| 5. Cutover and MVC removal | ⏳ Not started |

## Context
Calendarium is an ASP.NET Core MVC app on .NET 10. It has Razor views, `ViewBag`-driven pages, Identity Razor UI, jQuery and Bootstrap. The goal is a Minimal API backend with a React (Vite + TypeScript) frontend.

Decisions already made:
- **Auth:** Identity API endpoints (`MapIdentityApi`) with cookie auth.
- **Hosting:** the React app lives in the same project (`ClientApp/`) and ships in the same deploy. The Vite build goes to `wwwroot`, so the existing IIS/somee publish profiles keep working.
- **Rollout:** gradual. The new API and React UI run next to MVC until every page is ported. Then MVC is deleted.

---

## Phase 0: Prep ✅
- [x] Branch `feature/minimal-api-react`.
- [x] The somee connection string is gone from `appsettings.json`.
  - [ ] **You still need to rotate the somee password.** It's still in git history.
- [x] `Services/CalendarService.cs` now holds the calendar logic, and the MVC `HomeController` uses it too.
  - `Today()`: the current date in "FLE Standard Time".
  - `GetSheetRange(year, month)`: the 6×7 grid, weeks start Monday.
  - `BuildSheetAsync(year, month)`: loads events and sorts them by time.
  - `ExpandOccurrences(...)`: replaces `GenerateRepeatingEvents`.
  - `ApplySchedule(...)`: the Month=0 rule for Monthly and Year=0 rule for Yearly. It was copy-pasted in Create, Edit and UploadFile.
- [x] `Services/RequestHeaderLogger.cs` replaces `ProcessRequestHeaders` and `ProcessHeader`.
- [x] DTOs live in `Contracts/`: `CalEventDto`, `CalendarSheetDto`, `CalEventInput` (with the EveryXDays rule), `HistoryItemDto`, `NoteDto`, `NoteInput`.

Bugs fixed while extracting the logic (this changes behavior on purpose):
- **Feb 29 / 31st crash:** yearly and monthly events on dates a month doesn't have are clamped to the month's last day.
- **Infinite loop:** `EveryXdays` with `EveryXDays` set to 0 or empty no longer loops forever. Those events are skipped.
- **Missing yearly events:** a sheet spanning Dec→Jan used to drop them. They now appear.
- **Occurrences before the start date:** monthly and every-X-days events no longer show up before their `Started` date.
- **Header logging:** dedup now runs as an async query instead of a sync `SaveChanges`.

## Phase 1: Minimal API (next to MVC) ✅
`Program.cs` now has `AddProblemDetails`, `AddValidation` (.NET 10 validation for minimal APIs), `JsonStringEnumConverter`, `AddOpenApi` + `MapOpenApi` (dev only) and `UseStatusCodePages`. In production, `UseExceptionHandler("/Home/LogItem")` (a route that didn't exist) became `UseExceptionHandler()` + ProblemDetails.

The endpoints are in `Endpoints/*.cs`, mapped under `app.MapGroup("/api").RequireAuthorization()`:

| Endpoint | Replaces | Auth |
|---|---|---|
| `GET /api/calendar?year=&month=` (defaults to current; logs headers) | `Home/Index` | anonymous |
| `GET /api/history` | `Home/History` | auth |
| `GET /api/events` | `Home/FullList` | auth |
| `GET/POST/PUT/DELETE /api/events/{id}` | Create/Edit/Delete/Details | auth |
| `GET /api/events/export` (download `calendBkp.json`, PascalCase like before) | `Home/GetJson` | auth |
| `POST /api/events/import` (multipart `file`, dev only, ≤2 MB) | `Home/UploadFile` | auth (was anonymous) |
| `GET/POST/PUT/DELETE /api/notes[/{id}]` | `NotesController` | auth (MVC Notes had none) |

Verified so far:
- `dotnet build` passes.
- `/api/calendar` returns real LocalDB data, including the Jan 2027 sheet that starts Dec 28.
- An invalid month returns 400 ValidationProblem.
- Anonymous `/api/events` and `/api/notes` return 401, not a redirect.
- `/openapi/v1.json` lists every route.
- The MVC home page still returns 200.

Not verified yet: the authenticated write endpoints (POST/PUT/DELETE, import). These need a login, which is easier to test after Phase 4, or with the MVC login cookie.

BACS callback controllers (`api/bacs/callback`) are untouched. They're unrelated to the calendar.

---

## Phase 2: React app scaffold ⏳
1. Run `npm create vite@latest ClientApp -- --template react-ts`, then add `react-router-dom`, `@tanstack/react-query` and `bootstrap`.
2. In `vite.config.ts`:
   - Proxy `/api` to `https://localhost:7273` (`secure: false`).
   - Set `build.outDir = '../wwwroot/app'` and `base = '/app/'` while MVC still owns `/`.
3. In `Program.cs`, add `app.MapFallbackToFile("/app/{*path}", "app/index.html")`.
4. In `Calendarium.csproj`:
   - Exclude `ClientApp/**` from Compile/Content.
   - Add a publish target that runs `npm ci && npm run build` in `ClientApp`.
5. `src/api/` holds a fetch wrapper (`credentials: 'same-origin'`, 401 → login) and TanStack Query hooks per resource. Types can be generated from `/openapi/v1.json` with `openapi-typescript`.
6. Move `wwwroot/css/calend.css` and `all-site.css` into `src/styles/`. `Views/CALENDAR.html` is an optional newer design reference.

API notes for the client:
- Enums are strings (`"Monthly"`).
- `started` is `YYYY-MM-DD`.
- `time` is a `TimeSpan` string `"HH:mm:ss"`. Send `HH:mm:00` from `<input type="time">`.

## Phase 3: Port the pages one at a time ⏳
Compare each one against the MVC page for the same month.
1. **Calendar** (`Views/Home/Index.cshtml`, `_CalendPartial.cshtml`, `EventCalendView.cshtml`): `CalendarPage` with `MonthNav`, `CalendarGrid` and `DayCell` components.
   - Clicking a day number opens a new event on that date.
   - Clicking an event opens the edit form.
   - Sidebar: export link (signed-in only), dev-only import form (`importEnabled` in the response).
2. **Event form** (`Create.cshtml`/`Edit.cshtml`): one `EventForm` for both create and edit.
   - Show the EveryXDays field only when Repeat is EveryXdays.
   - Status radios on edit.
   - Delete with a confirm prompt.
   - Show server validation errors from ProblemDetails.
3. **FullList** (alternating row shading per month) and **History**.
4. **Notes** CRUD: the 5 views become one list page with inline or modal editing.

## Phase 4: Auth switch ⏳
1. Replace `AddDefaultIdentity<IdentityUser>()` with `AddIdentityApiEndpoints<IdentityUser>()`. The two register the same cookie scheme and can't coexist, so this is the cutover point for login.
2. Add `app.MapGroup("/api/auth").MapIdentityApi<IdentityUser>()`, plus:
   - `POST /api/auth/logout` (`SignInManager.SignOutAsync`)
   - `GET /api/auth/me`
   - `GET /api/auth/can-register`
3. Keep the current rule that registration is only allowed while no users exist (`_LoginPartial.cshtml`). Add an endpoint filter that blocks `/register` once a user exists.
4. `RequireConfirmedAccount = true` needs a real `IEmailSender<IdentityUser>`, and none is configured. Since registration is single-user, turn it off (or add a sender).
5. React calls `POST /api/auth/login?useCookies=true`. Build `LoginPage`, `RegisterPage` and an `AuthContext`.

## Phase 5: Cutover and MVC removal ⏳
1. Move the SPA to the root:
   - Set `outDir = '../wwwroot'` and `base = '/'`.
   - Use `app.MapFallbackToFile("index.html")`.
   - The production landing page (`StartPage` → `Index.html`) becomes a React route or stays a static file. Pick one.
2. Delete:
   - `Controllers/HomeController.cs`, `NotesController.cs`, `MODEL.cs`
   - `Views/`, `Areas/Identity/`
   - `Utils/ContentManager.cs`, `Utils/Utils.cs`
   - `bundleconfig.json`, `libman.json`
   - `wwwroot/jquery*` and `wwwroot/bootstrap`
3. Remove `AddControllersWithViews` (keep `AddControllers` if the BACS controllers stay), `MapControllerRoute`, `MapRazorPages` and `AddDatabaseDeveloperPageExceptionFilter`.
4. Drop the `Microsoft.AspNetCore.Identity.UI` and `Microsoft.VisualStudio.Web.CodeGeneration.Design` packages.
5. Optional: replace `EnsureCreated()` with `Database.Migrate()`, and bump `dotnet-ef` in `.config/dotnet-tools.json` to 10.x.
6. Update `CLAUDE.md` (commands and architecture).

## Verification (for the remaining phases)
- Call every endpoint through `/openapi/v1.json` or a `Calendarium.http` file:
  - Anonymous calls to auth-only endpoints get 401.
  - Invalid `EveryXdays` input gets a ValidationProblem.
- Start `dotnet run --launch-profile https` and `npm run dev` in `ClientApp`. Click through each ported page and compare it with MVC for the same month.
- Auth: register (only when no users exist), log in, `/api/auth/me`, log out.
- Release check:
  - `dotnet publish -c Release` produces `index.html` and hashed assets in `wwwroot`.
  - A hard refresh on a deep link (`/notes`) falls back to the SPA.
  - `/api/*` 404s return JSON, not `index.html`.
