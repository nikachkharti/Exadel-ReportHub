@ClientCRUD
Feature: Client CRUD
  To manage clients in the system
  As an authorized user
  I want to create, read, update, and delete clients

  Scenario: Create a new client
    Given I am authorized as super admin for client crud
    When I create a client with name "Acme Corp" and specialization "Logistics"
    Then the client should be created successfully
  
  @CreateClientFailure
  Scenario: Create client should fail
	Given I am authorized as owner for client crud
	When I create a client with name "Acme Corp" and specialization "Logistics" as owner
	Then the response should be forbidden status code while client creation

  Scenario: Get a client by ID
    Given I have created a client
    When I request the client by ID
    Then the response should contain the client

  Scenario: Delete a client
    Given I have created a client and got by id
    When I delete the client
    Then The client IsDeleted property should be true
