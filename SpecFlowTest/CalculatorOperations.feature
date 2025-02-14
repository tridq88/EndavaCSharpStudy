Feature: Calculator Operations
    In order to ensure the calculator works correctly
    As a user
    I want to add, subtract, multiply and divide two numbers

Background:
    Given I launch the Calculator app

@addition
Scenario: Adding two numbers
    When I add 5 and 3
    Then I should see the result as 8

@subtraction
Scenario: Subtracting two numbers
    When I subtract 5 from 3
    Then I should see the result as 2

@division @non_zero
Scenario: Dividing two numbers
    When I divide 6 by 2
    Then I should see the result as 3

@division @zero
Scenario: Dividing by zero
    When I divide 6 by 0
    Then I should see division by zero error message

@multiplication
Scenario Outline: Multiplying two numbers
    When I multiply <num1> by <num2>
    Then I should see the result as <expected_result>

    Examples:
        | num1 | num2 | expected_result |
        | 5    | 3    | 15              |
        | 6    | 2    | 12              |
        | 4    | 4    | 16              |
