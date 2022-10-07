using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIGestionFacturas.Migrations
{
    public partial class Actualizacióndefacturayestructuradelineasdefactura : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "InvoiceLines");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "InvoiceLines");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "InvoiceLines");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "InvoiceLines");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "InvoiceLines");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "InvoiceLines");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "InvoiceLines");

            migrationBuilder.AlterColumn<float>(
                name: "TaxPercentage",
                table: "Invoices",
                type: "real",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<float>(
                name: "TotalAmount",
                table: "Invoices",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "Invoices");

            migrationBuilder.AlterColumn<int>(
                name: "TaxPercentage",
                table: "Invoices",
                type: "int",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "InvoiceLines",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "InvoiceLines",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "InvoiceLines",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "InvoiceLines",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "InvoiceLines",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "InvoiceLines",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "InvoiceLines",
                type: "datetime2",
                nullable: true);
        }
    }
}
