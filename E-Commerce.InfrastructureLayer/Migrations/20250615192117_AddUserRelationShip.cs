using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Commerce.InfrastructureLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddUserRelationShip : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_cartItems",
                table: "cartItems");

            migrationBuilder.DropColumn(
                name: "Brand",
                table: "cartItems");

            migrationBuilder.DropColumn(
                name: "PictureUrl",
                table: "cartItems");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "cartItems");

            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "cartItems");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "cartItems");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "shoppingCarts",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "cartItems",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_cartItems",
                table: "cartItems",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_shoppingCarts_UserId",
                table: "shoppingCarts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_cartItems_ProductId",
                table: "cartItems",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_cartItems_products_ProductId",
                table: "cartItems",
                column: "ProductId",
                principalTable: "products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_shoppingCarts_AspNetUsers_UserId",
                table: "shoppingCarts",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_cartItems_products_ProductId",
                table: "cartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_shoppingCarts_AspNetUsers_UserId",
                table: "shoppingCarts");

            migrationBuilder.DropIndex(
                name: "IX_shoppingCarts_UserId",
                table: "shoppingCarts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_cartItems",
                table: "cartItems");

            migrationBuilder.DropIndex(
                name: "IX_cartItems_ProductId",
                table: "cartItems");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "shoppingCarts");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "cartItems");

            migrationBuilder.AddColumn<string>(
                name: "Brand",
                table: "cartItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PictureUrl",
                table: "cartItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "cartItems",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "cartItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "cartItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_cartItems",
                table: "cartItems",
                columns: new[] { "ProductId", "ShoppingCartId" });
        }
    }
}
