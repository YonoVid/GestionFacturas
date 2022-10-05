using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIGestionFacturas.Migrations
{
    public partial class Cambioderelacionesdelmodelo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceLines_Invoices_InvoiceId",
                table: "InvoiceLines");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Enterprises_EnterpriseId",
                table: "Invoices");

            migrationBuilder.RenameColumn(
                name: "quantity",
                table: "InvoiceLines",
                newName: "Quantity");

            migrationBuilder.AlterColumn<int>(
                name: "EnterpriseId",
                table: "Invoices",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TaxPercentage",
                table: "Invoices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "InvoiceId",
                table: "InvoiceLines",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceLines_Invoices_InvoiceId",
                table: "InvoiceLines",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Enterprises_EnterpriseId",
                table: "Invoices",
                column: "EnterpriseId",
                principalTable: "Enterprises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceLines_Invoices_InvoiceId",
                table: "InvoiceLines");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Enterprises_EnterpriseId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "TaxPercentage",
                table: "Invoices");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "InvoiceLines",
                newName: "quantity");

            migrationBuilder.AlterColumn<int>(
                name: "EnterpriseId",
                table: "Invoices",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "InvoiceId",
                table: "InvoiceLines",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceLines_Invoices_InvoiceId",
                table: "InvoiceLines",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Enterprises_EnterpriseId",
                table: "Invoices",
                column: "EnterpriseId",
                principalTable: "Enterprises",
                principalColumn: "Id");
        }
    }
}
