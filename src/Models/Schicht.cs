using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EWS.Models
{
    /// <summary>
    /// Repräsentiert eine Schicht in der Datenbank.
    /// </summary>
    [Table("schicht")]
    public class Schicht
    {
        /// <summary>
        /// Die Id der Schicht.
        /// </summary>
        [Key]
        [Column("schicht_id")]
        public int Id { get; set; }

        /// <summary>
        /// Foreign Key: ID der Tabelle Bohrprofil.
        /// </summary>
        [Column("bohrprofil_id")]
        public int BohrprofilId { get; set; }

        /// <summary>
        /// Foreign Key: ID der Tabelle Schicht.
        /// </summary>
        [Column("schichten_id")]
        public int SchichtenId { get; set; }

        /// <summary>
        /// Tiefe der Schichtgrenze [m].
        /// </summary>
        [Column("tiefe")]
        public float Tiefe { get; set; }

        /// <summary>
        /// Qualitätsangabe zur Schicht.
        /// </summary>
        [Column("quali")]
        public int Qualitaet { get; set; }

        /// <summary>
        /// Bemerkung zur Qualitätsangabe.
        /// </summary>
        [Column("qualibem")]
        public string? QualitaetBemerkung { get; set; }

        /// <summary>
        /// Bemerkung zur Schicht.
        /// </summary>
        [Column("bemerkung")]
        public string? Bemerkung { get; set; }

        /// <summary>
        /// Foreign Key: ID des Codetyps für Feld quali.
        /// </summary>
        [Column("h_quali")]
        public int HQualitaet { get; set; }

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
