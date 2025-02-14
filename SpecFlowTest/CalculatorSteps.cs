using System;
using TechTalk.SpecFlow;
using NUnit.Framework;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium;
using System.Threading;
using OpenQA.Selenium.Support.UI;

[Binding]
public class CalculatorSteps
{
    private WindowsDriver<WindowsElement> _driver = Hooks.g_DriverCalc;

    // Launch Calculator app
    [Given("I launch the Calculator app")]
    public void GivenILaunchTheCalculatorApp()
    {
        // Check if the Calculator app is launched
        Assert.IsNotNull(_driver);
    }

    // Perform addition
    [When("I add (.*) and (.*)")]
    public void WhenIAddTwoNumbers(int num1, int num2)
    {
        PerformOperation(num1, "Plus", num2);
    }

    // Perform subtraction
    [When("I subtract (.*) from (.*)")]
    public void WhenISubtractTwoNumbers(int num1, int num2)
    {
        PerformOperation(num1, "Minus", num2);
    }

    // Perform division
    [When("I divide (.*) by (.*)")]
    public void WhenIDivideTwoNumbers(int num1, int num2)
    {
        PerformOperation(num1, "Divide by", num2);
    }

    // Perform multiplication
    [When("I multiply (.*) by (.*)")]
    public void WhenIMultiplyTwoNumbers(int num1, int num2)
    {
        PerformOperation(num1, "Multiply by", num2);
    }

    // Verify the result
    [Then("I should see the result as (.*)")]
    public void ThenIShouldSeeTheResult(int expectedResult)
    {
        VerifyResult($"Display is {expectedResult}");
    }

    // Verify the division by 0 result
    [Then("I should see division by zero error message")]
    public void ThenIShouldSeeTheDivisionByZeroErrorMessage()
    {
        VerifyResult("Display is Cannot divide by zero");
    }

    // Helper method to perform operations
    private void PerformOperation(int num1, string operation, int num2)
    {
        var button1 = _driver.FindElementByAccessibilityId("num" + num1.ToString() + "Button");
        var operationButton = _driver.FindElementByName(operation);
        var button2 = _driver.FindElementByAccessibilityId("num" + num2.ToString() + "Button");
        var equalsButton = _driver.FindElementByName("Equals");

        button1.Click();
        operationButton.Click();
        button2.Click();
        equalsButton.Click();
    }

    // Helper method to verify results
    private void VerifyResult(string expectedResult)
    {
        var result = _driver.FindElementByAccessibilityId("CalculatorResults").Text;
        Assert.AreEqual(expectedResult, result);

        // Clean up the driver after the test
        _driver.Quit();
    }
}
