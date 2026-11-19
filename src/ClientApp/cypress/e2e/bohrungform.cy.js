import standorteGemeinde from "../fixtures/standorteGemeinde.json";
import standorte from "../fixtures/standorte.json";
import bohrung from "../fixtures/bohrung.json";
import userInRoleExtern from "../fixtures/user_in_role_extern.json";
import userInRoleSachbearbeiterAfu from "../fixtures/user_in_role_sachbearbeiter_afu.json";

// "Ergonomic Metal Tuna" has Freigabe AfU, so it is read-only for users in role 'Extern'.
const openBohrungForm = (user) => {
  cy.intercept("/user/self", user);
  cy.intercept("/standort", standorte);
  const standort = standorteGemeinde.find((s) => s.bezeichnung === "Ergonomic Metal Tuna");
  cy.intercept("/standort/" + standort.id, standort);
  cy.intercept(
    "/standort?gemeinde=Heinrichswil-Winistorf&gbnummer=&bezeichnung=&erstellungsdatum=&mutationsdatum=",
    standorteGemeinde,
  );
  cy.intercept("/bohrung/" + bohrung.id, bohrung);

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
const datumInput = () => bohrungForm().find("input[name=datum]");
const openCalendarButton = () => bohrungForm().find("button[aria-label^='Choose date']");
const calendarDay = (day) => cy.get("[role=grid]").contains("[role=gridcell]", new RegExp(`^${day}$`));

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

  it("keeps edited fields when the position is changed in the map", () => {
    bohrungForm().find("input[name=bezeichnung]").click({ force: true }).type(" Nord");
    openCalendarButton().click();
    calendarDay(15).click();
    bohrungForm().find("input[name=x_coordinate]").should("have.value", "2623182.0");

    bohrungForm().find(".detail-map").scrollIntoView().click(20, 20);

    bohrungForm().find("input[name=x_coordinate]").should("not.have.value", "2623182.0");
    bohrungForm()
      .find("input[name=bezeichnung]")
      .should("have.value", bohrung.bezeichnung + " Nord");
    datumInput().should("have.value", "15.12.2021");
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
