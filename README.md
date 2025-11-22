# 🤖 BotForge

![BotForge icon](res/icon_flat.png)

**BotForge** is a modular .NET chatbot building framework powered by a finite state machine (FSM) architecture.
It started as an internal tool for a university hackathon and evolved into an open-source project for developers who want structured, modular, and declarative bot logic.

---

## 🚀 Project Goals

- Provide a **clean FSM architecture** for complex conversational flows
- Offer a **modular system** with declarative state definitions and role-based access
- Deliver **Telegram integration** with room for other platforms in the future
- Simplify bot development through **hosting support**, **dependency injection**, and **extensibility**

---

## 📦 Solution Structure

| Package | Description |
|----------|-------------|
| `BotForge.Core` | Core FSM engine and dispatching logic |
| `BotForge.Telegram` | Telegram API integration layer |
| `BotForge.Persistence` | EF Core-based FSM state storage |
| `BotForge.Modules` | Modular system and declarative FSM attributes |
| `BotForge.Hosting` | Integration with `IHostBuilder` and DI container |
| `BotForge.Analyzers` | Roslyn analyzers for FSM attribute validation |

---

## 🧰 Current Status

> 🛠️ The project is in an **early development stage**.
> A public release will follow after the stabilization of the FSM core and Telegram adapter.

---

## 🗺️ Roadmap

- [x] Prepare repository and solution structure
- [X] Implement FSM core (`BotForge.Core`)
- [X] Add Telegram adapter
- [X] Build modular system
- [X] Create sample bots (`PingPongBot`)
- [ ] Write documentation and guides
- [ ] Publish first release to NuGet

See [`docs/roadmap.md`](docs/roadmap.md) for detailed progress tracking.

---

## ⚙️ Build Instructions
```bash
git clone https://github.com/IOExcept10n/BotForge.git
cd BotForge
dotnet restore
dotnet build
dotnet test
```
---

## 📄 License

This project is licensed under the **MIT License**.
See the [LICENSE](LICENSE) file for details.

---

## ✨ Author

Developed by [@IOExcept10n](https://github.com/IOExcept10n)
Originated as a Telegram bot for a university hackathon project.
