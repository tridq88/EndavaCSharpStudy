Feature: Calculator Operations
    In order to ensure the calculator works correctly
    As a user
    I want to add, subtract, multiply and divide two numbers

Scenario: Adding two numbers
    Given I launch the Calculator app
    When I add 5 and 3
    Then I should see the result as 8

Scenario: Dividing two numbers
    Given I launch the Calculator app
    When I divide 6 by 2
    Then I should see the result as 3

Scenario: Dividing by zero
    Given I launch the Calculator app
    When I divide 6 by 0
    Then I should see division by zero error message

Scenario Outline: Multiplying two numbers
    Given I launch the Calculator app
    When I multiply <num1> by <num2>
    Then I should see the result as <expected_result>

    Examples:
        | num1 | num2 | expected_result |
        | 5    | 3    | 15              |
        | 6    | 2    | 12              |
        | 4    | 4    | 16              |
