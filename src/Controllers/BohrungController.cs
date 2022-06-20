using EWS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;

namespace EWS;

[ApiController]
[Route("[controller]")]
public class BohrungController : ControllerBase
{
    private readonly EwsContext context;
    public BohrungController(EwsContext context)
    {
        this.context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BohrungDTO>>> GetAsync()
    {
        return await context.Bohrungen.Select(x => BohrungToDTO(x)).ToListAsync().ConfigureAwait(false);
    }

    private static BohrungDTO BohrungToDTO(Bohrung bohrung)
    {
        return new BohrungDTO
        {
            Id = bohrung.Id,
            StandortId = bohrung.StandortId,
            Bezeichnung = bohrung.Bezeichnung,
            Bemerkung = bohrung.Bemerkung,
            Ablenkung = bohrung.Ablenkung,
            Datum = bohrung.Datum,
            DurchmesserBohrloch = bohrung.DurchmesserBohrloch,
            Erstellungsdatum = bohrung.Erstellungsdatum,
            UserErstellung = bohrung.UserErstellung,
            HAblenkung = bohrung.HAblenkung,
            HQualitaet = bohrung.HQualitaet,
            Mutationsdatum = bohrung.Mutationsdatum,
            Qualitaet = bohrung.Qualitaet,
            QualitaetBemerkung = bohrung.QualitaetBemerkung,
            QuelleRef = bohrung.QuelleRef,
            UserMutation = bohrung.UserMutation,
            Geometrie = new Point((int)bohrung.Geometrie.X, (int)bohrung.Geometrie.Y),
        };
    }
}
