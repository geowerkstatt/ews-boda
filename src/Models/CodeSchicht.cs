using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EWS.Models
{
    /// <summary>
    /// Repräsentiert eine Codeschicht in der Datenbank.
    /// </summary>
    [Table("codeschicht")]
    public class CodeSchicht
    {
        /// <summary>
        /// Die Id der Codeschicht.
        /// </summary>
        [Key]
        [Column("codeschicht_id")]
        public int Id { get; set; }

        /// <summary>
        /// Kurzbezeichnung der Codeschicht.
        /// </summary>
        [Column("kurztext")]
        public string Kurztext { get; set; }

        /// <summary>
        /// Ausführliche Bezeichnung der Codeschicht.
        /// </summary>
        [Column("text")]
        public string Text { get; set; }

        /// <summary>
        /// Vorgabe für Reihenfolge der Schichten bei Erfassung in Tabelle schicht.
        /// </summary>
        [Column("sort")]
        public int? Sortierung { get; set; }

        /// <summary>
        /// Datum des Imports des Objektes.
        /// </summary>
        [Column("new_date")]
        public DateTime Erstellungsdatum { get; set; }

        /// <summary>
        /// Timestamp der letzten Änderung.
        /// </summary>
        [Column("mut_date")]
        public DateTime? Mutationsdatum { get; set; }

        /// <summary>
        /// Kürzel des Benutzers beim Anlegen des Objekts.
        /// </summary>
        [Column("new_usr")]
        public string UserErstellung { get; set; }

        /// <summary>
        /// Kürzel des Benutzers bei letzter Änderung.
        /// </summary>
        [Column("mut_usr")]
        public string? UserMutation { get; set; }
    }
}
