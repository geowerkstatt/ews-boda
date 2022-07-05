namespace EWS.Models
{
    public interface IDateUserSettable
    {
        /// <summary>
        /// Datum der Erstellung.
        /// </summary>
        DateTime? Erstellungsdatum { get; set; }

        /// <summary>
        /// Timestamp der letzten Änderung.
        /// </summary>
        DateTime? Mutationsdatum { get; set; }

        /// <summary>
        /// Kürzel des Benutzers beim Anlegen des Objekts.
        /// </summary>
        string? UserErstellung { get; set; }

        /// <summary>
        /// Kürzel des Benutzers bei letzter Änderung.
        /// </summary>
        string? UserMutation { get; set; }
    }
}
