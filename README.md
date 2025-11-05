# 😺 BookCat
#### BookCat is a full-stack web application built with ASP.NET Core MVC designed for users to browse, review, and catalog books. The platform integrates server-side rendering and clean REST-style endpoints.
- BookCat is a web application built for discovering and reviewing books.
- Users can create accounts, search for books locally or add books via external data sources.
- Users can see and write reviews.
- Users can see other users' profiles in which they can see the user's reviews, bio, and other relevant information.

## ✅ Possible Improvements
Since this is my first full stack project I can see a lot I could improve. I typically iterate on projects a lot while I'm working on them but I'm in a bit short on time on this one. So here are some things that bother me that I would change:
- Rethink the IRepo interface a little bit more. I also don't think it was too necessary to use a repository pattern but it didn't hurt.
- Better usage of LINQ and DbContext SQL. Pagination in Books/Details/ is kinda useless when it comes to performance because I request all reviews either way and they are all loaded into memory, but ideally I would implement pagination in the repos.
  Also, the repos load navigation properties automatically, which is really bad.
- There are roles and a flag for reviews to be admin deleted, but I didn't implement any kind of moderation.
- CSS could be reworked, after working with CSS a lot more I can see ways of improving the way I approach styling pages. CSS naming was quite messy, I know now what sort of code I should turn into reusable classes.

## 🧱 Architecture
- ASP.NET Core MVC for the presentation and controller layer.
- Entity Framework Core as ORM for database interaction (SQL Server).
- Repository pattern to abstract data access.
- Identity for user authentication, authorization, and account management.
- Dependency Injection for service and repository registration.
- DTOs and ViewModels to cleanly separate domain and UI data models.

### 🗄️ Database & Data
- SQL Server Express, managed through Entity Framework Migrations.
- Relational structure: Users ↔ Reviews ↔ Books.
- Book data integrated with the Google Books API to support search and cataloging of new titles.
- Each user can add books, write reviews, and maintain personalized profile data.

### 🧩 Front-End
- Server-rendered Razor Views with custom HTML/CSS (No Bootstrap).
- Responsive layout built with Flexbox and media queries for adaptive design.
- Client-side interactivity using vanilla JavaScript (e.g., star-rating system).
- Validation via Unobtrusive jQuery Validation.

### 🔒 Security & Sessions
- Cookie-based authentication managed through ASP.NET Identity.
- CSRF protection with [ValidateAntiForgeryToken].
- Custom Middleware that validates user existence and automatically signs out deleted accounts.

### 🖼️ Media & Storage
- User profile images uploaded via form and resized on the server using ImageSharp before storage.
- Optimized storage structure with unique filenames for automatic cache busting.

### ⚙️ Core Features
- Account registration, login, and profile management.
- Book catalog browsing and search integration with external API.
- User-generated reviews and ratings.
- Responsive UI with mobile support.

## 🚀 Getting Started
```bash
git clone git@github.com:holypeachy/BookCat.git

cd BookCat/BookCat.Site/

dotnet user-secrets set GoogleBooks:ApiKey YOUR_API_KEY_HERE

dotnet user-secrets set Db:ConnectionString YOUR_CONNECTION_STRING_HERE

dotnet run
```
Credentials for admin:  
> Email: testadmin@gmail.com  
  Password: Admin123!

## 📤 Output Example:
### Landing
<img width="2310" height="1322" alt="image" src="https://github.com/user-attachments/assets/2c0ec18f-790e-40e9-8e66-7bf9f100848d" />

### Catalog
<img width="2308" height="1320" alt="image" src="https://github.com/user-attachments/assets/49b83da1-accf-4953-8edd-9a11d9b663c3" />

### Book Details
<img width="2302" height="1319" alt="image" src="https://github.com/user-attachments/assets/66b4671c-983b-40c6-bfc6-54bd864cfe74" />

### User Dash
<img width="2302" height="1322" alt="image" src="https://github.com/user-attachments/assets/c8e11d7e-8874-43a7-bcc5-4cf8562ea113" />

### Search (Query: Music)
<img width="2311" height="1320" alt="image" src="https://github.com/user-attachments/assets/555f580f-33c3-48f3-9b51-11c5adf03ca1" />

### After clicking "Add" it does an external search
<img width="2309" height="1318" alt="image" src="https://github.com/user-attachments/assets/c146fd3d-63d8-44fc-b782-a38aadde80b4" />

### Pagination
<img width="1247" height="491" alt="image" src="https://github.com/user-attachments/assets/a100da1e-7c74-4639-9a2e-b6cf9dce9b8e" />

### Registration
<img width="2310" height="1317" alt="image" src="https://github.com/user-attachments/assets/0bab2715-f911-4ef7-b2da-6c6f9fc85f92" />

### Write and Edit Review. If you scroll up you can see the book's information.
<img width="2303" height="1316" alt="image" src="https://github.com/user-attachments/assets/d82e796c-5f73-4be8-9406-31628a823420" />

