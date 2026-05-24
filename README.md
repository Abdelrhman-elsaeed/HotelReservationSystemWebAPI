# 🏨 Hotel Reservation System Web API

A robust, scalable, and enterprise-level Hotel Reservation System built with **.NET Core**.
This project demonstrates advanced architectural patterns and best practices, specifically tailored to meet the high standards and requirements of modern software engineering markets.

The project heavily focuses on **Clean/Onion Architecture**, **CQRS**, and clean code principles to ensure maintainability and testability.

---

## ✨ Highlighted Features

- **🧅 Onion Architecture**: Strict separation of concerns keeping the Domain layer independent of UI and Infrastructure.
- **🔄 CQRS Pattern (MediatR)**: Command Query Responsibility Segregation to separate read and write operations, making the application highly scalable.
- **🛡️ Custom Middlewares**:
  - `GlobalErrorHandlerMiddleware`: Centralized exception handling to ensure consistent API responses without scattering `try-catch` blocks everywhere.
  - `TransactionMiddleware`: Automatically manages database transactions per request to ensure data integrity.
- **🚦 Custom Action Filters**:
  - `CustomAuthorizeFilter`: Fine-grained role and permission-based authorization.
- **🗄️ Repository Pattern**: Implementing `GenericRepository` alongside specific repositories (like `RoomRepository`) to decouple data access logic.
- **🔐 Security**: JWT-based Authentication and Authorization.
- **🗺️ Object Mapping**: Using `AutoMapper` to map between Domain Entities and DTOs/ViewModels smoothly.
- **🔍 Pagination, Filtering, and Sorting** *(in progress)*.
- **🚀 Distributed Caching (Redis)** *(in progress)*.
- **🧪 Unit and Integration Testing** *(in progress)*.

---

## 🏗️ Architecture Decision Records (ADRs) - The "Why" Behind the Code

> *"Programming is no longer just code that runs.. Programming has become documenting your thinking and architecture for your decisions."*

Here are the key architectural questions and decisions made during the design phase:

### 1. Why Onion Architecture over Traditional N-Tier?
- **Decision:** Use Onion Architecture.
- **Trade-offs:** N-Tier is easier to set up initially and familiar to beginners. However, it often leads to tight coupling with data access frameworks. Onion architecture requires more initial boilerplate and interfaces.
- **Advantages:** The Domain layer (business logic) is completely isolated at the center. We can swap out the database (Infrastructure) or the UI (API) without touching the core business rules. 
- **Disadvantages:** Higher learning curve and more files/folders to manage.

### 2. Why CQRS Pattern with MediatR (The Orchestrator)?
- **Decision:** Implement Command Query Responsibility Segregation (CQRS) using the MediatR library. *Note: CQRS is a Design Pattern, not an architecture.*
- **The "Fat Service" Problem:** In traditional architectures, mixing Read and Write operations in a single `UserService` or `ReservationService` leads to bloated classes. It forces us to inject multiple contexts and repositories into one place.
- **Advantage 1: Resolving Coupling & Cyclic Dependencies:** If a controller only needs one specific action, injecting an entire "Fat Service" (which might internally depend on 15 other services) creates high coupling and potential cycle dependencies. MediatR acts as an **orchestrator**, ensuring that we only inject and trigger the exact handler needed for that specific action.
- **Advantage 2: Data Source Separation:** In enterprise applications, the volume of reading is vastly higher than writing. CQRS allows us to separate these. We can use a lightweight SQL Server for writes, and a highly optimized database (like MongoDB or PostgreSQL) for reads/analytics. Data can be synchronized via Replication or Change Data Capture (CDC).
- **Trade-offs / Disadvantages:** CQRS and MediatR introduce a significant amount of boilerplate code (Commands/Queries, Handlers, Validators) compared to traditional architectures.

### 3. Why Custom Middlewares for Error Handling and Transactions?
- **Decision:** Use `GlobalErrorHandlerMiddleware` and `TransactionMiddleware`.
- **Trade-offs:** Developers could just use `try/catch` and `transaction.Commit()` in every MediatR handler or Controller.
- **Advantages:** Keeps the business logic extremely clean. If a command succeeds, the transaction commits. If it fails, it rolls back, and the error middleware catches the exception to format a standardized API error response.
- **Disadvantages:** Hides control flow slightly; new developers need to understand the middleware pipeline to know how errors and transactions are managed.

### 4. Future Development & Evolution (What's Next?)
- **Caching Layer:** Integrating Redis to cache frequent queries (like available rooms) to reduce database load *(in progress)*.
- **Validation Pipeline:** Implementing FluentValidation integrated with MediatR pipeline behaviors to validate commands before they even hit the handlers *(in progress)*.
- **Advanced Searching:** Implementing dynamic Expression Trees for complex filtering required by hotel search engines *(in progress)*.

---

## 📂 Project Structure

```text
HotelReservationSystemWebAPI
│
├── 🎯 Domain                  # Core Entities, Enums, and Repository Interfaces (No Dependencies)
├── ⚙️ Application             # CQRS Handlers, DTOs, AutoMapper Profiles, Business Logic
├── 🔌 Infrastructure          # EF Core DbContext, Migrations, Repository Implementations
└── 🌐 HotelReservationSystem.API # Controllers, Middlewares, Filters, Program.cs (Presentation)
---
## 🛠️ Technology Stack
- **Framework:** .NET 8 / 9
- **Architecture:** Onion Architecture, CQRS
- **Database:** SQL Server, Entity Framework Core (Code-First)
- **Libraries:** MediatR, AutoMapper
- **Security:** JWT (JSON Web Tokens)
---
## 🚀 Getting Started
### Prerequisites
- .NET SDK (8.0 or later)
- SQL Server
### Setup
1. Clone the repository:
   ```bash
   git clone https://github.com/Abdelrhman-elsaeed/HotelReservationSystemWebAPI.git
   ```
2. Navigate to the API directory:
   ```bash
   cd HotelReservationSystemWebAPI/HotelReservationSystem.API
   ```
3. Update the Connection String in `appsettings.json` to point to your local SQL Server instance.
4. Apply database migrations:
   ```bash
   dotnet ef database update --project ../Infrastructure --startup-project .
   ```
5. Run the application:
   ```bash
   dotnet run
   ```
---
## 📌 Roadmap & Status
- [x] Base Onion Architecture Setup
- [x] CQRS Implementation with MediatR
- [x] JWT Authentication & Authorization
- [x] Custom Middleware (Error Handling & Transactions)
- [x] Custom Action Filters
- [x] AutoMapper Configuration
- [ ] Room & Reservation Advanced CRUD *(in progress)*
- [ ] Advanced Filtering and Pagination *(in progress)*
- [ ] Redis Distributed Caching *(in progress)*
- [ ] FluentValidation Pipeline *(in progress)*
- [ ] Unit Testing *(in progress)*
---
*This README was designed not just to explain how to run the project, but to document the engineering mindset and architectural decisions behind it.*
