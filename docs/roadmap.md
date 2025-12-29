# 🗺️ BotForge Roadmap

The following roadmap outlines the planned development of **BotForge** — a modular .NET framework for building bots with an FSM (finite state machine) architecture.
Each milestone links to GitHub issues tracking its progress.

---

## 🧩 Milestone 1: Core Architecture (v0.1.0)

**Objective:** Establish the foundational FSM engine and message dispatching model.

**Deliverables:**
| Feature | Status | Issue |
|----------|--------|--------|
| FSM engine core (BotForge.Core) | ✅ Done | [#1](../../../issues/1) |
| State abstractions (IStateHandler, StateResult, etc.) | ✅ Done | [#2](../../../issues/2) |
| State registration & lifecycle management | ✅ Done | [#3](../../../issues/3) |
| In-memory persistence | ✅ Done | [#4](../../../issues/4) |
| Logging and diagnostics | ✅ Done | [#5](../../../issues/5) |

**Stretch Goals:**
- [ ] FSM visualization tool (`botforge fsm graph`)

---

## 💬 Milestone 2: Telegram Integration (v0.2.0)

**Objective:** Add a Telegram transport layer and adapter for BotForge FSM.

**Deliverables:**
| Feature | Status | Issue |
|----------|--------|--------|
| Telegram adapter (BotForge.Telegram) | ✅ Done | [#6](../../../issues/6) |
| BotListener & Update Dispatcher | ✅ Done | [#7](../../../issues/7) |
| Middleware for error handling | ⏳ In Progress | [#8](../../../issues/8) |
| Webhook support | ❌ Planned | [#9](../../../issues/9) |
| Docker hosting | ❌ Planned | [#10](../../../issues/10) |

**Stretch Goals:**
- [ ] Retry policy for API errors
- [ ] Inline command routing

---

## 🧱 Milestone 3: Persistence Layer (v0.3.0)

**Objective:** Add a persistence abstraction for user states.

**Deliverables:**
| Feature | Status | Issue |
|----------|--------|--------|
| EF Core integration | ✅ Done | [#11](../../../issues/11) |
| BotForgeDbContext base implementation | ✅ Done | [#12](../../../issues/12) |
| Repository pattern for state access | ✅ Done | [#13](../../../issues/13) |
| Default migrations and auto-migrate | ✅ Done | [#27](../../../issues/27) |

Stretch Goals:
- [X] JSON-based serialization for complex states

---

## 🧩 Milestone 4: Module System (v0.4.0)

**Objective:** Introduce modular design and declarative FSM state definitions.

**Deliverables:**
| Feature | Status | Issue |
|----------|--------|--------|
| Module base (ModuleBase) | ✅ Done | [#15](../../../issues/15) |
| Attribute-based FSM states | ✅ Done | [#16](../../../issues/16) |
| Role-based access control | ✅ Done | [#17](../../../issues/17) |
| Module auto-discovery | ✅ Done | [#18](../../../issues/18) |

Stretch Goals:
- [X] Sample modules: PingModule, FeedbackModule
- [X] Roslyn analyzer for invalid FSM attributes

---

## 🧩 Milestone 5: Developer Experience (v0.5.0)

**Objective:** Streamline development experience and project setup.

**Deliverables:**
| Feature | Status | Issue |
|----------|--------|--------|
| CLI templates (dotnet new botforge-bot) | ⏳ In Progress | [#19](../../../issues/19) |
| Hosting integration (BotForge.Hosting) | ✅ Done | [#20](../../../issues/20) |
| Logging and telemetry defaults | ✅ Done | [#21](../../../issues/21) |
| Roslyn analyzers (BotForge.Analyzers) | ✅ Done | [#22](../../../issues/22) |

**Stretch Goals:**
- [ ] IntelliSense XML docs and VS Code snippets
- [ ] Developer guide documentation

---

## 🚀 Milestone 6: Public Release (v1.0.0)

Objective: Stabilize and release BotForge to the public.

Deliverables:
| Feature | Status | Issue |
|----------|--------|--------|
| Documentation site | ⏳ In Progress | [#23](../../../issues/23) |
| CI/CD with GitHub Actions | ⏳ In Progress | [#24](../../../issues/24) |
| NuGet publishing (BotForge.*) | ✅ Done | [#25](../../../issues/25) |
| Example bots and tutorials | ⏳ In Progress | [#26](../../../issues/26) |

Stretch Goals:
- [ ] Discord & WebSocket adapters
- [ ] Community extension registry

**Last updated:** October 2025
**Maintainer:** [@IOExcept10n](https://github.com/IOExcept10n)
