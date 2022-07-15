using Microsoft.EntityFrameworkCore.Migrations;
namespace EWS.Migrations
{
    public partial class ChangeGrundbuchnummerToText : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP VIEW IF EXISTS bohrung.""GIS_standort"";");
            migrationBuilder.Sql(@"DROP VIEW IF EXISTS bohrung.""data_export"";");
            migrationBuilder.Sql("ALTER TABLE bohrung.standort ALTER COLUMN gbnummer TYPE text;");
            migrationBuilder.Sql(@"CREATE VIEW bohrung.""GIS_standort"" AS SELECT standort_id, bezeichnung, bemerkung, gemeinde, gbnummer, new_date, mut_date, new_usr, mut_usr, freigabe_afu, afu_usr, afu_date FROM bohrung.standort;");
            migrationBuilder.Sql(@"CREATE VIEW bohrung.""data_export"" AS
            SELECT
                standort.standort_id AS ""standort.standort_id"", standort.bezeichnung AS ""standort.bezeichnung"", standort.bemerkung AS ""standort.bemerkung"", COALESCE(anzbohrungen.anzahl, 0) AS ""standort.anzbohrloch"", standort.gbnummer AS ""standort.gbnummer"", standort.freigabe_afu AS ""standort.freigabe_afu"", standort.afu_usr AS ""standort.afu_usr"", standort.afu_date AS ""standort.afu_date"",
                bohrung.bohrung_id AS ""bohrung.bohrung_id"", bohrung.bezeichnung AS ""bohrung.bezeichnung"", bohrung.bemerkung AS ""bohrung.bemerkung"", bohrung.datum AS ""bohrung.datum"", bohrung.durchmesserbohrloch AS ""bohrung.durchmesserbohrloch"", bohrung.ablenkung AS ""bohrung.ablenkung"", bohrung.quali AS ""bohrung.quali"", bohrung.qualibem AS ""bohrung.qualibem"", bohrung.quelleref AS ""bohrung.quelleref"", bohrung.h_quali AS ""bohrung.h_quali"", ST_X(bohrung.wkb_geometry) AS ""bohrung.X"", ST_Y(bohrung.wkb_geometry) AS ""bohrung.Y"",
                bohrprofil.bohrprofil_id AS ""bohrprofil.bohrprofil_id"", bohrprofil.datum AS ""bohrprofil.datum"", bohrprofil.bemerkung AS ""bohrprofil.bemerkung"", bohrprofil.kote AS ""bohrprofil.kote"", bohrprofil.endteufe AS ""bohrprofil.endtiefe"", bohrprofil.tektonik AS ""bohrprofil.tektonik"", bohrprofil.fmfelso AS ""bohrprofil.fmfelso"", bohrprofil.fmeto AS ""bohrprofil.fmeto"", bohrprofil.quali AS ""bohrprofil.quali"", bohrprofil.qualibem AS ""bohrprofil.qualibem"", bohrprofil.h_quali AS ""bohrprofil.h_quali"", bohrprofil.h_tektonik AS ""bohrprofil.h_tektonik"", bohrprofil.h_fmeto AS ""bohrprofil.h_fmeto"", bohrprofil.h_fmfelso AS ""bohrprofil.h_fmfelso"",
                schicht.schicht_id AS ""schicht.schicht_id"", schicht.tiefe AS ""schicht.tiefe"", schicht.quali AS ""schicht.quali"", schicht.qualibem AS ""schicht.qualibem"", schicht.bemerkung AS ""schicht.bemerkung"", schicht.h_quali AS ""schicht.h_quali"",
                codeschicht.kurztext AS ""codeschicht.kurztext"", codeschicht.text AS ""codeschicht.text"", codeschicht.sort AS ""codeschicht.sort"",
                vorkommnis.vorkommnis_id AS ""vorkommnis.vorkommnis_id"", vorkommnis.typ AS ""vorkommnis.typ"", vorkommnis.tiefe AS ""vorkommnis.tiefe"", vorkommnis.bemerkung AS ""vorkommnis.bemerkung"", vorkommnis.quali AS ""vorkommnis.quali"", vorkommnis.qualibem AS ""vorkommnis.qualibem"", vorkommnis.h_quali AS ""vorkommnis.h_quali"", vorkommnis.h_typ AS ""vorkommnis.h_typ""
            FROM bohrung.standort standort
            LEFT OUTER JOIN(SELECT standort_id, COUNT(*) AS anzahl FROM bohrung.bohrung GROUP BY standort_id) anzbohrungen ON anzbohrungen.standort_id = standort.standort_id
            LEFT OUTER JOIN bohrung.bohrung bohrung ON bohrung.standort_id = standort.standort_id
            LEFT OUTER JOIN bohrung.bohrprofil bohrprofil ON bohrprofil.bohrung_id = bohrung.bohrung_id
            LEFT OUTER JOIN bohrung.schicht schicht ON schicht.bohrprofil_id = bohrprofil.bohrprofil_id
            LEFT OUTER JOIN bohrung.codeschicht codeschicht ON codeschicht.codeschicht_id = schicht.schichten_id
            LEFT OUTER JOIN bohrung.vorkommnis vorkommnis ON vorkommnis.bohrprofil_id = bohrprofil.bohrprofil_id
            ORDER BY standort.standort_id;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
