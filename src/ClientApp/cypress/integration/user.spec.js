import users from "../fixtures/users.json";

describe("User page tests", () => {
  it("Show all users", function () {
    cy.intercept("/user", users);
    cy.visit("/benutzerverwaltung");
    cy.contains("Benutzerverwaltung");
  });

  it("Show mapped role names", function () {
    cy.intercept("/user", users);
    cy.visit("/benutzerverwaltung");

    cy.contains("Administrator").should("exist");
    cy.contains("Extern").should("exist");
    cy.contains("Sachbearbeiter_AfU").should("exist");
  });

  it("Delete a specific user", function () {
    cy.intercept("/user", users);
    cy.intercept("DELETE", "/user?id=80002", { statusCode: 200 });

    cy.visit("/benutzerverwaltung");

    cy.get("tbody").children().should("have.length", 4);
    cy.contains("Simon_Felber").should("exist");

    cy.contains("td", "Simon_Felber").siblings().find("[aria-label='delete user']").click();
    cy.get('button:contains("Löschen")').click();

    cy.get("tbody").children().should("have.length", 3);
    cy.contains("Simon_Felber").should("not.exist");
  });

  it("Change role for an existing user", function () {
    cy.intercept("/user", users);
    cy.intercept("PUT", "/user?id=80003", { statusCode: 200 });

    cy.visit("/benutzerverwaltung");

    cy.contains("td", "Cornelia.Walter").parent().contains("td", "Sachbearbeiter_AfU").should("exist");
    cy.contains("td", "Cornelia.Walter").siblings().find("[aria-label='edit user']").click();

    cy.get("[data-cy='select-user-role']").click();
    cy.get("[data-cy='role-id-0']").click();

    cy.get('button:contains("Speichern")').click();
    cy.contains("td", "Cornelia.Walter").parent().contains("td", "Administrator").should("exist");
  });
});
