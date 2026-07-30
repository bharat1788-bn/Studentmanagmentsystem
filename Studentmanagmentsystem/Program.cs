using Oracle.ManagedDataAccess.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Oracle Database Connection Test
var connectionString = builder.Configuration.GetConnectionString("OracleConnection");

using (var connection = new OracleConnection(connectionString))
{
    try
    {
        connection.Open();
        Console.WriteLine("Oracle Database Connected Successfully");
    }
    catch (Exception ex)
    {
        Console.WriteLine("Database Connection Failed: " + ex.Message);
    }
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Index}/{id?}");

app.Run();
