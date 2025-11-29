using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyKhoHang.Migrations
{
    /// <inheritdoc />
    public partial class AllowNullSupplier : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseTransactions_Supplier_SupplierId",
                table: "WarehouseTransactions");

            migrationBuilder.AlterColumn<int>(
                name: "SupplierId",
                table: "WarehouseTransactions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseTransactions_Supplier_SupplierId",
                table: "WarehouseTransactions",
                column: "SupplierId",
                principalTable: "Supplier",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseTransactions_Supplier_SupplierId",
                table: "WarehouseTransactions");

            migrationBuilder.AlterColumn<int>(
                name: "SupplierId",
                table: "WarehouseTransactions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseTransactions_Supplier_SupplierId",
                table: "WarehouseTransactions",
                column: "SupplierId",
                principalTable: "Supplier",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
