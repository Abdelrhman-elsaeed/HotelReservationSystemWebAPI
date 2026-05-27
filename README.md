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

## 🏗️ Architecture Decision Records (ADRs)

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
   ```
---
## 🛠️ Technology Stack
- **Framework:** .NET 8 / 9
- **Architecture:** Onion Architecture, CQRS
- **Database:** SQL Server, Entity Framework Core (Code-First)
- **Libraries:** MediatR, AutoMapper
- **Security:** JWT (JSON Web Tokens)

---
# 🧪 Unit Testing — Hotel Reservation System
## 🗂️ Test Project Structure
All unit tests live in the `Hotel.UnitTests` project and follow a consistent structure aligned with **CQRS handlers and Orchestrators** in the `Application` layer.
```
Hotel.UnitTests/
├── Facility_Test.cs                    # Facility CRUD + Get
├── RoomType_Test.cs                    # RoomType Add, Update, CheckExist
├── Room_Test.cs                        # Room Add, Update, Delete, Queries
├── Guest_Test.cs                       # Guest Add, Update, Delete, Queries
├── Offer_Test.cs                       # Offer Add
├── Reservation_Test.cs                 # Reservation Cancel, Update, GetById
├── AddRoomOrchestrator_Test.cs         # Multi-step room creation orchestration
├── DeleteRoomOrchestrator_Test.cs      # Multi-step room deletion orchestration
├── AddOfferOrchestrator_Test.cs        # Multi-step offer + room assignment
└── UpdateReservationOrchestrator_Test.cs # Multi-step reservation update
```
---
## 🛠️ Tech Stack
| Tool | Role |
|---|---|
| **NUnit** | Test framework (`[TestFixture]`, `[Test]`, `[SetUp]`) |
| **Moq** | Mocking repositories, `IMediator` |
| **FluentAssertions** | Expressive, readable assertions |
| **coverlet.collector** | Code coverage data collection |
| **ReportGenerator** | HTML coverage report generation |
---
## 📐 Test Conventions
Every test file follows the same pattern inherited from `Facility_Test.cs`:
```csharp
[TestFixture]
public class SomeEntity_Test
{
    private Mock<IRepository<SomeEntity>> _repoMock = null!;
    [OneTimeSetUp]            // wire AutoMapper mock once for the class
    public void OneTimeSetUp() { ... }
    [SetUp]                   // reset mock before each test
    public void SetUp() { _repoMock = new Mock<...>(); }
    [Test]
    [Category("Happy")]       // success paths
    public async Task Command_Success_ReturnsSuccessResponse() { /* Arrange / Act / Assert + Verify */ }
    [Test]
    [Category("Business")]    // validation & failure paths
    public async Task Command_FailScenario_ReturnsFailureResponse() { ... }
}
```
### Categories
| Category | Meaning |
|---|---|
| `Happy` | The golden path — all dependencies succeed |
| `Business` | Validation failures, not-found cases, save failures |
Run by category:
```bash
dotnet test --filter "Category=Happy"
dotnet test --filter "Category=Business"
```
---
## ✅ Test Results Summary
```
Passed!  - Failed: 0, Passed: 82, Skipped: 0, Total: 82
```
| Test File | Tests | Handlers Covered |
|---|---|---|
| `Facility_Test.cs` | 9 | Add, Update, Delete, GetById |
| `RoomType_Test.cs` | 8 | Add, Update, CheckExist |
| `Room_Test.cs` | 17 | Add, Update, Delete, GetRoomType, IsRoomExist, GetTotalPrice |
| `Guest_Test.cs` | 12 | Add, Update, Delete, GetGuest, IsGuestExist |
| `Offer_Test.cs` | 2 | AddOffer |
| `Reservation_Test.cs` | 9 | Cancel, UpdateDetails, GetById |
| `AddRoomOrchestrator_Test.cs` | 7 | 5-step chain + data-flow verification |
| `DeleteRoomOrchestrator_Test.cs` | 4 | 3-step chain |
| `AddOfferOrchestrator_Test.cs` | 5 | 3-step chain + data-flow verification |
| `UpdateReservationOrchestrator_Test.cs` | 6 | 3-step chain + status guards |
| **Total** | **82** | |
---
## 📊 Code Coverage Report
Coverage was generated using `coverlet.collector` + `ReportGenerator`.
### Commands
**Step 1 – Collect coverage:**
```bash
dotnet test "Hotel.UnitTests" --collect:"XPlat Code Coverage" --results-directory "Hotel.UnitTests/TestResults"
```
**Step 2 – Install ReportGenerator (first time only):**
```bash
dotnet tool install -g dotnet-reportgenerator-globaltool
```
**Step 3 – Generate HTML report:**
```bash
reportgenerator \
  -reports:"Hotel.UnitTests/TestResults/**/*.xml" \
  -targetdir:"Hotel.UnitTests/CoverageReport" \
  -reporttypes:"Html;TextSummary" \
  -assemblyfilters:"+Application"
```
Then open `Hotel.UnitTests/CoverageReport/index.html` in your browser.
---
### Overall Coverage (Application Layer)
| Metric | Value |
|---|---|
| **Line Coverage** | 42.6% (551 / 1292 lines) |
| **Branch Coverage** | 42.3% (105 / 248 branches) |
| **Method Coverage** | 45.4% (179 / 394 methods) |
> **Note:** The overall 42% figure reflects the _entire_ Application layer, which includes Auth, ViewModels, AutoMapper profiles, and other components that are intentionally **out of scope** for unit testing (they are covered by integration/API tests). The handlers we _did_ test all show **100% coverage**.
---
### 100% Coverage — Tested CQRS Handlers
The following handlers are **fully covered (100% line + branch)**:
| Namespace | Handler |
|---|---|
| `Application.CQRS.Facility` | Add, Update, Delete, GetById |
| `Application.CQRS.Guest` | Add, Update, Delete, GetGuest, IsGuestExist |
| `Application.CQRS.Offer` | AddOffer |
| `Application.CQRS.Reservation` | Cancel, UpdateDetails, GetById |
| `Application.CQRS.ReservationRoom.Orchestrators` | UpdateReservationOrchestratorHandler |
| `Application.CQRS.Room` | Add, Update, Delete, GetRoomType, IsRoomExist, GetTotalPrice |
| `Application.CQRS.Room.Orchestrators` | AddRoomOrchestratorHandler, DeleteRoomOrchestratorHandler |
| `Application.CQRS.RoomOffer.Orchestrators` | AddOfferOrchestratorHandler |
| `Application.CQRS.RoomType` | Add, Update, CheckRoomTypeExist |
---
### Intentionally Excluded from Unit Tests
| Component | Reason |
|---|---|
| `Auth` (Login, Register, RefreshToken) | Requires Identity + JWT infrastructure — covered by integration tests |
| `AddReservationCommandHandler` | Orchestrates 3+ mediator calls — integration-level complexity |
| `GetAllRoomsQueryHandler` | Requires real EF Core predicate builder (LinqKit) — not mockable cleanly |
| `ViewModels`, `AutoMapper Profiles` | Pure mapping/validation — no business logic to unit test |
| `RoleFeature` | Requires role/permission infrastructure |
---
## 🔗 Orchestrator Tests — What Makes Them Special
Orchestrators coordinate multiple CQRS handlers in sequence. Their tests verify things normal command tests cannot:
### 1. Step Failure Short-Circuit
Each test verifies that when step N fails, steps N+1 onwards are **never called**:
```csharp
// If AddRoomType fails → AddRoomDetails, AddFacility, etc. must NOT be called
await act.Should().ThrowAsync<BusinessException>();
_mediatorMock.Verify(x => x.Send(It.IsAny<AddRoomDetailsCommand>(), ...), Times.Never);
```
### 2. Data Flow Between Steps
Verifies that output of step N is correctly passed as input to step N+1:
```csharp
// AddRoomOrchestrator: RoomType ID from step 1 must become RoomTypeId in step 2
capturedRoomTypeId.Should().Be(42,
    "the orchestrator must propagate the new RoomType ID to the room details step");
```
### 3. Business Rule Guards (UpdateReservationOrchestrator)
Tests that the status guard blocks updates on cancelled/rejected reservations:
```csharp
// Cancelled reservation → must throw, must NOT proceed to update steps
var cancelledReservation = new GetReservationDetailsDto { Status = "Cancelled" };
await act.Should().ThrowAsync<BusinessException>()
    .WithMessage("Cannot update a cancelled or rejected reservation");
```

---
## 🚀 Getting Started
### Prerequisites
- .NET SDK (8.0 or later)
- SQL Server
### Setup
1. Clone the repository:
   ```bash
   git clone https://github.com/Abdelrhman-elsaeed/HotelReservationSystemWebAPI.git

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
