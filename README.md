# BGDLib — Babylonian GameDev Library

![.NET MAUI](https://img.shields.io/badge/.NET%20MAUI-9.0-512BD4?logo=dotnet)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-9.0-512BD4?logo=dotnet)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-336791?logo=postgresql)
![License](https://img.shields.io/badge/License-MIT-green)
![Platform](https://img.shields.io/badge/Platform-Android-3DDC84?logo=android)

**BGDLib** is a mobile application for game developers — a knowledge hub that aggregates RSS news from leading gamedev resources, hosts a community feed for sharing posts and finding vacancies, and provides offline-capable note-taking tools, all in one dark-themed app.

Built as a college project at IT TOP (Saint Petersburg) by Crazy Animals Studio. Designed with real product ambitions in mind.

---

## 📱 Features

- 🗞 **RSS News Ribbon** — aggregates 12 gamedev sources (Unity Blog, Godot, Unreal, Gamasutra, r/gamedev, 80.lv, Habr and more), auto-categorized by topic
- 🏘 **Community Feed** — post articles, upvote/downvote, comment; powered by a real REST API
- 🔍 **Search** — full-text search across community posts and RSS articles
- 💼 **Vacancies** — browse gamedev job listings aggregated from Reddit and itch.io
- 👤 **Profile** — JWT-authenticated accounts with Google OAuth 2.0 support
- 🎒 **Local Backpack** — save favorite articles offline via SQLite
- 📝 **MD Writer** — in-app Markdown editor (EasyMDE via WebView) with local note storage
- 🌐 **Localization** — Russian and English UI

---

## 🛠 Tech Stack

### Client (Android)
| Layer | Technology |
|---|---|
| Framework | .NET MAUI 9 |
| Architecture | MVVM + CommunityToolkit.Mvvm (Source Generators) |
| Local storage | SQLite via sqlite-net-pcl |
| RSS parsing | CodeHollow.FeedReader |
| Icons | MauiIcons.Material |
| Markdown editor | EasyMDE in WebView |
| Auth | JWT Bearer + Google OAuth 2.0 |

### Backend
| Layer | Technology |
|---|---|
| Framework | ASP.NET Core 9 |
| ORM | Entity Framework Core 9 + Npgsql |
| Database | PostgreSQL 16 |
| Auth | JWT + BCrypt + Google OAuth 2.0 |
| Containerization | Docker Compose |

