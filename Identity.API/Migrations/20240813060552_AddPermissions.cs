using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Identity.API.Migrations
{
    /// <inheritdoc />
    public partial class AddPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RolePermission_Permission_PermissionId",
                schema: "dbo",
                table: "RolePermission");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RolePermission",
                schema: "dbo",
                table: "RolePermission");

            migrationBuilder.DropIndex(
                name: "IX_RolePermission_RoleId",
                schema: "dbo",
                table: "RolePermission");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Permission",
                schema: "dbo",
                table: "Permission");

            migrationBuilder.RenameTable(
                name: "Permission",
                schema: "dbo",
                newName: "permissions",
                newSchema: "dbo");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RolePermission",
                schema: "dbo",
                table: "RolePermission",
                columns: new[] { "RoleId", "PermissionId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_permissions",
                schema: "dbo",
                table: "permissions",
                column: "Id");

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "permissions",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "ReadMember" },
                    { 2, "UpdateMember" }
                });

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "RolePermission",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_RolePermission_PermissionId",
                schema: "dbo",
                table: "RolePermission",
                column: "PermissionId");

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermission_permissions_PermissionId",
                schema: "dbo",
                table: "RolePermission",
                column: "PermissionId",
                principalSchema: "dbo",
                principalTable: "permissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RolePermission_permissions_PermissionId",
                schema: "dbo",
                table: "RolePermission");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RolePermission",
                schema: "dbo",
                table: "RolePermission");

            migrationBuilder.DropIndex(
                name: "IX_RolePermission_PermissionId",
                schema: "dbo",
                table: "RolePermission");

            migrationBuilder.DropPrimaryKey(
                name: "PK_permissions",
                schema: "dbo",
                table: "permissions");

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "RolePermission",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "RolePermission",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 2, 1 });

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "permissions",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "permissions",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.RenameTable(
                name: "permissions",
                schema: "dbo",
                newName: "Permission",
                newSchema: "dbo");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RolePermission",
                schema: "dbo",
                table: "RolePermission",
                columns: new[] { "PermissionId", "RoleId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Permission",
                schema: "dbo",
                table: "Permission",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermission_RoleId",
                schema: "dbo",
                table: "RolePermission",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermission_Permission_PermissionId",
                schema: "dbo",
                table: "RolePermission",
                column: "PermissionId",
                principalSchema: "dbo",
                principalTable: "Permission",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
