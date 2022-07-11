using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EWS.Models
{
    /// <summary>
    /// Repräsentiert einen Benutzer in der Datenbank.
    /// </summary>
    [Table("user")]
    public class User
    {
        /// <summary>
        /// Die Id des Benutzers.
        /// </summary>
        [Key]
        [Column("user_id")]
        public int Id { get; set; }

        /// <summary>
        /// Der Name des Benutzers. Entspricht dem Claim `name` im JSON Web Token.
        /// </summary>
        [Column("user_name")]
        public string Name { get; set; }

        /// <summary>
        /// Bezeichnung der Benutzer-Rolle
        /// </summary>
        /// <seealso cref="UserRole"/>
        [Column("user_role")]
        public UserRole Role { get; set; }

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
