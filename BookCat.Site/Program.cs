using BookCat.Site.Data;
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

builder.Services.AddIdentityCore<IdentityUser>(
        options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
            options.User.RequireUniqueEmail = true;
        }
    )
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddSignInManager<SignInManager<IdentityUser>>()
    .AddUserManager<UserManager<IdentityUser>>();

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

app.MapStaticAssets();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();


/*
! ISSUES:
! 

TODO
TODO: 

? Future Ideas
? 

* Notes
* 

* Changes
* Remove scaffolded Identity pages and IdentityUI package
* Remove bootstrap
* Remove all styling and some default elements
* Start working on front end
*/