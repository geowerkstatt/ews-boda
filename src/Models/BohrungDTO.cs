using System.Drawing;

namespace EWS.Models
{
    public class BohrungDTO
    {
        /// <summary>
        /// Die Id der Bohrung.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Foreign Key: ID der Tabelle Standort.
        /// </summary>
        public int StandortId { get; set; }

        /// <summary>
        /// Bezeichnung der Bohrung.
        /// </summary>
        public string Bezeichnung { get; set; }

        /// <summary>
        /// Datum der Bohrung.
        /// </summary>
        public DateTime Datum { get; set; }

        /// <summary>
        /// Bemerkungen zur Bohrung.
        /// </summary>
        public string? Bemerkung { get; set; }

        /// <summary>
        /// Klassierung der Ablenkung.
        /// </summary>
        public int Ablenkung { get; set; }

        /// <summary>
        /// Durchmesser der Bohrlöcher [mm].
        /// </summary>
        public int DurchmesserBohrloch { get; set; }

        /// <summary>
        /// Qualität der Angaben zur Bohrung.
        /// </summary>
        public int Qualitaet { get; set; }

        /// <summary>
        /// Bemerkung zur Qualitätsangabe.
        /// </summary>
        public string QualitaetBemerkung { get; set; }

        /// <summary>
        /// Autor geologische Aufnahme (Firma, Bearbeiter, Jahr).
        /// </summary>
        public string QuelleRef { get; set; }

        /// <summary>
        /// Datum des Imports des Objektes.
        /// </summary>
        public DateTime Erstellungsdatum { get; set; }

        /// <summary>
        /// Timestamp der letzten Änderung.
        /// </summary>
        public DateTime Mutationsdatum { get; set; }

        /// <summary>
        /// Kürzel des Benutzers beim Anlegen des Objekts.
        /// </summary>
        public string UserErstellung { get; set; }

        /// <summary>
        /// Kürzel des Benutzers bei letzter Änderung.
        /// </summary>
        public string UserMutation { get; set; }

        /// <summary>
        /// Foreign Key: ID des Codetyps für Feld quali.
        /// </summary>
        public int HQualitaet { get; set; }

        /// <summary>
        /// Foreign Key: ID des Codetyps für Feld ablenkung.
        /// </summary>
        public int HAblenkung { get; set; }

        /// <summary>
        /// Koordinate der Bohrung.
        /// </summary>
        public Point Geometrie { get; set; }
    }
}
