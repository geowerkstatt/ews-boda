using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EWS.Models
{
    /// <summary>
    /// Repräsentiert eine Schicht in der Datenbank.
    /// </summary>
    [Table("schicht")]
    public class Schicht : EwsModelBase
    {
        /// <summary>
        /// Die Id der Schicht.
        /// </summary>
        [Key]
        [Column("schicht_id")]
        public override int Id { get; set; }

        /// <summary>
        /// Foreign Key: ID der Tabelle Bohrprofil.
        /// </summary>
        [Column("bohrprofil_id")]
        public int BohrprofilId { get; set; }

        /// <summary>
        /// Foreign Key: ID der Tabelle Codeschicht.
        /// </summary>
        [Column("schichten_id")]
        public int CodeSchichtId { get; set; }

        /// <summary>
        /// Codeschicht der Schicht.
        /// </summary>
        public CodeSchicht CodeSchicht { get; set; }

        /// <summary>
        /// Tiefe der Schichtgrenze [m].
        /// </summary>
        [Column("tiefe")]
        public float Tiefe { get; set; }

        /// <summary>
        /// Foreign Key: ID Qualitätsangabe zur Schicht.
        /// </summary>
        [Column("quali")]
        public int QualitaetId { get; set; }

        /// <summary>
        /// Qualitätsangabe zur Schicht.
        /// </summary>
        public Code Qualitaet { get; set; }

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
        /// Codetyp für Feld quali.
        /// </summary>
        [Column("h_quali")]
        public int HQualitaet { get; set; }
    }
}
