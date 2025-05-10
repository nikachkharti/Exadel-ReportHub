@CRUD
Feature: Customer CRUD
  To manage customer in the system
  As an authorized user
  I want to create, read, update, and delete clients

Scenario Outline: Create a customer with detailed information
	Given I am authorized as owner
	When I create a customer with name "<name>", email "<email>", and country ID "<countryId>"
	Then the customer should be created successfully
    
	Examples:
		| name | email          | countryId                |
		| test | test@gmail.com | 680398332b140001219385be |

  
@CRUDFailure
Scenario: Create customer should fail
	Given I am authorized as super admin
	When I create a customer with name "<name>", email "<email>", and country ID "<countryId>" as super admin
	Then the response should be forbidden status code
    
	Examples:
		| name | email          | countryId                |
		| test | test@gmail.com | 680398332b140001219385be |

Scenario: Get a customer by ID
	Given I have created a customer
	When I request the customer by ID
	Then the response should contain the customer

Scenario: Delete a customer
	Given I have created a customer and got by id
	When I delete the customer
	Then The customer IsDeleted property should be true
