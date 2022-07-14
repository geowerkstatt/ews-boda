import standorteGemeinde from "../fixtures/standorteGemeinde.json";
import standorteGbnummer from "../fixtures/standorteGbnummer.json";
import standorteBezeichnung from "../fixtures/standorteBezeichnung.json";
import standorte from "../fixtures/standorte.json";

describe("Home page tests", () => {
  it("Show search result box if search for gemeinde", function () {
    cy.intercept("/standort", standorte);

    cy.intercept(
      "/standort?gemeinde=Heinrichswil-Winistorf&gbnummer=&bezeichnung=&erstellungsdatum=&mutationsdatum=",
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
      `/standort?gemeinde=null&gbnummer=${gbnummer}&bezeichnung=&erstellungsdatum=&mutationsdatum=`,
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
      `/standort?gemeinde=null&gbnummer=&bezeichnung=Rustic%20Wooden%20Keyboard&erstellungsdatum=&mutationsdatum=`,
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
});
