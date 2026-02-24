using EmberOps.BuildingBlocks.Extensions;
using EmberOps.BuildingBlocks.Logging;
using EmberOps.Infrastructure.ServiceBus.RabbitMQ.Messaging;
using EmberOps.OrderService.Handlers;
using EmberOps.OrderService.Infrastructure.DB;
using EmberOps.OrderService.Infrastructure.Messaging;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();
builder.Host.ConfigureSerilog("OrderServiceLogs");

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//CommandHandlers
builder.Services.AddScoped<CreateOrderCommandHandler>();

//ServiceBus
builder.Services.AddMessageBus(builder.Configuration, new[]
{
    new EndpointConsumers("orders.create-order", typeof(CreateOrderConsumer))
});

builder.Services.AddDbContext<OrderDbContext>(options =>
{
    var cs = builder.Configuration.GetConnectionString("AppConnection");
    options.UseSqlServer(cs); 
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    
    dbContext.Database.Migrate();
}

app.UseSerilogRequestLogging();

app.UseMiddleware<RequestLoggingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

//Swagger Doc
app.UseSwagger();
app.UseSwaggerUI();

app.Run();
