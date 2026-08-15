You are a senior .NET Solution Architect and Software Engineer.

Build a complete production-quality E-Commerce Backend using ASP.NET Core 9 Web API.

Do NOT skip files.
Do NOT use placeholders.
Do NOT simplify.
Generate real code that compiles.

=========================
TECH STACK
=========================

Backend
- ASP.NET Core 9
- C#
- Clean Architecture
- OOP
- SOLID
- CQRS
- MediatR
- Entity Framework Core
- PostgreSQL
- Mapster
- FluentValidation
- Swagger

Do NOT implement authentication or authorization.
Security (Keycloak + JWT) will be added later.

=========================
ARCHITECTURE
=========================

Use Clean Architecture.

Projects

src/
    ECommerce.Domain
    ECommerce.Application
    ECommerce.Infrastructure
    ECommerce.Api

tests/
    ECommerce.UnitTests

=========================
PATTERNS
=========================

Use

Repository Pattern

Unit of Work

Result Pattern

Global Exception Middleware

Validation Pipeline

Pagination

Feature Folder Structure

Dependency Injection

Async/Await everywhere

CancellationToken

Domain Driven Design style entities

No Generic Repository.

Each aggregate has its own repository.

=========================
ENTITIES
=========================

Category

Product

InventoryTransaction

ShoppingCart

CartItem

CustomerProfile

Address

Order

OrderItem

ProductReview

Payment

Refund

=========================
MODULES
=========================

Category

Create
Update
Delete
Get
GetAll

Product

Create
Update
Delete
Search
Pagination
Filter
Deactivate
Activate

Customer

Create Profile
Update Profile
View Profile

Address

CRUD

Shopping Cart

Get Cart
Add Item
Update Quantity
Remove Item
Clear Cart

Order

Checkout
Create
Cancel
Confirm
Pack
Ship
Deliver
History

Review

Create
Update
Delete
Hide
List

Payment

Create
Verify
Mark Paid
Mark Failed

Refund

Request
Approve
Reject
Complete

Inventory

Stock In
Stock Out
Adjustment
History

=========================
BUSINESS RULES
=========================

Product price > 0

Stock cannot be negative

Customer cannot buy inactive product

Customer cannot review products not delivered

Only one review per product per customer

Refund amount cannot exceed payment

Cancelled order restores stock

Order total equals sum of order items

=========================
OUTPUT REQUIREMENTS
=========================

Generate EVERY file.

Each feature must contain

Entity

Repository Interface

Repository Implementation

EF Configuration

DTO

Commands

Queries

Handlers

Validators

Mapster Mapping

Controller

Unit Tests

Do not skip any code.

Do not summarize.

Do not write "implement later".

Write complete implementations.

After finishing one module, continue automatically with the next module until the whole project is complete.

Do not stop until every module has been generated.