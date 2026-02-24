using EmberOps.ApiGateway.Features.Orders;
using EmberOps.ApiGateway.Hubs;
using EmberOps.ApiGateway.Infrastructure.Messaging;
using EmberOps.ApiGateway.Infrastructure.Persistance;
using EmberOps.BuildingBlocks.Extensions;
using EmberOps.BuildingBlocks.Logging;
using EmberOps.Infrastructure.ServiceBus.RabbitMQ.Messaging;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

//Serilog
builder.Host.UseSerilog();
builder.Host.ConfigureSerilog("ApiGatewayLogs");

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// SignalR
builder.Services.AddSignalR();

// Service Bus subscriptions
builder.Services.AddMessageBus(builder.Configuration, new[]
{
    new EndpointConsumers("apigateway-order-events:orderCreated", typeof(OrderCreatedNotificationConsumer)),
    new EndpointConsumers("apigateway-order-events:orderCreatedProjection", typeof(OrderCreatedProjectionConsumer)),
});

builder.Services.AddDbContext<BffDbContext>(options =>
{
    var cs = builder.Configuration.GetConnectionString("AppConnection");
    options.UseSqlServer(cs);
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<BffDbContext>();

    dbContext.Database.Migrate();
}

app.UseSerilogRequestLogging();

app.UseMiddleware<RequestLoggingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();

//Minimal API imp

//Services for auth
app.MapPost("/api/auth/logIn", () => { Results.Ok("logeado"); });
app.MapPost("/api/auth/logOut", () => { Results.Ok("deslogeado"); });
app.MapPost("/api/auth/signIn", () => { Results.Ok("deslogeado"); });

//Services for orders
app.MapOrderEndpoints();


//Services for payment
app.MapPost("/api/payment", () => { Results.Ok("aqui pago una orden"); });
app.MapGet("/api/payment", () => { Results.Ok("retorno los pagos con sus estados"); });
app.MapGet("/api/payment/dashborad", () => { Results.Ok("retorno completados completadas vs pendientes"); });


//Services for inventory

app.MapPost("/api/inventory", () => { Results.Ok("aqui agrego producto al inventario"); });
app.MapPut("/api/inventory", () => { Results.Ok("aqui actualizo producto del inventario"); });
app.MapDelete("/api/inventory", () => { Results.Ok("aqui elimino producto del inventario"); });


//Swagger Doc
app.UseSwagger();
app.UseSwaggerUI();

// Hub notifications
app.MapHub<NotificationsHub>("/hubs/notifications");
app.MapHub<DashboardHub>("/hubs/notifications/dashboard");

app.Run();
