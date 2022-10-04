using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIGestionFacturas.Migrations
{
    public partial class Cambioenrelacionesdemodelos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddColumn<int>(
                name: "TaxPercentage",
                table: "Invoices",
                type: "int",
                nullable: false,
                defaultValue: 0);

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
                name: "FK_Invoices_Enterprises_EnterpriseId",
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

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Enterprises_EnterpriseId",
                table: "Invoices",
                column: "EnterpriseId",
                principalTable: "Enterprises",
                principalColumn: "Id");
        }
    }
}
