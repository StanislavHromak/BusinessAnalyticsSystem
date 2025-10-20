# BusinessAnalyticsSystem

## Description

Business Analytics System is a web application for financial analysis and business intelligence.  
The platform allows users to:

- Track key performance indicators (KPI)
- Analyze financial metrics (ROI, ROS, Break-Even)
- Optimize business strategies

Implemented with **ASP.NET Core MVC** using **SQLite**, **ASP.NET Identity**, and **OpenIddict** for OAuth2/OpenID Connect.

---

## Features

### Users and Authentication

- **Registration and login** using Identity
- **User profile** with editable information and password change
- **OAuth2/OpenID Connect** via OpenIddict
- **User roles**:
  - **Admin**: full access
  - **Owner**: manage financial data
  - **Investor**: view-only access

### Financial Calculations

- Enter financial data (`FinancialData`): date, costs, unit price, units sold, investment
- Automatic KPI calculations:
  - **Revenue** — total revenue
  - **GrossCosts** — gross costs
  - **TotalCosts** — total costs
  - **Profit** — profit
  - **MarginPerUnit** — margin per unit
  - **ROI (%)** — return on investment
  - **ROS (%)** — return on sales
  - **Break-Even** — break-even point (units)

- CRUD operations:
  - **Create**: add new financial data
  - **Read**: view list and details
  - **Update**: edit existing data
  - **Delete**: remove records

---

## Architecture

- **Models**:
  - `User` — custom Identity model with FullName and Phone
  - `FinancialData` — financial data model with KPI calculation methods
  - ViewModels: `RegisterView`, `ProfileView`, `UserView`
- **Data**: `AppDbContext` with Identity and OpenIddict
- **Controllers**:
  - `HomeController` — registration, login, profile, dashboard
  - `AnalyticsController` — financial data CRUD
  - `AdminController` — user and role administration
- **Views**: Razor Views for all features
- **OpenIddict**: OAuth2/OpenID Connect server implemented in `Program.cs`

---

## Requirements

- .NET 8 SDK or higher
- SQLite
- Browser for testing
- Visual Studio 2022 or VS Code

---

## Setup and Run

1. Clone the repository:
   ```bash
   git clone <repository_url>
   cd BusinessAnalyticsSystem
2. Install dependencies (NuGet restores automatically):
- Microsoft.AspNetCore.Identity.EntityFrameworkCore
- Microsoft.EntityFrameworkCore.Sqlite
- OpenIddict
- Bootstrap, FontAwesome
3. Run migrations (SQLite database is created automatically at startup)
4. Run the application:
