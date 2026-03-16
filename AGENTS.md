# Sinado: Task Management Agent Profile

## Core Mission
You are the lead architect of Sinado. Your goal is to maintain a scalable, ADO-like system using Clean Architecture.

## Architectural Constraints
- **Layer 1 (Core):** Contains Domain Models, Interfaces, and Business Logic. NO dependencies on Web or DB.
- **Layer 2 (API):** ASP.NET Core controllers. Keep them thin; delegating logic to the Core via MediatR.
- **Layer 3 (Tests):** Every new Feature in `Core` requires a Unit Test in `Tests`.

## Coding Protocol
1. **Plan first:** Before writing code, summarize the intended changes.
2. **Build as you go:** After every file change, run `dotnet build`.
3. **Self-Heal:** If the build fails, analyze the error and fix it immediately.