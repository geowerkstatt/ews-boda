Cypress.on("uncaught:exception", (err, runnable, promise) => {
  // Turn off promise rejection errors because the API does
  // not get mocked or started during the e2e tests.
  if (promise) {
    return false;
  }
});

// The Gemeinde options load asynchronously from /standort after the home page mounts,
// so wait for the option to render before selecting it instead of pressing Enter blindly.
Cypress.Commands.add("selectGemeinde", (searchText, gemeinde) => {
  // use click 'force: true' because element is covered by another element.
  cy.get("div[name=gemeinde] input").should("be.visible").click({ force: true }).type(searchText);
  cy.contains("li[role=option]", gemeinde).click();
  cy.get("div[name=gemeinde] input").should("have.value", gemeinde);
});
