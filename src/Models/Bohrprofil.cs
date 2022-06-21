using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EWS.Models
{
    /// <summary>
    /// Repräsentiert ein Bohrprofil in der Datenbank.
    /// </summary>
    [Table("bohrprofil")]
    public class Bohrprofil
    {
        /// <summary>
        /// Die Id des Bohrprofils.
        /// </summary>
        [Key]
        [Column("bohrprofil_id")]
        public int Id { get; set; }

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
        /// Qualität der Angaben zum Bohrprofil.
        /// </summary>
        [Column("quali")]
        public int? Qualitaet { get; set; }

        /// <summary>
        /// Bemerkung zur Qualitätsangabe.
        /// </summary>
        [Column("qualibem")]
        public string? QualitaetBemerkung { get; set; }

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
    }
}
