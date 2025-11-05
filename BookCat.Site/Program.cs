using BookCat.Site.Data;
using BookCat.Site.Middleware;
using BookCat.Site.Models;
using BookCat.Site.Repos;
using BookCat.Site.Services;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(
    options => options.UseSqlServer(builder.Configuration["Db:ConnectionString"])
);

builder.Services.AddIdentityCore<AppUser>(
        options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
            options.User.RequireUniqueEmail = true;
        }
    )
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddSignInManager<SignInManager<AppUser>>()
    .AddUserManager<UserManager<AppUser>>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
})
.AddCookie(IdentityConstants.ApplicationScheme, options =>
{
    options.LoginPath = "/Account/Login";
});


// Dependency Injection
builder.Services.Configure<GoogleBooksOptions>(builder.Configuration.GetSection("GoogleBooks"));
builder.Services.AddHttpClient<GoogleBooksService>();

builder.Services.AddScoped<GoogleBooksService>();
builder.Services.AddScoped<IRepo<Book>, BookRepo>();
builder.Services.AddScoped<IRepo<Review>, ReviewRepo>();
builder.Services.AddScoped<IRepo<BookIdentifier>, BooksIdentifierRepo>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    // Seeding Data
    Console.WriteLine("ℹ️  Seeding Data to Db");
    using (var scope = app.Services.CreateAsyncScope())
    {
        await DataSeeder.SeedRolesAsync(scope.ServiceProvider);
        await DataSeeder.SeedUsersAsync(scope.ServiceProvider);
    }
    Console.WriteLine("ℹ️  Seeding Ends Here\n");
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ValidateUserMiddleware>();

app.MapStaticAssets();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();


/*
? Things I could improve (for readme)
? Since this is my first full stack project I can see a lot I could improve.
    I typically iterate on projects a lot while I'm working on them but I'm in a bit of a time crunch on this one.
    So here are some things that bother me that I could change:
? I'm sure I could rethink the IRepo interface a little bit more. I actually don't think it was too necessary to use a repository pattern but it didn't hurt.
? Better usage of LINQ and DbContext SQL. Pagination in Books/Details/ is kinda useless when talking about performance because I request all reviews either way and they are loaded into memory, but ideally I would implement pagination in the repos
    to prevent it all being loaded to memory. Also, the book repos load navigation properties automatically, which is really bad.
? There are roles and a flag for reviews to be admin deleted, but I didn't implement any kind of moderation.
? CSS could be reworked, after working with CSS a lot more I can see ways of improving the way I approach styling pages.
? CSS naming was quite messy, I know now what sort of code I should turn into reusable classes.

* Changes
* Fix pagination bug
* Change Book AddedOn to DateTime
* Make sure reviews are loaded in searches and AllBooks
* 
*/