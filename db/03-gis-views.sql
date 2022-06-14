-- Add GIS views

\connect ews

DROP VIEW IF EXISTS bohrung."GIS_standort";
CREATE VIEW bohrung."GIS_standort" AS
SELECT standort_id, bezeichnung, bemerkung, gemeinde, gbnummer, new_date, mut_date, new_usr, mut_usr, freigabe_afu, afu_usr, afu_date FROM bohrung.standort;

DROP VIEW IF EXISTS bohrung."GIS_bohrung";
CREATE VIEW bohrung."GIS_bohrung" AS
SELECT bohrung_id, standort_id, bezeichnung, bemerkung, datum, durchmesserbohrloch, ablenkung, quali, qualibem, new_date, quelleref, mut_date, new_usr, mut_usr, h_quali, h_ablenkung, wkb_geometry FROM bohrung.bohrung;

DROP VIEW IF EXISTS bohrung."GIS_bohrprofil";
CREATE VIEW bohrung."GIS_bohrprofil" AS
SELECT bohrprofil_id, bohrung_id, datum, bemerkung, kote, endteufe, tektonik, fmfelso, fmeto, quali, qualibem, new_date, mut_date, new_usr, mut_usr, h_quali, h_tektonik, h_fmeto, h_fmfelso FROM bohrung.bohrprofil;

DROP VIEW IF EXISTS bohrung."GIS_schicht";
CREATE VIEW bohrung."GIS_schicht" AS
SELECT schicht_id, bohrprofil_id, schichten_id, tiefe, quali, qualibem, bemerkung, new_date, mut_date, new_usr, mut_usr, h_quali FROM bohrung.schicht;

DROP VIEW IF EXISTS bohrung."GIS_vorkommnis";
CREATE VIEW bohrung."GIS_vorkommnis" AS
SELECT vorkommnis_id, bohrprofil_id, typ, tiefe, bemerkung, new_date, mut_date, new_usr, mut_usr, quali, qualibem, h_quali, h_typ FROM bohrung.vorkommnis;

DROP VIEW IF EXISTS bohrung."GIS_code";
CREATE VIEW bohrung."GIS_code" AS
SELECT code_id, codetyp_id, kurztext, text, new_date, mut_date, new_usr, mut_usr, sort FROM bohrung.code;

DROP VIEW IF EXISTS bohrung."GIS_codeschicht";
CREATE VIEW bohrung."GIS_codeschicht" AS
SELECT codeschicht_id, kurztext, text, sort, new_date, mut_date, new_usr, mut_usr FROM bohrung.codeschicht;

DROP VIEW IF EXISTS bohrung."GIS_codetyp";
CREATE VIEW bohrung."GIS_codetyp" AS
SELECT codetyp_id, kurztext, text, new_date, mut_date, new_usr, mut_usr FROM bohrung.codetyp;
