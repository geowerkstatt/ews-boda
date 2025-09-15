using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EWS.Migrations
{
    /// <inheritdoc />
    public partial class NormalizeQualitaetBemerkungLinebreaks : Migration
    {
        /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"CREATE OR REPLACE VIEW bohrung.""data_export"" AS
            SELECT
                standort.standort_id AS ""standort.standort_id"",
                standort.bezeichnung AS ""standort.bezeichnung"",
                regexp_replace(standort.bemerkung, '[\r\n]+', ' ', 'g') AS ""standort.bemerkung"",
                COALESCE(anzbohrungen.anzahl, 0) AS ""standort.anzbohrloch"",
                standort.gbnummer AS ""standort.gbnummer"",
                standort.freigabe_afu AS ""standort.freigabe_afu"",
                standort.afu_usr AS ""standort.afu_usr"",
                standort.afu_date AS ""standort.afu_date"",
                bohrung.bohrung_id AS ""bohrung.bohrung_id"",
                bohrung.bezeichnung AS ""bohrung.bezeichnung"",
                regexp_replace(bohrung.bemerkung, '[\r\n]+', ' ', 'g') AS ""bohrung.bemerkung"",
                bohrung.datum AS ""bohrung.datum"",
                bohrung.durchmesserbohrloch AS ""bohrung.durchmesserbohrloch"",
                bohrung.ablenkung AS ""bohrung.ablenkung"",
                bohrung.quali AS ""bohrung.quali"",
                bohrung_quali.text AS ""bohrung.quali_text"",
                bohrung_quali.kurztext AS ""bohrung.quali_kurztext"",
                regexp_replace(bohrung.qualibem, '[\r\n]+', ' ', 'g') AS ""bohrung.qualibem"",
                bohrung.quelleref AS ""bohrung.quelleref"",
                bohrung.h_quali AS ""bohrung.h_quali"",
                (CASE WHEN bohrung.h_quali IS NULL THEN '' ELSE 'Interpretationsqualitäten' END) AS ""bohrung.h_quali_text"",
                (CASE WHEN bohrung.h_quali IS NULL THEN '' ELSE 'quali' END) AS ""bohrung.h_quali_kurztext"",
                ST_X(bohrung.wkb_geometry) AS ""bohrung.X"",
                ST_Y(bohrung.wkb_geometry) AS ""bohrung.Y"",
                bohrprofil.bohrprofil_id AS ""bohrprofil.bohrprofil_id"",
                bohrprofil.datum AS ""bohrprofil.datum"",
                regexp_replace(bohrprofil.bemerkung, '[\r\n]+', ' ', 'g') AS ""bohrprofil.bemerkung"",
                bohrprofil.kote AS ""bohrprofil.kote"",
                bohrprofil.endteufe AS ""bohrprofil.endtiefe"",
                bohrprofil.tektonik AS ""bohrprofil.tektonik"",
                bohrprofil_tektonik.text AS ""bohrprofil.tektonik_text"",
                bohrprofil_tektonik.kurztext AS ""bohrprofil.tektonik_kurztext"",
                bohrprofil.fmfelso AS ""bohrprofil.fmfelso"",
                bohrprofil_fmfelso.text AS ""bohrprofil.fmfelso_text"",
                bohrprofil_fmfelso.kurztext AS ""bohrprofil.fmfelso_kurztext"",
                bohrprofil.fmeto AS ""bohrprofil.fmeto"",
                bohrprofil_fmeto.text AS ""bohrprofil.fmeto_text"",
                bohrprofil_fmeto.kurztext AS ""bohrprofil.fmeto_kurztext"",
                bohrprofil.quali AS ""bohrprofil.quali"",
                bohrprofil_quali.text AS ""bohrprofil.quali_text"",
                bohrprofil_quali.kurztext AS ""bohrprofil.quali_kurztext"",
                regexp_replace(bohrprofil.qualibem, '[\r\n]+', ' ', 'g') AS ""bohrprofil.qualibem"",
                bohrprofil.h_quali AS ""bohrprofil.h_quali"",
                (CASE WHEN bohrprofil.h_quali IS NULL THEN '' ELSE 'Qualität des Bohrprofils' END) AS ""bohrprofil.h_quali_text"",
                (CASE WHEN bohrprofil.h_quali IS NULL THEN '' ELSE 'quali_bohrprofil' END) AS ""bohrprofil.h_quali_kurztext"",
                bohrprofil.h_tektonik AS ""bohrprofil.h_tektonik"",
                (CASE WHEN bohrprofil.h_tektonik IS NULL THEN '' ELSE 'Tektonik' END) AS ""bohrprofil.h_tektonik_text"",
                (CASE WHEN bohrprofil.h_tektonik IS NULL THEN '' ELSE 'tektonik' END) AS ""bohrprofil.h_tektonik_kurztext"",
                bohrprofil.h_fmeto AS ""bohrprofil.h_fmeto"",
                '' AS ""bohrprofil.h_fmeto_text"",
                (CASE WHEN bohrprofil.h_fmeto IS NULL THEN '' ELSE 'fmfelso_fmeto' END) AS ""bohrprofil.h_fmeto_kurztext"",
                bohrprofil.h_fmfelso AS ""bohrprofil.h_fmfelso"",
                '' AS ""bohrprofil.h_fmfelso_text"",
                (CASE WHEN bohrprofil.h_fmfelso IS NULL THEN '' ELSE 'fmfelso_fmeto' END) AS ""bohrprofil.h_fmfelso_kurztext"",
                schicht.schicht_id AS ""schicht.schicht_id"",
                schicht.tiefe AS ""schicht.tiefe"",
                schicht.quali AS ""schicht.quali"",
                schicht_quali.text AS ""schicht.quali_text"",
                schicht_quali.kurztext AS ""schicht.quali_kurztext"",
                regexp_replace(schicht.qualibem, '[\r\n]+', ' ', 'g') AS ""schicht.qualibem"",
                regexp_replace(schicht.bemerkung, '[\r\n]+', ' ', 'g') AS ""schicht.bemerkung"",
                schicht.h_quali AS ""schicht.h_quali"",
                (CASE WHEN schicht.h_quali IS NULL THEN '' ELSE 'Schichtvorzeichen' END) AS ""schicht.h_quali_text"",
                (CASE WHEN schicht.h_quali IS NULL THEN '' ELSE 'Schichtvorzeichen' END) AS ""schicht.h_quali_kurztext"",
                codeschicht.kurztext AS ""codeschicht.kurztext"",
                codeschicht.text AS ""codeschicht.text"",
                codeschicht.sort AS ""codeschicht.sort"",
                vorkommnis.vorkommnis_id AS ""vorkommnis.vorkommnis_id"",
                vorkommnis.typ AS ""vorkommnis.typ"",
                vorkommnis_typ.text AS ""vorkommnis.typ_text"",
                vorkommnis_typ.kurztext AS ""vorkommnis.typ_kurztext"",
                vorkommnis.tiefe AS ""vorkommnis.tiefe"",
                regexp_replace(vorkommnis.bemerkung, '[\r\n]+', ' ', 'g') AS ""vorkommnis.bemerkung"",
                vorkommnis.quali AS ""vorkommnis.quali"",
                vorkommnis_quali.text AS ""vorkommnis.quali_text"",
                vorkommnis_quali.kurztext AS ""vorkommnis.quali_kurztext"",
                regexp_replace(vorkommnis.qualibem, '[\r\n]+', ' ', 'g') AS ""vorkommnis.qualibem"",
                vorkommnis.h_quali AS ""vorkommnis.h_quali"",
                (CASE WHEN vorkommnis.h_quali IS NULL THEN '' ELSE 'Interpretationsqualitäten' END) AS ""vorkommnis.h_quali_text"",
                (CASE WHEN vorkommnis.h_quali IS NULL THEN '' ELSE 'quali' END) AS ""vorkommnis.h_quali_kurztext"",
                vorkommnis.h_typ AS ""vorkommnis.h_typ"",
                (CASE WHEN vorkommnis.h_typ IS NULL THEN '' ELSE 'Typ von Bohrvorkommnisse' END) AS ""vorkommnis.h_typ_text"",
                (CASE WHEN vorkommnis.h_typ IS NULL THEN '' ELSE 'typ' END) AS ""vorkommnis.h_typ_kurztext""
            FROM bohrung.standort standort
            LEFT OUTER JOIN (SELECT standort_id, COUNT(*) AS anzahl FROM bohrung.bohrung GROUP BY standort_id) anzbohrungen ON anzbohrungen.standort_id = standort.standort_id
            LEFT OUTER JOIN bohrung.bohrung bohrung ON bohrung.standort_id = standort.standort_id
            LEFT OUTER JOIN bohrung.code bohrung_quali ON bohrung_quali.code_id = bohrung.quali
            LEFT OUTER JOIN bohrung.bohrprofil bohrprofil ON bohrprofil.bohrung_id = bohrung.bohrung_id
            LEFT OUTER JOIN bohrung.code bohrprofil_tektonik ON bohrprofil_tektonik.code_id = bohrprofil.tektonik
            LEFT OUTER JOIN bohrung.code bohrprofil_fmfelso ON bohrprofil_fmfelso.code_id = bohrprofil.fmfelso
            LEFT OUTER JOIN bohrung.code bohrprofil_fmeto ON bohrprofil_fmeto.code_id = bohrprofil.fmeto
            LEFT OUTER JOIN bohrung.code bohrprofil_quali ON bohrprofil_quali.code_id = bohrprofil.quali
            LEFT OUTER JOIN bohrung.schicht schicht ON schicht.bohrprofil_id = bohrprofil.bohrprofil_id
            LEFT OUTER JOIN bohrung.code schicht_quali ON schicht_quali.code_id = schicht.quali
            LEFT OUTER JOIN bohrung.codeschicht codeschicht ON codeschicht.codeschicht_id = schicht.schichten_id
            LEFT OUTER JOIN bohrung.vorkommnis vorkommnis ON vorkommnis.bohrprofil_id = bohrprofil.bohrprofil_id
            LEFT OUTER JOIN bohrung.code vorkommnis_typ ON vorkommnis_typ.code_id = vorkommnis.typ
            LEFT OUTER JOIN bohrung.code vorkommnis_quali ON vorkommnis_quali.code_id = vorkommnis.quali
            ORDER BY standort.standort_id;");
    }
}
