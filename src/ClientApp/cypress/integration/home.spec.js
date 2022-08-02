import standorteGemeinde from "../fixtures/standorteGemeinde.json";
import standorteGbnummer from "../fixtures/standorteGbnummer.json";
import standorteBezeichnung from "../fixtures/standorteBezeichnung.json";
import standorte from "../fixtures/standorte.json";
import userInRoleExtern from "../fixtures/user_in_role_extern.json";
import userInRoleSachbearbeiterAfu from "../fixtures/user_in_role_sachbearbeiter_afu.json";
import userInRoleAdministrator from "../fixtures/user_in_role_administrator.json";

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
      `/standort?gemeinde=&gbnummer=${gbnummer}&bezeichnung=&erstellungsdatum=&mutationsdatum=`,
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
      `/standort?gemeinde=&gbnummer=&bezeichnung=Rustic%20Wooden%20Keyboard&erstellungsdatum=&mutationsdatum=`,
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
    cy.intercept("/standort", standorte);

    const standort = standorteGemeinde.find((s) => s.bezeichnung === "Ergonomic Metal Tuna");
    cy.intercept("/standort/" + standort.id, standort);

    cy.intercept(
      "/standort?gemeinde=Heinrichswil-Winistorf&gbnummer=&bezeichnung=&erstellungsdatum=&mutationsdatum=",
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

    cy.contains("button", "Standort speichern").scrollIntoView().should("be.visible").click();
  });

  it("Delete Standort", function () {
    cy.intercept("/standort", standorte);

    cy.intercept(
      "/standort?gemeinde=Heinrichswil-Winistorf&gbnummer=&bezeichnung=&erstellungsdatum=&mutationsdatum=",
      standorteGemeinde
    );
    cy.intercept("DELETE", "/standort*", {
      statusCode: 200,
    });

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

  it("Check and uncheck AfU Freigabe", function () {
    cy.intercept("/standort", standorte);
    cy.intercept(
      "/standort?gemeinde=Heinrichswil-Winistorf&gbnummer=&bezeichnung=&erstellungsdatum=&mutationsdatum=",
      standorteGemeinde
    );
    const standort = standorteGemeinde.find((s) => s.bezeichnung === "Generic Steel Pants");
    cy.intercept("/standort/" + standort.id, standort);

    cy.intercept("/user/self", userInRoleSachbearbeiterAfu);
    cy.intercept("PUT", "/standort").as("editStandortFreigabe");

    cy.visit("/");
    cy.get("div[name=gemeinde] input").should("be.visible").click({ force: true }).type("Hein{downarrow}{enter}");
    cy.get("button[name=submit-button]").should("be.visible").click({ force: true });

    // check Freigabe AfU
    cy.contains("td", "Generic Steel Pants").siblings().find("[aria-label='edit standort']").click({ force: true });
    cy.get('[type="checkbox"]').should("not.be.checked").check().should("be.checked");
    cy.get('button:contains("speichern")').click({ force: true });
    cy.wait("@editStandortFreigabe")
      .its("request")
      .then((request) => {
        expect(JSON.stringify(request.body)).to.include('"freigabeAfu":true');
      });

    // Uncheck Freigabe AfU
    cy.contains("td", "Generic Steel Pants").siblings().find("[aria-label='edit standort']").click({ force: true });
    cy.get('[type="checkbox"]').should("be.checked").uncheck().should("not.be.checked");
    cy.get('button:contains("speichern")').click({ force: true });
    cy.wait("@editStandortFreigabe")
      .its("request")
      .then((request) => {
        expect(JSON.stringify(request.body)).to.include('"freigabeAfu":false');
      });
  });

  describe("AfU Freigabe checkbox", () => {
    it("is not available for users in role 'Extern'", () => {
      cy.intercept("/standort*", standorteGemeinde);
      cy.intercept("/user/self", userInRoleExtern);
      cy.visit("/");
      cy.get("div[name=gemeinde] input").should("be.visible").click({ force: true }).type("Hein{downarrow}{enter}");
      cy.get("button[name=submit-button]").should("be.visible").click();
      cy.contains("td", "Generic Steel Pants").siblings().find("[aria-label='edit standort']").click();
      cy.get('[type="checkbox"]').should("not.exist");
    });
    it("is available for users in role 'SachbearbeiterAfU'", () => {
      cy.intercept("/standort*", standorteGemeinde);
      const standort = standorteGemeinde.find((s) => s.bezeichnung === "Generic Steel Pants");
      cy.intercept("/standort/" + standort.id, standort);

      cy.intercept("/user/self", userInRoleSachbearbeiterAfu);
      cy.visit("/");
      cy.get("div[name=gemeinde] input").should("be.visible").click({ force: true }).type("Hein{downarrow}{enter}");
      cy.get("button[name=submit-button]").should("be.visible").click();
      cy.contains("td", "Generic Steel Pants").siblings().find("[aria-label='edit standort']").click();
      cy.get('[type="checkbox"]').should("exist");
    });
    it("is available for users in role 'Administrator'", () => {
      cy.intercept("/standort*", standorteGemeinde);
      const standort = standorteGemeinde.find((s) => s.bezeichnung === "Generic Steel Pants");
      cy.intercept("/standort/" + standort.id, standort);
      cy.intercept("/user/self", userInRoleAdministrator);
      cy.visit("/");
      cy.get("div[name=gemeinde] input").should("be.visible").click({ force: true }).type("Hein{downarrow}{enter}");
      cy.get("button[name=submit-button]").should("be.visible").click();
      cy.contains("td", "Generic Steel Pants").siblings().find("[aria-label='edit standort']").click();
      cy.get('[type="checkbox"]').should("exist");
    });
  });

  describe("Edit and Delete Standort for Standort with Freigabe AfU", () => {
    it("is not available for users in role 'Extern'", () => {
      cy.intercept("/standort*", standorteGemeinde);
      cy.intercept("/user/self", userInRoleExtern);
      cy.visit("/");
      cy.get("div[name=gemeinde] input").should("be.visible").click({ force: true }).type("Hein{downarrow}{enter}");
      cy.get("button[name=submit-button]").should("be.visible").click();
      cy.get("[data-cy='edit-standort-35978-button']").should("be.disabled");
      cy.get("[data-cy='delete-standort-35978-button']").should("be.disabled");
    });
    it("is available for users in role 'SachbearbeiterAfU'", () => {
      cy.intercept("/standort*", standorteGemeinde);
      cy.intercept("/user/self", userInRoleSachbearbeiterAfu);
      cy.visit("/");
      cy.get("div[name=gemeinde] input").should("be.visible").click({ force: true }).type("Hein{downarrow}{enter}");
      cy.get("button[name=submit-button]").should("be.visible").click();
      cy.get("[data-cy='edit-standort-35978-button']").should("not.be.disabled");
      cy.get("[data-cy='delete-standort-35978-button']").should("not.be.disabled");
    });
    it("is available for users in role 'Administrator'", () => {
      cy.intercept("/standort*", standorteGemeinde);
      cy.intercept("/user/self", userInRoleAdministrator);
      cy.visit("/");
      cy.get("div[name=gemeinde] input").should("be.visible").click({ force: true }).type("Hein{downarrow}{enter}");
      cy.get("button[name=submit-button]").should("be.visible").click();
      cy.get("[data-cy='edit-standort-35978-button']").should("not.be.disabled");
      cy.get("[data-cy='delete-standort-35978-button']").should("not.be.disabled");
    });
  });
});
