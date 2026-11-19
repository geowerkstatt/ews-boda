import standorteGemeinde from "../fixtures/standorteGemeinde.json";
import standorte from "../fixtures/standorte.json";
import bohrung from "../fixtures/bohrung.json";
import userInRoleExtern from "../fixtures/user_in_role_extern.json";
import userInRoleSachbearbeiterAfu from "../fixtures/user_in_role_sachbearbeiter_afu.json";

// "Ergonomic Metal Tuna" has Freigabe AfU, so it is read-only for users in role 'Extern'.
const openBohrungForm = (user, bohrungResponse = bohrung) => {
  // Keep the geo.so.ch base map out of these tests. When it loads within the 250 ms OpenLayers waits
  // before a singleclick, the layer canvases are rebuilt, the clicked canvas leaves the page and
  // OpenLayers drops the click. The failed request ends as a promise rejection, which e2e.js ignores.
  cy.intercept("GET", "https://geo.so.ch/api/wmts/1.0.0/WMTSCapabilities.xml", { forceNetworkError: true });
  cy.intercept("/user/self", user);
  cy.intercept("/standort", standorte);
  const standort = standorteGemeinde.find((s) => s.bezeichnung === "Ergonomic Metal Tuna");
  cy.intercept("/standort/" + standort.id, standort);
  cy.intercept(
    "/standort?gemeinde=Heinrichswil-Winistorf&gbnummer=&bezeichnung=&erstellungsdatum=&mutationsdatum=",
    standorteGemeinde,
  );
  cy.intercept("/bohrung/" + bohrung.id, bohrungResponse);

  cy.visit("/");
  cy.selectGemeinde("Hein", "Heinrichswil-Winistorf");
  cy.get("button[name=submit-button]").should("be.visible").click();
  cy.contains("td", "Ergonomic Metal Tuna")
    .parent("tr")
    .children("td")
    .find("button[name=edit-button]")
    .scrollIntoView()
    .click();
  cy.contains("td", bohrung.bezeichnung)
    .parent("tr")
    .children("td")
    .find("button[name=edit-button]")
    .scrollIntoView()
    .click();
  cy.get("form[name=bohrung-form]").should("contain", "Bohrung bearbeiten");
};

const bohrungForm = () => cy.get("form[name=bohrung-form]");
const bohrprofilForm = () => cy.get("form[name=bohrprofil-form]");
// The picker shows its value in editable sections and keeps it as text in a hidden input beside them.
const datumInput = (form = bohrungForm()) => form.find("input[name=datum]");
const datumField = (form = bohrungForm()) => datumInput(form).parent();
const shouldShowDatum = (value, { invalid }) => {
  datumInput().should("have.value", value);
  datumField().should("have.attr", "aria-invalid", String(invalid));
};
const openCalendarButton = () => bohrungForm().find("button[aria-label^='Choose date']");
const calendarDay = (day) => cy.get("[role=grid]").contains("[role=gridcell]", new RegExp(`^${day}$`));

// Clicking near the left edge selects the day section, then each digit fills day, month and year in turn.
const typeDate = (field, keys) => {
  field.scrollIntoView().click(20, 10);
  cy.focused().type(keys);
};

// OpenLayers ignores clicks until it has rendered its first frame, and it creates the layer canvases
// only in that render. The zoom buttons sit in the map's top-left corner and the Bohrung marker in its
// centre, so click somewhere in between to set a new position.
const clickMapPosition = () => {
  bohrungForm().find(".detail-map canvas").should("exist");
  bohrungForm().find(".detail-map").scrollIntoView().click(150, 100);
};

const openStoredBohrprofil = () => {
  cy.contains("form[name=bohrung-form] td", "12345678")
    .parent("tr")
    .children("td")
    .find("button[name=edit-button]")
    .scrollIntoView()
    .click();
  bohrprofilForm().should("contain", "Bohrprofil bearbeiten");
};

describe("Bohrung form date picker", () => {
  beforeEach(() => {
    openBohrungForm(userInRoleSachbearbeiterAfu);
  });

  it("shows the stored Bohrbeginn", () => {
    datumInput().should("have.value", "03.12.2021");
    bohrungForm().find("button[type=submit]").should("be.disabled");
  });

  it("saves a date picked in the calendar as UTC date", () => {
    cy.intercept("PUT", "/bohrung", { statusCode: 200 }).as("editBohrung");

    openCalendarButton().should("not.be.disabled").click();
    calendarDay(15).click();
    datumInput().should("have.value", "15.12.2021");

    cy.contains("button", "Bohrung speichern").scrollIntoView().should("not.be.disabled").click();
    cy.wait("@editBohrung")
      .its("request.body")
      .then((body) => {
        const sentBohrung = typeof body === "string" ? JSON.parse(body) : body;
        expect(sentBohrung.id).to.equal(bohrung.id);
        expect(sentBohrung.datum).to.equal("2021-12-15T00:00:00.000Z");
      });
  });

  it("saves a date typed with the keyboard as UTC date", () => {
    cy.intercept("PUT", "/bohrung", { statusCode: 200 }).as("editBohrung");

    typeDate(datumField(), "15062020");
    datumInput().should("have.value", "15.06.2020");

    cy.contains("button", "Bohrung speichern").scrollIntoView().should("not.be.disabled").click();
    cy.wait("@editBohrung")
      .its("request.body")
      .then((body) => {
        const sentBohrung = typeof body === "string" ? JSON.parse(body) : body;
        expect(sentBohrung.datum).to.equal("2020-06-15T00:00:00.000Z");
      });
  });

  it("blocks saving while the typed date is incomplete", () => {
    typeDate(datumField(), "{ctrl+a}{del}15");

    shouldShowDatum("15.MM.YYYY", { invalid: true });
    bohrungForm().should("contain", "Geben Sie ein gültiges Datum im Format TT.MM.JJJJ ein");
    bohrungForm().find("button[type=submit]").should("be.disabled");

    typeDate(datumField(), "15062020");

    shouldShowDatum("15.06.2020", { invalid: false });
    bohrungForm().should("not.contain", "Geben Sie ein gültiges Datum");
    bohrungForm().find("button[type=submit]").should("not.be.disabled");
  });

  it("blocks saving an impossible date", () => {
    typeDate(datumField(), "31022021");

    shouldShowDatum("31.02.2021", { invalid: true });
    bohrungForm().should("contain", "Geben Sie ein gültiges Datum im Format TT.MM.JJJJ ein");
    bohrungForm().find("button[type=submit]").should("be.disabled");
  });

  it("blocks saving a date in the future", () => {
    typeDate(datumField(), "01012030");

    shouldShowDatum("01.01.2030", { invalid: true });
    bohrungForm().should("contain", "Das Datum darf nicht in der Zukunft liegen");
    bohrungForm().find("button[type=submit]").should("be.disabled");
  });

  it("keeps the stored date when pasted text cannot be read", () => {
    typeDate(datumField(), "{ctrl+a}");
    cy.focused().trigger("paste", { clipboardData: { getData: () => "15062020" } });

    bohrungForm().should("contain", "Das eingefügte Datum muss im Format TT.MM.JJJJ sein");
    datumInput().should("have.value", "03.12.2021");
    bohrungForm().find("button[type=submit]").should("be.disabled");
  });

  it("saves a cleared date as no date", () => {
    cy.intercept("PUT", "/bohrung", { statusCode: 200 }).as("editBohrung");

    typeDate(datumField(), "{ctrl+a}{del}");

    cy.contains("button", "Bohrung speichern").scrollIntoView().should("not.be.disabled").click();
    cy.wait("@editBohrung")
      .its("request.body")
      .then((body) => {
        const sentBohrung = typeof body === "string" ? JSON.parse(body) : body;
        expect(sentBohrung.datum).to.equal(null);
      });
  });

  it("keeps edited fields when the position is changed in the map", () => {
    bohrungForm().find("input[name=bezeichnung]").click({ force: true }).type(" Nord");
    openCalendarButton().click();
    calendarDay(15).click();
    bohrungForm().find("input[name=x_coordinate]").should("have.value", "2623182.0");

    clickMapPosition();

    bohrungForm().find("input[name=x_coordinate]").should("not.have.value", "2623182.0");
    bohrungForm()
      .find("input[name=bezeichnung]")
      .should("have.value", bohrung.bezeichnung + " Nord");
    datumInput().should("have.value", "15.12.2021");
  });

  it("shows the stored name and date again after a map click and a visit to a Bohrprofil", () => {
    bohrungForm().find("input[name=bezeichnung]").click({ force: true }).type(" Nord");
    clickMapPosition();
    bohrungForm().find("input[name=x_coordinate]").should("not.have.value", "2623182.0");

    openStoredBohrprofil();
    cy.contains("button", "Zurück").click();

    bohrungForm().find("input[name=bezeichnung]").should("have.value", bohrung.bezeichnung);
    datumInput().should("have.value", "03.12.2021");
  });
});

describe("Bohrprofil form date picker", () => {
  const [storedBohrprofil] = bohrung.bohrprofile;
  const bohrungWithDatedBohrprofile = {
    ...bohrung,
    bohrprofile: [
      { ...storedBohrprofil, datum: "2022-03-10T00:00:00" },
      {
        ...storedBohrprofil,
        id: storedBohrprofil.id + 1,
        endteufe: 222,
        datum: "2023-07-20T00:00:00",
        schichten: [],
        vorkommnisse: [],
      },
    ],
  };
  const bohrprofilDatum = () => datumInput(bohrprofilForm());

  beforeEach(() => {
    openBohrungForm(userInRoleSachbearbeiterAfu, bohrungWithDatedBohrprofile);
    openStoredBohrprofil();
  });

  it("shows the date of each Bohrprofil when navigating, replacing a typed date", () => {
    bohrprofilDatum().should("have.value", "10.03.2022");
    typeDate(datumField(bohrprofilForm()), "05052022");
    bohrprofilDatum().should("have.value", "05.05.2022");

    bohrprofilForm().find("button[aria-label='Zum nächsten Bohrprofil']").click();
    bohrprofilDatum().should("have.value", "20.07.2023");

    bohrprofilForm().find("button[aria-label='Zum vorherigen Bohrprofil']").click();
    bohrprofilDatum().should("have.value", "10.03.2022");
  });
});

describe("Bohrung form in read-only state", () => {
  beforeEach(() => {
    openBohrungForm(userInRoleExtern);
  });

  it("shows the values but disables all inputs, the date picker and saving", () => {
    datumInput().should("have.value", "03.12.2021").and("be.disabled");
    openCalendarButton().should("be.disabled");
    bohrungForm().find("input[name=bezeichnung]").should("have.value", bohrung.bezeichnung).and("be.disabled");
    bohrungForm().find("textarea[name=bemerkung]").should("be.disabled");
    bohrungForm().find("input[name=x_coordinate]").should("be.disabled");
    bohrungForm().find("button[type=submit]").should("be.disabled");
    bohrungForm().contains("button", "Schliessen").should("be.visible");
  });
});
