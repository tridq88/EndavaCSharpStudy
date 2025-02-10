Feature: Calculator Operations
    In order to ensure the calculator works correctly
    As a user
    I want to add two numbers

Scenario: Adding two numbers
    Given I launch the Calculator app
    When I add 5 and 3
    Then I should see the result as 8

