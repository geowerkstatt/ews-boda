using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EWS.Models;

/// <summary>
/// Repräsentiert einen Code in der Datenbank.
/// </summary>
[Table("code")]
public class Code : EwsModelBase
{
    /// <summary>
    /// Die Id des Codes.
    /// </summary>
    [Key]
    [Column("code_id")]
    public override int Id { get; set; }

    /// <summary>
    /// Foreign Key: ID der Tabelle Codetyp.
    /// </summary>
    [Column("codetyp_id")]
    public int CodetypId { get; set; }

    /// <summary>
    /// Codetyp des Codes.
    /// </summary>
    [ForeignKey("CodetypId")]
    public CodeTyp? Codetyp { get; set; }

    /// <summary>
    /// Kurzbezeichnung des Codes.
    /// </summary>
    [Column("kurztext")]
    public string Kurztext { get; set; }

    /// <summary>
    /// Ausführliche Bezeichnung des Codes.
    /// </summary>
    [Column("text")]
    public string? Text { get; set; }

    /// <summary>
    /// Vermutliche Reihenfolge von Codes eines Codetypen.
    /// </summary>
    [Column("sort")]
    public int? Sortierung { get; set; }
}
