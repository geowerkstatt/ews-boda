describe('General app tests', () => {
  it('Displays the application version number', () => {
    cy.visit('/')

    cy.contains('Version: ')
  })
})
