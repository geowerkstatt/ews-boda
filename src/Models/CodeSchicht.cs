using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EWS.Models
{
    /// <summary>
    /// Repräsentiert eine Codeschicht in der Datenbank.
    /// </summary>
    [Table("codeschicht")]
    public class CodeSchicht : EwsModelBase
    {
        /// <summary>
        /// Die Id der Codeschicht.
        /// </summary>
        [Key]
        [Column("codeschicht_id")]
        public override int Id { get; set; }

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
    }
}
