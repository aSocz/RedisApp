using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace RedisApp.Migrations
{
    public partial class adds_unique_constraints_and_indices : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItem_Orders_OrderId",
                schema: "RedisApp",
                table: "OrderItem");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItem_Products_ProductId",
                schema: "RedisApp",
                table: "OrderItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderItem",
                schema: "RedisApp",
                table: "OrderItem");

            migrationBuilder.DropIndex(
                name: "IX_OrderItem_ProductId",
                schema: "RedisApp",
                table: "OrderItem");

            migrationBuilder.RenameTable(
                name: "OrderItem",
                schema: "RedisApp",
                newName: "OrderItems");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItem_OrderId",
                schema: "RedisApp",
                table: "OrderItems",
                newName: "IX_OrderItems_OrderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderItems",
                schema: "RedisApp",
                table: "OrderItems",
                column: "OrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Name_CategoryId",
                schema: "RedisApp",
                table: "Products",
                columns: new[] { "Name", "CategoryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ClientName",
                schema: "RedisApp",
                table: "Orders",
                column: "ClientName");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Date",
                schema: "RedisApp",
                table: "Orders",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductId_OrderId",
                schema: "RedisApp",
                table: "OrderItems",
                columns: new[] { "ProductId", "OrderId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                schema: "RedisApp",
                table: "Categories",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Orders_OrderId",
                schema: "RedisApp",
                table: "OrderItems",
                column: "OrderId",
                principalSchema: "RedisApp",
                principalTable: "Orders",
                principalColumn: "OrderId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Products_ProductId",
                schema: "RedisApp",
                table: "OrderItems",
                column: "ProductId",
                principalSchema: "RedisApp",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Orders_OrderId",
                schema: "RedisApp",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Products_ProductId",
                schema: "RedisApp",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_Products_Name_CategoryId",
                schema: "RedisApp",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Orders_ClientName",
                schema: "RedisApp",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_Date",
                schema: "RedisApp",
                table: "Orders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderItems",
                schema: "RedisApp",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_ProductId_OrderId",
                schema: "RedisApp",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_Categories_Name",
                schema: "RedisApp",
                table: "Categories");

            migrationBuilder.RenameTable(
                name: "OrderItems",
                schema: "RedisApp",
                newName: "OrderItem");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItems_OrderId",
                schema: "RedisApp",
                table: "OrderItem",
                newName: "IX_OrderItem_OrderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderItem",
                schema: "RedisApp",
                table: "OrderItem",
                column: "OrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_ProductId",
                schema: "RedisApp",
                table: "OrderItem",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItem_Orders_OrderId",
                schema: "RedisApp",
                table: "OrderItem",
                column: "OrderId",
                principalSchema: "RedisApp",
                principalTable: "Orders",
                principalColumn: "OrderId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItem_Products_ProductId",
                schema: "RedisApp",
                table: "OrderItem",
                column: "ProductId",
                principalSchema: "RedisApp",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
