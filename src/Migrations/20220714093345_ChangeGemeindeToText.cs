using Microsoft.EntityFrameworkCore.Migrations;

namespace EWS.Migrations;

public partial class ChangeGemeindeToText : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"DROP VIEW IF EXISTS bohrung.""GIS_standort"";");
        migrationBuilder.Sql("ALTER TABLE bohrung.standort ALTER COLUMN gemeinde TYPE text;");
        migrationBuilder.Sql(@"CREATE VIEW bohrung.""GIS_standort"" AS SELECT standort_id, bezeichnung, bemerkung, gemeinde, gbnummer, new_date, mut_date, new_usr, mut_usr, freigabe_afu, afu_usr, afu_date FROM bohrung.standort;");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
    }
}
