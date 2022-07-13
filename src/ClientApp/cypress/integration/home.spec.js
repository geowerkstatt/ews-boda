import standorteGemeinde from "../fixtures/standorteGemeinde.json";
import standorteGbnummer from "../fixtures/standorteGbnummer.json";
import standorteBezeichnung from "../fixtures/standorteBezeichnung.json";

describe("Home page tests", () => {
  it("Show search result box if search for gemeinde", function () {
    cy.intercept(
      "/standort?gemeindenummer=2521&gbnummer=&bezeichnung=&erstellungsdatum=&mutationsdatum=",
      standorteGemeinde
    );

    cy.visit("/");
    cy.get("div[name=home-container]").should("not.contain", "Standorte");

    // use click 'force: true' because element is covered by another element.
    cy.get("div[name=gemeinde] input").should("be.visible").click({ force: true }).type("Hein{downarrow}{enter}");
    cy.get("button[name=submit-button]").should("be.visible").click();
    cy.get("div[name=home-container]").should("contain", "Standorte");
    cy.get("tbody").children().should("have.length", 4);
  });

  it("Show search result box if search for grundbuchnummer", function () {
    const gbnummer = "h5r0wdwsz6ef39zb2d31a0zfou7i4tdguvddcklb";
    cy.intercept(
      `/standort?gemeindenummer=&gbnummer=${gbnummer}&bezeichnung=&erstellungsdatum=&mutationsdatum=`,
      standorteGbnummer
    );
    cy.visit("/");
    cy.get("div[name=home-container]").should("not.contain", "Standorte");
    cy.get("input[name=gbnummer]").should("be.visible").click({ force: true }).type(gbnummer);
    cy.get("button[name=submit-button]").should("be.visible").click();
    cy.get("div[name=home-container]").should("contain", "Standorte");
    cy.get("tbody").children().should("have.length", 3);
  });

  it("Show search result box if search for bezeichnung", function () {
    cy.intercept(
      `/standort?gemeindenummer=&gbnummer=&bezeichnung=Rustic%20Wooden%20Keyboard&erstellungsdatum=&mutationsdatum=`,
      standorteBezeichnung
    );
    cy.visit("/");
    cy.get("div[name=home-container]").should("not.contain", "Standorte");
    cy.get("input[name=bezeichnung]").should("be.visible").click({ force: true }).type("Rustic Wooden Keyboard");
    cy.get("button[name=submit-button]").should("be.visible").click();
    cy.get("div[name=home-container]").should("contain", "Standorte");
    cy.get("tbody").children().should("have.length", 1);
  });

  it("Show map", function () {
    cy.visit("/");
    cy.get("div[class=map-container]").should("be.visible");
  });

  it("Open Standort Edit Form", function () {
    cy.intercept(
      "/standort?gemeindenummer=2521&gbnummer=&bezeichnung=&erstellungsdatum=&mutationsdatum=",
      standorteGemeinde
    );
    cy.visit("/");
    cy.get("div[name=home-container]").should("not.contain", "Standorte");
    // get search results
    cy.get("div[name=gemeinde] input").should("be.visible").click({ force: true }).type("Hein{downarrow}{enter}");
    cy.get("button[name=submit-button]").should("be.visible").click();
    cy.get("div[name=home-container]").should("contain", "Standorte");
    cy.get("tbody").children().should("have.length", 4);

    cy.contains("td", "Ergonomic Metal Tuna").parent("tr").children("td").find("button[name=edit-button]").click();
    // submit button should only be visible if form is dirty
    cy.get("button[type=submit]").should("not.be.visible");
    cy.get("form[name=standort-form]")
      .find("input[name=bezeichnung]")
      .should("be.visible")
      .click({ force: true })
      .type(" And More");

    cy.contains("button", "Standort Speichern").scrollIntoView().should("be.visible").click();
  });

  it("Delete Standort", function () {
    cy.intercept(
      "/standort?gemeindenummer=2521&gbnummer=&bezeichnung=&erstellungsdatum=&mutationsdatum=",
      standorteGemeinde
    );

    cy.intercept("DELETE", "/standort?id=35979", { statusCode: 200 });

    cy.visit("/");
    cy.get("div[name=home-container]").should("not.contain", "Standorte");
    // get search results
    cy.get("div[name=gemeinde] input").should("be.visible").click({ force: true }).type("Hein{downarrow}{enter}");
    cy.get("button[name=submit-button]").should("be.visible").click();
    cy.get("div[name=home-container]").should("contain", "Standorte");
    cy.get("tbody").children().should("have.length", 4);

    cy.contains("td", "Ergonomic Metal Tuna").parent("tr").children("td").find("button[name=delete-button]").click();
    cy.contains("button", "OK").click();

    cy.get("tbody").children().should("have.length", 3);
  });
});
