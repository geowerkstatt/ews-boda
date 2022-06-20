describe("Home page tests", () => {
  it("Show search result box only if search term is present", function () {
    cy.visit("/");
    cy.get("div[name=home-container]").should("not.contain", "Suchresultate");

    // use click 'force: true' because element is covered by another element.
    cy.get("div[name=search-bar] input").should("be.visible").click({ force: true }).type("The Dark{downarrow}{enter}");
    cy.get("div[name=home-container]").should("contain", "Suchresultate");
  });
});
