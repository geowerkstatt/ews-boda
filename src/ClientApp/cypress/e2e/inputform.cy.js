import standorteGemeinde from "../fixtures/standorteGemeinde.json";
import standorte from "../fixtures/standorte.json";
import bohrung from "../fixtures/bohrung.json";

describe("Input form tests", () => {
  beforeEach(() => {
    cy.intercept("/standort", standorte);
    const standort = standorteGemeinde.find((s) => s.bezeichnung === "Ergonomic Metal Tuna");
    cy.intercept("/standort/" + standort.id, standort);
    cy.intercept(
      "/standort?gemeinde=Heinrichswil-Winistorf&gbnummer=&bezeichnung=&erstellungsdatum=&mutationsdatum=",
      standorteGemeinde
    );
    cy.intercept("/bohrung/" + 41063, bohrung);

    // Open edit form
    cy.visit("/");
    cy.get("div[name=home-container]").should("not.contain", "Standorte");
    cy.get("div[name=gemeinde] input").should("be.visible").click({ force: true }).type("Hein{downarrow}{enter}");
    cy.get("button[name=submit-button]").should("be.visible").click();
    cy.get("div[name=home-container]").should("contain", "Standorte");
    cy.get("tbody").children().should("have.length", 4);
    cy.contains("td", "Ergonomic Metal Tuna")
      .parent("tr")
      .children("td")
      .find("button[name=edit-button]")
      .scrollIntoView()
      .click();
  });

  it("Open Bohrung Add Form", function () {
    cy.get("button[name=add-button]").scrollIntoView().click();

    cy.get("button[type=submit]").should("be.disabled");
    cy.get("form[name=bohrung-form]").should("contain", "Bohrung erstellen");
    cy.get("form[name=bohrung-form]")
      .find("input[name=bezeichnung]")
      .should("be.visible")
      .click({ force: true })
      .type(" And More");

    cy.contains("button", "Bohrung speichern").scrollIntoView().should("not.be.disabled").scrollIntoView().click();
  });

  it("Open Bohrung Edit Form", function () {
    cy.contains("td", "Rustic Plastic Tuna")
      .parent("tr")
      .children("td")
      .find("button[name=edit-button]")
      .scrollIntoView()
      .click();

    cy.get("button[type=submit]").should("be.disabled");
    cy.get("form[name=bohrung-form]").should("contain", "Bohrung bearbeiten");
    cy.get("form[name=bohrung-form]")
      .find("input[name=bezeichnung]")
      .should("be.visible")
      .click({ force: true })
      .type(" And More");

    cy.contains("button", "Bohrung speichern").scrollIntoView().should("not.be.disabled").scrollIntoView().click();
  });

  it("Delete Bohrung", function () {
    cy.contains("td", "Rustic Plastic Tuna")
      .parent("tr")
      .children("td")
      .find("button[name=delete-button]")
      .scrollIntoView()
      .click();
    cy.contains("button", "Löschen").click();
  });
});
