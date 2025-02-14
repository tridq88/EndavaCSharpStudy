using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Appium;
using TechTalk.SpecFlow;

[Binding]
public class Hooks
{
    public static WindowsDriver<WindowsElement> g_DriverCalc;
    private static Process _driverPath;
    private static string _winAppPath;

    public Hooks()
    {
    }

    // This will be executed before each scenario
    [BeforeScenario]
    public static void StartCalculatorApp()
    {
        var options = new AppiumOptions();
        options.AddAdditionalCapability("app", "Microsoft.WindowsCalculator_8wekyb3d8bbwe!App");
        options.AddAdditionalCapability("platformName", "Windows");
        options.AddAdditionalCapability("deviceName", "WindowsPC");

        options.AddAdditionalCapability("logLevel", "debug");  // Enable debugging log

        try
        {
            // Start WindowsDriver with the options set above
            g_DriverCalc = new WindowsDriver<WindowsElement>(
                new Uri("http://127.0.0.1:4723"), options);
            g_DriverCalc.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            Console.WriteLine("Calculator app launched successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            Console.WriteLine("Stack Trace: " + ex.StackTrace);
            throw;
        }
    }

    // This will be executed after each scenario to clean up
    [AfterScenario]
    public static void CloseCalculatorApp()
    {
        try
        {
            g_DriverCalc?.Quit(); // Ensure that the driver is properly closed after each test
            Console.WriteLine("Calculator app closed successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error while closing app: " + ex.Message);
        }
    }

    [BeforeTestRun]
    public static void StartWinAppDriver()
    {
        try
        {
            _winAppPath = @"C:\Program Files (x86)\Windows Application Driver\WinAppDriver.exe";
            _driverPath = Process.Start(_winAppPath);
        }
        catch (Exception e)
        {
            Console.WriteLine("Could not locate WinAppDriver.exe, get it from " +
                "https://github.com/Microsoft/WinAppDriver/releases " +
                "and change the winAppPath in app.settings accordingly");
            throw new FileNotFoundException("Could not locate File WinAppDriver.exe", e);
        }
    }

    [AfterTestRun]
    public static void KillWinAppDriver()
    {
        _driverPath.Kill();
    }
}