-- Add user table

\connect ews

DROP TABLE IF EXISTS bohrung."user";

CREATE TABLE bohrung.user (
    user_id integer NOT NULL,
    user_name text NOT NULL,
    user_role integer NOT NULL,
    new_date timestamp without time zone DEFAULT now() NOT NULL,
    mut_date timestamp without time zone,
    new_usr character varying DEFAULT "current_user"() NOT NULL,
    mut_usr character varying
);

CREATE SEQUENCE bohrung.user_user_id_seq
    START WITH 100
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;

ALTER SEQUENCE bohrung.user_user_id_seq OWNED BY bohrung.user.user_id;
ALTER TABLE ONLY bohrung.user ALTER COLUMN user_id SET DEFAULT nextval('bohrung.user_user_id_seq'::regclass);
ALTER TABLE ONLY bohrung.user ADD CONSTRAINT pkey_user_user_id PRIMARY KEY (user_id);
