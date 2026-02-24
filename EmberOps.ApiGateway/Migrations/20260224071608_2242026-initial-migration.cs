using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmberOps.ApiGateway.Migrations
{
    /// <inheritdoc />
    public partial class _2242026initialmigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "bff");

            migrationBuilder.EnsureSchema(
                name: "orders");

            migrationBuilder.CreateTable(
                name: "InboxMessages",
                schema: "bff",
                columns: table => new
                {
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProcessedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Consumer = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InboxMessages", x => x.EventId);
                });

            migrationBuilder.CreateTable(
                name: "OrdersReadModel",
                schema: "orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubmittedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PaidAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastEventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdersReadModel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderItemsReadModel",
                schema: "orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Sku = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NameSnapshot = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    UnitPriceSnapshot = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItemsReadModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItemsReadModel_OrdersReadModel_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "orders",
                        principalTable: "OrdersReadModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemsReadModel_OrderId",
                schema: "orders",
                table: "OrderItemsReadModel",
                column: "OrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InboxMessages",
                schema: "bff");

            migrationBuilder.DropTable(
                name: "OrderItemsReadModel",
                schema: "orders");

            migrationBuilder.DropTable(
                name: "OrdersReadModel",
                schema: "orders");
        }
    }
}
