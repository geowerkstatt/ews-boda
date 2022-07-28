using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EWS.Migrations;

public partial class AddUserTable : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "user",
            schema: "bohrung",
            columns: table => new
            {
                user_id = table.Column<int>(type: "SERIAL", nullable: false),
                user_name = table.Column<string>(maxLength: 50, nullable: false),
                user_role = table.Column<int>(nullable: false),
                new_date = table.Column<DateTime>(type: "timestamp without time zone", defaultValueSql: "now()", nullable: false),
                mut_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                new_usr = table.Column<string>(type: "character varying", defaultValueSql: "current_user", nullable: false),
                mut_usr = table.Column<string>(type: "character varying", nullable: true),
            },
            constraints: table =>
            {
                table.PrimaryKey("pkey_user_user_id", x => x.user_id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder) =>
        migrationBuilder.DropTable(name: "user");
}
