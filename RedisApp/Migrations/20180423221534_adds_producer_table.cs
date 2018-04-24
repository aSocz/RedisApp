using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace RedisApp.Migrations
{
    public partial class adds_producer_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Producer",
                schema: "RedisApp",
                table: "Products");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "RedisApp",
                table: "Products",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProducerId",
                schema: "RedisApp",
                table: "Products",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Producers",
                schema: "RedisApp",
                columns: table => new
                {
                    ProducerId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Producers", x => x.ProducerId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProducerId",
                schema: "RedisApp",
                table: "Products",
                column: "ProducerId");

            migrationBuilder.CreateIndex(
                name: "IX_Producers_Name",
                schema: "RedisApp",
                table: "Producers",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Producers_ProducerId",
                schema: "RedisApp",
                table: "Products",
                column: "ProducerId",
                principalSchema: "RedisApp",
                principalTable: "Producers",
                principalColumn: "ProducerId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Producers_ProducerId",
                schema: "RedisApp",
                table: "Products");

            migrationBuilder.DropTable(
                name: "Producers",
                schema: "RedisApp");

            migrationBuilder.DropIndex(
                name: "IX_Products_ProducerId",
                schema: "RedisApp",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ProducerId",
                schema: "RedisApp",
                table: "Products");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "RedisApp",
                table: "Products",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 255);

            migrationBuilder.AddColumn<string>(
                name: "Producer",
                schema: "RedisApp",
                table: "Products",
                nullable: true);
        }
    }
}
