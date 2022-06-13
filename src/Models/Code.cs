using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EWS.Models
{
    /// <summary>
    /// Repräsentiert einen Code in der Datenbank.
    /// </summary>
    [Table("code")]
    public class Code
    {
        /// <summary>
        /// Die Id des Codes.
        /// </summary>
        [Key]
        [Column("code_id")]
        public int Id { get; set; }

        /// <summary>
        /// Foreign Key: ID der Tabelle Codetyp.
        /// </summary>
        [Column("codetyp_id")]
        public int CodetypId { get; set; }

        /// <summary>
        /// Kurzbezeichnung des Codes.
        /// </summary>
        [Column("kurztext")]
        public string Kurztext { get; set; }

        /// <summary>
        /// Ausführliche Bezeichnung des Codes.
        /// </summary>
        [Column("text")]
        public string Text { get; set; }

        /// <summary>
        /// Vermutliche Reihenfolge von Codes eines Codetypen.
        /// </summary>
        [Column("sort")]
        public int Sortierung { get; set; }

        /// <summary>
        /// Datum des Imports des Objektes.
        /// </summary>
        [Column("new_date")]
        public DateTime Erstellungsdatum { get; set; }

        /// <summary>
        /// Timestamp der letzten Änderung.
        /// </summary>
        [Column("mut_date")]
        public DateTime Mutationsdatum { get; set; }

        /// <summary>
        /// Kürzel des Benutzers beim Anlegen des Objekts.
        /// </summary>
        [Column("new_usr")]
        public string UserErstellung { get; set; }

        /// <summary>
        /// Kürzel des Benutzers bei letzter Änderung.
        /// </summary>
        [Column("mut_usr")]
        public string UserMutation { get; set; }
    }
}
