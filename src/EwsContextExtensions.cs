using Bogus;
using EWS.Models;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using System.Globalization;

namespace EWS;

public static class EwsContextExtensions
{
    private static readonly string[] gemeinden = new[] { "Egerkingen", "Härkingen", "Kestenholz", "Neuendorf", "Niederbuchsiten", "Oberbuchsiten", "Oensingen", "Wolfwil", "Aedermannsdorf", "Balsthal", "Gänsbrunnen", "Herbetswil", "Holderbank (SO)", "Laupersdorf", "Matzendorf", "Mümliswil-Ramiswil", "Welschenrohr", "Aetigkofen", "Aetingen", "Balm bei Messen", "Bibern (SO)", "Biezwil", "Brügglen", "Brunnenthal", "Gossliwil", "Hessigkofen", "Küttigkofen", "Kyburg-Buchegg", "Lüsslingen", "Lüterkofen-Ichertswil", "Lüterswil-Gächliwil", "Messen", "Mühledorf (SO)", "Nennigkofen", "Oberramsern", "Schnottwil", "Tscheppach", "Unterramsern", "Bättwil", "Büren (SO)", "Dornach", "Gempen", "Hochwald", "Hofstetten-Flüh", "Metzerlen-Mariastein", "Nuglar-St. Pantaleon", "Rodersdorf", "Seewen", "Witterswil", "Hauenstein-Ifenthal", "Kienberg", "Lostorf", "Niedererlinsbach", "Niedergösgen", "Obererlinsbach", "Obergösgen", "Rohr (SO)", "Stüsslingen", "Trimbach", "Winznau", "Wisen (SO)", "Aeschi (SO)", "Biberist", "Bolken", "Deitingen", "Derendingen", "Etziken", "Gerlafingen", "Halten", "Heinrichswil-Winistorf", "Hersiwil", "Horriwil", "Hüniken", "Kriegstetten", "Lohn-Ammannsegg", "Luterbach", "Obergerlafingen", "Oekingen", "Recherswil", "Steinhof", "Subingen", "Zuchwil", "Balm bei Günsberg", "Bellach", "Bettlach", "Feldbrunnen-St. Niklaus", "Flumenthal", "Grenchen", "Günsberg", "Hubersdorf", "Kammersrohr", "Langendorf", "Lommiswil", "Niederwil (SO)", "Oberdorf (SO)", "Riedholz", "Rüttenen", "Selzach", "Boningen", "Däniken", "Dulliken", "Eppenberg-Wöschnau", "Fulenbach", "Gretzenbach", "Gunzgen", "Hägendorf", "Kappel (SO)", "Olten", "Rickenbach (SO)", "Schönenwerd", "Starrkirch-Wil", "Walterswil (SO)", "Wangen bei Olten", "Solothurn", "Bärschwil", "Beinwil (SO)", "Breitenbach", "Büsserach", "Erschwil", "Fehren", "Grindel", "Himmelried", "Kleinlützel", "Meltingen", "Nunningen", "Zullwil" };

    /// <summary>
    /// Seed data for <see cref="CodeTyp"/>, <see cref="Code"/>,
    /// <see cref="CodeSchicht"/>, <see cref="Schicht"/>, <see cref="Standort"/>,
    /// <see cref="Bohrung"/>, <see cref="Bohrprofil"/> and <see cref="Vorkommnis"/>.
    /// </summary>
    public static void SeedData(this EwsContext context)
    {
        using var transaction = context.Database.BeginTransaction();

        // Set Bogus Data System Clock
        Bogus.DataSets.Date.SystemClock = () => DateTime.Parse("01.01.2022 00:00:00", new CultureInfo("de_CH", false));

        // Seed CodeTypen
        var codetyp_ids = 1;
        var codetypenRange = Enumerable.Range(codetyp_ids, 20);
        var fakeCodeTypen = new Faker<CodeTyp>()
           .StrictMode(true)
           .RuleFor(o => o.Id, f => codetyp_ids++)
           .RuleFor(o => o.Text, f => f.Random.Words())
           .RuleFor(o => o.Kurztext, f => f.Address.StreetName())
           .RuleFor(o => o.Erstellungsdatum, f => f.Date.Past())
           .RuleFor(o => o.Mutationsdatum, f => f.Date.Past())
           .RuleFor(o => o.UserErstellung, f => f.Person.UserName)
           .RuleFor(o => o.UserMutation, f => f.Internet.UserName().OrNull(f, .8f));
        CodeTyp SeededCodetypen(int seed) => fakeCodeTypen.UseSeed(seed).Generate();
        context.CodeTypen.AddRange(codetypenRange.Select(SeededCodetypen));
        context.SaveChangesWithoutUpdatingChangeInformation();

        // Seed Codes
        var code_ids = 1;
        var codesRange = Enumerable.Range(code_ids, 400);
        var fakeCodes = new Faker<Code>()
           .StrictMode(true)
           .RuleFor(o => o.Id, f => code_ids++)
           .RuleFor(o => o.CodetypId, f => f.PickRandom(codetypenRange))
           .RuleFor(o => o.Text, f => f.Commerce.ProductName())
           .RuleFor(o => o.Kurztext, f => f.Address.City())
           .RuleFor(o => o.Sortierung, f => f.Random.Int(0, 30000))
           .RuleFor(o => o.Erstellungsdatum, f => f.Date.Past())
           .RuleFor(o => o.Mutationsdatum, f => f.Date.Past())
           .RuleFor(o => o.UserErstellung, f => f.Person.UserName)
           .RuleFor(o => o.UserMutation, f => f.Internet.UserName().OrNull(f, .6f))
           .RuleFor(o => o.Codetyp, _ => default!);

        Code SeededCodes(int seed) => fakeCodes.UseSeed(seed).Generate();
        context.Codes.AddRange(codesRange.Select(SeededCodes));
        context.SaveChangesWithoutUpdatingChangeInformation();

        // store generated codes in variable for later use
        var codesToAdd = context.Codes.ToList();

        // Seed CodeSchichten
        var codeschicht_ids = 20001;
        var codeschichtenRange = Enumerable.Range(codeschicht_ids, 100);
        var fakeCodeschichten = new Faker<CodeSchicht>()
           .StrictMode(true)
           .RuleFor(o => o.Id, f => codeschicht_ids++)
           .RuleFor(o => o.Text, f => f.Internet.UrlWithPath())
           .RuleFor(o => o.Kurztext, f => f.Random.Words())
           .RuleFor(o => o.Sortierung, f => f.Random.Int(0, 30000))
           .RuleFor(o => o.Erstellungsdatum, f => f.Date.Past())
           .RuleFor(o => o.Mutationsdatum, f => f.Date.Past())
           .RuleFor(o => o.UserErstellung, f => f.Person.UserName)
           .RuleFor(o => o.UserMutation, f => f.Internet.UserName());
        CodeSchicht SeededCodeschichten(int seed) => fakeCodeschichten.UseSeed(seed).Generate();
        context.CodeSchichten.AddRange(codeschichtenRange.Select(SeededCodeschichten));
        context.SaveChangesWithoutUpdatingChangeInformation();

        // Seed Standorte
        var standort_ids = 30001;
        var standorteRange = Enumerable.Range(standort_ids, 6000);
        var fakeStandorte = new Faker<Standort>()
           .StrictMode(true)
           .RuleFor(o => o.Id, f => standort_ids++)
           .RuleFor(o => o.Bezeichnung, f => f.Commerce.ProductName())
           .RuleFor(o => o.Bemerkung, f => f.Address.Country())
           .RuleFor(o => o.Gemeinde, f => f.PickRandom(gemeinden))
           .RuleFor(o => o.GrundbuchNr, f => f.Random.AlphaNumeric(40))
           .RuleFor(o => o.Erstellungsdatum, f => f.Date.Past())
           .RuleFor(o => o.Mutationsdatum, f => f.Date.Past())
           .RuleFor(o => o.UserErstellung, f => f.Person.UserName)
           .RuleFor(o => o.UserMutation, f => f.Internet.UserName().OrNull(f, .1f))
           .RuleFor(o => o.FreigabeAfu, f => f.Random.Bool())
           .RuleFor(o => o.AfuUser, f => f.Person.FullName)
           .RuleFor(o => o.AfuDatum, f => f.Date.Past())
           .RuleFor(o => o.Bohrungen, _ => default!);
        Standort SeededStandorte(int seed) => fakeStandorte.UseSeed(seed).Generate();
        context.Standorte.AddRange(standorteRange.Select(SeededStandorte));
        context.SaveChangesWithoutUpdatingChangeInformation();

        // Seed Bohrungen
        var bohrung_ids = 40001;
        var bohrungenRange = Enumerable.Range(bohrung_ids, 8000);
        var fakeBohrungen = new Faker<Bohrung>()
           .StrictMode(true)
           .RuleFor(o => o.Id, f => bohrung_ids++)
           .RuleFor(o => o.Datum, f => f.Date.Past())
           .RuleFor(o => o.DurchmesserBohrloch, f => f.Random.Int(0, 30000))
           .RuleFor(o => o.AblenkungId, f => f.PickRandom(codesToAdd.Where(s => s.CodetypId == 9).Select(s => s.Id).ToList()))
           .RuleFor(o => o.Ablenkung, _ => default!)
           .RuleFor(o => o.QualitaetId, f => f.PickRandom(codesToAdd.Where(s => s.CodetypId == 3).Select(s => s.Id).ToList()))
           .RuleFor(o => o.Qualitaet, _ => default!)
           .RuleFor(o => o.QuelleRef, f => f.Company.CompanyName())
           .RuleFor(o => o.QualitaetBemerkung, f => f.Rant.Review())
           .RuleFor(o => o.Bezeichnung, f => f.Commerce.ProductName())
           .RuleFor(o => o.Erstellungsdatum, f => f.Date.Past())
           .RuleFor(o => o.Mutationsdatum, f => f.Date.Past())
           .RuleFor(o => o.UserErstellung, f => f.Person.UserName)
           .RuleFor(o => o.UserMutation, f => f.Internet.UserName())
           .RuleFor(o => o.Bemerkung, f => f.Company.CatchPhrase().OrNull(f, .2f))
           .RuleFor(o => o.HQualitaet, f => 3)
           .RuleFor(o => o.HAblenkung, f => 9)
           .RuleFor(o => o.Geometrie, f => f.PickRandomParam(new Point(new Coordinate(f.Random.Int(2592400, 2644800), f.Random.Int(1213500, 1261500)))).OrNull(f, .05f)) // Geometries in bounding box of Kanton Solothurn
           .RuleFor(o => o.StandortId, f => f.PickRandom(standorteRange))
           .RuleFor(o => o.Bohrprofile, _ => default!);

        Bohrung SeededBohrungen(int seed) => fakeBohrungen.UseSeed(seed).Generate();
        context.Bohrungen.AddRange(bohrungenRange.Select(SeededBohrungen));
        context.SaveChangesWithoutUpdatingChangeInformation();

        // Seed Bohrprofile
        var bohrprofil_ids = 50001;
        var bohrprofileRange = Enumerable.Range(bohrprofil_ids, 9000);
        var fakeBohrprofile = new Faker<Bohrprofil>()
           .StrictMode(true)
           .RuleFor(o => o.Id, f => bohrprofil_ids++)
           .RuleFor(o => o.BohrungId, f => f.PickRandom(bohrungenRange))
           .RuleFor(o => o.Bemerkung, f => f.Random.Word())
           .RuleFor(o => o.Kote, f => f.Random.Int(0, 30000))
           .RuleFor(o => o.TektonikId, f => f.PickRandom(codesToAdd.Where(s => s.CodetypId == 10).Select(s => s.Id).ToList()))
           .RuleFor(o => o.Tektonik, _ => default!)
           .RuleFor(o => o.Endteufe, f => f.Random.Int(0, 30000))
           .RuleFor(o => o.QualitaetId, f => f.PickRandom(codesToAdd.Where(s => s.CodetypId == 12).Select(s => s.Id).ToList()))
           .RuleFor(o => o.Qualitaet, _ => default!)
           .RuleFor(o => o.QualitaetBemerkung, f => f.Rant.Review().OrNull(f, .8f))
           .RuleFor(o => o.FormationFelsId, f => f.PickRandom(codesToAdd.Where(s => s.CodetypId == 5).Select(s => s.Id).ToList()))
           .RuleFor(o => o.FormationFels, _ => default!)
           .RuleFor(o => o.FormationEndtiefeId, f => f.PickRandom(codesToAdd.Where(s => s.CodetypId == 5).Select(s => s.Id).ToList()))
           .RuleFor(o => o.FormationEndtiefe, _ => default!)
           .RuleFor(o => o.Erstellungsdatum, f => f.Date.Past())
           .RuleFor(o => o.Mutationsdatum, f => f.Date.Past())
           .RuleFor(o => o.UserErstellung, f => f.Person.UserName)
           .RuleFor(o => o.UserMutation, f => f.Internet.UserName().OrNull(f, .2f))
           .RuleFor(o => o.Datum, f => f.Date.Past())
           .RuleFor(o => o.HTektonik, f => 10)
           .RuleFor(o => o.HQualitaet, f => 12)
           .RuleFor(o => o.HFormationFels, f => 5)
           .RuleFor(o => o.HFormationEndtiefe, f => 5)
           .RuleFor(o => o.Schichten, _ => default!)
           .RuleFor(o => o.Vorkommnisse, _ => default!);
        Bohrprofil SeededBohrprofile(int seed) => fakeBohrprofile.UseSeed(seed).Generate();
        context.Bohrprofile.AddRange(bohrprofileRange.Select(SeededBohrprofile));
        context.SaveChangesWithoutUpdatingChangeInformation();

        // Seed Schichten
        var schicht_ids = 70001;
        var schichtenRange = Enumerable.Range(schicht_ids, 14000);
        var fakeSchichten = new Faker<Schicht>()
           .StrictMode(true)
           .RuleFor(o => o.Id, f => schicht_ids++)
           .RuleFor(o => o.Tiefe, f => f.Random.Float())
           .RuleFor(o => o.QualitaetId, f => f.PickRandom(codesToAdd.Where(s => s.CodetypId == 11).Select(s => s.Id).ToList()))
           .RuleFor(o => o.Qualitaet, _ => default!)
           .RuleFor(o => o.QualitaetBemerkung, f => f.Rant.Review().OrNull(f, .9f))
           .RuleFor(o => o.Erstellungsdatum, f => f.Date.Past())
           .RuleFor(o => o.Mutationsdatum, f => f.Date.Past())
           .RuleFor(o => o.UserErstellung, f => f.Person.UserName)
           .RuleFor(o => o.UserMutation, f => f.Internet.UserName())
           .RuleFor(o => o.Bemerkung, f => f.Company.CatchPhrase())
           .RuleFor(o => o.HQualitaet, f => 11)
           .RuleFor(o => o.BohrprofilId, f => f.PickRandom(bohrprofileRange))
           .RuleFor(o => o.CodeSchichtId, f => f.PickRandom(codeschichtenRange))
           .RuleFor(o => o.CodeSchicht, _ => default!);
        Schicht SeededSchichten(int seed) => fakeSchichten.UseSeed(seed).Generate();
        context.Schichten.AddRange(schichtenRange.Select(SeededSchichten));
        context.SaveChangesWithoutUpdatingChangeInformation();

        // Seed Vorkommnisse
        var vorkommnis_ids = 90001;
        var vorkommnisseRange = Enumerable.Range(vorkommnis_ids, 14000);
        var fakeVorkommnisse = new Faker<Vorkommnis>()
           .StrictMode(true)
           .RuleFor(o => o.Id, f => vorkommnis_ids++)
           .RuleFor(o => o.Tiefe, f => f.Random.Float())
           .RuleFor(o => o.QualitaetId, f => f.PickRandom(codesToAdd.Where(s => s.CodetypId == 3).Select(s => s.Id).ToList()))
           .RuleFor(o => o.Qualitaet, _ => default!)
           .RuleFor(o => o.QualitaetBemerkung, f => f.Rant.Review().OrNull(f, .8f))
           .RuleFor(o => o.Erstellungsdatum, f => f.Date.Past())
           .RuleFor(o => o.Mutationsdatum, f => f.Date.Past())
           .RuleFor(o => o.UserErstellung, f => f.Person.UserName)
           .RuleFor(o => o.UserMutation, f => f.Internet.UserName())
           .RuleFor(o => o.Bemerkung, f => f.Random.Word())
           .RuleFor(o => o.HQualitaet, f => 3)
           .RuleFor(o => o.BohrprofilId, f => f.PickRandom(bohrprofileRange))
           .RuleFor(o => o.TypId, f => f.PickRandom(codesToAdd.Where(s => s.CodetypId == 2).Select(s => s.Id).ToList()))
           .RuleFor(o => o.Typ, _ => default!)
           .RuleFor(o => o.HTyp, f => 2);
        Vorkommnis SeededVorkommnisse(int seed) => fakeVorkommnisse.UseSeed(seed).Generate();
        context.Vorkommnisse.AddRange(vorkommnisseRange.Select(SeededVorkommnisse));
        context.SaveChangesWithoutUpdatingChangeInformation();

        // Seed Users
        var user_ids = 80001;
        var usersRange = Enumerable.Range(user_ids, 100);
        var fakeUsers = new Faker<User>("de_CH")
           .StrictMode(true)
           .RuleFor(o => o.Id, f => user_ids++)
           .RuleFor(o => o.Name, f => f.Person.UserName)
           .RuleFor(o => o.Role, f => f.PickRandom<UserRole>())
           .RuleFor(o => o.Erstellungsdatum, f => f.Date.Past())
           .RuleFor(o => o.Mutationsdatum, f => f.Date.Past())
           .RuleFor(o => o.UserErstellung, f => f.Person.UserName)
           .RuleFor(o => o.UserMutation, f => f.Person.FullName);
        User SeededUsers(int seed) => fakeUsers.UseSeed(seed).Generate();
        context.Users.AddRange(usersRange.Select(SeededUsers));
        context.SaveChangesWithoutUpdatingChangeInformation();

        // Sync all database sequences
        context.Database.ExecuteSql($"SELECT setval(pg_get_serial_sequence('bohrung.codetyp', 'codetyp_id'), {codetyp_ids - 1})");
        context.Database.ExecuteSql($"SELECT setval(pg_get_serial_sequence('bohrung.code', 'code_id'), {code_ids - 1})");
        context.Database.ExecuteSql($"SELECT setval(pg_get_serial_sequence('bohrung.codeschicht', 'codeschicht_id'),{codeschicht_ids - 1})");
        context.Database.ExecuteSql($"SELECT setval(pg_get_serial_sequence('bohrung.standort', 'standort_id'),{standort_ids - 1})");
        context.Database.ExecuteSql($"SELECT setval(pg_get_serial_sequence('bohrung.bohrung', 'bohrung_id'),{bohrung_ids - 1})");
        context.Database.ExecuteSql($"SELECT setval(pg_get_serial_sequence('bohrung.bohrprofil', 'bohrprofil_id'),{bohrprofil_ids - 1})");
        context.Database.ExecuteSql($"SELECT setval(pg_get_serial_sequence('bohrung.schicht', 'schicht_id'),{schicht_ids - 1})");
        context.Database.ExecuteSql($"SELECT setval(pg_get_serial_sequence('bohrung.vorkommnis', 'vorkommnis_id'),{vorkommnis_ids - 1})");
        context.Database.ExecuteSql($"SELECT setval(pg_get_serial_sequence('bohrung.user', 'user_id'),{user_ids - 1})");

        transaction.Commit();
    }
}
