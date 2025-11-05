# 😺 BookCat
#### BookCat is a **full-stack ASP.NET Core MVC web application** for discovering, reviewing, and cataloging books. It integrates server-side rendering, clean REST-style endpoints, and responsive front-end design.
- Create an account and browse the catalog.
- Add books from local or external sources (Google Books API).
- Write and edit reviews, rate titles, and view other users’ profiles.

## 🧩 Future Improvements
This project was built under a tight timeline, but I’ve identified areas I’d like to revisit:
- Refine the repository pattern (consider leaner abstractions or direct DbContext queries).
- Implement true pagination in the reviews repo to avoid loading large collections.
- Add moderation tools leveraging existing role flags.
- Improve CSS naming and modularity for more reusable components.

## 🧪 Testing
Special thanks to my buddy [JK_Bizcuits](https://github.com/JKBizcuits): 
> I shit talked my friend's project until he fixed it.

## 🧰 Tech Stack (Summary)
| Layer          | Technologies                              |
| -------------- | ----------------------------------------- |
| Backend        | ASP.NET Core 9, C#, Entity Framework Core |
| Database       | SQL Server Express                        |
| Frontend       | Razor Views, HTML5, CSS3, JavaScript      |
| Auth           | ASP.NET Identity                          |
| External APIs  | Google Books API                          |
| Image Handling | ImageSharp                                |


### 🧱 Architecture
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
Prerequisites: .NET 9 SDK, SQL Server Express, and a Google Books API key.
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

