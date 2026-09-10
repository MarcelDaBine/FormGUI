# FormGUI

A cross-platform desktop application built with [Avalonia UI](https://avaloniaui.net/) (.NET 8) for entering, validating, and reporting **ballistic test data** (velocity/Doppler radar readings, V50 statistics, projectile/powder/barrel specs, environmental conditions, etc.), with the ability to save/load reports as JSON and submit them to a backend API.

## Context

This was built for a real client engagement. Worth knowing before reading the code:

- The project started earlier, but the bulk of the functionality was written in about **2 weeks**, once the client asked to see a working demo.
- I picked it back up after roughly **2 years away from it (and from Avalonia)** to prepare this repo, so some context on my own original decisions was genuinely rebuilt from scratch by re-reading the code.
- Development was mostly solo. A teammate ([`Cannisedd`](https://github.com/Cannisedd)) contributed UI/UX polish (layout rearrangement, styling touch-ups) on top of the base implementation.
- I'm sharing this **as-is**, warts and all, rather than cleaning it up to look better than it is. See [Known Issues](#known-issues--what-id-do-differently) below for an honest list of what needs work.

## Features

- **Login screen** authenticating against a REST API (email/password).
- **Dashboard** for entering ballistic test report data: shooter/recorder info, date/time, environmental conditions (temperature, humidity), projectile/powder/barrel selection, sample and standards rows (dynamically added/removed).
- **Live charts** (via LiveChartsCore + SkiaSharp) for velocity/V50 statistics.
- **Custom numeric input controls** (float/double/positive-only text boxes) with input validation behaviors.
- **Save/load reports** to and from JSON files.
- Custom-drawn title bar / chrome and side-pane navigation.

## Tech Stack

- **.NET 8**, C#
- **Avalonia UI 11** (Fluent theme) - cross-platform XAML UI framework
- **ReactiveUI** + **CommunityToolkit.Mvvm** - MVVM/reactive bindings
- **LiveChartsCore.SkiaSharpView** - charting
- **System.Text.Json** - report persistence

## Running It

Requires the [.NET 8 SDK](https://dotnet.microsoft.com/download).

```powershell
dotnet restore FormGUI/FormGUI.sln
dotnet run --project FormGUI/FormGUI.Desktop
```

The login screen expects an auth API at `http://localhost:8090/api/v1/auth/authenticate` - without a running backend, the dashboard can still be reached by adjusting `MainViewModel` to start on `DashboardViewModel` (already the current default in this repo).

> **Note:** the dashboard was designed for and demoed on a fullscreen display. It's usable at smaller sizes, but the layout isn't fully reactive yet, so a horizontal scrollbar kicks in below that size.

> **Note:** the V50 chart won't populate until enough shots are classified - it follows the standard V50 ballistic-limit bracketing method, needing at least 3 "PP" (partial penetration) and 3 "CP" (complete penetration) shots with valid velocities, extending to 5 or 7 of each depending on how spread out the values are.

## Known Issues / What I'd Do Differently

Being upfront about this rather than hiding it:

- **`DashboardViewModel` is way too large** and takes on too many responsibilities (form state, validation, chart building, JSON serialization, dynamic row management). It should be split into smaller view models/services.
- Several UI-control collections (e.g. lists of `TextBox`/`ComboBox`/`Button` tracked manually in the view model) are a workaround from working directly against Avalonia's control tree instead of pure data-binding - a cleaner data-first approach would remove most of this.
- No automated tests.
- Minimal error handling around network calls and file I/O.
- The auth endpoint is hardcoded rather than configurable - this one wasn't a conscious tradeoff, just an accidental oversight I'd catch now.
- The dashboard layout isn't fully reactive below Full HD resolution (see note above) - proper responsive layout was attempted but dropped due to time.

Given the 2-week turnaround to get a client demo out the door, most of these were conscious shortcuts rather than oversights - but they're the first things I'd fix with more time.
