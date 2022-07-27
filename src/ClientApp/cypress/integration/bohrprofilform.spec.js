import standorteGemeinde from "../fixtures/standorteGemeinde.json";
import standorte from "../fixtures/standorte.json";

describe("Input form tests", () => {
  beforeEach(() => {
    cy.intercept("/standort", standorte);
    cy.intercept(
      "/standort?gemeinde=Heinrichswil-Winistorf&gbnummer=&bezeichnung=&erstellungsdatum=&mutationsdatum=",
      standorteGemeinde
    );
    // Open bohrung edit form
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

    cy.contains("td", "Rustic Plastic Tuna")
      .parent("tr")
      .children("td")
      .find("button[name=edit-button]")
      .scrollIntoView()
      .click();
  });

  it("Open Bohrprofil Add Form", function () {
    cy.get("button[name=add-button]").scrollIntoView().click();
    cy.get("button[type=submit]").should("be.disabled");
    cy.get("form[name=bohrprofil-form]").should("contain", "Bohrprofil erstellen");
    cy.get("form[name=bohrprofil-form]")
      .scrollIntoView()
      .find("textarea[name=bemerkung]")
      .should("be.visible")
      .click({ force: true })
      .type(" And More");
    cy.contains("button", "Bohrprofil speichern").scrollIntoView().should("not.be.disabled").click();
  });

  it("Open Bohrprofil Edit Form", function () {
    cy.contains("td", "12345678").parent("tr").children("td").find("button[name=edit-button]").scrollIntoView().click();
    cy.get("button[type=submit]").should("be.disabled");
    cy.get("form[name=bohrprofil-form]").should("contain", "Bohrprofil bearbeiten");
    cy.get("form[name=bohrprofil-form]")
      .scrollIntoView()
      .find("textarea[name=bemerkung]")
      .should("be.visible")
      .click({ force: true })
      .type(" And More");

    cy.contains("button", "Bohrprofil speichern").scrollIntoView().should("be.visible").click();
  });

  it("Delete Bohrprofil", function () {
    cy.contains("td", "12345678")
      .parent("tr")
      .children("td")
      .find("button[name=delete-button]")
      .scrollIntoView()
      .click();
    cy.contains("button", "OK").click();
  });
});
