# FoodOutletRESTAPIDatabase

A full-stack food outlet review web application built using **ASP.NET Empty Core** and vanilla **JavaScript**. This was developed as part of an Advanced Programming university project with the goal of designing a complete web service including a RESTful API, secure authentication.

## Tech Stack

- **Backend**: ASP.NET Empty Core
- **Frontend**: HTML, CSS, JavaScript
- **Database**: MariaDB
- **Authentication**: JWT with HTTP-only cookies
- **Password Security**: Salted + hashed using PBKDF2 (HMACSHA256)

### Data Models

-   **User**
-   **Review**
-   **FoodOutlet**
    
Each user can write multiple reviews, and each outlet can have many reviews (one-to-many relationships).

## API Overview

### Controllers

-   `UserController`
-   `FoodOutletController`
-   `LoginController`
-   `ReviewsController`

### User Roles & Security

-   All new accounts are created with the role `"User"` by default.
-   Role `"Admin"` **must be assigned manually** in the database.

This is an intentional design decision to prevent privilege escalation via API manipulation.
