using Microsoft.EntityFrameworkCore;
using CommerceSystem.Api.Data;
using CommerceSystem.Api.Services;
using CommerceSystem.Api.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// DbContext
builder.Services.AddDbContext<StoreDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IProductService, ProductService>(); // Dependency Injection
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();

builder.Services.AddSwaggerGen(); // Adding Swager
builder.Services.AddEndpointsApiExplorer();


var app = builder.Build();

/*
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
*/
app.UseHttpsRedirection();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Serves the JSON at /swagger/v1/swagger.json    
    app.UseSwaggerUI(); // Serves the UI at /swagger
}

app.UseAuthorization();

app.MapControllers();

app.Run();