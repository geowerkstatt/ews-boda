-- Database schema for ews-boda

CREATE DATABASE ews WITH TEMPLATE = template0 ENCODING = 'UTF8';

\connect ews

CREATE EXTENSION postgis;

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = off;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET escape_string_warning = off;
SET row_security = off;

CREATE SCHEMA bohrung;

--
-- Name: _archive_date_insert(); Type: FUNCTION; Schema: bohrung
--

CREATE FUNCTION bohrung._archive_date_insert() RETURNS trigger
    LANGUAGE plpgsql
    AS $$

BEGIN

IF NEW.archive=1 THEN
NEW.archive_date := now();
END IF;


RETURN NEW;

END;
$$;

--
-- Name: FUNCTION _archive_date_insert(); Type: COMMENT; Schema: bohrung
--

COMMENT ON FUNCTION bohrung._archive_date_insert() IS 'Triggerfunktion zum Setzen des archive_date, wenn ein neues Bohrprofil archiviert wird';


--
-- Name: _archive_date_update(); Type: FUNCTION; Schema: bohrung
--

CREATE FUNCTION bohrung._archive_date_update() RETURNS trigger
    LANGUAGE plpgsql
    AS $$

BEGIN

IF NEW.archive=1 and OLD.archive=0 THEN
	NEW.archive_date := now();
END IF;

IF NEW.archive=0 and OLD.archive=1 THEN
	NEW.archive_date := null;
END IF;



RETURN NEW;

END;
$$;


--
-- Name: FUNCTION _archive_date_update(); Type: COMMENT; Schema: bohrung
--

COMMENT ON FUNCTION bohrung._archive_date_update() IS 'Triggerfunktion zum Setzen des archive_date, wenn ein bestehendes Bohrprofil archiviert wird oder ein archiviertes Profil aktiv wird';


--
-- Name: _archive_dummy(); Type: FUNCTION; Schema: bohrung
--

CREATE FUNCTION bohrung._archive_dummy() RETURNS trigger
    LANGUAGE plpgsql
    AS $$

BEGIN



RETURN OLD;

END;
$$;


--
-- Name: FUNCTION _archive_dummy(); Type: COMMENT; Schema: bohrung
--

COMMENT ON FUNCTION bohrung._archive_dummy() IS 'Triggerfunktion zum Verweis auf bestehende Constraints';


--
-- Name: _checkarchive(); Type: FUNCTION; Schema: bohrung
--

CREATE FUNCTION bohrung._checkarchive() RETURNS trigger
    LANGUAGE plpgsql
    AS $$

DECLARE
v_bohrung_id integer;

BEGIN



IF TG_OP = 'DELETE' THEN
v_bohrung_id = OLD.bohrung_id;
ELSE
v_bohrung_id = NEW.bohrung_id;
END IF;

/**
Allow the deletion of an active bohrprofil if it is the last profile
**/

IF TG_OP = 'DELETE' and (select count(*) from bohrung.bohrprofil where v_bohrung_id=bohrung_id and archive=0) <= 1 and (select count(*) from bohrung.bohrprofil where v_bohrung_id=bohrung_id and archive=1) = 0 THEN
	RAISE NOTICE 'try to delete another block1 %', TG_TABLE_NAME;
	RETURN NEW;
ELSE
	IF (select count(*) from bohrung.bohrprofil where v_bohrung_id=bohrung_id and archive =0) != 1 THEN
		RAISE EXCEPTION 'Constraint Trigger: Genau ein Bohrprofil muss aktiv sein (archive=0)' ;
	END IF;
END IF;


RETURN NEW;

END;
$$;


--
-- Name: FUNCTION _checkarchive(); Type: COMMENT; Schema: bohrung
--

COMMENT ON FUNCTION bohrung._checkarchive() IS 'Triggerfunktion zur Sicherstellung des konsistenten Zustands des Archives an Bohrprofilen';


--
-- Name: _insert_bohrprofil(); Type: FUNCTION; Schema: bohrung
--

CREATE FUNCTION bohrung._insert_bohrprofil() RETURNS trigger
    LANGUAGE plpgsql
    AS $$

BEGIN

IF (select count(bohrprofil_id) from bohrung.bohrprofil where bohrprofil_id = NEW.bohrprofil_id) > 0
THEN
NEW.bohrprofil_id := nextval('bohrung.bohrprofil_bohrprofil_id_seq');

END IF;

RETURN NEW;

END;
$$;


--
-- Name: FUNCTION _insert_bohrprofil(); Type: COMMENT; Schema: bohrung
--

COMMENT ON FUNCTION bohrung._insert_bohrprofil() IS 'Korrigiert die bohrprofil_id bei Verwendung der Klonfunktion';


--
-- Name: _insert_schicht(); Type: FUNCTION; Schema: bohrung
--

CREATE FUNCTION bohrung._insert_schicht() RETURNS trigger
    LANGUAGE plpgsql
    AS $$

BEGIN

IF (select count(schicht_id) from bohrung.schicht where schicht_id = NEW.schicht_id) > 0
THEN
NEW.schicht_id := nextval('bohrung.schicht_schicht_id_seq');

END IF;

RETURN NEW;

END;
$$;

--
-- Name: FUNCTION _insert_schicht(); Type: COMMENT; Schema: bohrung
--

COMMENT ON FUNCTION bohrung._insert_schicht() IS 'Korrigiert die schicht_id bei Verwendung der Klonfunktion';


--
-- Name: _insert_vorkommnis(); Type: FUNCTION; Schema: bohrung
--

CREATE FUNCTION bohrung._insert_vorkommnis() RETURNS trigger
    LANGUAGE plpgsql
    AS $$

BEGIN

IF (select count(vorkommnis_id) from bohrung.vorkommnis where vorkommnis_id = NEW.vorkommnis_id) > 0
THEN
NEW.vorkommnis_id := nextval('bohrung.vorkommnis_vorkommnis_id_seq');

END IF;

RETURN NEW;

END;
$$;


--
-- Name: FUNCTION _insert_vorkommnis(); Type: COMMENT; Schema: bohrung
--

COMMENT ON FUNCTION bohrung._insert_vorkommnis() IS 'Korrigiert die vorkommnis_id bei Verwendung der Klonfunktion';


--
-- Name: _mutation(); Type: FUNCTION; Schema: bohrung
--

CREATE FUNCTION bohrung._mutation() RETURNS trigger
    LANGUAGE plpgsql
    AS $$

BEGIN

NEW.mut_date := now();
NEW.mut_usr := CURRENT_USER;

RETURN NEW;

END;
$$;


--
-- Name: FUNCTION _mutation(); Type: COMMENT; Schema: bohrung
--

COMMENT ON FUNCTION bohrung._mutation() IS 'Generische Mutationstriggerfunktion zum Setzen von mut_date und mut_usr';


--
-- Name: bohrprofil_clone(integer); Type: FUNCTION; Schema: bohrung
--

CREATE FUNCTION bohrung.bohrprofil_clone(p_bohrprofil_id integer) RETURNS integer
    LANGUAGE plpgsql
    AS $$

declare v_bohrprofil_id integer;
declare v_a_bohrprofil_id integer;
declare v_bohrprofil bohrung.bohrprofil%ROWTYPE;
declare v_vorkommnis bohrung.vorkommnis%ROWTYPE;
declare v_schicht bohrung.schicht%ROWTYPE;

BEGIN

select * into v_bohrprofil from bohrung.bohrprofil where bohrprofil_id = p_bohrprofil_id;

if v_bohrprofil.archive = 1 then
	select bohrprofil_id into v_a_bohrprofil_id from bohrung.bohrprofil where bohrung_id = v_bohrprofil.bohrung_id and archive=0;
else
	v_a_bohrprofil_id := v_bohrprofil.bohrprofil_id;
end if;

v_bohrprofil.archive := 0;
v_bohrprofil.archive_date := null;
v_bohrprofil.mut_usr := null;
v_bohrprofil.mut_date := null;
v_bohrprofil.new_date := "now"();
v_bohrprofil.new_usr := "current_user"();


update bohrung.bohrprofil set archive=1 where bohrprofil_id = v_a_bohrprofil_id;
insert into bohrung.bohrprofil values (v_bohrprofil.*)
returning bohrprofil_id into v_bohrprofil_id;

for v_schicht in select * from bohrung.schicht where bohrprofil_id = p_bohrprofil_id LOOP

	v_schicht.bohrprofil_id := v_bohrprofil_id;
	v_schicht.mut_usr := null;
	v_schicht.mut_date := null;
	v_schicht.new_date := "now"();
	v_schicht.new_usr := "current_user"();

	insert into bohrung.schicht values (v_schicht.*);
end loop;

for v_vorkommnis IN select * from bohrung.vorkommnis where bohrprofil_id=p_bohrprofil_id LOOP

	v_vorkommnis.bohrprofil_id := v_bohrprofil_id;
	v_vorkommnis.mut_usr := null;
	v_vorkommnis.mut_date := null;
	v_vorkommnis.new_date := "now"();
	v_vorkommnis.new_usr := "current_user"();

	insert into bohrung.vorkommnis values (v_vorkommnis.*);
end loop;

return v_bohrprofil_id;

END;
$$;

--
-- Name: FUNCTION bohrprofil_clone(p_bohrprofil_id integer); Type: COMMENT; Schema: bohrung
--

COMMENT ON FUNCTION bohrung.bohrprofil_clone(p_bohrprofil_id integer) IS 'Erstellt eine Kopie (Klon) eines Bohrprofils sowie der davon abhÃ¤ngenden Eintraege in den Tabellen "schicht" und "vorkommnis"';


SET default_tablespace = '';

--
-- Name: bohrprofil; Type: TABLE; Schema: bohrung
--

CREATE TABLE bohrung.bohrprofil (
    bohrprofil_id integer NOT NULL,
    bohrung_id integer,
    datum date,
    bemerkung text,
    kote smallint,
    endteufe smallint,
    tektonik integer,
    fmfelso integer,
    fmeto integer,
    quali integer,
    qualibem text,
    wkb_geometry public.geometry,
    wkb_geometry95 public.geometry,
    archive_date timestamp without time zone DEFAULT '9999-01-01'::date,
    archive integer DEFAULT 0 NOT NULL,
    new_date timestamp without time zone DEFAULT now() NOT NULL,
    mut_date timestamp without time zone,
    new_usr character varying DEFAULT "current_user"() NOT NULL,
    mut_usr character varying,
    h_quali integer DEFAULT 12 NOT NULL,
    h_tektonik integer DEFAULT 10 NOT NULL,
    h_fmeto integer DEFAULT 5 NOT NULL,
    h_fmfelso integer DEFAULT 5 NOT NULL,
    CONSTRAINT chk_bohrprofil_fmeto_fmeto CHECK ((h_fmeto = 5)),
    CONSTRAINT chk_bohrprofil_fmfelso_fmfelso CHECK ((h_fmfelso = 5)),
    CONSTRAINT chk_bohrprofil_quali_quali CHECK ((h_quali = 12)),
    CONSTRAINT chk_bohrprofil_tektonik_tektonik CHECK ((h_tektonik = 10)),
    CONSTRAINT enforce_dims_wkb_geometry CHECK ((public.st_ndims(wkb_geometry) = 2)),
    CONSTRAINT enforce_dims_wkb_geometry95 CHECK ((public.st_ndims(wkb_geometry95) = 2))
);

--
-- Name: TABLE bohrprofil; Type: COMMENT; Schema: bohrung
--

COMMENT ON TABLE bohrung.bohrprofil IS 'Informationen zum Bohrprofil. Ein Bohrprofil entspricht einer Bohrung bzw. eine Interpretation davon.';


--
-- Name: COLUMN bohrprofil.bohrprofil_id; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.bohrprofil.bohrprofil_id IS 'Feature ID';


--
-- Name: COLUMN bohrprofil.bohrung_id; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.bohrprofil.bohrung_id IS 'Foreign Key: ID der Tabelle Bohrung';


--
-- Name: COLUMN bohrprofil.datum; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.bohrprofil.datum IS 'Datum des Bohrprofils';


--
-- Name: COLUMN bohrprofil.bemerkung; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.bohrprofil.bemerkung IS 'Bemerkung zum Bohrprofil';


--
-- Name: COLUMN bohrprofil.kote; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.bohrprofil.kote IS 'Terrainkote der Bohrung [m]';


--
-- Name: COLUMN bohrprofil.endteufe; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.bohrprofil.endteufe IS 'Endtiefe der Bohrung [m]';


--
-- Name: COLUMN bohrprofil.tektonik; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.bohrprofil.tektonik IS 'Klassierung Tektonik';


--
-- Name: COLUMN bohrprofil.fmfelso; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.bohrprofil.fmfelso IS 'Formation Fels';


--
-- Name: COLUMN bohrprofil.fmeto; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.bohrprofil.fmeto IS 'Formation Endtiefe';


--
-- Name: COLUMN bohrprofil.quali; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.bohrprofil.quali IS 'Qualität der Angaben zum Bohrprofil';


--
-- Name: COLUMN bohrprofil.qualibem; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.bohrprofil.qualibem IS 'Bemerkung zur Qualitätsangabe';


--
-- Name: COLUMN bohrprofil.wkb_geometry; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.bohrprofil.wkb_geometry IS 'OGC WKB Geometrie SRID 21781 LV03';


--
-- Name: COLUMN bohrprofil.wkb_geometry95; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.bohrprofil.wkb_geometry95 IS 'OGC WKB Geometrie SRID 2056 LV95';


--
-- Name: COLUMN bohrprofil.archive_date; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.bohrprofil.archive_date IS 'Datum der Archvierung des Objektes';


--
-- Name: COLUMN bohrprofil.archive; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.bohrprofil.archive IS '0: aktiv, 1: archiviert';


--
-- Name: COLUMN bohrprofil.new_date; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.bohrprofil.new_date IS 'Datum des Imports des Objektes';


--
-- Name: COLUMN bohrprofil.mut_date; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.bohrprofil.mut_date IS 'Timestamp letzte Änderung';


--
-- Name: COLUMN bohrprofil.new_usr; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.bohrprofil.new_usr IS 'Kürzel des Benutzers bei Anlage';


--
-- Name: COLUMN bohrprofil.mut_usr; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.bohrprofil.mut_usr IS 'Kürzel des Benutzers bei letzter Änderung';


--
-- Name: COLUMN bohrprofil.h_quali; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.bohrprofil.h_quali IS 'Foreign Key: ID des Codetyps für Feld quali';


--
-- Name: COLUMN bohrprofil.h_tektonik; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.bohrprofil.h_tektonik IS 'Foreign Key: ID des Codetyps für Feld tektonik';


--
-- Name: COLUMN bohrprofil.h_fmeto; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.bohrprofil.h_fmeto IS 'Foreign Key: ID des Codetyps für Feld fmeto';


--
-- Name: COLUMN bohrprofil.h_fmfelso; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.bohrprofil.h_fmfelso IS 'Foreign Key: ID des Codetyps für Feld fmfelso';


--
-- Name: bohrung; Type: TABLE; Schema: bohrung
--

CREATE TABLE bohrung.bohrung (
    bohrung_id integer NOT NULL,
    standort_id integer NOT NULL,
    bezeichnung text NOT NULL,
    bemerkung text,
    datum date,
    besitzer text,
    durchmesserbohrloch smallint,
    bohrart integer,
    bohrzweck integer,
    ablenkung integer,
    ablenkungbem text,
    quali integer,
    qualibem text,
    new_date timestamp without time zone DEFAULT now() NOT NULL,
    quelleref text,
    hotlinka text,
    hotlinkf text,
    mut_date timestamp without time zone,
    new_usr character varying DEFAULT "current_user"() NOT NULL,
    mut_usr character varying,
    h_quali integer DEFAULT 3 NOT NULL,
    h_bohrart integer DEFAULT 7 NOT NULL,
    h_bohrzweck integer DEFAULT 8 NOT NULL,
    h_ablenkung integer DEFAULT 9 NOT NULL,
    fremd_bohr_id integer,
    CONSTRAINT chk_bohrung_ablenkung_ablenkung CHECK ((h_ablenkung = 9)),
    CONSTRAINT chk_bohrung_bohrart_bohrart CHECK ((h_bohrart = 7)),
    CONSTRAINT chk_bohrung_bohrzweck_bohrzweck CHECK ((h_bohrzweck = 8)),
    CONSTRAINT chk_bohrung_quali_quali CHECK ((h_quali = 3))
);


--
-- Name: TABLE bohrung; Type: COMMENT; Schema: bohrung
--

COMMENT ON TABLE bohrung.bohrung IS 'Informationen zur Bohrung';


--
-- Name: COLUMN bohrung.bohrung_id; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.bohrung.bohrung_id IS 'Feature ID';


--
-- Name: COLUMN bohrung.standort_id; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.bohrung.standort_id IS 'Foreign Key: ID der Tabelle Anlage';


--
-- Name: COLUMN bohrung.bezeichnung; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.bohrung.bezeichnung IS 'Bezeichnung der Bohrung';


--
-- Name: COLUMN bohrung.bemerkung; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.bohrung.bemerkung IS 'Bemerkungen zur Bohrung';


--
-- Name: COLUMN bohrung.datum; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.bohrung.datum IS 'Datum des Bohrbeginns';


--
-- Name: COLUMN bohrung.besitzer; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.bohrung.besitzer IS 'Besitzer der Bohrung';


--
-- Name: COLUMN bohrung.durchmesserbohrloch; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.bohrung.durchmesserbohrloch IS 'Durchmesser der Bohrlöcher [mm]';


--
-- Name: COLUMN bohrung.bohrart; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.bohrung.bohrart IS 'Art der Bohrung';


--
-- Name: COLUMN bohrung.bohrzweck; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.bohrung.bohrzweck IS 'Zweck der Bohrung';


--
-- Name: COLUMN bohrung.ablenkung; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.bohrung.ablenkung IS 'Klassierung der Ablenkung';


--
-- Name: COLUMN bohrung.ablenkungbem; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.bohrung.ablenkungbem IS 'Bemerkung zu Ablenkung';


--
-- Name: COLUMN bohrung.quali; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.bohrung.quali IS 'Qualitätsangabe zur Bohrprofilschicht';


--
-- Name: COLUMN bohrung.qualibem; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.bohrung.qualibem IS 'Bemerkung zur Qualitätsangabe';


--
-- Name: COLUMN bohrung.new_date; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.bohrung.new_date IS 'Datum des Imports des Objektes';


--
-- Name: COLUMN bohrung.quelleref; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.bohrung.quelleref IS 'Autor geol. Aufnahme (Firma, Bearbeiter, Jahr)';


--
-- Name: COLUMN bohrung.hotlinka; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.bohrung.hotlinka IS 'Pfad zur Ablage der gescannten Bohrungsprofile etc';


--
-- Name: COLUMN bohrung.hotlinkf; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.bohrung.hotlinkf IS 'Dateiname des gescannten Bohrungsprofile u. Dokumente';


--
-- Name: COLUMN bohrung.mut_date; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.bohrung.mut_date IS 'Timestamp letzte Änderung';


--
-- Name: COLUMN bohrung.new_usr; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.bohrung.new_usr IS 'Kürzel des Benutzers bei Anlage';


--
-- Name: COLUMN bohrung.mut_usr; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.bohrung.mut_usr IS 'Kürzel des Benutzers bei letzter Änderung';


--
-- Name: COLUMN bohrung.h_quali; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.bohrung.h_quali IS 'Foreign Key: ID des Codetyps für Feld quali';


--
-- Name: COLUMN bohrung.h_bohrart; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.bohrung.h_bohrart IS 'Foreign Key: ID des Codetyps für Feld bohrart';


--
-- Name: COLUMN bohrung.h_bohrzweck; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.bohrung.h_bohrzweck IS 'Foreign Key: ID des Codetyps für Feld bohrzweck';


--
-- Name: COLUMN bohrung.h_ablenkung; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.bohrung.h_ablenkung IS 'Foreign Key: ID des Codetyps für Feld ablenkung';


--
-- Name: COLUMN bohrung.fremd_bohr_id; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.bohrung.fremd_bohr_id IS 'ID der Bohrung in einer anderen Datenbank';


--
-- Name: code; Type: TABLE; Schema: bohrung
--

CREATE TABLE bohrung.code (
    code_id integer NOT NULL,
    codetyp_id integer NOT NULL,
    kurztext character varying NOT NULL,
    text character varying,
    new_date timestamp without time zone DEFAULT now() NOT NULL,
    mut_date timestamp without time zone,
    new_usr character varying DEFAULT "current_user"() NOT NULL,
    mut_usr character varying,
    sort smallint
);


--
-- Name: TABLE code; Type: COMMENT; Schema: bohrung
--

COMMENT ON TABLE bohrung.code IS 'Verwaltung der Codes';


--
-- Name: COLUMN code.code_id; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.code.code_id IS 'Feature ID';


--
-- Name: COLUMN code.codetyp_id; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.code.codetyp_id IS 'Referenz auf die Spalte codetypid in der Tabelle codetyp';


--
-- Name: COLUMN code.kurztext; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.code.kurztext IS 'Kurzbezeichnung des Codes';


--
-- Name: COLUMN code.text; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.code.text IS 'Ausfuehrliche Bezeichnung des Codes';


--
-- Name: COLUMN code.new_date; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.code.new_date IS 'Timestamp Anlage';


--
-- Name: COLUMN code.mut_date; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.code.mut_date IS 'Timestamp letzte Änderung';


--
-- Name: COLUMN code.new_usr; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.code.new_usr IS 'Kürzel des Benutzers bei Anlage';


--
-- Name: COLUMN code.mut_usr; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.code.mut_usr IS 'Kürzel des Benutzers bei letzter Änderung';


--
-- Name: COLUMN code.sort; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.code.sort IS 'Vermutliche Reihenfolge von Codes eines Codetypen';


--
-- Name: codeschicht; Type: TABLE; Schema: bohrung
--

CREATE TABLE bohrung.codeschicht (
    codeschicht_id integer NOT NULL,
    kurztext text NOT NULL,
    text text NOT NULL,
    sort smallint,
    new_date timestamp without time zone DEFAULT now() NOT NULL,
    mut_date timestamp without time zone,
    new_usr character varying DEFAULT "current_user"() NOT NULL,
    mut_usr character varying
);


--
-- Name: COLUMN codeschicht.codeschicht_id; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.codeschicht.codeschicht_id IS 'Primärschlüssel';


--
-- Name: COLUMN codeschicht.kurztext; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.codeschicht.kurztext IS 'Kürzel zur Identifizierung der Schicht';


--
-- Name: COLUMN codeschicht.text; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.codeschicht.text IS 'Bezeichnung der Schicht';


--
-- Name: COLUMN codeschicht.sort; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.codeschicht.sort IS 'Vorgabe für Reihenfolge der Schichten bei Erfassung in Tabelle schicht';


--
-- Name: COLUMN codeschicht.new_date; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.codeschicht.new_date IS 'Timestamp Anlage';


--
-- Name: COLUMN codeschicht.mut_date; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.codeschicht.mut_date IS 'Timestamp letzte Änderung';


--
-- Name: COLUMN codeschicht.new_usr; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.codeschicht.new_usr IS 'Kürzel des Benutzers bei Anlage';


--
-- Name: COLUMN codeschicht.mut_usr; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.codeschicht.mut_usr IS 'Kürzel des Benutzers bei letzter Änderung';


--
-- Name: schicht; Type: TABLE; Schema: bohrung
--

CREATE TABLE bohrung.schicht (
    schicht_id integer NOT NULL,
    bohrprofil_id integer NOT NULL,
    schichten_id integer NOT NULL,
    tiefe real NOT NULL,
    quali integer NOT NULL,
    qualibem text,
    bemerkung text,
    new_date timestamp without time zone DEFAULT now() NOT NULL,
    mut_date timestamp without time zone,
    new_usr character varying DEFAULT "current_user"() NOT NULL,
    mut_usr character varying,
    h_quali integer DEFAULT 11 NOT NULL,
    CONSTRAINT chk_schicht_quali_quali CHECK ((h_quali = 11))
);


--
-- Name: TABLE schicht; Type: COMMENT; Schema: bohrung
--

COMMENT ON TABLE bohrung.schicht IS 'Erfassung der einzelnen Bohrprofilschichten';


--
-- Name: COLUMN schicht.schicht_id; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.schicht.schicht_id IS 'Feature ID';


--
-- Name: COLUMN schicht.bohrprofil_id; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.schicht.bohrprofil_id IS 'Foreign Key: ID der Tabelle bohrprofil';


--
-- Name: COLUMN schicht.schichten_id; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.schicht.schichten_id IS 'Feature ID';


--
-- Name: COLUMN schicht.tiefe; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.schicht.tiefe IS 'Tiefe der Schichtgrenze [m]';


--
-- Name: COLUMN schicht.quali; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.schicht.quali IS 'Qualitätsangabe zur Borhprofilschicht';


--
-- Name: COLUMN schicht.qualibem; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.schicht.qualibem IS 'Bemerkung zur Qualitätsangabe';


--
-- Name: COLUMN schicht.bemerkung; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.schicht.bemerkung IS 'Bemerkung zur Schicht';


--
-- Name: COLUMN schicht.new_date; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.schicht.new_date IS 'Datum des Imports des Objektes';


--
-- Name: COLUMN schicht.mut_date; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.schicht.mut_date IS 'Kürzel des Benutzers bei Anlage';


--
-- Name: COLUMN schicht.new_usr; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.schicht.new_usr IS 'Kürzel des Benutzers bei Anlage';


--
-- Name: COLUMN schicht.mut_usr; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.schicht.mut_usr IS 'Kürzel des Benutzers bei letzter Änderung';


--
-- Name: COLUMN schicht.h_quali; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.schicht.h_quali IS 'Foreign Key: ID des Codetyps für Feld quali';


--
-- Name: 141016_Schichtdaten_Lieferung_ab2014; Type: VIEW; Schema: bohrung
--

CREATE VIEW bohrung."141016_Schichtdaten_Lieferung_ab2014" AS
SELECT b.standort_id, s.schicht_id, s.bohrprofil_id, s.schichten_id, ((cs.kurztext || '_'::text) || "substring"((c.kurztext)::text, 1, 3)) AS markerbeschreibung, s.tiefe, s.quali, c.kurztext AS qualitext, s.qualibem, s.bemerkung, s.new_date, s.mut_date, s.new_usr, s.mut_usr, s.h_quali FROM ((((bohrung.schicht s JOIN bohrung.codeschicht cs ON ((s.schichten_id = cs.codeschicht_id))) JOIN bohrung.bohrprofil bp ON ((s.bohrprofil_id = bp.bohrprofil_id))) JOIN bohrung.bohrung b ON ((bp.bohrung_id = b.bohrung_id))) JOIN bohrung.code c ON ((s.quali = c.code_id))) WHERE (bp.new_date > '2014-01-01 00:00:00'::timestamp without time zone) ORDER BY b.standort_id, s.tiefe;


--
-- Name: 141016_Stammdaten_Lieferung_ab2014; Type: VIEW; Schema: bohrung
--

CREATE VIEW bohrung."141016_Stammdaten_Lieferung_ab2014" AS
SELECT a.standort_id, a.bezeichnung, a.bemerkung AS bohrungbemerkung, a.besitzer, a.bohrart, a.bohrzweck, a.hotlinkf, public.st_x(b.wkb_geometry) AS st_x, public.st_y(b.wkb_geometry) AS st_y, b.bohrprofil_id, b.bohrung_id, b.datum, b.bemerkung, b.kote, b.endteufe, b.tektonik, b.fmfelso, b.fmeto, b.quali, b.qualibem, b.wkb_geometry, b.wkb_geometry95, b.archive_date, b.archive, b.new_date, b.mut_date, b.new_usr, b.mut_usr, b.h_quali, b.h_tektonik, b.h_fmeto, b.h_fmfelso FROM bohrung.bohrung a, bohrung.bohrprofil b WHERE (((a.bohrung_id = b.bohrung_id) AND (b.archive = 0)) AND (b.new_date > '2014-01-01 00:00:00'::timestamp without time zone));


--
-- Name: Alle unarchivierten Bohrprofile mit Bohrnamen; Type: VIEW; Schema: bohrung
--

CREATE VIEW bohrung."Alle unarchivierten Bohrprofile mit Bohrnamen" AS
SELECT a.bohrung_id, a.standort_id, a.bezeichnung, a.bemerkung, a.besitzer, a.bohrart, a.bohrzweck, a.hotlinkf, b.quali, b.qualibem, b.wkb_geometry, b.kote, b.endteufe, b.fmfelso, b.fmeto FROM bohrung.bohrung a, bohrung.bohrprofil b WHERE ((a.bohrung_id = b.bohrung_id) AND (b.archive = 0));


--
-- Name: BOE_Schichtdaten; Type: VIEW; Schema: bohrung
--

CREATE VIEW bohrung."BOE_Schichtdaten" AS
SELECT a.schicht_id, a.bohrprofil_id, a.schichten_id, a.tiefe, b.kurztext, b.text, a.quali, c.kurztext AS qualibez, (((c.kurztext)::text || '___'::text) || b.kurztext) AS markerbez, a.qualibem, a.bemerkung, a.new_date, a.mut_date, a.new_usr, a.mut_usr, a.h_quali FROM ((bohrung.schicht a JOIN bohrung.codeschicht b ON ((a.schichten_id = b.codeschicht_id))) JOIN bohrung.code c ON ((a.quali = c.code_id))) ORDER BY a.bohrprofil_id, a.tiefe;


--
-- Name: BOE_Schichtdaten_mit_standortID; Type: VIEW; Schema: bohrung
--

CREATE VIEW bohrung."BOE_Schichtdaten_mit_standortID" AS
SELECT a.schicht_id, a.bohrprofil_id, bp.bohrung_id, bb.standort_id, a.schichten_id, a.tiefe, b.kurztext, b.text, a.quali, c.kurztext AS qualibez, (((c.kurztext)::text || '___'::text) || b.kurztext) AS markerbez, a.qualibem, a.bemerkung, a.new_date, a.mut_date, a.new_usr, a.mut_usr, a.h_quali FROM ((((bohrung.schicht a JOIN bohrung.codeschicht b ON ((a.schichten_id = b.codeschicht_id))) JOIN bohrung.code c ON ((a.quali = c.code_id))) JOIN bohrung.bohrprofil bp ON ((a.bohrprofil_id = bp.bohrprofil_id))) JOIN bohrung.bohrung bb ON ((bp.bohrung_id = bb.bohrung_id))) ORDER BY a.bohrprofil_id, a.tiefe;


--
-- Name: BOE_Stammdaten; Type: VIEW; Schema: bohrung
--

CREATE VIEW bohrung."BOE_Stammdaten" AS
SELECT b.standort_id, a.bohrprofil_id, a.bohrung_id, a.kote, a.endteufe, a.tektonik, a.fmfelso, a.fmeto, a.quali, a.qualibem, public.st_x(a.wkb_geometry) AS st_x, public.st_y(a.wkb_geometry) AS st_y, b.hotlinkf, a.new_usr, a.new_date, a.mut_date, a.mut_usr FROM bohrung.bohrprofil a, bohrung.bohrung b WHERE (((a.bohrung_id = b.bohrung_id) AND (a.archive = 0)) AND (b.hotlinkf !~~ '%.jpg%'::text)) ORDER BY b.hotlinkf;


--
-- Name: BOE_Stammdaten_Klartext; Type: VIEW; Schema: bohrung
--

CREATE VIEW bohrung."BOE_Stammdaten_Klartext" AS
SELECT b.standort_id, a.bohrprofil_id, a.bohrung_id, a.kote, a.endteufe, c1.kurztext AS tektonik, c2.kurztext AS fmfelso, c3.kurztext AS fmeto, c4.kurztext AS quali, a.qualibem, public.st_x(a.wkb_geometry) AS st_x, public.st_y(a.wkb_geometry) AS st_y, b.hotlinkf, a.new_usr, a.new_date, a.mut_date, a.mut_usr FROM (((((bohrung.bohrprofil a JOIN bohrung.bohrung b ON ((a.bohrung_id = b.bohrung_id))) LEFT JOIN bohrung.code c1 ON ((a.tektonik = c1.code_id))) LEFT JOIN bohrung.code c2 ON ((a.fmfelso = c2.code_id))) LEFT JOIN bohrung.code c3 ON ((a.fmeto = c3.code_id))) LEFT JOIN bohrung.code c4 ON ((a.quali = c4.code_id))) WHERE (a.archive = 0) ORDER BY b.standort_id;


--
-- Name: BOE_Stammdaten_Klartext_LV95; Type: VIEW; Schema: bohrung
--

CREATE VIEW bohrung."BOE_Stammdaten_Klartext_LV95" AS
SELECT b.standort_id, a.bohrprofil_id, a.bohrung_id, a.kote, a.endteufe, c1.kurztext AS tektonik, c2.kurztext AS fmfelso, c3.kurztext AS fmeto, c4.kurztext AS quali, a.qualibem, public.st_x(a.wkb_geometry) AS x_lv03, public.st_y(a.wkb_geometry) AS y_lv03, public.st_x(a.wkb_geometry95) AS x_lv95, public.st_y(a.wkb_geometry95) AS y_lv95, b.hotlinkf, a.new_usr, a.new_date, a.mut_date, a.mut_usr FROM (((((bohrung.bohrprofil a JOIN bohrung.bohrung b ON ((a.bohrung_id = b.bohrung_id))) LEFT JOIN bohrung.code c1 ON ((a.tektonik = c1.code_id))) LEFT JOIN bohrung.code c2 ON ((a.fmfelso = c2.code_id))) LEFT JOIN bohrung.code c3 ON ((a.fmeto = c3.code_id))) LEFT JOIN bohrung.code c4 ON ((a.quali = c4.code_id))) WHERE (a.archive = 0) ORDER BY b.standort_id;


--
-- Name: Bohrungen_mit_BasisQuartär; Type: VIEW; Schema: bohrung
--

CREATE VIEW bohrung."Bohrungen_mit_BasisQuartär" AS
SELECT b.standort_id, s.schicht_id, s.bohrprofil_id, public.st_x(bp.wkb_geometry) AS st_x, public.st_y(bp.wkb_geometry) AS st_y, ((bp.kote)::double precision - s.tiefe) AS topfels, s.tiefe AS depthfels, s.quali, s.qualibem, s.bemerkung, s.h_quali, bp.endteufe, c.kurztext FROM (((bohrung.schicht s JOIN bohrung.bohrprofil bp ON ((s.bohrprofil_id = bp.bohrprofil_id))) JOIN bohrung.code c ON ((s.quali = c.code_id))) JOIN bohrung.bohrung b ON ((bp.bohrung_id = b.bohrung_id))) WHERE ((s.schichten_id = 2) AND (bp.archive = 0));


--
-- Name: Schichtdaten_DIA; Type: VIEW; Schema: bohrung
--

CREATE VIEW bohrung."Schichtdaten_DIA" AS
SELECT a.schicht_id, a.bohrprofil_id, a.schichten_id, a.tiefe, b.kurztext, b.text, a.quali, a.qualibem, a.bemerkung, a.new_date, a.mut_date, a.new_usr, a.mut_usr, a.h_quali FROM bohrung.schicht a, bohrung.codeschicht b WHERE ((a.schichten_id = b.codeschicht_id) AND (a.quali <> ALL (ARRAY[166, 165, 164]))) ORDER BY a.bohrprofil_id, a.tiefe;


--
-- Name: Stammdaten_DIA; Type: VIEW; Schema: bohrung
--

CREATE VIEW bohrung."Stammdaten_DIA" AS
SELECT b.standort_id, a.bohrprofil_id, a.bohrung_id, a.kote, a.endteufe, a.tektonik, a.fmfelso, a.fmeto, a.quali, a.qualibem, public.st_x(a.wkb_geometry) AS st_x, public.st_y(a.wkb_geometry) AS st_y, b.hotlinkf, a.new_usr FROM bohrung.bohrprofil a, bohrung.bohrung b WHERE (((a.bohrung_id = b.bohrung_id) AND (a.archive = 0)) AND (b.hotlinkf !~~ '%.jpg%'::text)) ORDER BY b.hotlinkf;


--
-- Name: aktuelleBohrprofile; Type: VIEW; Schema: bohrung
--

CREATE VIEW bohrung."aktuelleBohrprofile" AS
SELECT a.bohrung_id, a.standort_id, a.bezeichnung, a.bemerkung, a.besitzer, a.bohrart, a.bohrzweck, a.hotlinkf, b.quali, b.qualibem, public.st_x(b.wkb_geometry) AS st_x, public.st_y(b.wkb_geometry) AS st_y, b.kote, b.endteufe, b.fmfelso, b.fmeto FROM bohrung.bohrung a, bohrung.bohrprofil b WHERE ((a.bohrung_id = b.bohrung_id) AND (b.archive = 0));


--
-- Name: aktuelleBohrprofile_neu; Type: VIEW; Schema: bohrung
--

CREATE VIEW bohrung."aktuelleBohrprofile_neu" AS
SELECT a.bohrung_id, b.bohrprofil_id, a.standort_id, a.bezeichnung, a.bemerkung, a.besitzer, a.bohrart, a.bohrzweck, a.hotlinkf, b.quali, b.qualibem, public.st_x(b.wkb_geometry) AS st_x, public.st_y(b.wkb_geometry) AS st_y, b.kote, b.endteufe, b.fmfelso, b.fmeto FROM bohrung.bohrung a, bohrung.bohrprofil b WHERE ((a.bohrung_id = b.bohrung_id) AND (b.archive = 0));


--
-- Name: aktuelleBohrprofile_neu_neu; Type: VIEW; Schema: bohrung
--

CREATE VIEW bohrung."aktuelleBohrprofile_neu_neu" AS
SELECT a.standort_id, a.bezeichnung, a.bemerkung AS bohrungbemerkung, a.besitzer, a.bohrart, a.bohrzweck, a.hotlinkf, public.st_x(b.wkb_geometry) AS st_x, public.st_y(b.wkb_geometry) AS st_y, b.bohrprofil_id, b.bohrung_id, b.datum, b.bemerkung, b.kote, b.endteufe, b.tektonik, b.fmfelso, b.fmeto, b.quali, b.qualibem, b.wkb_geometry, b.wkb_geometry95, b.archive_date, b.archive, b.new_date, b.mut_date, b.new_usr, b.mut_usr, b.h_quali, b.h_tektonik, b.h_fmeto, b.h_fmfelso FROM bohrung.bohrung a, bohrung.bohrprofil b WHERE (((a.bohrung_id = b.bohrung_id) AND (b.archive = 0)) AND (b.new_date > '2014-01-01 00:00:00'::timestamp without time zone));


--
-- Name: VIEW "aktuelleBohrprofile_neu_neu"; Type: COMMENT; Schema: bohrung
--

COMMENT ON VIEW bohrung."aktuelleBohrprofile_neu_neu" IS 'Enthält nun alle Spalten aus Tabelle Borhprofil';


--
-- Name: bohrprofil_bohrprofil_id_seq; Type: SEQUENCE; Schema: bohrung
--

CREATE SEQUENCE bohrung.bohrprofil_bohrprofil_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: bohrprofil_bohrprofil_id_seq; Type: SEQUENCE OWNED BY; Schema: bohrung
--

ALTER SEQUENCE bohrung.bohrprofil_bohrprofil_id_seq OWNED BY bohrung.bohrprofil.bohrprofil_id;


--
-- Name: bohrung_bohrung_id_seq; Type: SEQUENCE; Schema: bohrung
--

CREATE SEQUENCE bohrung.bohrung_bohrung_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: bohrung_bohrung_id_seq; Type: SEQUENCE OWNED BY; Schema: bohrung
--

ALTER SEQUENCE bohrung.bohrung_bohrung_id_seq OWNED BY bohrung.bohrung.bohrung_id;


--
-- Name: bohrung_novegas; Type: VIEW; Schema: bohrung
--

CREATE VIEW bohrung.bohrung_novegas AS
SELECT bohrung.bohrung_id, bohrung.standort_id, bohrung.bezeichnung, bohrung.bemerkung, bohrung.datum, bohrung.besitzer, bohrung.durchmesserbohrloch, bohrung.bohrart, bohrung.bohrzweck, bohrung.ablenkung, bohrung.ablenkungbem, bohrung.quali, bohrung.qualibem, bohrung.new_date, bohrung.quelleref, bohrung.hotlinka, bohrung.hotlinkf, bohrung.mut_date, bohrung.new_usr, bohrung.mut_usr, bohrung.h_quali, bohrung.h_bohrart, bohrung.h_bohrzweck, bohrung.h_ablenkung FROM bohrung.bohrung WHERE (bohrung.bezeichnung !~~ '%VEGAS%'::text);


--
-- Name: bohrungen_mit_bte; Type: VIEW; Schema: bohrung
--

CREATE VIEW bohrung.bohrungen_mit_bte AS
SELECT b.standort_id, s.schicht_id, s.bohrprofil_id, public.st_x(bp.wkb_geometry) AS st_x, public.st_y(bp.wkb_geometry) AS st_y, ((bp.kote)::double precision - s.tiefe) AS z, s.tiefe AS depth, bp.kote, s.quali, s.qualibem, s.bemerkung, s.h_quali, bp.endteufe, c.kurztext FROM (((bohrung.schicht s JOIN bohrung.bohrprofil bp ON ((s.bohrprofil_id = bp.bohrprofil_id))) JOIN bohrung.code c ON ((s.quali = c.code_id))) JOIN bohrung.bohrung b ON ((bp.bohrung_id = b.bohrung_id))) WHERE ((s.schichten_id = 8) AND (bp.archive = 0));


--
-- Name: code_code_id_seq; Type: SEQUENCE; Schema: bohrung
--

CREATE SEQUENCE bohrung.code_code_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: code_code_id_seq; Type: SEQUENCE OWNED BY; Schema: bohrung
--

ALTER SEQUENCE bohrung.code_code_id_seq OWNED BY bohrung.code.code_id;


--
-- Name: codeschicht_codeschicht_id_seq; Type: SEQUENCE; Schema: bohrung
--

CREATE SEQUENCE bohrung.codeschicht_codeschicht_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: codeschicht_codeschicht_id_seq; Type: SEQUENCE OWNED BY; Schema: bohrung
--

ALTER SEQUENCE bohrung.codeschicht_codeschicht_id_seq OWNED BY bohrung.codeschicht.codeschicht_id;


--
-- Name: codetyp; Type: TABLE; Schema: bohrung
--

CREATE TABLE bohrung.codetyp (
    codetyp_id integer NOT NULL,
    kurztext character varying NOT NULL,
    text character varying NOT NULL,
    new_date timestamp without time zone DEFAULT now() NOT NULL,
    mut_date timestamp without time zone,
    new_usr character varying DEFAULT "current_user"() NOT NULL,
    mut_usr character varying
);


--
-- Name: TABLE codetyp; Type: COMMENT; Schema: bohrung
--

COMMENT ON TABLE bohrung.codetyp IS 'Zentrale Tabelle der Codetypen';


--
-- Name: COLUMN codetyp.codetyp_id; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.codetyp.codetyp_id IS 'Feature ID';


--
-- Name: COLUMN codetyp.kurztext; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.codetyp.kurztext IS 'Kurzbezeichnung des Codetypen';


--
-- Name: COLUMN codetyp.text; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.codetyp.text IS 'Ausfuehrliche Bezeichnung des Codetypen';


--
-- Name: COLUMN codetyp.new_date; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.codetyp.new_date IS 'Timestamp Anlage';


--
-- Name: COLUMN codetyp.mut_date; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.codetyp.mut_date IS 'Timestamp letzte Änderung';


--
-- Name: COLUMN codetyp.new_usr; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.codetyp.new_usr IS 'Kürzel des Benutzers bei Anlage';


--
-- Name: COLUMN codetyp.mut_usr; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.codetyp.mut_usr IS 'Kürzel des Benutzers bei letzter Änderung';


--
-- Name: codetyp_codetyp_id_seq; Type: SEQUENCE; Schema: bohrung
--

CREATE SEQUENCE bohrung.codetyp_codetyp_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: codetyp_codetyp_id_seq; Type: SEQUENCE OWNED BY; Schema: bohrung
--

ALTER SEQUENCE bohrung.codetyp_codetyp_id_seq OWNED BY bohrung.codetyp.codetyp_id;


--
-- Name: schicht_schicht_id_seq; Type: SEQUENCE; Schema: bohrung
--

CREATE SEQUENCE bohrung.schicht_schicht_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: schicht_schicht_id_seq; Type: SEQUENCE OWNED BY; Schema: bohrung
--

ALTER SEQUENCE bohrung.schicht_schicht_id_seq OWNED BY bohrung.schicht.schicht_id;


--
-- Name: standort; Type: TABLE; Schema: bohrung
--

CREATE TABLE bohrung.standort (
    standort_id integer NOT NULL,
    bezeichnung text NOT NULL,
    bemerkung text,
    anzbohrloch smallint,
    gembfs integer,
    gbnummer character(40),
    gaso_nr integer,
    quali integer,
    qualibem text,
    wkb_geometry public.geometry NOT NULL,
    wkb_geometry95 public.geometry,
    new_date timestamp without time zone DEFAULT now() NOT NULL,
    mut_date timestamp without time zone,
    new_usr character varying DEFAULT "current_user"() NOT NULL,
    mut_usr character varying,
    h_quali integer DEFAULT 3 NOT NULL,
    CONSTRAINT chk_standort_quali_quali CHECK ((h_quali = 3)),
    CONSTRAINT enforce_dims_wkb_geometry CHECK ((public.st_ndims(wkb_geometry) = 2)),
    CONSTRAINT enforce_dims_wkb_geometry95 CHECK ((public.st_ndims(wkb_geometry95) = 2))
);


--
-- Name: TABLE standort; Type: COMMENT; Schema: bohrung
--

COMMENT ON TABLE bohrung.standort IS 'Allgemeine Informationen zu einer zusammengehörigen Gruppe von Bohrungen';


--
-- Name: COLUMN standort.standort_id; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.standort.standort_id IS 'Feature ID';


--
-- Name: COLUMN standort.bezeichnung; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.standort.bezeichnung IS 'Bezeichnung des Standorts';


--
-- Name: COLUMN standort.bemerkung; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.standort.bemerkung IS 'Bemerkungen zum Standort';


--
-- Name: COLUMN standort.anzbohrloch; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.standort.anzbohrloch IS 'Anzahl Bohrloecher';


--
-- Name: COLUMN standort.gembfs; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.standort.gembfs IS 'Gemeinde-Nr.';


--
-- Name: COLUMN standort.gbnummer; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.standort.gbnummer IS 'Grundbuch-Nr , ';


--
-- Name: COLUMN standort.gaso_nr; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.standort.gaso_nr IS 'Gaso-Nr. (mobj_id) falls vorhanden';


--
-- Name: COLUMN standort.quali; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.standort.quali IS 'Qualität der Stammdaten';


--
-- Name: COLUMN standort.qualibem; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.standort.qualibem IS 'Bemerkung zur Qualitätsangabe';


--
-- Name: COLUMN standort.wkb_geometry; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.standort.wkb_geometry IS 'OGC WKB Geometrie SRID 21781 LV03';


--
-- Name: COLUMN standort.wkb_geometry95; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.standort.wkb_geometry95 IS 'OGC WKB Geometrie SRID 2056 LV95';


--
-- Name: COLUMN standort.new_date; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.standort.new_date IS 'Datum des Imports des Objektes';


--
-- Name: COLUMN standort.mut_date; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.standort.mut_date IS 'Timestamp letzte Änderung';


--
-- Name: COLUMN standort.new_usr; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.standort.new_usr IS 'Kürzel des Benutzers bei Anlage';


--
-- Name: COLUMN standort.mut_usr; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.standort.mut_usr IS 'Kürzel des Benutzers bei letzter Änderung';


--
-- Name: COLUMN standort.h_quali; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.standort.h_quali IS 'Foreign Key: ID des Codetyps für Feld quali';


--
-- Name: standort_standort_id_seq; Type: SEQUENCE; Schema: bohrung
--

CREATE SEQUENCE bohrung.standort_standort_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: standort_standort_id_seq; Type: SEQUENCE OWNED BY; Schema: bohrung
--

ALTER SEQUENCE bohrung.standort_standort_id_seq OWNED BY bohrung.standort.standort_id;


--
-- Name: vegasBohrungen; Type: TABLE; Schema: bohrung
--

CREATE TABLE bohrung."vegasBohrungen" (
    gid integer NOT NULL,
    vegas_id double precision,
    dokument_i double precision,
    bezeichnun character varying(254),
    dateiendun character varying(254),
    x double precision,
    y double precision,
    psql_stand character varying(200)
);


--
-- Name: vegasBohrungen_gid_seq; Type: SEQUENCE; Schema: bohrung
--

CREATE SEQUENCE bohrung."vegasBohrungen_gid_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;

--
-- Name: vegasBohrungen_gid_seq; Type: SEQUENCE OWNED BY; Schema: bohrung
--

ALTER SEQUENCE bohrung."vegasBohrungen_gid_seq" OWNED BY bohrung."vegasBohrungen".gid;


--
-- Name: vorkommnis; Type: TABLE; Schema: bohrung
--

CREATE TABLE bohrung.vorkommnis (
    vorkommnis_id integer NOT NULL,
    bohrprofil_id integer NOT NULL,
    typ integer NOT NULL,
    subtyp integer,
    tiefe real,
    bemerkung text,
    new_date timestamp without time zone DEFAULT now() NOT NULL,
    mut_date timestamp without time zone,
    new_usr character varying DEFAULT "current_user"() NOT NULL,
    mut_usr character varying,
    quali integer,
    qualibem text,
    h_quali integer DEFAULT 3 NOT NULL,
    h_typ integer DEFAULT 2 NOT NULL,
    h_subtyp integer DEFAULT 2 NOT NULL,
    CONSTRAINT chk_vorkommnis_quali_quali CHECK ((h_quali = 3)),
    CONSTRAINT chk_vorkommnis_subtyp_subtyp CHECK ((h_subtyp = 2)),
    CONSTRAINT chk_vorkommnis_typ_typ CHECK ((h_typ = 2))
);


--
-- Name: TABLE vorkommnis; Type: COMMENT; Schema: bohrung
--

COMMENT ON TABLE bohrung.vorkommnis IS 'Vorkommnisse bei der Bohrung: Karteser, Karst, Gas/Öl, Sulfat, technische Komplikationen';


--
-- Name: COLUMN vorkommnis.vorkommnis_id; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.vorkommnis.vorkommnis_id IS 'Feature ID';


--
-- Name: COLUMN vorkommnis.bohrprofil_id; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.vorkommnis.bohrprofil_id IS 'Foreign Key: ID der Tabelle bohrprofil';


--
-- Name: COLUMN vorkommnis.typ; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.vorkommnis.typ IS 'Art des Vorkommnisses, z.B. Arteser';


--
-- Name: COLUMN vorkommnis.subtyp; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.vorkommnis.subtyp IS 'Weitere Spezifizierung, z.B. Klassierung betr. artesischem Überlauf';


--
-- Name: COLUMN vorkommnis.tiefe; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.vorkommnis.tiefe IS 'Tiefe des Vorkommnisses';


--
-- Name: COLUMN vorkommnis.bemerkung; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.vorkommnis.bemerkung IS 'Bemerkung zum Vorkommnis';


--
-- Name: COLUMN vorkommnis.new_date; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.vorkommnis.new_date IS 'Timestamp Anlage';


--
-- Name: COLUMN vorkommnis.mut_date; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.vorkommnis.mut_date IS 'Timestamp letzte Änderung';


--
-- Name: COLUMN vorkommnis.new_usr; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.vorkommnis.new_usr IS 'Kürzel des Benutzers bei Anlage';


--
-- Name: COLUMN vorkommnis.mut_usr; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.vorkommnis.mut_usr IS 'Kürzel des Benutzers bei letzter Änderung';


--
-- Name: COLUMN vorkommnis.quali; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.vorkommnis.quali IS 'Qualitätsangabe zum Vorkommnis';


--
-- Name: COLUMN vorkommnis.qualibem; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.vorkommnis.qualibem IS 'Bemerkung zur Qualitätsangabe';


--
-- Name: COLUMN vorkommnis.h_quali; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.vorkommnis.h_quali IS 'Foreign Key: ID des Codetyps für Feld quali';


--
-- Name: COLUMN vorkommnis.h_typ; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.vorkommnis.h_typ IS 'Foreign Key: ID des Codetyps für Feld typ';


--
-- Name: COLUMN vorkommnis.h_subtyp; Type: COMMENT; Schema: bohrung
--

COMMENT ON COLUMN bohrung.vorkommnis.h_subtyp IS 'Foreign Key: ID des Codetyps für Feld subtyp';


--
-- Name: vorkommnis_vorkommnis_id_seq; Type: SEQUENCE; Schema: bohrung
--

CREATE SEQUENCE bohrung.vorkommnis_vorkommnis_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: vorkommnis_vorkommnis_id_seq; Type: SEQUENCE OWNED BY; Schema: bohrung
--

ALTER SEQUENCE bohrung.vorkommnis_vorkommnis_id_seq OWNED BY bohrung.vorkommnis.vorkommnis_id;


--
-- Name: bohrprofil bohrprofil_id; Type: DEFAULT; Schema: bohrung
--

ALTER TABLE ONLY bohrung.bohrprofil ALTER COLUMN bohrprofil_id SET DEFAULT nextval('bohrung.bohrprofil_bohrprofil_id_seq'::regclass);


--
-- Name: bohrung bohrung_id; Type: DEFAULT; Schema: bohrung
--

ALTER TABLE ONLY bohrung.bohrung ALTER COLUMN bohrung_id SET DEFAULT nextval('bohrung.bohrung_bohrung_id_seq'::regclass);


--
-- Name: code code_id; Type: DEFAULT; Schema: bohrung
--

ALTER TABLE ONLY bohrung.code ALTER COLUMN code_id SET DEFAULT nextval('bohrung.code_code_id_seq'::regclass);


--
-- Name: codeschicht codeschicht_id; Type: DEFAULT; Schema: bohrung
--

ALTER TABLE ONLY bohrung.codeschicht ALTER COLUMN codeschicht_id SET DEFAULT nextval('bohrung.codeschicht_codeschicht_id_seq'::regclass);


--
-- Name: codetyp codetyp_id; Type: DEFAULT; Schema: bohrung
--

ALTER TABLE ONLY bohrung.codetyp ALTER COLUMN codetyp_id SET DEFAULT nextval('bohrung.codetyp_codetyp_id_seq'::regclass);


--
-- Name: schicht schicht_id; Type: DEFAULT; Schema: bohrung
--

ALTER TABLE ONLY bohrung.schicht ALTER COLUMN schicht_id SET DEFAULT nextval('bohrung.schicht_schicht_id_seq'::regclass);


--
-- Name: standort standort_id; Type: DEFAULT; Schema: bohrung
--

ALTER TABLE ONLY bohrung.standort ALTER COLUMN standort_id SET DEFAULT nextval('bohrung.standort_standort_id_seq'::regclass);


--
-- Name: vegasBohrungen gid; Type: DEFAULT; Schema: bohrung
--

ALTER TABLE ONLY bohrung."vegasBohrungen" ALTER COLUMN gid SET DEFAULT nextval('bohrung."vegasBohrungen_gid_seq"'::regclass);


--
-- Name: vorkommnis vorkommnis_id; Type: DEFAULT; Schema: bohrung
--

ALTER TABLE ONLY bohrung.vorkommnis ALTER COLUMN vorkommnis_id SET DEFAULT nextval('bohrung.vorkommnis_vorkommnis_id_seq'::regclass);


--
-- Name: code chk_unique; Type: CONSTRAINT; Schema: bohrung
--

ALTER TABLE ONLY bohrung.code
   ADD CONSTRAINT chk_unique UNIQUE (codetyp_id, code_id);


--
-- Name: codeschicht pk_codeschicht_codeschicht_id; Type: CONSTRAINT; Schema: bohrung
--

ALTER TABLE ONLY bohrung.codeschicht
   ADD CONSTRAINT pk_codeschicht_codeschicht_id PRIMARY KEY (codeschicht_id);


--
-- Name: vorkommnis pk_vorkommnis_vorkommnis_id; Type: CONSTRAINT; Schema: bohrung
--

ALTER TABLE ONLY bohrung.vorkommnis
   ADD CONSTRAINT pk_vorkommnis_vorkommnis_id PRIMARY KEY (vorkommnis_id);


--
-- Name: bohrprofil pkey_bohrprofil_bohrprofil_id; Type: CONSTRAINT; Schema: bohrung
--

ALTER TABLE ONLY bohrung.bohrprofil
   ADD CONSTRAINT pkey_bohrprofil_bohrprofil_id PRIMARY KEY (bohrprofil_id);


--
-- Name: bohrung pkey_bohrung_bohrung_id; Type: CONSTRAINT; Schema: bohrung
--

ALTER TABLE ONLY bohrung.bohrung
   ADD CONSTRAINT pkey_bohrung_bohrung_id PRIMARY KEY (bohrung_id);


--
-- Name: code pkey_code_code_id; Type: CONSTRAINT; Schema: bohrung
--

ALTER TABLE ONLY bohrung.code
   ADD CONSTRAINT pkey_code_code_id PRIMARY KEY (code_id);


--
-- Name: codetyp pkey_codetyp_codetyp_id; Type: CONSTRAINT; Schema: bohrung
--

ALTER TABLE ONLY bohrung.codetyp
   ADD CONSTRAINT pkey_codetyp_codetyp_id PRIMARY KEY (codetyp_id);


--
-- Name: schicht pkey_schicht_schicht_id; Type: CONSTRAINT; Schema: bohrung
--

ALTER TABLE ONLY bohrung.schicht
   ADD CONSTRAINT pkey_schicht_schicht_id PRIMARY KEY (schicht_id);


--
-- Name: standort pkey_standort_standort_id; Type: CONSTRAINT; Schema: bohrung
--

ALTER TABLE ONLY bohrung.standort
   ADD CONSTRAINT pkey_standort_standort_id PRIMARY KEY (standort_id);


--
-- Name: vegasBohrungen vegasBohrungen_pkey; Type: CONSTRAINT; Schema: bohrung
--

ALTER TABLE ONLY bohrung."vegasBohrungen"
   ADD CONSTRAINT "vegasBohrungen_pkey" PRIMARY KEY (gid);


--
-- Name: bohrprofil archive_check_delete; Type: TRIGGER; Schema: bohrung
--

CREATE TRIGGER archive_check_delete BEFORE DELETE ON bohrung.bohrprofil FOR EACH ROW EXECUTE PROCEDURE bohrung._archive_dummy();


--
-- Name: TRIGGER archive_check_delete ON bohrprofil; Type: COMMENT; Schema: bohrung
--

COMMENT ON TRIGGER archive_check_delete ON bohrung.bohrprofil IS 'Notiz: Es zusÃ¤tzlich ein Check Constraint definiert. In Pgadmin nicht sichtbar.';


--
-- Name: bohrprofil archive_check_insert; Type: TRIGGER; Schema: bohrung
--

CREATE TRIGGER archive_check_insert BEFORE INSERT ON bohrung.bohrprofil FOR EACH ROW EXECUTE PROCEDURE bohrung._archive_date_insert();


--
-- Name: TRIGGER archive_check_insert ON bohrprofil; Type: COMMENT; Schema: bohrung
--

COMMENT ON TRIGGER archive_check_insert ON bohrung.bohrprofil IS 'Notiz: Es zusÃ¤tzlich ein Check Constraint definiert. In Pgadmin nicht sichtbar.';


--
-- Name: bohrprofil archive_check_update; Type: TRIGGER; Schema: bohrung
--

CREATE TRIGGER archive_check_update BEFORE UPDATE ON bohrung.bohrprofil FOR EACH ROW EXECUTE PROCEDURE bohrung._archive_date_update();


--
-- Name: TRIGGER archive_check_update ON bohrprofil; Type: COMMENT; Schema: bohrung
--

COMMENT ON TRIGGER archive_check_update ON bohrung.bohrprofil IS 'Notiz: Es zusÃ¤tzlich ein Check Constraint definiert. In Pgadmin nicht sichtbar.';


--
-- Name: bohrprofil checkarchive; Type: TRIGGER; Schema: bohrung
--

CREATE CONSTRAINT TRIGGER checkarchive AFTER INSERT OR DELETE OR UPDATE ON bohrung.bohrprofil DEFERRABLE INITIALLY DEFERRED FOR EACH ROW EXECUTE PROCEDURE bohrung._checkarchive();


--
-- Name: bohrprofil insertcheck; Type: TRIGGER; Schema: bohrung
--

CREATE TRIGGER insertcheck BEFORE INSERT ON bohrung.bohrprofil FOR EACH ROW EXECUTE PROCEDURE bohrung._insert_bohrprofil();


--
-- Name: schicht insertcheck; Type: TRIGGER; Schema: bohrung
--

CREATE TRIGGER insertcheck BEFORE INSERT ON bohrung.schicht FOR EACH ROW EXECUTE PROCEDURE bohrung._insert_schicht();


--
-- Name: vorkommnis insertcheck; Type: TRIGGER; Schema: bohrung
--

CREATE TRIGGER insertcheck BEFORE INSERT ON bohrung.vorkommnis FOR EACH ROW EXECUTE PROCEDURE bohrung._insert_vorkommnis();


--
-- Name: bohrprofil mutation; Type: TRIGGER; Schema: bohrung
--

CREATE TRIGGER mutation BEFORE UPDATE ON bohrung.bohrprofil FOR EACH ROW EXECUTE PROCEDURE bohrung._mutation();


--
-- Name: bohrung mutation; Type: TRIGGER; Schema: bohrung
--

CREATE TRIGGER mutation BEFORE UPDATE ON bohrung.bohrung FOR EACH ROW EXECUTE PROCEDURE bohrung._mutation();


--
-- Name: code mutation; Type: TRIGGER; Schema: bohrung
--

CREATE TRIGGER mutation BEFORE UPDATE ON bohrung.code FOR EACH ROW EXECUTE PROCEDURE bohrung._mutation();


--
-- Name: codeschicht mutation; Type: TRIGGER; Schema: bohrung
--

CREATE TRIGGER mutation BEFORE UPDATE ON bohrung.codeschicht FOR EACH ROW EXECUTE PROCEDURE bohrung._mutation();


--
-- Name: codetyp mutation; Type: TRIGGER; Schema: bohrung
--

CREATE TRIGGER mutation BEFORE UPDATE ON bohrung.codetyp FOR EACH ROW EXECUTE PROCEDURE bohrung._mutation();


--
-- Name: schicht mutation; Type: TRIGGER; Schema: bohrung
--

CREATE TRIGGER mutation BEFORE UPDATE ON bohrung.schicht FOR EACH ROW EXECUTE PROCEDURE bohrung._mutation();


--
-- Name: standort mutation; Type: TRIGGER; Schema: bohrung
--

CREATE TRIGGER mutation BEFORE UPDATE ON bohrung.standort FOR EACH ROW EXECUTE PROCEDURE bohrung._mutation();


--
-- Name: vorkommnis mutation; Type: TRIGGER; Schema: bohrung
--

CREATE TRIGGER mutation BEFORE UPDATE ON bohrung.vorkommnis FOR EACH ROW EXECUTE PROCEDURE bohrung._mutation();


--
-- Name: bohrprofil fkey_bohrprofil_bohrung_bohrung_id; Type: FK CONSTRAINT; Schema: bohrung
--

ALTER TABLE ONLY bohrung.bohrprofil
   ADD CONSTRAINT fkey_bohrprofil_bohrung_bohrung_id FOREIGN KEY (bohrung_id) REFERENCES bohrung.bohrung(bohrung_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: bohrprofil fkey_bohrprofil_h_fmeto_codetyp; Type: FK CONSTRAINT; Schema: bohrung
--

ALTER TABLE ONLY bohrung.bohrprofil
   ADD CONSTRAINT fkey_bohrprofil_h_fmeto_codetyp FOREIGN KEY (h_fmeto, fmeto) REFERENCES bohrung.code(codetyp_id, code_id) ON UPDATE CASCADE ON DELETE RESTRICT;


--
-- Name: bohrprofil fkey_bohrprofil_h_fmfelso_codetyp; Type: FK CONSTRAINT; Schema: bohrung
--

ALTER TABLE ONLY bohrung.bohrprofil
   ADD CONSTRAINT fkey_bohrprofil_h_fmfelso_codetyp FOREIGN KEY (h_fmfelso, fmfelso) REFERENCES bohrung.code(codetyp_id, code_id) ON UPDATE CASCADE ON DELETE RESTRICT;


--
-- Name: bohrprofil fkey_bohrprofil_h_quali_codetyp; Type: FK CONSTRAINT; Schema: bohrung
--

ALTER TABLE ONLY bohrung.bohrprofil
   ADD CONSTRAINT fkey_bohrprofil_h_quali_codetyp FOREIGN KEY (h_quali, quali) REFERENCES bohrung.code(codetyp_id, code_id) ON UPDATE CASCADE ON DELETE RESTRICT;


--
-- Name: bohrprofil fkey_bohrprofil_h_tektonik_codetyp; Type: FK CONSTRAINT; Schema: bohrung
--

ALTER TABLE ONLY bohrung.bohrprofil
   ADD CONSTRAINT fkey_bohrprofil_h_tektonik_codetyp FOREIGN KEY (h_tektonik, tektonik) REFERENCES bohrung.code(codetyp_id, code_id) ON UPDATE CASCADE ON DELETE RESTRICT;


--
-- Name: bohrprofil fkey_bohrprofil_quali_code; Type: FK CONSTRAINT; Schema: bohrung
--

ALTER TABLE ONLY bohrung.bohrprofil
   ADD CONSTRAINT fkey_bohrprofil_quali_code FOREIGN KEY (quali) REFERENCES bohrung.code(code_id);


--
-- Name: schicht fkey_bohrprofilschichten_bohrprofil_bohrprofilid; Type: FK CONSTRAINT; Schema: bohrung
--

ALTER TABLE ONLY bohrung.schicht
   ADD CONSTRAINT fkey_bohrprofilschichten_bohrprofil_bohrprofilid FOREIGN KEY (bohrprofil_id) REFERENCES bohrung.bohrprofil(bohrprofil_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: bohrung fkey_bohrung_h_ablenkung_codetyp; Type: FK CONSTRAINT; Schema: bohrung
--

ALTER TABLE ONLY bohrung.bohrung
   ADD CONSTRAINT fkey_bohrung_h_ablenkung_codetyp FOREIGN KEY (h_ablenkung, ablenkung) REFERENCES bohrung.code(codetyp_id, code_id) ON UPDATE CASCADE ON DELETE RESTRICT;


--
-- Name: bohrung fkey_bohrung_h_bohrart_codetyp; Type: FK CONSTRAINT; Schema: bohrung
--

ALTER TABLE ONLY bohrung.bohrung
   ADD CONSTRAINT fkey_bohrung_h_bohrart_codetyp FOREIGN KEY (h_bohrart, bohrart) REFERENCES bohrung.code(codetyp_id, code_id) ON UPDATE CASCADE ON DELETE RESTRICT;


--
-- Name: bohrung fkey_bohrung_h_bohrzweck_codetyp; Type: FK CONSTRAINT; Schema: bohrung
--

ALTER TABLE ONLY bohrung.bohrung
   ADD CONSTRAINT fkey_bohrung_h_bohrzweck_codetyp FOREIGN KEY (h_bohrzweck, bohrzweck) REFERENCES bohrung.code(codetyp_id, code_id) ON UPDATE CASCADE ON DELETE RESTRICT;


--
-- Name: bohrung fkey_bohrung_h_quali_codetyp; Type: FK CONSTRAINT; Schema: bohrung
--

ALTER TABLE ONLY bohrung.bohrung
   ADD CONSTRAINT fkey_bohrung_h_quali_codetyp FOREIGN KEY (h_quali, quali) REFERENCES bohrung.code(codetyp_id, code_id) ON UPDATE CASCADE ON DELETE RESTRICT;


--
-- Name: bohrung fkey_bohrung_standort_standort_id; Type: FK CONSTRAINT; Schema: bohrung
--

ALTER TABLE ONLY bohrung.bohrung
   ADD CONSTRAINT fkey_bohrung_standort_standort_id FOREIGN KEY (standort_id) REFERENCES bohrung.standort(standort_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: code fkey_code_codetyp_codetypid; Type: FK CONSTRAINT; Schema: bohrung
--

ALTER TABLE ONLY bohrung.code
   ADD CONSTRAINT fkey_code_codetyp_codetypid FOREIGN KEY (codetyp_id) REFERENCES bohrung.codetyp(codetyp_id) ON UPDATE CASCADE ON DELETE RESTRICT;


--
-- Name: schicht fkey_schicht_codeschicht_codeschicht_id; Type: FK CONSTRAINT; Schema: bohrung
--

ALTER TABLE ONLY bohrung.schicht
   ADD CONSTRAINT fkey_schicht_codeschicht_codeschicht_id FOREIGN KEY (schichten_id) REFERENCES bohrung.codeschicht(codeschicht_id) ON UPDATE CASCADE ON DELETE RESTRICT;


--
-- Name: schicht fkey_schicht_h_quali_codetyp; Type: FK CONSTRAINT; Schema: bohrung
--

ALTER TABLE ONLY bohrung.schicht
   ADD CONSTRAINT fkey_schicht_h_quali_codetyp FOREIGN KEY (h_quali, quali) REFERENCES bohrung.code(codetyp_id, code_id) ON UPDATE CASCADE ON DELETE RESTRICT;


--
-- Name: schicht fkey_schicht_quali_code; Type: FK CONSTRAINT; Schema: bohrung
--

ALTER TABLE ONLY bohrung.schicht
   ADD CONSTRAINT fkey_schicht_quali_code FOREIGN KEY (quali) REFERENCES bohrung.code(code_id);


--
-- Name: standort fkey_standort_h_quali_codetyp; Type: FK CONSTRAINT; Schema: bohrung
--

ALTER TABLE ONLY bohrung.standort
   ADD CONSTRAINT fkey_standort_h_quali_codetyp FOREIGN KEY (h_quali, quali) REFERENCES bohrung.code(codetyp_id, code_id) ON UPDATE CASCADE ON DELETE RESTRICT;


--
-- Name: vorkommnis fkey_vorkommnins_bohrprofil_bohrprofil_id; Type: FK CONSTRAINT; Schema: bohrung
--

ALTER TABLE ONLY bohrung.vorkommnis
   ADD CONSTRAINT fkey_vorkommnins_bohrprofil_bohrprofil_id FOREIGN KEY (bohrprofil_id) REFERENCES bohrung.bohrprofil(bohrprofil_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: vorkommnis fkey_vorkommnis_h_quali_codetyp; Type: FK CONSTRAINT; Schema: bohrung
--

ALTER TABLE ONLY bohrung.vorkommnis
   ADD CONSTRAINT fkey_vorkommnis_h_quali_codetyp FOREIGN KEY (h_quali, quali) REFERENCES bohrung.code(codetyp_id, code_id) ON UPDATE CASCADE ON DELETE RESTRICT;


--
-- Name: vorkommnis fkey_vorkommnis_h_subtyp_codetyp; Type: FK CONSTRAINT; Schema: bohrung
--

ALTER TABLE ONLY bohrung.vorkommnis
   ADD CONSTRAINT fkey_vorkommnis_h_subtyp_codetyp FOREIGN KEY (h_subtyp, subtyp) REFERENCES bohrung.code(codetyp_id, code_id) ON UPDATE CASCADE ON DELETE RESTRICT;


--
-- Name: vorkommnis fkey_vorkommnis_h_typ_codetyp; Type: FK CONSTRAINT; Schema: bohrung
--

ALTER TABLE ONLY bohrung.vorkommnis
   ADD CONSTRAINT fkey_vorkommnis_h_typ_codetyp FOREIGN KEY (h_typ, typ) REFERENCES bohrung.code(codetyp_id, code_id) ON UPDATE CASCADE ON DELETE RESTRICT;

