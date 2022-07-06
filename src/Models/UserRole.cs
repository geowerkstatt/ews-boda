namespace EWS.Models
{
    /// <summary>
    /// Repräsentiert eine Benutzer-Rolle.
    /// </summary>
    public enum UserRole
    {
        /*
         * Wichtig:
         * Da in der Datenbank nur die Ids (int) der Benutzer-Rollen gespeichert werden,
         * müssen die Werte für die Rollen stabil bleiben. Allfällige neue Rollen
         * erhalten neue Ids.
         */

        /// <summary>
        /// Standard Benutzer-Rolle.
        /// Benutzer mit der Rolle <see cref="Extern"/> dürfen Standorte
        /// erstellen und mutieren solange diese noch nicht freigegeben sind.
        /// </summary>
        /// <seealso cref="Standort"/>
        Extern = 1000,

        /// <summary>
        /// Sachberarbeiter AfU.
        /// Zusätzlich zur Rolle <see cref="Extern"/> dürfen Benutzer mit der Rolle
        /// <see cref="SachbearbeiterAfU"/> Standorte freigeben und löschen.
        /// </summary>
        /// <seealso cref="Standort"/>
        SachbearbeiterAfU = 1001,

        /// <summary>
        /// Administrator.
        /// Zusätzlich zur Rolle <see cref="SachbearbeiterAfU"/> dürfen Benutzer mit der Rolle
        /// <see cref="Administrator"/> Benutzer beliebigen Rollen zuweisen.
        /// </summary>
        /// <seealso cref="User"/>
        /// <seealso cref="UserRole"/>
        Administrator = 0,
    }
}
