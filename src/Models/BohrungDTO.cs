using System.Drawing;

namespace EWS.Models
{
    public class BohrungDTO
    {
        public int Id { get; set; }

        public int StandortId { get; set; }

        public string Bezeichnung { get; set; }

        public Point Geometrie { get; set; }
    }
}
