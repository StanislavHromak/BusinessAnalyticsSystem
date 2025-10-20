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

1. **Clone the repository**:
   ```bash
   git clone <repository_url>
   cd BusinessAnalyticsSystem
2. **Install dependencies** (NuGet restores automatically):
- Microsoft.AspNetCore.Identity.EntityFrameworkCore
- Microsoft.EntityFrameworkCore.Sqlite
- OpenIddict
3. **Run migrations** (SQLite database is created automatically at startup)
4. **Run the application**.

---

## Usage

### Authentication & Authorization
- **Register** a new account via `/Home/Register`.
- **Login** via `/Home/Login`.
- **Roles**:
  - **Admin** – full access, can manage users and financial data.
  - **Owner** – can add/edit/delete financial data.
  - **Investor** – view-only access to financial data.
- **Profile** management available at `/Home/Profile`:
  - Update username, full name, email, phone number.
  - Change password (optional).

### Dashboard & Financial Data
- After login, users are redirected to the **Dashboard**, which lists financial records.
- **Admin/Owner** can:
  - Add new financial data (`/Analytics/AddData`)
  - Edit existing records (`/Analytics/Edit/{id}`)
  - Delete records (`/Analytics/Delete/{id}`)
- **Investor** can only view:
  - List of financial data (`/Analytics/List`)
  - Details of individual records (`/Analytics/Details/{id}`)

### Financial KPIs
For each financial record, the following metrics are calculated automatically:
- **Revenue** = Price per Unit × Units Sold
- **Gross Costs** = Fixed Costs + (Variable Cost per Unit × Units Sold)
- **Total Costs** = Gross Costs + Investment
- **Profit** = Revenue − Total Costs
- **Margin per Unit** = Price per Unit − Variable Cost per Unit
- **ROI (%)** = Profit ÷ Investment × 100
- **ROS (%)** = Profit ÷ Revenue × 100
- **Break-Even Point (units)** = Fixed Costs ÷ (Price per Unit − Variable Cost per Unit)

---

## Project Structure

The project follows a standard ASP.NET Core MVC architecture with additional models for financial calculations and identity management.

BusinessAnalyticsSystem/
│
├─ Controllers/
│ ├─ HomeController.cs # Handles authentication, registration, profile, dashboard
│ ├─ AdminController.cs # Manages users and roles (Admin only)
│ └─ AnalyticsController.cs # CRUD operations for financial data and KPI calculations
│
├─ Models/
│ ├─ User.cs # Custom IdentityUser with FullName
│ ├─ RegisterView.cs # ViewModel for user registration
│ ├─ ProfileView.cs # ViewModel for editing user profile
│ ├─ UserView.cs # Helper ViewModel with User and Role
│ └─ FinancialData.cs # Financial record model with calculated KPIs
│
├─ Data/
│ └─ AppDbContext.cs # EF Core DbContext including Identity and FinancialData
│
├─ Views/
│ ├─ Home/ # Views for login, register, profile, dashboard
│ ├─ Admin/ # Views for user management
│ └─ Analytics/ # Views for AddData, List, Edit, Delete, Details
│
├─ wwwroot/ # Static files (CSS, JS, fonts)
├─ Program.cs # Application entry point and service configuration
└─ appsettings.json # App configuration

---

## Roles and Access

The system implements role-based access control (RBAC) using ASP.NET Core Identity. The following roles exist:

- **Admin**: Full access to user management, financial data CRUD operations, and system settings.
- **Owner**: Can add, edit, and delete financial records; view all data.
- **Investor**: Can view financial records and dashboards; no modification rights.

---

## Contacts

- GitHub: https://github.com/StanislavHromak

- Email: gromakstanislav@knu.ua

