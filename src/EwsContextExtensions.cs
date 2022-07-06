using Bogus;
using EWS.Models;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using System.Globalization;

namespace EWS
{
    public static class EwsContextExtensions
    {
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

            TimeZoneInfo ut = TimeZoneInfo.Utc;

            // Seed CodeTypen
            var codetyp_ids = 1;
            var codetypenRange = Enumerable.Range(codetyp_ids, 20);
            var fakeCodeTypen = new Faker<CodeTyp>()
               .StrictMode(true)
               .RuleFor(o => o.Id, f => codetyp_ids++)
               .RuleFor(o => o.Text, f => f.Random.Words())
               .RuleFor(o => o.Kurztext, f => f.Address.StreetName())
               .RuleFor(o => o.Erstellungsdatum, f => TimeZoneInfo.ConvertTimeToUtc(f.Date.Past(), ut))
               .RuleFor(o => o.Mutationsdatum, f => TimeZoneInfo.ConvertTimeToUtc(f.Date.Past(), ut))
               .RuleFor(o => o.UserErstellung, f => f.Person.UserName)
               .RuleFor(o => o.UserMutation, f => f.Internet.UserName().OrNull(f, .8f));
            CodeTyp SeededCodetypen(int seed) => fakeCodeTypen.UseSeed(seed).Generate();
            context.CodeTypen.AddRange(codetypenRange.Select(SeededCodetypen));
            context.SaveChangesWithoutUpdates();

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
               .RuleFor(o => o.Erstellungsdatum, f => TimeZoneInfo.ConvertTimeToUtc(f.Date.Past(), ut))
               .RuleFor(o => o.Mutationsdatum, f => TimeZoneInfo.ConvertTimeToUtc(f.Date.Past(), ut))
               .RuleFor(o => o.UserErstellung, f => f.Person.UserName)
               .RuleFor(o => o.UserMutation, f => f.Internet.UserName().OrNull(f, .6f))
               .RuleFor(o => o.Codetyp, _ => default!);

            Code SeededCodes(int seed) => fakeCodes.UseSeed(seed).Generate();
            context.Codes.AddRange(codesRange.Select(SeededCodes));
            context.SaveChangesWithoutUpdates();

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
               .RuleFor(o => o.Erstellungsdatum, f => TimeZoneInfo.ConvertTimeToUtc(f.Date.Past(), ut))
               .RuleFor(o => o.Mutationsdatum, f => TimeZoneInfo.ConvertTimeToUtc(f.Date.Past(), ut))
               .RuleFor(o => o.UserErstellung, f => f.Person.UserName)
               .RuleFor(o => o.UserMutation, f => f.Internet.UserName());
            CodeSchicht SeededCodeschichten(int seed) => fakeCodeschichten.UseSeed(seed).Generate();
            context.CodeSchichten.AddRange(codeschichtenRange.Select(SeededCodeschichten));
            context.SaveChangesWithoutUpdates();

            // Seed Standorte
            var standort_ids = 30001;
            var standorteRange = Enumerable.Range(standort_ids, 6000);
            var fakeStandorte = new Faker<Standort>()
               .StrictMode(true)
               .RuleFor(o => o.Id, f => standort_ids++)
               .RuleFor(o => o.Bezeichnung, f => f.Commerce.ProductName())
               .RuleFor(o => o.Bemerkung, f => f.Address.Country())
               .RuleFor(o => o.Gemeinde, f => f.Random.Int(2401, 2622))
               .RuleFor(o => o.GrundbuchNr, f => f.Random.AlphaNumeric(40))
               .RuleFor(o => o.Erstellungsdatum, f => TimeZoneInfo.ConvertTimeToUtc(f.Date.Past(), ut))
               .RuleFor(o => o.Mutationsdatum, f => TimeZoneInfo.ConvertTimeToUtc(f.Date.Past(), ut))
               .RuleFor(o => o.UserErstellung, f => f.Person.UserName)
               .RuleFor(o => o.UserMutation, f => f.Internet.UserName().OrNull(f, .1f))
               .RuleFor(o => o.FreigabeAfu, f => f.Random.Bool())
               .RuleFor(o => o.AfuUser, f => f.Person.FullName)
               .RuleFor(o => o.AfuDatum, f => TimeZoneInfo.ConvertTimeToUtc(f.Date.Past(), ut))
               .RuleFor(o => o.Bohrungen, _ => default!);
            Standort SeededStandorte(int seed) => fakeStandorte.UseSeed(seed).Generate();
            context.Standorte.AddRange(standorteRange.Select(SeededStandorte));
            context.SaveChangesWithoutUpdates();

            // Seed Bohrungen
            var bohrung_ids = 40001;
            var bohrungenRange = Enumerable.Range(bohrung_ids, 8000);
            var fakeBohrungen = new Faker<Bohrung>()
               .StrictMode(true)
               .RuleFor(o => o.Id, f => bohrung_ids++)
               .RuleFor(o => o.Datum, f => TimeZoneInfo.ConvertTimeToUtc(f.Date.Past(), ut))
               .RuleFor(o => o.DurchmesserBohrloch, f => f.Random.Int(0, 30000))
               .RuleFor(o => o.AblenkungId, f => f.PickRandom(codesToAdd.Where(s => s.CodetypId == 9).Select(s => s.Id).ToList()))
               .RuleFor(o => o.Ablenkung, _ => default!)
               .RuleFor(o => o.Qualitaet, f => f.PickRandom(codesToAdd.Where(s => s.CodetypId == 3).Select(s => s.Id).ToList()))
               .RuleFor(o => o.QuelleRef, f => f.Company.CompanyName())
               .RuleFor(o => o.QualitaetBemerkung, f => f.Rant.Review())
               .RuleFor(o => o.Bezeichnung, f => f.Commerce.ProductName())
               .RuleFor(o => o.Erstellungsdatum, f => TimeZoneInfo.ConvertTimeToUtc(f.Date.Past(), ut))
               .RuleFor(o => o.Mutationsdatum, f => TimeZoneInfo.ConvertTimeToUtc(f.Date.Past(), ut))
               .RuleFor(o => o.UserErstellung, f => f.Person.UserName)
               .RuleFor(o => o.UserMutation, f => f.Internet.UserName())
               .RuleFor(o => o.Bemerkung, f => f.Company.CatchPhrase().OrNull(f, .2f))
               .RuleFor(o => o.HQualitaetId, f => 3)
               .RuleFor(o => o.HQualitaet, _ => default!)
               .RuleFor(o => o.HAblenkung, f => 9)
               .RuleFor(o => o.Geometrie, f => new Point(new Coordinate(f.Random.Int(2592400, 2644800), f.Random.Int(1213500, 1261500)))) // Geometries in bounding box of Kanton Solothurn
               .RuleFor(o => o.StandortId, f => f.PickRandom(standorteRange))
               .RuleFor(o => o.Bohrprofile, _ => default!);

            Bohrung SeededBohrungen(int seed) => fakeBohrungen.UseSeed(seed).Generate();
            context.Bohrungen.AddRange(bohrungenRange.Select(SeededBohrungen));
            context.SaveChangesWithoutUpdates();

            // Seed Bohrprofile
            var bohrprofil_ids = 50001;
            var bohrprofileRange = Enumerable.Range(bohrprofil_ids, 9000);
            var fakeBohrprofile = new Faker<Bohrprofil>()
               .StrictMode(true)
               .RuleFor(o => o.Id, f => bohrprofil_ids++)
               .RuleFor(o => o.BohrungId, f => f.PickRandom(bohrungenRange))
               .RuleFor(o => o.Bemerkung, f => f.Random.Word())
               .RuleFor(o => o.Kote, f => f.Random.Int(0, 30000))
               .RuleFor(o => o.Tektonik, f => f.PickRandom(codesToAdd.Where(s => s.CodetypId == 10).Select(s => s.Id).ToList()))
               .RuleFor(o => o.Endteufe, f => f.Random.Int(0, 30000))
               .RuleFor(o => o.QualitaetId, f => f.PickRandom(codesToAdd.Where(s => s.CodetypId == 12).Select(s => s.Id).ToList()))
               .RuleFor(o => o.Qualitaet, _ => default!)
               .RuleFor(o => o.QualitaetBemerkung, f => f.Rant.Review().OrNull(f, .8f))
               .RuleFor(o => o.FormationFels, f => f.PickRandom(codesToAdd.Where(s => s.CodetypId == 5).Select(s => s.Id).ToList()))
               .RuleFor(o => o.FormationEndtiefe, f => f.PickRandom(codesToAdd.Where(s => s.CodetypId == 5).Select(s => s.Id).ToList()))
               .RuleFor(o => o.Erstellungsdatum, f => TimeZoneInfo.ConvertTimeToUtc(f.Date.Past(), ut))
               .RuleFor(o => o.Mutationsdatum, f => TimeZoneInfo.ConvertTimeToUtc(f.Date.Past(), ut))
               .RuleFor(o => o.UserErstellung, f => f.Person.UserName)
               .RuleFor(o => o.UserMutation, f => f.Internet.UserName().OrNull(f, .2f))
               .RuleFor(o => o.Datum, f => TimeZoneInfo.ConvertTimeToUtc(f.Date.Past(), ut))
               .RuleFor(o => o.HTektonikId, f => 10)
               .RuleFor(o => o.HTektonik, _ => default!)
               .RuleFor(o => o.HQualitaetId, f => 12)
               .RuleFor(o => o.HQualitaet, _ => default!)
               .RuleFor(o => o.HFormationFelsId, f => 5)
               .RuleFor(o => o.HFormationFels, _ => default!)
               .RuleFor(o => o.HFormationEndtiefeId, f => 5)
               .RuleFor(o => o.HFormationEndtiefe, _ => default!)
               .RuleFor(o => o.Schichten, _ => default!)
               .RuleFor(o => o.Vorkomnisse, _ => default!);
            Bohrprofil SeededBohrprofile(int seed) => fakeBohrprofile.UseSeed(seed).Generate();
            context.Bohrprofile.AddRange(bohrprofileRange.Select(SeededBohrprofile));
            context.SaveChangesWithoutUpdates();

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
               .RuleFor(o => o.Erstellungsdatum, f => TimeZoneInfo.ConvertTimeToUtc(f.Date.Past(), ut))
               .RuleFor(o => o.Mutationsdatum, f => TimeZoneInfo.ConvertTimeToUtc(f.Date.Past(), ut))
               .RuleFor(o => o.UserErstellung, f => f.Person.UserName)
               .RuleFor(o => o.UserMutation, f => f.Internet.UserName())
               .RuleFor(o => o.Bemerkung, f => f.Company.CatchPhrase())
               .RuleFor(o => o.HQualitaetId, f => 11)
               .RuleFor(o => o.HQualitaet, _ => default!)
               .RuleFor(o => o.BohrprofilId, f => f.PickRandom(bohrprofileRange))
               .RuleFor(o => o.CodeSchichtId, f => f.PickRandom(codeschichtenRange))
               .RuleFor(o => o.CodeSchicht, _ => default!);
            Schicht SeededSchichten(int seed) => fakeSchichten.UseSeed(seed).Generate();
            context.Schichten.AddRange(schichtenRange.Select(SeededSchichten));
            context.SaveChangesWithoutUpdates();

            // Seed Vorkommnisse
            var vorkommnis_ids = 90001;
            var vorkommnisseRange = Enumerable.Range(vorkommnis_ids, 14000);
            var fakeVorkommnisse = new Faker<Vorkommnis>()
               .StrictMode(true)
               .RuleFor(o => o.Id, f => vorkommnis_ids++)
               .RuleFor(o => o.Tiefe, f => f.Random.Float())
               .RuleFor(o => o.Qualitaet, f => f.PickRandom(codesToAdd.Where(s => s.CodetypId == 3).Select(s => s.Id).ToList()))
               .RuleFor(o => o.QualitaetBemerkung, f => f.Rant.Review().OrNull(f, .8f))
               .RuleFor(o => o.Erstellungsdatum, f => TimeZoneInfo.ConvertTimeToUtc(f.Date.Past(), ut))
               .RuleFor(o => o.Mutationsdatum, f => TimeZoneInfo.ConvertTimeToUtc(f.Date.Past(), ut))
               .RuleFor(o => o.UserErstellung, f => f.Person.UserName)
               .RuleFor(o => o.UserMutation, f => f.Internet.UserName())
               .RuleFor(o => o.Bemerkung, f => f.Random.Word())
               .RuleFor(o => o.HQualitaetId, f => 3)
               .RuleFor(o => o.HQualitaet, _ => default!)
               .RuleFor(o => o.BohrprofilId, f => f.PickRandom(bohrprofileRange))
               .RuleFor(o => o.Typ, f => f.PickRandom(codesToAdd.Where(s => s.CodetypId == 2).Select(s => s.Id).ToList()))
               .RuleFor(o => o.HTypId, f => 2)
               .RuleFor(o => o.HTyp, _ => default!);
            Vorkommnis SeededVorkommnisse(int seed) => fakeVorkommnisse.UseSeed(seed).Generate();
            context.Vorkommnisse.AddRange(vorkommnisseRange.Select(SeededVorkommnisse));
            context.SaveChangesWithoutUpdates();

            // Sync all database sequences
            context.Database.ExecuteSqlRaw($"SELECT setval(pg_get_serial_sequence('bohrung.codetyp', 'codetyp_id'), {codetyp_ids - 1})");
            context.Database.ExecuteSqlRaw($"SELECT setval(pg_get_serial_sequence('bohrung.code', 'code_id'), {code_ids - 1})");
            context.Database.ExecuteSqlRaw($"SELECT setval(pg_get_serial_sequence('bohrung.codeschicht', 'codeschicht_id'),{codeschicht_ids - 1})");
            context.Database.ExecuteSqlRaw($"SELECT setval(pg_get_serial_sequence('bohrung.standort', 'standort_id'),{standort_ids - 1})");
            context.Database.ExecuteSqlRaw($"SELECT setval(pg_get_serial_sequence('bohrung.bohrung', 'bohrung_id'),{bohrung_ids - 1})");
            context.Database.ExecuteSqlRaw($"SELECT setval(pg_get_serial_sequence('bohrung.bohrprofil', 'bohrprofil_id'),{bohrprofil_ids - 1})");
            context.Database.ExecuteSqlRaw($"SELECT setval(pg_get_serial_sequence('bohrung.schicht', 'schicht_id'),{schicht_ids - 1})");
            context.Database.ExecuteSqlRaw($"SELECT setval(pg_get_serial_sequence('bohrung.vorkommnis', 'vorkommnis_id'),{vorkommnis_ids - 1})");

            transaction.Commit();
        }
    }
}
