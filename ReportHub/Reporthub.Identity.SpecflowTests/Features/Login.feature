Feature: Full Login Flow
  As a user
  I want to log in and switch context
  So that I can access client-specific data

  Scenario: Login with valid credentials
    Given I have credentials "admin@example.com" and "Admin123$"
    When I send a login request
    Then I receive an access token

  Scenario: Retrieve the list of clients after login
    Given I have valid credentials and I have received an access token
    When I request the list of my clients
    Then I receive a list of clients

  Scenario: Switch context for a client
    Given I have valid credentials, I have received an access token, and I have a client ID
    When I send a switch-context request
    Then I receive a token scoped to that client
