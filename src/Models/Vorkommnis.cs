using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EWS.Models;

/// <summary>
/// Repräsentiert ein Vorkommnis in der Datenbank.
/// </summary>
[Table("vorkommnis")]
public class Vorkommnis : EwsModelBase
{
    /// <summary>
    /// Die Id des Vorkomnisses.
    /// </summary>
    [Key]
    [Column("vorkommnis_id")]
    public override int Id { get; set; }

    /// <summary>
    /// Foreign Key: ID der Tabelle Bohrprofil.
    /// </summary>
    [Column("bohrprofil_id")]
    public int BohrprofilId { get; set; }

    /// <summary>
    /// Art des Vorkommnisses, z.B. Arteser.
    /// </summary>
    [Column("typ")]
    public int TypId { get; set; }

    /// <summary>
    /// Codetyp Klassierung der Formation Endtiefe.
    /// </summary>
    [ForeignKey("TypId")]
    public Code? Typ { get; set; }

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
    /// Codetyp für Feld quali.
    /// </summary>
    [Column("h_quali")]
    public int HQualitaet { get; set; }

    /// <summary>
    /// Codetyp für Feld typ.
    /// </summary>
    [Column("h_typ")]
    public int HTyp { get; set; }
}
