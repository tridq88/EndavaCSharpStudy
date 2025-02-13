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
    private WindowsDriver<WindowsElement> _driver;

    // Launch Calculator app
    [Given("I launch the Calculator app")]
    public void GivenILaunchTheCalculatorApp()
    {
        var options = new AppiumOptions();
        options.AddAdditionalCapability("app", "Microsoft.WindowsCalculator_8wekyb3d8bbwe!App");
        options.AddAdditionalCapability("platformName", "Windows");
        options.AddAdditionalCapability("deviceName", "WindowsPC");

        options.AddAdditionalCapability("logLevel", "debug");  // Enable debugging log

        try
        {
            _driver = new WindowsDriver<WindowsElement>(
                new Uri("http://127.0.0.1:4723"), options);
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            Console.WriteLine("Calculator app launched successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            Console.WriteLine("Stack Trace: " + ex.StackTrace);
            throw;
        }

    }

    // Perform addition
    [When("I add (.*) and (.*)")]
    public void WhenIAddTwoNumbers(int num1, int num2)
    {
        // Find the buttons for the numbers and operations
        var button1 = _driver.FindElementByAccessibilityId("num" + num1.ToString() + "Button");
        var addButton = _driver.FindElementByName("Plus");
        var button2 = _driver.FindElementByAccessibilityId("num" + num2.ToString() + "Button");
        var equalsButton = _driver.FindElementByName("Equals");
        // Perform the operations by clicking the buttons
        button1.Click();
        addButton.Click();
        button2.Click();
        equalsButton.Click();
    }

    // Verify the result
    [Then("I should see the result as (.*)")]
    public void ThenIShouldSeeTheResult(int expectedResult)
    {
        // Get the result from the Calculator's display
        var result = _driver.FindElementByAccessibilityId("CalculatorResults").Text;
        Assert.AreEqual($"Display is {expectedResult}", result);

        // Clean up the driver after the test
        _driver.Quit();
    }

    // Perform subtraction
    [When("I subtract (.*) from (.*)")]
    public void WhenISubtractTwoNumbers(int num1, int num2)
    {
        // Find the buttons for the numbers and operations
        var button1 = _driver.FindElementByAccessibilityId("num" + num1.ToString() + "Button");
        var subButton = _driver.FindElementByName("Minus");
        var button2 = _driver.FindElementByAccessibilityId("num" + num2.ToString() + "Button");
        var equalsButton = _driver.FindElementByName("Equals");
        // Perform the operations by clicking the buttons
        button1.Click();
        subButton.Click();
        button2.Click();
        equalsButton.Click();
    }

    // Perform division
    [When("I divide (.*) by (.*)")]
    public void WhenIDivideTwoNumbers(int num1, int num2)
    {
        // Find the buttons for the numbers and operations
        var button1 = _driver.FindElementByAccessibilityId("num" + num1.ToString() + "Button");
        var divButton = _driver.FindElementByName("Divide by");
        var button2 = _driver.FindElementByAccessibilityId("num" + num2.ToString() + "Button");
        var equalsButton = _driver.FindElementByName("Equals");
        // Perform the operations by clicking the buttons
        button1.Click();
        divButton.Click();
        button2.Click();
        equalsButton.Click();
    }

    // Verify the division by 0 result
    [Then("I should see division by zero error message")]
    public void ThenIShouldSeeTheDivisionByZeroErrorMessage()
    {
        // Get the result from the Calculator's display
        var result = _driver.FindElementByAccessibilityId("CalculatorResults").Text;
        var expectedResult = "Cannot divide by zero";
        Assert.AreEqual($"Display is {expectedResult}", result);

        // Clean up the driver after the test
        _driver.Quit();
    }

    // Perform multiplication
    [When("I multiply (.*) by (.*)")]
    public void WhenIMultiplyTwoNumbers(int num1, int num2)
    {
        // Find the buttons for the numbers and operations
        var button1 = _driver.FindElementByAccessibilityId("num" + num1.ToString() + "Button");
        var mulButton = _driver.FindElementByName("Multiply by");
        var button2 = _driver.FindElementByAccessibilityId("num" + num2.ToString() + "Button");
        var equalsButton = _driver.FindElementByName("Equals");
        // Perform the operations by clicking the buttons
        button1.Click();
        mulButton.Click();
        button2.Click();
        equalsButton.Click();
    }
}
