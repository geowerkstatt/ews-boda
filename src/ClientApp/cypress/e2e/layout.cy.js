import userInRoleExtern from "../fixtures/user_in_role_extern.json";
import userInRoleSachbearbeiterAfu from "../fixtures/user_in_role_sachbearbeiter_afu.json";
import userInRoleAdministrator from "../fixtures/user_in_role_administrator.json";

describe("General app tests", () => {
  it("Downloads the comma-separated (CSV) file", () => {
    cy.intercept("/export", (request) => {
      request.reply({
        headers: { "Content-Disposition": "attachment; filename=data_export.csv" },
        fixture: "data_export.csv",
      });
    });

    cy.visit("/");
    cy.get('a[href*="/export"]').click();
    cy.readFile("cypress/downloads/data_export.csv", "utf8", { timeout: 10000 }).should(
      "contain",
      "standort.standort_id,standort.bezeichnung",
    );
  });

  describe("Benutzerverwaltung", () => {
    it("is not available for users in role 'Extern'", () => {
      cy.intercept("/user/self", userInRoleExtern);
      cy.visit("/");
      cy.contains("Benutzerverwaltung").should("not.exist");
    });
    it("is not available for users in role 'SachbearbeiterAfU'", () => {
      cy.intercept("/user/self", userInRoleSachbearbeiterAfu);
      cy.visit("/");
      cy.contains("Benutzerverwaltung").should("not.exist");
    });
    it("is available for users in role 'Administrator'", () => {
      cy.intercept("/user/self", userInRoleAdministrator);
      cy.visit("/");
      cy.contains("Benutzerverwaltung").should("exist");
    });
  });
});
