using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WNC.G06.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tblPermissions",
                columns: table => new
                {
                    PermissionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblPermissions", x => x.PermissionID);
                });

            migrationBuilder.CreateTable(
                name: "tblUsers",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "varchar(max)", nullable: false),
                    Email = table.Column<string>(type: "varchar(max)", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    PermissionID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblUsers", x => x.UserID);
                    table.ForeignKey(
                        name: "FK_tblUsers_tblPermissions_PermissionID",
                        column: x => x.PermissionID,
                        principalTable: "tblPermissions",
                        principalColumn: "PermissionID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblCafes",
                columns: table => new
                {
                    CafeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CafeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "varchar(10)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblCafes", x => x.CafeID);
                    table.ForeignKey(
                        name: "FK_tblCafes_tblUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "tblUsers",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblPayments",
                columns: table => new
                {
                    PaymentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<float>(type: "real", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblPayments", x => x.PaymentID);
                    table.ForeignKey(
                        name: "FK_tblPayments_tblUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "tblUsers",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblOrders",
                columns: table => new
                {
                    OrderID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TotalAmount = table.Column<float>(type: "real", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    CafeID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblOrders", x => x.OrderID);
                    table.ForeignKey(
                        name: "FK_tblOrders_tblCafes_CafeID",
                        column: x => x.CafeID,
                        principalTable: "tblCafes",
                        principalColumn: "CafeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tblOrders_tblUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "tblUsers",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblProducts",
                columns: table => new
                {
                    ProductID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<float>(type: "real", nullable: false),
                    imgUrl = table.Column<string>(type: "varchar(max)", nullable: false),
                    Description = table.Column<string>(type: "varchar(max)", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    CafeID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblProducts", x => x.ProductID);
                    table.ForeignKey(
                        name: "FK_tblProducts_tblCafes_CafeID",
                        column: x => x.CafeID,
                        principalTable: "tblCafes",
                        principalColumn: "CafeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblOrderDetails",
                columns: table => new
                {
                    OrderDetailID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<float>(type: "real", nullable: false),
                    OrderID = table.Column<int>(type: "int", nullable: false),
                    ProductID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblOrderDetails", x => x.OrderDetailID);
                    table.ForeignKey(
                        name: "FK_tblOrderDetails_tblOrders_OrderID",
                        column: x => x.OrderID,
                        principalTable: "tblOrders",
                        principalColumn: "OrderID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tblOrderDetails_tblProducts_ProductID",
                        column: x => x.ProductID,
                        principalTable: "tblProducts",
                        principalColumn: "ProductID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tblCafes_UserID",
                table: "tblCafes",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_tblOrderDetails_OrderID",
                table: "tblOrderDetails",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_tblOrderDetails_ProductID",
                table: "tblOrderDetails",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_tblOrders_CafeID",
                table: "tblOrders",
                column: "CafeID");

            migrationBuilder.CreateIndex(
                name: "IX_tblOrders_UserID",
                table: "tblOrders",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_tblPayments_UserID",
                table: "tblPayments",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_tblProducts_CafeID",
                table: "tblProducts",
                column: "CafeID");

            migrationBuilder.CreateIndex(
                name: "IX_tblUsers_PermissionID",
                table: "tblUsers",
                column: "PermissionID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tblOrderDetails");

            migrationBuilder.DropTable(
                name: "tblPayments");

            migrationBuilder.DropTable(
                name: "tblOrders");

            migrationBuilder.DropTable(
                name: "tblProducts");

            migrationBuilder.DropTable(
                name: "tblCafes");

            migrationBuilder.DropTable(
                name: "tblUsers");

            migrationBuilder.DropTable(
                name: "tblPermissions");
        }
    }
}
