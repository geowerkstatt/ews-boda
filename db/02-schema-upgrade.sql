-- Upgrade the database schema of ews-boda:
-- * Remove obsolete tables/columns
-- * Rename and simplify some objects

-- Remove obsolete table "vegasBohrungen"
DROP VIEW  IF EXISTS bohrung.bohrung_novegas;
DROP TABLE IF EXISTS bohrung."vegasBohrungen";

-- Drop all obsolete mutation triggers and functions
DROP FUNCTION IF EXISTS bohrung._mutation() CASCADE;

-- Upgrade table "standort"
ALTER TABLE bohrung.standort
    DROP COLUMN IF EXISTS anzbohrloch,
    DROP COLUMN IF EXISTS gaso_nr,
    DROP COLUMN IF EXISTS quali,
    DROP COLUMN IF EXISTS qualibem,
    DROP COLUMN IF EXISTS h_quali,
    DROP COLUMN IF EXISTS wkb_geometry,
    DROP COLUMN IF EXISTS wkb_geometry95,

    ADD COLUMN IF NOT EXISTS freigabe_afu BOOL NOT NULL DEFAULT true,
    ADD COLUMN IF NOT EXISTS afu_usr varchar,
    ADD COLUMN IF NOT EXISTS afu_date timestamp;

ALTER TABLE bohrung.standort RENAME COLUMN gembfs TO gemeinde;

-- Upgrade table "bohrung" and associated views
DROP VIEW IF EXISTS bohrung."141016_Stammdaten_Lieferung_ab2014";
CREATE VIEW bohrung."141016_Stammdaten_Lieferung_ab2014" AS
SELECT a.standort_id, a.bezeichnung, a.bemerkung AS bohrungbemerkung, b.bohrprofil_id, b.bohrung_id, b.datum, b.bemerkung, b.kote, b.endteufe, b.tektonik, b.fmfelso, b.fmeto, b.quali, b.qualibem, b.wkb_geometry95, b.new_date, b.mut_date, b.new_usr, b.mut_usr, b.h_quali, b.h_tektonik, b.h_fmeto, b.h_fmfelso FROM bohrung.bohrung a, bohrung.bohrprofil b WHERE a.bohrung_id = b.bohrung_id AND b.new_date > '2014-01-01 00:00:00'::timestamp;

DROP VIEW IF EXISTS bohrung."Alle unarchivierten Bohrprofile mit Bohrnamen";
CREATE VIEW bohrung."Alle unarchivierten Bohrprofile mit Bohrnamen" AS
SELECT a.bohrung_id, a.standort_id, a.bezeichnung, a.bemerkung, b.quali, b.qualibem, b.kote, b.endteufe, b.fmfelso, b.fmeto FROM bohrung.bohrung a, bohrung.bohrprofil b WHERE a.bohrung_id = b.bohrung_id;

DROP VIEW IF EXISTS bohrung."aktuelleBohrprofile";
CREATE VIEW bohrung."aktuelleBohrprofile" AS
SELECT a.bohrung_id, a.standort_id, a.bezeichnung, a.bemerkung, b.quali, b.qualibem, b.kote, b.endteufe, b.fmfelso, b.fmeto FROM bohrung.bohrung a, bohrung.bohrprofil b WHERE a.bohrung_id = b.bohrung_id;

DROP VIEW IF EXISTS bohrung."aktuelleBohrprofile_neu";
CREATE VIEW bohrung."aktuelleBohrprofile_neu" AS
SELECT a.bohrung_id, b.bohrprofil_id, a.standort_id, a.bezeichnung, a.bemerkung, b.quali, b.qualibem, b.kote, b.endteufe, b.fmfelso, b.fmeto FROM bohrung.bohrung a, bohrung.bohrprofil b WHERE a.bohrung_id = b.bohrung_id;

DROP VIEW IF EXISTS bohrung."aktuelleBohrprofile_neu_neu";
CREATE VIEW bohrung."aktuelleBohrprofile_neu_neu" AS
SELECT a.standort_id, a.bezeichnung, a.bemerkung AS bohrungbemerkung, b.bohrprofil_id, b.bohrung_id, b.datum, b.bemerkung, b.kote, b.endteufe, b.tektonik, b.fmfelso, b.fmeto, b.quali, b.qualibem, b.wkb_geometry95, b.new_date, b.mut_date, b.new_usr, b.mut_usr, b.h_quali, b.h_tektonik, b.h_fmeto, b.h_fmfelso FROM bohrung.bohrung a, bohrung.bohrprofil b WHERE a.bohrung_id = b.bohrung_id AND b.new_date > '2014-01-01 00:00:00'::timestamp;

DROP VIEW IF EXISTS bohrung."BOE_Stammdaten";
CREATE VIEW bohrung."BOE_Stammdaten" AS
SELECT b.standort_id, a.bohrprofil_id, a.bohrung_id, a.kote, a.endteufe, a.tektonik, a.fmfelso, a.fmeto, a.quali, a.qualibem, a.new_usr, a.new_date, a.mut_date, a.mut_usr FROM bohrung.bohrprofil a, bohrung.bohrung b WHERE a.bohrung_id = b.bohrung_id;

DROP VIEW IF EXISTS bohrung."BOE_Stammdaten_Klartext";
CREATE VIEW bohrung."BOE_Stammdaten_Klartext" AS
SELECT b.standort_id, a.bohrprofil_id, a.bohrung_id, a.kote, a.endteufe, c1.kurztext AS tektonik, c2.kurztext AS fmfelso, c3.kurztext AS fmeto, c4.kurztext AS quali, a.qualibem, a.new_usr, a.new_date, a.mut_date, a.mut_usr FROM bohrung.bohrprofil a JOIN bohrung.bohrung b ON a.bohrung_id = b.bohrung_id LEFT JOIN bohrung.code c1 ON a.tektonik = c1.code_id LEFT JOIN bohrung.code c2 ON a.fmfelso = c2.code_id LEFT JOIN bohrung.code c3 ON a.fmeto = c3.code_id LEFT JOIN bohrung.code c4 ON a.quali = c4.code_id ORDER BY b.standort_id;

DROP VIEW IF EXISTS bohrung."BOE_Stammdaten_Klartext_LV95";
CREATE VIEW bohrung."BOE_Stammdaten_Klartext_LV95" AS
SELECT b.standort_id, a.bohrprofil_id, a.bohrung_id, a.kote, a.endteufe, c1.kurztext AS tektonik, c2.kurztext AS fmfelso, c3.kurztext AS fmeto, c4.kurztext AS quali, a.qualibem, public.st_x(a.wkb_geometry95) AS x_lv95, public.st_y(a.wkb_geometry95) AS y_lv95, a.new_usr, a.new_date, a.mut_date, a.mut_usr FROM bohrung.bohrprofil a JOIN bohrung.bohrung b ON a.bohrung_id = b.bohrung_id LEFT JOIN bohrung.code c1 ON a.tektonik = c1.code_id LEFT JOIN bohrung.code c2 ON a.fmfelso = c2.code_id LEFT JOIN bohrung.code c3 ON a.fmeto = c3.code_id LEFT JOIN bohrung.code c4 ON a.quali = c4.code_id ORDER BY b.standort_id;

DROP VIEW IF EXISTS bohrung."Stammdaten_DIA";
CREATE VIEW bohrung."Stammdaten_DIA" AS
SELECT b.standort_id, a.bohrprofil_id, a.bohrung_id, a.kote, a.endteufe, a.tektonik, a.fmfelso, a.fmeto, a.quali, a.qualibem, a.new_usr FROM bohrung.bohrprofil a, bohrung.bohrung b WHERE a.bohrung_id = b.bohrung_id;

ALTER TABLE bohrung.bohrung
    DROP CONSTRAINT IF EXISTS chk_bohrung_bohrart_bohrart,
    DROP COLUMN IF EXISTS bohrart,
    DROP COLUMN IF EXISTS h_bohrart,

    DROP CONSTRAINT IF EXISTS chk_bohrung_bohrzweck_bohrzweck,
    DROP COLUMN IF EXISTS bohrzweck,
    DROP COLUMN IF EXISTS h_bohrzweck,

    DROP COLUMN IF EXISTS besitzer,
    DROP COLUMN IF EXISTS ablenkungbem,
    DROP COLUMN IF EXISTS hotlinka,
    DROP COLUMN IF EXISTS hotlinkf,
    DROP COLUMN IF EXISTS fremd_bohr_id;

-- Upgrade table "bohrprofil", associated views and functions
DROP FUNCTION IF EXISTS bohrung._archive_date_insert() CASCADE;
DROP FUNCTION IF EXISTS bohrung._archive_date_update() CASCADE;
DROP FUNCTION IF EXISTS bohrung._archive_dummy() CASCADE;
DROP FUNCTION IF EXISTS bohrung._checkarchive() CASCADE;
DROP FUNCTION IF EXISTS bohrung.bohrprofil_clone(integer) CASCADE;

DROP VIEW IF EXISTS bohrung."Bohrungen_mit_BasisQuartär";
CREATE VIEW bohrung."Bohrungen_mit_BasisQuartär" AS
SELECT b.standort_id, s.schicht_id, s.bohrprofil_id, bp.kote::double precision - s.tiefe AS topfels, s.tiefe AS depthfels, s.quali, s.qualibem, s.bemerkung, s.h_quali, bp.endteufe, c.kurztext FROM bohrung.schicht s JOIN bohrung.bohrprofil bp ON s.bohrprofil_id = bp.bohrprofil_id JOIN bohrung.code c ON s.quali = c.code_id JOIN bohrung.bohrung b ON bp.bohrung_id = b.bohrung_id WHERE s.schichten_id = 2;

DROP VIEW IF EXISTS bohrung.bohrungen_mit_bte;
CREATE VIEW bohrung.bohrungen_mit_bte AS
SELECT b.standort_id, s.schicht_id, s.bohrprofil_id, bp.kote::double precision - s.tiefe AS z, s.tiefe AS depth, bp.kote, s.quali, s.qualibem, s.bemerkung, s.h_quali, bp.endteufe, c.kurztext FROM bohrung.schicht s JOIN bohrung.bohrprofil bp ON s.bohrprofil_id = bp.bohrprofil_id JOIN bohrung.code c ON s.quali = c.code_id JOIN bohrung.bohrung b ON bp.bohrung_id = b.bohrung_id WHERE s.schichten_id = 8;

ALTER TABLE bohrung.bohrprofil
    DROP COLUMN IF EXISTS wkb_geometry,
    DROP COLUMN IF EXISTS archive_date,
    DROP COLUMN IF EXISTS archive;

-- Move geometry from "bohrprofil" to "bohrung"
ALTER TABLE bohrung.bohrung ADD COLUMN IF NOT EXISTS wkb_geometry95 geometry;

UPDATE bohrung.bohrung b
SET wkb_geometry95 = bp.wkb_geometry95
FROM bohrung.bohrprofil bp
WHERE bp.bohrung_id = b.bohrung_id;

DROP VIEW IF EXISTS bohrung."141016_Stammdaten_Lieferung_ab2014";
CREATE VIEW bohrung."141016_Stammdaten_Lieferung_ab2014" AS
SELECT a.standort_id, a.bezeichnung, a.bemerkung AS bohrungbemerkung, b.bohrprofil_id, b.bohrung_id, b.datum, b.bemerkung, b.kote, b.endteufe, b.tektonik, b.fmfelso, b.fmeto, b.quali, b.qualibem, a.wkb_geometry95, b.new_date, b.mut_date, b.new_usr, b.mut_usr, b.h_quali, b.h_tektonik, b.h_fmeto, b.h_fmfelso FROM bohrung.bohrung a, bohrung.bohrprofil b WHERE a.bohrung_id = b.bohrung_id AND b.new_date > '2014-01-01 00:00:00'::timestamp;

DROP VIEW IF EXISTS bohrung."aktuelleBohrprofile_neu_neu";
CREATE VIEW bohrung."aktuelleBohrprofile_neu_neu" AS
SELECT a.standort_id, a.bezeichnung, a.bemerkung AS bohrungbemerkung, b.bohrprofil_id, b.bohrung_id, b.datum, b.bemerkung, b.kote, b.endteufe, b.tektonik, b.fmfelso, b.fmeto, b.quali, b.qualibem, a.wkb_geometry95, b.new_date, b.mut_date, b.new_usr, b.mut_usr, b.h_quali, b.h_tektonik, b.h_fmeto, b.h_fmfelso FROM bohrung.bohrung a, bohrung.bohrprofil b WHERE a.bohrung_id = b.bohrung_id AND b.new_date > '2014-01-01 00:00:00'::timestamp;

DROP VIEW IF EXISTS bohrung."BOE_Stammdaten_Klartext_LV95";
CREATE VIEW bohrung."BOE_Stammdaten_Klartext_LV95" AS
SELECT b.standort_id, a.bohrprofil_id, a.bohrung_id, a.kote, a.endteufe, c1.kurztext AS tektonik, c2.kurztext AS fmfelso, c3.kurztext AS fmeto, c4.kurztext AS quali, a.qualibem, public.st_x(b.wkb_geometry95) AS x_lv95, public.st_y(b.wkb_geometry95) AS y_lv95, a.new_usr, a.new_date, a.mut_date, a.mut_usr FROM bohrung.bohrprofil a JOIN bohrung.bohrung b ON a.bohrung_id = b.bohrung_id LEFT JOIN bohrung.code c1 ON a.tektonik = c1.code_id LEFT JOIN bohrung.code c2 ON a.fmfelso = c2.code_id LEFT JOIN bohrung.code c3 ON a.fmeto = c3.code_id LEFT JOIN bohrung.code c4 ON a.quali = c4.code_id ORDER BY b.standort_id;

ALTER TABLE bohrung.bohrprofil DROP COLUMN IF EXISTS wkb_geometry95;

-- Upgrade table "vorkommnis"
ALTER TABLE bohrung.vorkommnis
    DROP COLUMN IF EXISTS subtyp,
    DROP COLUMN IF EXISTS h_subtyp;

-- Add settings table
CREATE TABLE IF NOT EXISTS bohrung.settings (
    key varchar(80) PRIMARY KEY NOT NULL,
    value varchar(80)
);
