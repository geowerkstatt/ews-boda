using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EWS.Models
{
    /// <summary>
    /// Repräsentiert ein Bohrprofil in der Datenbank.
    /// </summary>
    [Table("bohrprofil")]
    public class Bohrprofil : EwsModelBase
    {
        /// <summary>
        /// Die Id des Bohrprofils.
        /// </summary>
        [Key]
        [Column("bohrprofil_id")]
        public override int Id { get; set; }

        /// <summary>
        /// Foreign Key: ID der Tabelle Bohrung.
        /// </summary>
        [Column("bohrung_id")]
        public int? BohrungId { get; set; }

        /// <summary>
        /// Datum des Bohrprofils.
        /// </summary>
        [Column("datum")]
        public DateTime? Datum { get; set; }

        /// <summary>
        /// Bemerkung zum Bohrprofil.
        /// </summary>
        [Column("bemerkung")]
        public string? Bemerkung { get; set; }

        /// <summary>
        ///  Terrainkote der Bohrung [m].
        /// </summary>
        [Column("kote")]
        public int? Kote { get; set; }

        /// <summary>
        /// Endtiefe der Bohrung [m].
        /// </summary>
        [Column("endteufe")]
        public int? Endteufe { get; set; }

        /// <summary>
        /// Klassierung Tektonik.
        /// </summary>
        [Column("tektonik")]
        public int? Tektonik { get; set; }

        /// <summary>
        /// Formation Fels.
        /// </summary>
        [Column("fmfelso")]
        public int? FormationFels { get; set; }

        /// <summary>
        /// Formation Endtiefe.
        /// </summary>
        [Column("fmeto")]
        public int? FormationEndtiefe { get; set; }

        /// <summary>
        /// QualitätcodeId des Bohrprofils.
        /// </summary>
        [Column("quali")]
        public int? QualitaetId { get; set; }

        /// <summary>
        /// Qualitätscode des Bohrprofils.
        /// </summary>
        [Column("quali")]
        public Code? Qualitaet { get; set; }

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
        /// Foreign Key: ID des Codetyps für Feld tektonik.
        /// </summary>
        [Column("h_tektonik")]
        public int HTektonik { get; set; }

        /// <summary>
        /// Foreign Key: ID des Codetyps für Feld fmfelso.
        /// </summary>
        [Column("h_fmfelso")]
        public int HFormationFels { get; set; }

        /// <summary>
        /// Foreign Key: ID des Codetyps für Feld fmeto.
        /// </summary>
        [Column("h_fmeto")]
        public int HFormationEndtiefe { get; set; }

        /// <summary>
        /// Schichten die dem Bohrprofil zugeordnet sind.
        /// </summary>
        public List<Schicht> Schichten { get; set; }

        /// <summary>
        /// Vorkommnisse die dem Bohrprofil zugeordnet sind.
        /// </summary>
        public List<Vorkommnis> Vorkomnisse { get; set; }
    }
}
