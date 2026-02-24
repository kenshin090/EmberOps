using EmberOps.ApiGateway.Application.Dto;
using EmberOps.ApiGateway.Infrastructure.DTO;
using EmberOps.ApiGateway.Infrastructure.Persistance;
using EmberOps.Contracts.Order;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace EmberOps.ApiGateway.Features.Orders;

public static class OrderEndpointMappings
{
    public static IEndpointRouteBuilder MapOrderEndpoints(
        this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/orders", CreateOrder)
            .Accepts<CreateOrderHttpRequest>("application/json")
            .Produces(StatusCodes.Status202Accepted)
            .WithOpenApi();
        app.MapGet("/api/orders", GetOrders)
            .Produces(StatusCodes.Status200OK)
            .WithOpenApi();
        app.MapGet("/api/orders/dashborad", GetOrdersDashboard)
           .Produces(StatusCodes.Status200OK)
           .WithOpenApi();        
        app.MapPost("/api/orders/cancel", () => { Results.Ok("aqui cancelo una orden"); });

        return app;
    }

    private static async Task<IResult> CreateOrder(
        CreateOrderHttpRequest request,
        ISendEndpointProvider send,
        CancellationToken ct)
    {
        var correlationId = Guid.NewGuid();

        var endpoint = await send.GetSendEndpoint(
            new Uri("queue:orders.create-order"));

        await endpoint.Send(
            new CreateOrderRequest(
                correlationId,
                request.ProductsInOrder
                    .Select(i => new ProductInOrder
                    (
                        i.Sku,
                        i.Quantity,
                        i.UnitPrice,
                        i.Name
                    ))
                    .ToList()
            ),
            ctx => ctx.CorrelationId = correlationId,
            ct);

        return Results.Accepted("Ok", new { correlationId });
    }

    private static async Task<IResult> GetOrders(BffDbContext dbContext, CancellationToken ct)
    {
        var orderReadModelList = await dbContext.Orders.AsNoTracking().ToListAsync(ct);
        return Results.Ok(orderReadModelList);
    }

    private static async Task<IResult> GetOrdersDashboard(BffDbContext dbContext, CancellationToken ct)
    {
        var orderReadModelList = await dbContext.Orders.AsNoTracking().ToListAsync(ct);

        var dashboardDto = new DashboardOrdersDto()
        {
            OrdersInDraft = orderReadModelList.Where(o => o.Status == 0).Count(),
            OrdersSubmitted = orderReadModelList.Where(o => o.Status == 1).Count(),
            OrdersPaid = orderReadModelList.Where(o => o.Status == 2).Count(),
            OrdersCancelled = orderReadModelList.Where(o => o.Status == 3).Count(),

        };

        return Results.Ok(dashboardDto);
    }
}