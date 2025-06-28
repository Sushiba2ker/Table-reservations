using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TableReservations.Migrations
{
    /// <inheritdoc />
    public partial class Khoi_Tao_Bang : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Name" },
                values: new object[] { "Sách Động Vật" });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Name" },
                values: new object[] { "Sách Sinh Vật" });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Name" },
                values: new object[] { "Sinh Thực Vật" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
