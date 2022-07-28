using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EWS.Models;

/// <summary>
/// Repräsentiert einen Standort in der Datenbank.
/// </summary>
[Table("standort")]
public class Standort : EwsModelBase
{
    /// <summary>
    /// Die Id des Standorts.
    /// </summary>
    [Key]
    [Column("standort_id")]
    public override int Id { get; set; }

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
    public string? Gemeinde { get; set; }

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
    /// Bohrungen die dem Standort zugeordnet sind.
    /// </summary>
    public List<Bohrung>? Bohrungen { get; set; }
}
