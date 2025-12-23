using Microsoft.EntityFrameworkCore;
using odev.Data;
using odev.Services;
using odev.Middlewares;
using odev.DTOs;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// DB Context
// DB Context
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.ConfigureWarnings(w => w.Default(WarningBehavior.Ignore));
});

// Dependency Injection
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IFlightService, FlightService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

var app = builder.Build();

// Middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi(); // .NET 9 Standard
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Minimal API Endpoints (Requirement: CRUD via Minimal API as well)
// We create separate groups to ensure they are listed under their respective Entity names in Swagger,
// fitting the user's request for "CRUD under entity name".

// Helper to map ApiResponse to Minimal API Results
static IResult GetResult<T>(ApiResponse<T> response)
{
    return response.Success 
        ? (response.StatusCode == 201 && response.Data != null 
            ? Results.Created("", response) // Minimal API handles location better if needed, but this works
            : Results.Json(response, statusCode: response.StatusCode))
        : Results.Json(response, statusCode: response.StatusCode);
}

// 1. Users Minimal API
var usersGroup = app.MapGroup("/api/min/users").WithTags("Users");

usersGroup.MapGet("/", async (IUserService service) => GetResult(await service.GetAllAsync()));
usersGroup.MapGet("/{id}", async (int id, IUserService service) => GetResult(await service.GetByIdAsync(id)));
usersGroup.MapPost("/", async ([FromBody] CreateUserDto dto, IUserService service) => 
{
    var res = await service.CreateAsync(dto);
    return res.Success ? Results.Created($"/api/min/users/{res.Data!.Id}", res) : GetResult(res);
});
usersGroup.MapPut("/{id}", async (int id, [FromBody] UpdateUserDto dto, IUserService service) => GetResult(await service.UpdateAsync(id, dto)));
usersGroup.MapDelete("/{id}", async (int id, IUserService service) => GetResult(await service.DeleteAsync(id)));

// 2. Flights Minimal API
var flightsGroup = app.MapGroup("/api/min/flights").WithTags("Flights");

flightsGroup.MapGet("/", async (IFlightService service) => GetResult(await service.GetAllAsync()));
flightsGroup.MapPost("/", async (CreateFlightDto dto, IFlightService service) => 
{
    var res = await service.CreateAsync(dto);
    return Results.Created($"/api/min/flights/{res.Data!.Id}", res);
});
flightsGroup.MapPut("/{id}", async (int id, UpdateFlightDto dto, IFlightService service) => GetResult(await service.UpdateAsync(id, dto)));
flightsGroup.MapDelete("/{id}", async (int id, IFlightService service) => GetResult(await service.DeleteAsync(id)));

// 3. Bookings Minimal API
var bookingsGroup = app.MapGroup("/api/min/bookings").WithTags("Bookings");

bookingsGroup.MapGet("/", async (IBookingService service) => GetResult(await service.GetAllAsync()));
bookingsGroup.MapPost("/", async (CreateBookingDto dto, IBookingService service) => 
{
    var res = await service.CreateAsync(dto);
    return res.Success ? Results.Created($"/api/min/bookings/{res.Data!.Id}", res) : GetResult(res);
});
bookingsGroup.MapDelete("/{id}", async (int id, IBookingService service) => GetResult(await service.DeleteAsync(id)));

// 4. Payments Minimal API
var paymentsGroup = app.MapGroup("/api/min/payments").WithTags("Payments");

paymentsGroup.MapGet("/", async (IPaymentService service) => GetResult(await service.GetAllAsync()));
paymentsGroup.MapPost("/", async (CreatePaymentDto dto, IPaymentService service) => 
{
    var res = await service.CreateAsync(dto);
    return res.Success ? Results.Created($"/api/min/payments/{res.Data!.Id}", res) : GetResult(res);
});

// Auto-Migration on startup for convenience
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    // db.Database.EnsureDeleted(); // Removed to prevent file locking and data loss
    db.Database.EnsureCreated();
}

app.Run();
