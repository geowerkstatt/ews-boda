using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EWS.Models
{
    /// <summary>
    /// Repräsentiert einen Codetyp in der Datenbank.
    /// </summary>
    [Table("codetyp")]
    public class CodeTyp
    {
        /// <summary>
        /// Die Id des Codetyps.
        /// </summary>
        [Key]
        [Column("codetyp_id")]
        public int Id { get; set; }

        /// <summary>
        /// Kurzbezeichnung des Codetyps.
        /// </summary>
        [Column("kurztext")]
        public string Kurztext { get; set; }

        /// <summary>
        /// Ausführliche Bezeichnung des Codetyps.
        /// </summary>
        [Column("text")]
        public string Text { get; set; }

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
