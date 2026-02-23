using EmberOps.ApiGateway.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// SignalR
builder.Services.AddSignalR();

var app = builder.Build();

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
app.MapGet("/api/orders", () => { Results.Ok("retorno las ordenes"); });
app.MapGet("/api/orders/dashborad", () => { Results.Ok("retorno ordenes completadas vs pendientes"); });
app.MapPost("/api/orders", () => { Results.Ok("aqui creo una orden"); });
app.MapPost("/api/orders/cancel", () => { Results.Ok("aqui cancelo una orden"); });

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
