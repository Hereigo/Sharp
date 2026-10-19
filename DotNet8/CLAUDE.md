# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

"Calendarium" — an ASP.NET Core MVC app (net10.0, despite the `DotNet8` folder name) for a personal calendar of recurring events plus a simple notes list. Project file is `Calendarium.csproj`, root namespace `Calendarium`. It lives inside the parent `Sharp` repo (git root is `..`) and is referenced from `../DotNet8.sln` alongside unrelated sibling projects.

## Commands

```bash
dotnet build                               # build
dotnet run --launch-profile https          # run (https://localhost:7273, http://localhost:5286)
dotnet tool restore                        # restores local dotnet-ef (pinned 8.0.8 in .config/dotnet-tools.json)
dotnet ef migrations add <Name>            # add EF migration
dotnet ef database update                  # apply migrations
```

There is no test project and no linter configured.

CSS/JS bundling is defined in `bundleconfig.json` (`all-site.css` + `calend.css` → `wwwroot/css/site.min.css`); it is produced by the Visual Studio Bundler & Minifier extension, not by `dotnet build`. Edit the source CSS files and regenerate the `.min` output — the layout links `site.min.css`.

## Architecture

- **Single DbContext, split across files**: `ApplicationDbContext` is a `partial` `IdentityDbContext` declared in `Data/ApplicationDbContext.cs` and `DbLogger/ApplicationDbContext.cs`. It holds ASP.NET Identity tables plus `CalEvents`, `CalEventCategories`, `RequestsHeaders`, `Notes`. SQL Server via `ConnectionStrings:DefaultConnection` (LocalDB by default).
- **Startup DB init**: `Data/Database.CreateDbIfNotExists` calls `EnsureCreated()` and seeds a demo event/note. Note that `EnsureCreated` bypasses migrations — on a fresh DB the schema comes from the model, not `Migrations/`.
- **Environment-dependent routing** (`Program.cs`): in Development the default route is `Home/Index` (the calendar); in Production it is `Home/StartPage`, which serves the static `Index.html` from the content root (copied to output). Production error handler points to `/Home/LogItem`, which does not currently exist.
- **Calendar rendering** (`HomeController.Index`): builds a fixed 6×7 sheet (weeks start Monday) for the current/prev/next month (`pMonth` = `prev`/`next`), using the `"FLE Standard Time"` (Helsinki) time zone. Repeating events (`CalEventRepeat`: Once/Yearly/Monthly/EveryXdays) are expanded into concrete occurrences by `GenerateRepeatingEvents`, then empty placeholder `CalEvent`s are appended per day. Lots of state is passed via `ViewBag`. Each visit to `Index` also records request headers into `RequestsHeaders` (shown in `History`).
- **Auth**: `HomeController` is `[Authorize]` at class level with selected actions `[AllowAnonymous]`; Identity uses the default Razor UI (`Areas/Identity`), `RequireConfirmedAccount = true`.
- **Static asset versioning**: `IContentManager` (singleton) appends an MD5 hash query string to wwwroot files; used in `_Layout.cshtml`.
- **`DbLogger/`**: a custom `ILoggerProvider` writing logs to a DB table. Not wired up in `Program.cs`; `DbLogger/DbLogger_ReadMe.md` shows the config/registration needed to enable it.
- `BacsCallbackController` (`api/bacs/callback`) and `AaaBacsCallbackController` are standalone API endpoints that accept arbitrary JSON callbacks, unrelated to the calendar.

## Notes

- `TODO.md` holds the owner's backlog of planned features.
- Deployment publish profiles (folder, IIS, somee-hosted site) are in `Properties/PublishProfiles/`.
