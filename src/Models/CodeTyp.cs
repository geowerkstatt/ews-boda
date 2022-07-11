using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EWS.Models
{
    /// <summary>
    /// Repräsentiert einen Codetyp in der Datenbank.
    /// </summary>
    [Table("codetyp")]
    public class CodeTyp : EwsModelBase
    {
        /// <summary>
        /// Die Id des Codetyps.
        /// </summary>
        [Key]
        [Column("codetyp_id")]
        public override int Id { get; set; }

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
    }
}
