using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EWS.Models
{
    /// <summary>
    /// Repräsentiert ein Vorkommnis in der Datenbank.
    /// </summary>
    [Table("vorkommnis")]
    public class Vorkommnis
    {
        /// <summary>
        /// Die Id des Vorkomnisses.
        /// </summary>
        [Key]
        [Column("vorkommnis_id")]
        public int Id { get; set; }

        /// <summary>
        /// Foreign Key: ID der Tabelle Bohrprofil.
        /// </summary>
        [Column("bohrprofil_id")]
        public int BohrprofilId { get; set; }

        /// <summary>
        /// Art des Vorkommnisses, z.B. Arteser.
        /// </summary>
        [Column("typ")]
        public int Typ { get; set; }

        /// <summary>
        /// Tiefe des Vorkommnisses [m].
        /// </summary>
        [Column("tiefe")]
        public float? Tiefe { get; set; }

        /// <summary>
        /// Bemerkung zum Vorkommnis.
        /// </summary>
        [Column("bemerkung")]
        public string? Bemerkung { get; set; }

        /// <summary>
        /// Qualitätsangabe zum Vorkommnis.
        /// </summary>
        [Column("quali")]
        public int? Qualitaet { get; set; }

        /// <summary>
        /// Bemerkung zur Qualitätsangabe.
        /// </summary>
        [Column("qualibem")]
        public string? QualitaetBemerkung { get; set; }

        /// <summary>
        /// Foreign Key: ID des Codetyps für Feld quali.
        /// </summary>
        [Column("h_quali")]
        public int HQualitaet { get; set; }

        /// <summary>
        /// Foreign Key: ID des Codetyps für Feld typ.
        /// </summary>
        [Column("h_typ")]
        public int HTyp { get; set; }

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
