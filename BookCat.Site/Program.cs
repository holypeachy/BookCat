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
? Things I could improve (for readme)
? Since this is my first full stack project I can see a lot I could improve.
    I typically iterate on projects a lot while I'm working on them but I'm in a bit of a time crunch on this one.
    So here are some things that bother me that I could change:
? I'm sure I could rethink the IRepo a little bit more.
? Better usage of LINQ and DbContext SQL.
? Pagination in Books/Details/ is kinda useless when talking about performance because I request all reviews either way and they are loaded into memory, but ideally I would implement pagination in the repos
    to prevent it all being loaded to memory.
? I'm not happy with the naming schemes of the entire project. I haven't used CSS to this scale so naming on my CSS
   classes is something I could improve a lot.
? 

* Changes
* use regex to remove html tags from descriptions in GoogleBooksService
* add review pagination for book details
* add pagination to front end
* style reviews a bit more
* remove test action from Books controller and view
* rename test.js to stars.js, rename css as well
* style write review form
* 
*/