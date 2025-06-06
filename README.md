# FoodOutlet REST API & SPA

A full-stack food outlet review web application built using **ASP.NET Core (Empty)** and vanilla **JavaScript**. This was developed as part of an Advanced Programming university project, with the goal of designing a complete web service — including a RESTful API, secure authentication, and a responsive Single Page Application (SPA).

---

## 🔧 Tech Stack

- **Backend**: ASP.NET Core (Empty)
- **Frontend**: HTML, CSS, JavaScript (SPA)
- **Database**: MySQL (via XAMPP)
- **Authentication**: JWT with HTTP-only cookies
- **Password Security**: Salted + hashed using PBKDF2 (HMACSHA256)
- **Logging**: Custom-built logger system

---

## 🧠 Key Features

### ✅ User Authentication

- **JWT-based Auth** with access/refresh token flow
- **HTTP-only Cookies** for session security
- **Register/Login/Logout**
- **Refresh Token Endpoint** to keep sessions alive
- **Change Username / Password / Delete Account**

### 🔐 Password Hashing

- Randomly generated salt per user
- Stored alongside user in DB
- Example:
  ```csharp
  public string HashPassword(string password, byte[] salt)
  {
      string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
          password: password!,
          salt: salt,
          prf: KeyDerivationPrf.HMACSHA256,
          iterationCount: 10000,
          numBytesRequested: 256 / 8));
      return hashed;
  }

  public byte[] CreateSalt(int bits)
  {
      return RandomNumberGenerator.GetBytes(bits / 8);
  }
  ```
### 🧾 Data Models

-   **User**
    
-   **Review**
    
-   **FoodOutlet**
    

Each user can write multiple reviews, and each outlet can have many reviews (one-to-many relationships).

## 📡 API Overview

### Controllers

-   `UserController`
    
-   `FoodOutletController`
    
-   `LoginController`
    
-   `ReviewsController`
    

### DTO Usage

-   Avoids deep chaining issues and keeps HTTP responses clean.    

### Status Codes

-   Proper use of HTTP status codes (`200`, `400`, `401`, `500`, etc.) for success/failure responses.

-   Custom-written **JSON error messages** via `Newtonsoft.Json`.

### 🧾 Logging (Custom-Built Logger)

Forget `ILogger`, this project ships with its **own** logger (namespaced under `FoodOutletRESTAPIDatabase.Services.Logger`).

-   **Features**:
    
    -   Severity levels: `INFO`, `WARN`, `DEBUG`, `ERROR`
        
    -   Debug messages only print if explicitly enabled via:
 ```csharp
	 Logger.setDebugOutput(false); // Disable debug logs
```
-   Timestamped logs with color-coded output
    
-   Super easy to use:
```csharp
Logger.Log(Severity.DEBUG, $"Username updated to {updateUsername.Username}");
```
⚠️ `Severity.DEBUG` messages are **silenced by default** in production — toggle manually when needed.

### 👤 User Roles & Security

-   All new accounts are created with the role `"User"` by default.
    
-   Role `"Admin"` **must be assigned manually** in the database.
    
-   This is an intentional design decision to prevent privilege escalation via API manipulation.
    

> ✅ Secure-by-default: Admin access requires database-level changes.

## 🖥️ Frontend SPA

-   All HTML, CSS, and JavaScript are written from scratch inside the `wwwroot` folder.
    
-   Pages:
    
    -   🏠 **Homepage**
        
    -   🔐 **Login**
        
    -   📝 **Register**
        
    -   ⚙️ **User Settings** (Change name/password or delete account)
        
    -   📄 **Profile Page** (See all your reviews)
        
    -   🛠️ **Admin Page**
  
 ### 🎯 Frontend SPA – No Frameworks, No Nonsense

This project includes a handcrafted **Single Page Application (SPA)** written in **vanilla JavaScript**.

> **No frameworks** were used — all DOM manipulation, routing logic, API requests, and page transitions are 100% custom-written.

-    Fully modular structure to avoid redundant API/database calls

### Routing & UX

-   **Redirect logic** to send users to appropriate pages based on auth status.
    
-   Plans for fallback/error pages in future updates.
    
-   Optimized JavaScript to minimize redundant API calls and keep UI snappy.
