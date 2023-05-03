using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HajurKoCarRental.Migrations
{
    /// <inheritdoc />
    public partial class addingtabales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "PricePerDay",
                table: "CarInfo",
                newName: "RentPrice");

            migrationBuilder.RenameColumn(
                name: "Model",
                table: "CarInfo",
                newName: "CarModel");

            migrationBuilder.RenameColumn(
                name: "IsAvailable",
                table: "CarInfo",
                newName: "is_available");

            migrationBuilder.RenameColumn(
                name: "Brand",
                table: "CarInfo",
                newName: "CarDescription");

            migrationBuilder.RenameColumn(
                name: "IsRegularCustomer",
                table: "AspNetUsers",
                newName: "is_RegularCustomer");

            migrationBuilder.RenameColumn(
                name: "CitizenPaperFileName",
                table: "AspNetUsers",
                newName: "CitizenshipFileName");

            migrationBuilder.RenameColumn(
                name: "CitizenPaper",
                table: "AspNetUsers",
                newName: "Citizenship");

            migrationBuilder.AddColumn<string>(
                name: "CarBrand",
                table: "CarInfo",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CarNumber",
                table: "CarInfo",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "AspNetUsers",
                type: "nvarchar(100)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CarBrand",
                table: "CarInfo");

            migrationBuilder.DropColumn(
                name: "CarNumber",
                table: "CarInfo");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "is_available",
                table: "CarInfo",
                newName: "IsAvailable");

            migrationBuilder.RenameColumn(
                name: "RentPrice",
                table: "CarInfo",
                newName: "PricePerDay");

            migrationBuilder.RenameColumn(
                name: "CarModel",
                table: "CarInfo",
                newName: "Model");

            migrationBuilder.RenameColumn(
                name: "CarDescription",
                table: "CarInfo",
                newName: "Brand");

            migrationBuilder.RenameColumn(
                name: "is_RegularCustomer",
                table: "AspNetUsers",
                newName: "IsRegularCustomer");

            migrationBuilder.RenameColumn(
                name: "CitizenshipFileName",
                table: "AspNetUsers",
                newName: "CitizenPaperFileName");

            migrationBuilder.RenameColumn(
                name: "Citizenship",
                table: "AspNetUsers",
                newName: "CitizenPaper");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "AspNetUsers",
                type: "nvarchar(100)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "nvarchar(100)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "nvarchar(100)",
                nullable: false,
                defaultValue: "");
        }
    }
}
