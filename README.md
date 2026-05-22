# Website

A Blazor WebAssembly personal portfolio site with two interactive UIs: a retro **Windows 95–style desktop** for desktop browsers and a **BlackBerry-style phone** for mobile. Everything — copy, links, resume, projects, colors, OS names — is driven by JSON config files so the project is easy to fork and repurpose.

## Features

- **Win95 desktop** — draggable windows, taskbar, and a working CMD prompt with a virtual filesystem
- **CMD** — `dir`, `cd`, `type`, `open`, and shortcut commands backed by a configurable VFS defined in JSON
- **Notepad** — opens any text/JSON file from the VFS in an editable window
- **Paint** — canvas paint app; loads images from the VFS or via CMD (`open image.png`)
- **BlackBerry mobile UI** — rendered automatically for viewports under 820 px; shows About, Projects, and Contacts apps
- **Fully data-driven** — all content lives in `wwwroot/data/*.json`; no C# changes required to customize copy
- **Zero backend** — static Blazor WASM; deploys to GitHub Pages, Netlify, Azure Static Web Apps, etc.

## Tech stack

| Layer | Choice |
|---|---|
| Framework | Blazor WebAssembly (.NET 8) |
| Language | C# |
| Styling | Scoped CSS per component |
| JS interop | Drag helper, Paint canvas, device/viewport detection |
| Hosting | Static file host (no server required) |

## Quick start

```bash
git clone https://github.com/jackleh/Win95-Blackberry-Themed-Website.git
cd Win95-Blackberry-Themed-Website
dotnet run --project Website/Website.csproj
```

Then open `https://localhost:5001` (or the URL printed in the terminal).

## Customizing

All content lives in `Website/wwwroot/data/`. Edit these JSON files and the site updates automatically — no C# changes needed.

| File | What it controls |
|---|---|
| `person.json` | Name, title, LinkedIn URL |
| `aboutMe.json` | Summary, skills, languages, location, GitHub URL |
| `resume.json` | Tagline, email, phone, work history, education, skills |
| `projects.json` | Project list shown in CMD and mobile Projects app |
| `contact.json` | Contact note and call-to-action banner in CMD |
| `siteConfig.json` | Win95 OS name/version, desktop wallpaper, app exe names, video URLs, virtual filesystem (VFS) |

### Changing the OS theme

In `siteConfig.json`, set `Win95OsName`, `Win95OsDirName`, `Win95OsVersion`, and `Win95OsCopyright` to whatever you like. The CMD prompt, window title bars, and VFS paths all update to match.

```json
{
  "Win95OsName": "Copland OS",
  "Win95OsDirName": "COPLAND",
  "Win95OsVersion": "Copland OS Enterprise [Version 8.00.001]",
  "Win95OsCopyright": "(C) Copyright Copland Corp 1994-1996."
}
```

### Adding files to CMD

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

### Desktop videos

Set `Win95Video1Url`, `Win95Video2Url`, `Win95Video3Url` to any MP4 URLs (used in the video sequence intro mode). Set the matching `*Title` fields to what should display in the media player title bar.

## Project structure

```
Website.sln
Website/
  App.razor                   # Root component
  Program.cs                  # WASM host setup
  _Imports.razor              # Global using directives
  Components/
    Win95/                    # Desktop UI (razor + code-behind + scoped CSS)
    Mobile/                   # BlackBerry phone UI
  Pages/
    Home.razor                # Route "/" — picks Win95 or mobile based on viewport
  Models/                     # C# POCOs that map to JSON files
  ViewModels/
    BaseViewModel.cs          # Loads all JSON data on startup
  Layout/
    MainLayout.razor          # Thin shell layout
  wwwroot/
    data/                     # All content JSON (edit these to customize)
    css/                      # Global + Bootstrap styles
    js/                       # dragHelper, paintHelper, deviceHelper
    media/                    # Images and resume download
```

## Deployment

Build a release artifact:

```bash
dotnet publish Website/Website.csproj -c Release -o publish
```

The `publish/wwwroot` folder is a self-contained static site. Point any static host at it. For GitHub Pages, set the base path in `wwwroot/index.html` to match your repo name if deploying to a sub-path.
