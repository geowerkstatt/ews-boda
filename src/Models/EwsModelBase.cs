using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EWS.Models
{
    /// <summary>
    /// Basisklasse für EWS Bohrdaten Datenbank-Objekte.
    /// </summary>
    public abstract class EwsModelBase : IEwsModelBase, IDateUserSettable
    {
        /// <summary>
        /// Die Id des Vorkomnisses.
        /// </summary>
        [Key]
        public abstract int Id { get; set; }

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
    }
}
