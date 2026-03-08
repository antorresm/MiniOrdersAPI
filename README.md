# Mini Orders API

A small REST API built with **.NET 8** for managing products and orders.

This project was implemented as a take-home challenge and demonstrates clean architecture, basic validation, persistence using EF Core, and simple performance optimizations.

---

# Tech Stack

- .NET 8 Web API
- Entity Framework Core
- SQLite
- Swagger (OpenAPI)

---

# How to run the project

Clone the repository and run the following commands:

```bash
dotnet restore
dotnet ef database update
dotnet run

## Performance considerationst

The API includes some small performance optimizations:

- LINQ projection is used when listing orders to avoid loading unnecessary data.
- Pagination is implemented using Skip/Take so filtering and paging are executed in the database.
- Indexes were added on Orders.CreatedAt and Orders.Status to improve query performance when filtering and sorting orders.