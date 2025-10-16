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
* Fix crash when adding a review with a newly registered user
* Assign user role when a new user register
* Add overload Details action for pagination, logic is mostly done
* Temp fix null user after deleting account on Db
* Add GoogleService removes common html tags from book description
* 
*/