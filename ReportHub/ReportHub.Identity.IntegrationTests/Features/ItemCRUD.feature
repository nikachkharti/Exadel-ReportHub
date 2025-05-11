@CRUD
Feature: item CRUD
  To manage item in the system
  As an authorized user
  I want to create, read, update, and delete items

Scenario Outline: Create a item with detailed information
	Given I am authorized as owner for item crud
	When I create a item with name "<name>", description "<description>", price "<price>", and currency "<currency>"
	Then the item should be created successfully
    
	Examples:
		| name | description  | price | currency |
		| test | testing item | 1000  | USD      |

  
@CRUDFailure
Scenario: Create item should fail
	Given I am authorized as super admin for item crud
	When I create a item with name "<name>", description "<description>", price "<price>", and currency "<currency>" as super admin
	Then the response should be forbidden status code for item creation
    
	Examples:
		| name | description  | price | currency |
		| test | testing item | 1000  | USD      |

Scenario: Get a item by ID
	Given I have created a item
	When I request the item by ID
	Then the response should contain the item

Scenario: Delete a item
	Given I have created a item and got by id
	When I delete the item
	Then The item IsDeleted property should be true
