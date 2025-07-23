# 🎨 CollabBoard – Real-Time Collaborative Whiteboard & Spreadsheet

> **One link → unlimited creativity.**  
> Up to **50 concurrent users** per board, **live cursors**, **chat**, **infinite undo/redo**, **history replay**, and **Microsoft-Office-style drawing tools**.

---

## 🌟 Features

| Area | Highlights |
|------|------------|
| **Drawing & Editing** | Free-hand, shapes, lines, text, sticky notes, images, Excel-like cell grid, zoom & pan |
| **Live Collaboration** | Sub-second real-time sync via SignalR + Azure SignalR Service |
| **Presence & Chat** | Side-pane with online members list and instant messaging |
| **History & Audit** | Every stroke / cell change saved → full replay timeline |
| **Sharing** | Public or invite-only boards with role-based access (viewer / editor / owner) |
| **Performance** | Optimistic locking, row versioning, snapshots, OT/CRDT hybrid model |

---

## 🏗️ Architecture

```text
┌──────────────┐   ┌──────────────┐   ┌──────────────┐
│   Angular    │⇄ │   .NET 9     │⇄ │ PostgreSQL   │
│   (SPA)      │   │  Web API     │   │   + Redis    │
└──────────────┘   └──────────────┘   └──────────────┘
         ⇅ SignalR            ⇅ EF Core
         ⇅ Yjs / OT           ⇅ Snapshot service
```

- **Clean Architecture** with layered separation  
- **CQRS + MediatR** for commands and queries  
- **EF Core 9 + PostgreSQL** as the source of truth  
- **Redis** for user presence and in-memory sync  
- **Azure Container Apps** + **Azure SignalR** for scalable real-time infrastructure

---

## 🚀 Quick Start

### 1. Clone & Run Backend
```bash
git clone https://github.com/<you>/CollabBoard.git
cd backend
cp .env.example .env           # optional: edit DB passwords
docker compose up -d           # start Postgres + Redis
dotnet restore
dotnet ef database update --project src/CollabBoard.Infrastructure --startup-project src/CollabBoard.Api
dotnet run --project src/CollabBoard.Api --launch-profile "https"
```

### 2. Start Angular App
```bash
cd ../whiteboard-ui
npm ci
npm start
```

Open `https://localhost:4200` → create a board → collaborate live.

---

## 📦 Tech Stack

| Layer | Technology |
|-------|------------|
| **Front-end** | Angular 19, TypeScript, KonvaJS, Yjs |
| **Back-end**  | .NET 9, ASP.NET Core, SignalR, EF Core 9 |
| **Database**  | PostgreSQL 16 (code-first), Redis (presence) |
| **Infrastructure** | Docker, Azure Container Apps, Azure SignalR Service |
| **DevOps**    | GitHub Actions, Bicep / AZD |
| **Patterns**  | Clean Architecture, CQRS, Optimistic Locking, Snapshotting, Event-Driven |

---

## 🧩 Database Entity Overview

All entities inherit from `BaseEntity<T>` with `Id`, `CreatedUtc`, `UpdatedUtc`, and `DomainEvents`.

### 👥 Core Relationships

```text
AppUser 1─►* BoardMember *◄─Board 1─►* Page *─►* ContentBlock
                                │
                                ├──►* ChatMessage
                                ├──►* OperationLog
                                └──►* Snapshot (latest only)
```

### 📊 Key Tables

| Table | Description |
|-------|-------------|
| `AppUsers` | Registered users with name/email |
| `Boards` | Collaboration spaces (canvas) |
| `BoardMembers` | Join table for user-board roles |
| `Pages` | Multi-page board support |
| `ContentBlocks` | JSON-based shapes, text, images, etc. |
| `ChatMessages` | In-board messaging |
| `OperationLogs` | Deltas for undo/redo/history replay |
| `Snapshots` | Gzipped JSON of full board state |
| `Presence` | Redis-powered live user tracking |

---

## ⚙️ Development Scripts

| Command | Description |
|---------|-------------|
| `dotnet ef migrations add <name>` | Add new EF Core migration |
| `dotnet ef database update` | Apply migrations to DB |
| `dotnet test` | Run backend unit & integration tests |
| `npm run test` | Run frontend unit tests |
| `azd up` | One-command full Azure deployment |

---

## 🛡️ Security & Reliability

- ✅ JWT authentication (Azure AD / IdentityServer ready)  
- ✅ Role-based access (owner / editor / viewer)  
- ✅ Row-level optimistic locking via EF Core `IsRowVersion()`  
- ✅ PostgreSQL indexes: BRIN (replay), GIN (search in JSON), covering indexes  
- ✅ HTTPS with CSP & secure headers  
- ✅ Optional rate-limiting middleware for API abuse prevention  

---

## ☁️ Azure Cloud Deploy (Optional)

```bash
azd init         # configure subscription and environment
azd provision    # spin up infrastructure (Postgres, SignalR, ACA)
azd deploy       # deploy app to Azure
```

Get public board URL after 2–3 minutes.

---

## 📄 License

MIT – free for commercial or personal use.

---

## 🤝 Contributing

PRs welcome! Open an issue first if it's a large change or architectural refactor.

---