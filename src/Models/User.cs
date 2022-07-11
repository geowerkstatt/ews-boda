using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EWS.Models
{
    /// <summary>
    /// Repräsentiert einen Benutzer in der Datenbank.
    /// </summary>
    [Table("user")]
    public class User : EwsModelBase
    {
        /// <summary>
        /// Die Id des Benutzers.
        /// </summary>
        [Key]
        [Column("user_id")]
        public override int Id { get; set; }

        /// <summary>
        /// Der Name des Benutzers. Entspricht dem Claim `name` im JSON Web Token.
        /// </summary>
        [Column("user_name")]
        public string Name { get; set; }

        /// <summary>
        /// Bezeichnung der dem Benutzer zugeordneten Benutzer-Rolle.
        /// </summary>
        /// <seealso cref="UserRole"/>
        [Column("user_role")]
        public UserRole Role { get; set; }
    }
}
