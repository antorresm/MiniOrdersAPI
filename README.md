# Mini Orders API

A small REST API built with **.NET 8** for managing products and orders.

This project was implemented as a take-home challenge and demonstrates clean architecture, validation, persistence using Entity Framework Core, and simple performance optimizations.

---

## Tech Stack

- .NET 8 Web API
- Entity Framework Core
- SQLite
- Swagger (OpenAPI)

---

## Quick Start

### Prerequisites

- .NET 8 SDK installed
- Git

### Installation & Running

Clone the repository:

```bash
git clone <repo-url>
cd MiniOrdersAPI
```

Restore dependencies:

```bash
dotnet restore
```

Install EF Core CLI tools (if not already installed):

```bash
dotnet tool install --global dotnet-ef
```

Apply database migrations:

```bash
dotnet ef database update
```

Run the API:

```bash
dotnet run
```

Once the application starts, open Swagger in your browser:

```
https://localhost:{port}/swagger
```

From Swagger you can test all endpoints interactively.

---

## Database Setup

The project uses **SQLite** as the database.

Running the following command will create the database and apply migrations:

```bash
dotnet ef database update
```

This will generate the database file:

```
orders.db
```

### Default Seeded Products

When the application starts, some products are automatically seeded if the database is empty:

- Laptop
- Mouse
- Keyboard

These can be used immediately to create orders.

---

## API Endpoints

### Create Order

**POST** `/api/orders`

Example request:

```json
{
  "customerName": "Test NewTest",
  "lines": [
    { "productId": 1, "quantity": 2 },
    { "productId": 2, "quantity": 1 }
  ]
}
```

Response: `201 Created` with order details.

---

### Get Order by Id

**GET** `/api/orders/{id}`

Example:

```
GET /api/orders/1
```

Response: Order details with line items and status.

---

### List Orders (Paged)

**GET** `/api/orders?page=1&pageSize=20`

Optional filter by status:

```
GET /api/orders?status=Draft&page=1&pageSize=10
```

Orders are sorted by `CreatedAt` in descending order.

Response: Paginated list of orders with total count.

---

### Update Order Status

**PATCH** `/api/orders/{id}/status`

Example request:

```json
{
  "status": "Submitted"
}
```

**Status transition rules:**

| From | To | Allowed |
|---|---|---|
| Draft | Submitted | ✔ |
| Draft | Cancelled | ✔ |
| Submitted | Cancelled | ❌ |

Invalid transitions return `400 Bad Request`.

---

## Example curl Requests

### Create Order

```bash
curl -X POST https://localhost:5001/api/orders \
  -H "Content-Type: application/json" \
  -d '{
    "customerName": "Ada Lovelace",
    "lines": [
      {"productId": 1, "quantity": 2}
    ]
  }'
```

### Get Order by Id

```bash
curl https://localhost:5001/api/orders/1
```

### List Orders

```bash
curl https://localhost:5001/api/orders?page=1&pageSize=10
```

### Update Order Status

```bash
curl -X PATCH https://localhost:5001/api/orders/1/status \
  -H "Content-Type: application/json" \
  -d '{"status": "Submitted"}'
```

---

## Architecture & Design Decisions

### Layered Architecture

- **Controllers**: Kept minimal and intentionally thin. They validate input and delegate to services.
- **Services**: Encapsulate all business logic and orchestration.
- **Domain**: Entities and enums representing the core business model.
- **Data**: Entity Framework Core DbContext and configurations.

### DTOs Instead of Exposing Entities

**Decision**: Use DTOs for request and response models instead of exposing domain entities directly.

### SQLite for Persistence

**Decision**: Use SQLite instead of a heavier database.

**Why**:
- Requires no external setup or containers
- Ideal for a small project
- Provides enough functionality for the problem scope

**Trade-off**: Not suitable for high-concurrency production systems.

### Pagination with Database-Level Filtering

**Decision**: Implement pagination using `Skip()` and `Take()` in the database query.

**Why**:
- Filtering and paging are executed at the database level, not in memory
- Avoids loading unnecessary data into the application
- Scales better with larger datasets

### LINQ Projection for Listing Orders

**Decision**: Use LINQ projection when listing orders to select only necessary fields.

**Why**:
- Reduces memory footprint
- Database returns only the data needed for the response
- Improves performance on large result sets

### Indexes for Common Queries

**Decision**: Added indexes on `Orders.CreatedAt` and `Orders.Status`.

**Why**:
- Improves query performance for filtering by status
- Speeds up sorting by creation date (used in list endpoint)
