using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EWS.Models
{
    /// <summary>
    /// Repräsentiert eine Bohrung in der Datenbank.
    /// </summary>
    [Table("bohrung")]
    public class Bohrung : EwsModelBase
    {
        /// <summary>
        /// Die Id der Bohrung.
        /// </summary>
        [Key]
        [Column("bohrung_id")]
        public override int Id { get; set; }

        /// <summary>
        /// Foreign Key: ID der Tabelle Standort.
        /// </summary>
        [Column("standort_id")]
        public int StandortId { get; set; }

        /// <summary>
        /// Bezeichnung der Bohrung.
        /// </summary>
        [Column("bezeichnung")]
        public string Bezeichnung { get; set; }

        /// <summary>
        /// Datum der Bohrung.
        /// </summary>
        [Column("datum")]
        public DateTime? Datum { get; set; }

        /// <summary>
        /// Bemerkungen zur Bohrung.
        /// </summary>
        [Column("bemerkung")]
        public string? Bemerkung { get; set; }

        /// <summary>
        /// Foreign Key: ID Klassierung der Ablenkung.
        /// </summary>
        [Column("ablenkung")]
        public int? AblenkungId { get; set; }

        /// <summary>
        /// Codetyp Klassierung der Ablenkung.
        /// </summary>
        [ForeignKey("AblenkungId")]
        public Code? Ablenkung { get; set; }

        /// <summary>
        /// Durchmesser der Bohrlöcher [mm].
        /// </summary>
        [Column("durchmesserbohrloch")]
        public int? DurchmesserBohrloch { get; set; }

        /// <summary>
        /// Qualität der Angaben zur Bohrung.
        /// </summary>
        [Column("quali")]
        public int? QualitaetId { get; set; }

        /// <summary>
        /// Codetyp Klassierung der Ablenkung.
        /// </summary>
        [ForeignKey("QualitaetId")]
        public Code? Qualitaet { get; set; }

        /// <summary>
        /// Bemerkung zur Qualitätsangabe.
        /// </summary>
        [Column("qualibem")]
        public string? QualitaetBemerkung { get; set; }

        /// <summary>
        /// Autor geologische Aufnahme (Firma, Bearbeiter, Jahr).
        /// </summary>
        [Column("quelleref")]
        public string? QuelleRef { get; set; }

        /// <summary>
        /// Foreign Key: ID des Codetyps für Feld quali.
        /// </summary>
        [Column("h_quali")]
        public int HQualitaet { get; set; }

        /// <summary>
        /// Foreign Key: ID des Codetyps für Feld ablenkung.
        /// </summary>
        [Column("h_ablenkung")]
        public int HAblenkung { get; set; }

        /// <summary>
        /// Koordinate der Bohrung.
        /// </summary>
        [Column("wkb_geometry")]
        public Point? Geometrie { get; set; }

        /// <summary>
        /// Bohrprofile die der Bohrung zugeordnet sind.
        /// </summary>
        public List<Bohrprofil> Bohrprofile { get; set; }
    }
}
