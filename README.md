# Movie Rental Web Application

A learning project evolving through multiple homework phases: from in‑memory lists, to AJAX, authentication + validation, and finally a SQL Server + stored procedures–backed rental system with pagination, admin tools, and rental transfer logic.

## 1. Evolution (Homework Milestones)

| Phase | Focus | Key Additions |
|-------|-------|---------------|
| HW1 | In-memory domain & basic REST | Movie & User classes, static lists, Insert/Read, uniqueness rules, basic GET filters (title, release date) |
| HW2 | Client consumption | Two pages (index / MyMovies), render JSON, add-to-cart -> static list, delete, server-side filtering |
| HW3 | Auth & Forms | Register/Login, validation (password, names), add-movie form with full field rules, authorization guarding pages |
| HW4 | DB Integration | Replace static lists with SQL tables + Stored Procedures, rental entity, logical deletes (deletedAt), priceToRent, rentalCount |
| HW5 (+ bonuses) | Advanced flows | Rental transfer, overlapping date validation, grossWorldwide accumulation, admin activation toggle, pagination, profile edit |
| **Refactor** | **Code Hardening & Structure** | **Moved HTML files to a `pages` directory, enhanced JS routing, added backend null-safety, and improved exception handling.** |

## 2. Core Features

Movies:
- Pagination, search by title, filter by date range.
- Add new movie (validated form).
- Rent with dynamic total price (date range * priceToRent).
- View current rentals; per-rental total price; rental count.
- Transfer active rental to another user (conflict & date guards).
- Logical delete support (deletedAt).

Users:
- Register (validation + default Active = true).
- Login (body credentials).
- Edit profile (name/email/password).
- Activate / Deactivate (admin page) – inactive users blocked from login.
- Logical delete safeguards inside rental SP logic (cannot rent with deleted user).

Rentals:
- Prevent overlapping self-rent duplicates.
- Track rentalCount (increments per rental, not per day).
- Add totalPrice into grossWorldwide.
- Transfer only if rental still active (current date within range).

Admin:
- List users, toggle Active, filter/search client-side.

Authorization:
- Unauthenticated users limited to main page; protected pages redirect to login.
- Renting / transfer actions require authentication.

## 3. Technology Stack

Backend:
- ASP.NET Core Web API (C#)
- SQL Server with Stored Procedures (CRUD + Rent + Transfer)
- ADO.NET / DBServices abstraction
Frontend:
- HTML, CSS, jQuery (AJAX)
Shared:
- JSON over HTTPS (dev: localhost; prod URL configurable in JS)

## 4. Folder Structure (after rename suggestion)

```
/README.md
/client
  pages/
    index.html
    MyMovies.html
    addMovie.html
    login.html
    register.html
    editProfile.html
    admin.html
    navbar.html
  css/
  JS/
    clientMethods.js
/server
  hw4.csproj
  Program.cs
  Controllers/
  Models/
  DTO/
  DAL/
  Properties/
```

## 5. Data Model (Conceptual)

Movies:
- id (IDENTITY), url, primaryTitle, description, primaryImage, year, releaseDate, language,
  budget, grossWorldwide, genres (string), isAdult, runtimeMinutes,
  averageRating, numVotes, priceToRent, rentalCount, deletedAt (nullable)

Users:
- id (IDENTITY), name, email, passwordHash (recommended), active (bit), isAdmin (optional), deletedAt

RentedMovie:
- id (IDENTITY), userId (FK), movieId (FK), rentStart, rentEnd, totalPrice, deletedAt

## 6. Stored Procedures (Representative)

Users:
- sp_Users_Insert / sp_Users_Update / sp_Users_Delete (logical) / sp_Users_Select / sp_Users_SetActive
Movies:
- sp_Movies_Insert / sp_Movies_Update / sp_Movies_Delete (logical) / sp_Movies_SelectPaged / sp_Movies_SearchByTitle / sp_Movies_FilterByDate
Rentals:
- sp_Rentals_Rent (insert + increment rentalCount + add totalPrice to grossWorldwide)
- sp_Rentals_GetActiveByUser
- sp_Rentals_Transfer (date overlap + validity checks)
(Names illustrative—match your actual implementation.)

## 7. API Summary (High-Level)

Movie:
- POST /api/Movie
- GET /api/Movie?page=&pageSize=
- GET /api/Movie/{id}
- GET /api/Movie/search?title=
- GET /api/Movie/searchByDate?startDate=&endDate=
- DELETE /api/Movie/{id}
- POST /api/Movie/rent
- DELETE /api/Movie/rent/{userId}/{movieId}
- GET /api/Movie/rented/{userId}
- POST /api/Movie/transferRental

User:
- POST /api/User/register
- POST /api/User/login
- GET /api/User
- PUT /api/User/{id}
- PUT /api/User/{id}/active

## 8. Frontend Key Scripts

Main orchestration & AJAX utilities: [`clientMethods.js`](client/JS/clientMethods.js)  
Notable functions:
- Initialization: [`init`](client/JS/clientMethods.js) (now with enhanced page detection for routing)
- Generic AJAX: [`ajaxCall`](client/JS/clientMethods.js)
- Movie loading & pagination: [`getAllMoviesListFromServer`](client/JS/clientMethods.js)
- Search: [`searchMoviesByTitle`](client/JS/clientMethods.js)
- Rental modal: [`openRentModal`](client/JS/clientMethods.js), [`setupRentModalEvents`](client/JS/clientMethods.js)
- Transfer modal: [`openTransferModal`](client/JS/clientMethods.js), [`setupTransferModalEvents`](client/JS/clientMethods.js)
- Auth flows: [`handleLogin`](client/JS/clientMethods.js), [`handleRegister`](client/JS/clientMethods.js)
- Profile edit: [`initEditProfileForm`](client/JS/clientMethods.js)
- Admin page: [`initAdminPage`](client/JS/clientMethods.js)

## 9. Development Setup

Backend:
```
cd server
dotnet restore
dotnet run
```
Configure connection string + any Secrets (do not commit passwords).

Frontend:
- Open `/client` with Live Server or host via simple static server.
- Ensure ports in [`clientMethods.js`](client/JS/clientMethods.js) (dev section) match backend HTTPS port.

Database:
- Run schema scripts (tables + SPs).
- Seed movies using one-time loader page or bulk import.
- Confirm priceToRent (10–30) and rentalCount defaults.
- The database for movies is also available as JSON data in the [`movies-db.js`](client/JS/movies-db.js) file.  
- Use the [`HtmlPage.html`](client/pages/HtmlPage.html) file to import this JSON data into the database. This page is designed for anyone who wants to try the project and populate the database with sample data.

## 10. Validation Rules (Highlights)

Registration:
- Name: letters only ≥2 chars
- Password: ≥8 chars, 1 uppercase, 1 digit
Movie Form:
- primaryTitle, primaryImage, year, releaseDate, language, runtimeMinutes required
- Year must match releaseDate year
- budget ≥ 100000 if provided
- priceToRent 10–30
- genres comma-separated words

## 11. Security Notes

- Passwords are hashed using BCrypt (per‑password salt + adaptive work factor) before persistence (see [`DBservices.Login`](server/DAL/DBservices.cs)); plaintext is never stored.
- Secrets (connection strings, etc.) must be provided via environment / user secrets and not committed.
- Logical deletes (deletedAt) prevent interaction with soft‑removed entities (e.g., renting deleted movies or by deleted users).


## 12. Future Improvements ׂ(AI Recommendation)

- JWT-based auth & refresh tokens
- Role-based authorization
- Server-side pagination metadata & sorting
- Rate limiting
- Unit + integration tests
- CI/CD pipeline
- Image validation / CDN

## 13. Naming / Conventions

- Controllers: Singular (MovieController, UserController)
- Stored Procedures: sp_<Entity>_<Action>
- DTOs: <Entity><Purpose>Dto
- Logical delete: set deletedAt NOT physical removal (except cascading requirement on rental table rows)
---