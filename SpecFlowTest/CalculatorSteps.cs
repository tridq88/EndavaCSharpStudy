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
        Console.WriteLine("Adding two numbers: " + num1 + " and " + num2);
        // Find the buttons for the numbers and operations
        var button1 = _driver.FindElementByAccessibilityId("num" + num1.ToString() + "Button");
        var addButton = _driver.FindElementByName("Plus");
        var button2 = _driver.FindElementByAccessibilityId("num" + num2.ToString() + "Button");
        var equalsButton = _driver.FindElementByName("Equals");


        // Perform the operations by clicking the buttons
        try 
        {
            button1.Click();
            addButton.Click();
            button2.Click();
            equalsButton.Click();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            Console.WriteLine("Stack Trace: " + ex.StackTrace);
            throw;
        }

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
}
