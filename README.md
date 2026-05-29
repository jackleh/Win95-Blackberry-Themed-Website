# Win95 / BlackBerry Themed Website

A retro personal-portfolio project in two parts: a **public website** that renders as a Windows 95 desktop on large screens and a BlackBerry phone on mobile, and a **Windows desktop app** for editing all of the site's content without hand-editing JSON.

Both halves are driven by the same JSON files in `Website/wwwroot/data/` — the website *reads* them, the builder *edits* them.

| Project | What it is |
|---|---|
| [`Website/`](#the-website) | Blazor WebAssembly portfolio — Win95 desktop UI + BlackBerry mobile UI. Zero backend, deploys as static files. |
| [`WebsiteBuilder/`](#the-website-builder) | .NET MAUI **Windows** desktop app — a Win95-themed content editor that reads/writes the site's JSON and copies in media. |

## Repository layout

```
Website.sln
Website/                      # Blazor WASM portfolio site
  Components/Win95/           # Desktop UI (razor + code-behind + scoped CSS)
  Components/Mobile/          # BlackBerry phone UI
  Pages/Home.razor            # Picks Win95 or mobile based on viewport
  Models/                     # POCOs that map to the JSON files
  ViewModels/BaseViewModel.cs # Loads all JSON data on startup
  wwwroot/data/               # All content JSON (the single source of truth)
  wwwroot/{css,js,media}/     # Styles, JS interop helpers, images/video
WebsiteBuilder/               # .NET MAUI Windows content editor
  Pages/                      # One editor page per data file
  Models/                     # POCOs mirroring the Website models
  Services/WebsiteDataService.cs  # Loads/saves the JSON, copies media
```

---

## The Website

A Blazor WebAssembly personal portfolio with two interactive UIs: a retro **Windows 95–style desktop** for desktop browsers and a **BlackBerry-style phone** for mobile. Everything — copy, links, resume, projects, colors, OS names — is driven by JSON config so the project is easy to fork and repurpose.

### Features

- **Win95 desktop** — draggable windows, taskbar, Start menu, and a working CMD prompt with a virtual filesystem
- **CMD** — `dir`, `cd`, `type`, `open`, and shortcut commands backed by a configurable VFS; **Up/Down arrows recall previous commands** like a real shell
- **Notepad** — opens any text/JSON file from the VFS in an editable window
- **Paint** — canvas paint app; loads images from the VFS or via CMD (`open image.png`)
- **BlackBerry mobile UI** — rendered automatically for viewports under 820 px; shows About, Projects, and Contacts apps
- **Fully data-driven** — all content lives in `wwwroot/data/*.json`; no C# changes required to customize copy
- **Zero backend** — static Blazor WASM; deploys to GitHub Pages, Netlify, Azure Static Web Apps, etc.

### Tech stack

| Layer | Choice |
|---|---|
| Framework | Blazor WebAssembly (.NET 8) |
| Language | C# |
| Styling | Scoped CSS per component |
| JS interop | Drag helper, Paint canvas, device/viewport detection |
| Hosting | Static file host (no server required) |

### Quick start

```bash
git clone https://github.com/jackleh/Win95-Blackberry-Themed-Website.git
cd Win95-Blackberry-Themed-Website
dotnet run --project Website/Website.csproj
```

Then open `https://localhost:5001` (or the URL printed in the terminal).

### Customizing

All content lives in `Website/wwwroot/data/`. Edit these JSON files directly (or use the [Website Builder](#the-website-builder) GUI) and the site updates automatically — no C# changes needed.

| File | What it controls |
|---|---|
| `person.json` | Name, title, LinkedIn URL, GitHub URL |
| `resume.json` | Tagline, email, phone, location, work history, education, skills |
| `projects.json` | Project list shown in CMD and mobile Projects app |
| `contact.json` | Contact note and call-to-action banner in CMD |
| `siteConfig.json` | Win95 OS name/version, desktop wallpaper, app exe names, video URLs, virtual filesystem (VFS) |

#### Changing the OS theme

In `siteConfig.json`, set `Win95OsName`, `Win95OsDirName`, `Win95OsVersion`, and `Win95OsCopyright` to whatever you like. The CMD prompt, window title bars, and VFS paths all update to match.

```json
{
  "Win95OsName": "Copland OS",
  "Win95OsDirName": "COPLAND",
  "Win95OsVersion": "Copland OS Enterprise [Version 8.00.001]",
  "Win95OsCopyright": "(C) Copyright Copland Corp 1994-1996."
}
```

#### Adding files to CMD

The virtual filesystem is defined by the `Win95VfsEntries` array in `siteConfig.json`. Each entry is:

```json
{
  "Path": "C:\\Desktop",
  "Name": "resume.md",
  "IsDir": false,
  "FileType": "desktop",
  "Action": "open-resume"
}
```

| `FileType` | Behavior when opened |
|---|---|
| `desktop` | Triggers a built-in action (`open-resume`, `open-projects`, `open-contact`) |
| `text` | Opens the file in Notepad (fetches `FileUrl`) |
| `image` | Opens the file in Paint (fetches `FileUrl`) |
| `exe` | Runs a built-in app (`run-cmd`, `run-paint`, `run-notepad`) |
| `binary` | Prints "Cannot open binary file" |

#### Desktop videos

Set `Win95Video1Url`, `Win95Video2Url`, `Win95Video3Url` to any MP4 URLs (used in the video sequence intro mode). Set the matching `*Title` fields to what should display in the media player title bar.

### Deployment

```bash
dotnet publish Website/Website.csproj -c Release -o publish
```

The `publish/wwwroot` folder is a self-contained static site. Point any static host at it. For GitHub Pages, set the base path in `wwwroot/index.html` to match your repo name if deploying to a sub-path.

---

## The Website Builder

A **.NET MAUI Windows desktop app** that edits the website's content through forms instead of raw JSON. The editor itself wears the Win95 look (navy title bars, silver panels, square 3D buttons, a permanently-docked navigation flyout) to match the site it builds.

### Requirements

- Windows 10 (1809+) / Windows 11
- .NET 10 SDK with the MAUI workload (`dotnet workload install maui`)
- A local clone of this repository (the app edits the JSON files in `Website/wwwroot/data/`)

### Run it

From Visual Studio 2022, open `Website.sln`, set **WebsiteBuilder** as the startup project, and press F5.

Or from the CLI:

```bash
dotnet build WebsiteBuilder/WebsiteBuilder.csproj -f net10.0-windows10.0.19041.0 -t:Run
```

> **If launching fails with `REGDB_E_CLASSNOTREG`**, the machine is missing a matching Windows App SDK runtime. Either run from Visual Studio (which deploys it), or build self-contained so the runtime is bundled:
> ```bash
> dotnet build WebsiteBuilder/WebsiteBuilder.csproj -f net10.0-windows10.0.19041.0 -p:WindowsAppSDKSelfContained=true
> ```

### Using it

1. Click **Open Website Folder** and pick your **repository root** (the folder containing `Website/`). The app loads the JSON from `Website/wwwroot/data/`.
2. Edit content on any page:

   | Page | Edits |
   |---|---|
   | **Person** | Name, title, LinkedIn URL, GitHub URL → `person.json` |
   | **Contact** | Contact note and call-to-action lines → `contact.json` |
   | **Projects** | Add/remove projects (name, description, URL, status, tech) → `projects.json` |
   | **Resume** | Tagline, contact, work history, skill groups, education → `resume.json` |
   | **Site Config** | Win95 toggle, OS branding, app names, donate link, videos, and wallpaper/Paint images → `siteConfig.json` |

3. Use the **Browse…** buttons to pick images and videos — they're copied into `Website/wwwroot/media/` and the path is filled in for you.
4. Click **Save All** to write the JSON back to the website.

### Notes

- JSON keys are written in camelCase; the website reads them case-insensitively, so casing never affects loading.
- The app targets Windows only. It edits content and copies media — it does not build or deploy the site.
