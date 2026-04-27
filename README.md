# Commerce System API

## Overview

The Commerce System API is a RESTful web service built using ASP.NET Core and Entity Framework Core. It simulates a basic e-commerce backend that manages users, products, and orders. The system supports product management, order creation, and retrieval of user-specific order history.

The project follows a layered architecture design to separate concerns between the API, business logic, and data access layers.

---

## Tech Stack

- ASP.NET Core Web API (.NET)
- Entity Framework Core
- SQL Server
- xUnit (Unit Testing)
- Swagger / OpenAPI

---

## Architecture

The application follows a 3-layer architecture:

- **Controllers** – Handle HTTP requests and responses
- **Services** – Contain business logic and validation rules
- **Repositories** – Handle database access using Entity Framework Core

Data Transfer Objects (DTOs) are used to separate internal models from API responses and requests.

---

## Features

### User Management
- Create users
- Retrieve user by ID
- Retrieve all orders for a specific user

### Product Management
- Create products
- Retrieve products
- Update product details (via PATCH)
- Enforced validation for price and stock quantity

### Order Management
- Create orders for a user
- Validate product availability and stock
- Calculate order totals
- Retrieve orders by user

### Validation
- Data annotations used for request validation
- Business rule validation in service layer
- Prevents invalid states (e.g., negative pricing or stock)

### Error Handling
- Custom exceptions for domain errors (e.g., UserNotFound, InsufficientStock)
- Proper HTTP status code responses from controllers

---

## API Endpoints

### Users
- `POST /users` – Create a user
- `GET /users/{id}` – Get user by ID
- `GET /users/{id}/orders` – Get all orders for a user
- `PATCH /users/{id}` – Update user info (name and/or email)

### Products
- `POST /products` – Create product
- `GET /products/{id}` – Get product by ID
- `GET /products` – Get all products
- `PATCH /products/{id}` – Update product fields
- `DELETE /products/{id}` – Delete product (Project requirement- debatable if useful)

### Orders
- `POST /orders` – Create an order
- `GET /orders/{id}` – Get order by ID (Authorization not currently added)
- `PATCH /orders/{id}` – Update order status

---

## Testing

Unit tests were implemented using xUnit.

Test coverage includes:
- Order creation logic
- Stock validation and insufficient stock scenarios
- Product creation and update logic
- User-related service functionality

Tests focus on service layer behavior and business rules.

---

## Design Notes

- Orders are linked to users (one-to-many relationship)
- Orders contain multiple order items (one-to-many relationship)
- Products are referenced within order items
- DTOs are used to prevent exposing domain models directly
- Stock cannot be negative, and price must be zero or greater

---

## Assumptions

- No authentication or authorization is implemented
- This project is focused on backend architecture and business logic
- Product stock is managed at the time of order creation
- Order totals are calculated server-side

---

## Future Improvements

- Add authentication (JWT-based?)
- Add pagination for product and order queries
- Mock payment (with encryption)
- Admin utility/priviledge
- Add discounts (checkout codes? sales table?)
- Introduce logging and monitoring

---

## How to Run

1. Update the connection string in `appsettings.json`
2. Run database migrations: dotnet ef database update
3. Run the application: dotnet run
4. Open Swagger UI: https://localhost:{port}/swagger

---

## Author

Nicholas Matsuda
