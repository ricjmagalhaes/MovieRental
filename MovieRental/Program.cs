using MovieRental.Customer;
using MovieRental.Data;
using MovieRental.Movie;
using MovieRental.PaymentProviders;
using MovieRental.Rental;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddEntityFrameworkSqlite().AddDbContext<MovieRentalDbContext>();

// AggregateException Injection for Rental Features
//.NET is saying that are trying to inject a DbContext (which is scoped) into a service registered as a singleton.
//builder.Services.AddSingleton<IRentalFeatures, RentalFeatures>();

//Service Injection for Rental Features with AddScoped
builder.Services.AddScoped<IRentalFeatures, RentalFeatures>();

//Independent Payment Providers Injection
builder.Services.AddScoped<IPaymentProvider, MbWayProvider>();
builder.Services.AddScoped<IPaymentProvider, PayPalProvider>();

builder.Services.AddScoped<IMovieFeatures, MovieFeatures>();
builder.Services.AddScoped<ICustomerFeatures, CustomerFeatures>();

//Use on swagger xml comments
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

// Logging by default
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug(); 
  

// Use Serilog on host builder
builder.Host.UseSerilog();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var client = new MovieRentalDbContext())
{
	client.Database.EnsureCreated();
}

app.Run();
