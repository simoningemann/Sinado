# Sinado

**A High-Performance .NET 10 Web API built with Clean Architecture and Automated DevOps.**

---

## 🚀 Live Demo
The application is deployed to Azure (Denmark East) and can be accessed here:
👉 **[View Live API](https://sinado-e4aeg3hceuf5hteq.denmarkeast-01.azurewebsites.net/)**

---

## 🛠 Project Overview
Sinado is a modern portfolio piece designed to showcase a professional-grade development workflow. It utilizes the latest features of **.NET 10** and emphasizes a "Shift-Left" approach to testing and deployment.

### Core Architecture
The project follows **Clean Architecture** principles to ensure the business logic remains decoupled from external frameworks:
* **Domain & Application:** Business rules and logic using the **CQRS (Command Query Responsibility Segregation)** pattern.
* **Infrastructure:** External concerns like database access and third-party integrations.
* **API:** A thin layer handling HTTP requests and responses.

---

## 🏗 Technical Stack

* **Runtime:** `.NET 10.0`
* **Pattern:** `MediatR` for decoupled messaging and process handling.
* **Testing:** `xUnit v3` (Alpha/Beta) integrated with the new `Microsoft.Testing.Platform`.
* **CI/CD:** `GitHub Actions` providing a fully automated build-test-deploy pipeline.
* **Host:** `Azure App Service` running on Windows.

---

## 🧪 DevOps & Testing
This project highlights a modern 2026 testing workflow:
* **Deterministic Builds:** Using `global.json` to lock the SDK and runner behavior across environments.
* **Modern Test Runner:** Migrated from legacy VSTest to the high-performance **Microsoft.Testing.Platform (MTP)**, allowing the test suite to run as a native executable.
* **Automated Quality Gate:** The CI/CD pipeline ensures that code cannot be merged or deployed unless 100% of the test suite passes.