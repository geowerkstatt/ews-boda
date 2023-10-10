Cypress.on("uncaught:exception", (err, runnable, promise) => {
  // Turn off promise rejection errors because the API does
  // not get mocked or started during the e2e tests.
  if (promise) {
    return false;
  }
});
