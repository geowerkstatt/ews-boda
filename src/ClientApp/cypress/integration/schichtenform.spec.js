import standorteGemeinde from "../fixtures/standorteGemeinde.json";
import standorte from "../fixtures/standorte.json";

describe("Input form tests", () => {
  beforeEach(() => {
    cy.intercept("/standort", standorte);
    let standort = standorteGemeinde.find((s) => s.bezeichnung === "Ergonomic Metal Tuna");
    cy.intercept("/standort/" + standort.id, standort);
    cy.intercept(
      "/standort?gemeinde=Heinrichswil-Winistorf&gbnummer=&bezeichnung=&erstellungsdatum=&mutationsdatum=",
      standorteGemeinde
    );
    cy.visit("/");
    cy.get("div[name=home-container]").should("not.contain", "Standorte");
    cy.get("div[name=gemeinde] input").should("be.visible").click({ force: true }).type("Hein{downarrow}{enter}");
    cy.get("button[name=submit-button]").should("be.visible").click();
    cy.get("div[name=home-container]").should("contain", "Standorte");
    cy.get("tbody").children().should("have.length", 4);

    //Open standort
    cy.contains("td", "Ergonomic Metal Tuna")
      .parent("tr")
      .children("td")
      .find("button[name=edit-button]")
      .scrollIntoView()
      .click();
    //Open Bohrung
    cy.contains("td", "Rustic Plastic Tuna")
      .parent("tr")
      .children("td")
      .find("button[name=edit-button]")
      .scrollIntoView()
      .click();

    //Open Bohrprofil
    cy.contains("td", "12345678").parent("tr").children("td").find("button[name=edit-button]").scrollIntoView().click();
  });

  it("Open Schicht Add Form", function () {
    cy.get("button[name=add-button]").scrollIntoView().click();
    cy.get("button[type=submit]").should("be.disabled");
    cy.get("form[name=schicht-form]").should("contain", "Schicht erstellen");
    cy.get("form[name=schicht-form]")
      .scrollIntoView()
      .find("textarea[name=bemerkung]")
      .should("be.visible")
      .click({ force: true })
      .type(" And More");
    cy.contains("button", "Schicht speichern").scrollIntoView().should("not.be.disabled").click();
  });

  it("Open Schicht Edit Form", function () {
    cy.contains("td", "Avon Intelligent Cotton Keyboard")
      .parent("tr")
      .children("td")
      .find("button[name=edit-button]")
      .scrollIntoView()
      .click();
    cy.get("button[type=submit]").should("be.disabled");
    cy.get("form[name=schicht-form]").should("contain", "Schicht bearbeiten");
    cy.get("form[name=schicht-form]")
      .scrollIntoView()
      .find("textarea[name=bemerkung]")
      .should("be.visible")
      .click({ force: true })
      .type(" And More");

    cy.contains("button", "Schicht speichern").scrollIntoView().should("be.visible").click();
  });

  it("Delete Schicht", function () {
    cy.contains("td", "Avon Intelligent Cotton Keyboard")
      .parent("tr")
      .children("td")
      .find("button[name=delete-button]")
      .scrollIntoView()
      .click();
    cy.contains("button", "OK").click();
  });
});
