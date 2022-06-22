describe("Home page tests", () => {
  it("Show search result box if search for gemeinde", function () {
    cy.visit("/");
    cy.get("div[name=home-container]").should("not.contain", "Suchresultate");

    // use click 'force: true' because element is covered by another element.
    cy.get("div[name=gemeinde] input").should("be.visible").click({ force: true }).type("Hein{downarrow}{enter}");
    cy.get("button[name=submit-button]").should("be.visible").click();
    cy.get("div[name=home-container]").should("contain", "Suchresultate");
    cy.get("tbody").children().should("have.length", 27);
  });

  it("Show search result box if search for grundbuchnummer", function () {
    cy.visit("/");
    cy.get("div[name=home-container]").should("not.contain", "Suchresultate");
    cy.get("input[name=gbnummer]")
      .should("be.visible")
      .click({ force: true })
      .type("h5r0wdwsz6ef39zb2d31a0zfou7i4tdguvddcklb");
    cy.get("button[name=submit-button]").should("be.visible").click();
    cy.get("div[name=home-container]").should("contain", "Suchresultate");
    cy.get("tbody").children().should("have.length", 1);
  });

  it("Show search result box if search for bezeichnung", function () {
    cy.visit("/");
    cy.get("div[name=home-container]").should("not.contain", "Suchresultate");
    cy.get("input[name=bezeichnung]").should("be.visible").click({ force: true }).type("Rustic Wooden Keyboard");
    cy.get("button[name=submit-button]").should("be.visible").click();
    cy.get("div[name=home-container]").should("contain", "Suchresultate");
    cy.get("tbody").children().should("have.length", 1);
  });
});
