using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EWS.Models
{
    /// <summary>
    /// Repräsentiert einen Standort in der Datenbank.
    /// </summary>
    [Table("standort")]
    public class Standort : IDateUserSettable
    {
        /// <summary>
        /// Die Id des Standorts.
        /// </summary>
        [Key]
        [Column("standort_id")]
        public int Id { get; set; }

        /// <summary>
        /// Bezeichnung des Standorts.
        /// </summary>
        [Column("bezeichnung")]
        public string Bezeichnung { get; set; }

        /// <summary>
        /// Bemerkung zum Standort.
        /// </summary>
        [Column("bemerkung")]
        public string? Bemerkung { get; set; }

        /// <summary>
        /// Gemeindenummer.
        /// </summary>
        [Column("gemeinde")]
        public int? Gemeinde { get; set; }

        /// <summary>
        /// Grundbuchnummer.
        /// </summary>
        [Column("gbnummer")]
        public string? GrundbuchNr { get; set; }

        /// <summary>
        /// Freigabe durch das AfU.
        /// </summary>
        [Column("freigabe_afu")]
        public bool FreigabeAfu { get; set; }

        /// <summary>
        /// Kürzel des AfU-Benutzers bei der Freigabe des Objekts.
        /// </summary>
        [Column("afu_usr")]
        public string? AfuUser { get; set; }

        /// <summary>
        /// Datum der Freigabe des Objekts durch das Afu.
        /// </summary>
        [Column("afu_date")]
        public DateTime? AfuDatum { get; set; }

        /// <summary>
        /// Datum des Imports des Objektes.
        /// </summary>
        [Column("new_date")]
        public DateTime? Erstellungsdatum { get; set; }

        /// <summary>
        /// Timestamp der letzten Änderung.
        /// </summary>
        [Column("mut_date")]
        public DateTime? Mutationsdatum { get; set; }

        /// <summary>
        /// Kürzel des Benutzers beim Anlegen des Objekts.
        /// </summary>
        [Column("new_usr")]
        public string? UserErstellung { get; set; }

        /// <summary>
        /// Kürzel des Benutzers bei letzter Änderung.
        /// </summary>
        [Column("mut_usr")]
        public string? UserMutation { get; set; }

        /// <summary>
        /// Bohrungen die dem Standort zugeordnet sind.
        /// </summary>
        public List<Bohrung> Bohrungen { get; set; }
    }
}
