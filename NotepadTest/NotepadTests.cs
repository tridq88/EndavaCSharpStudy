using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;
using NUnit.Framework;
using System;
using System.Threading;

namespace NotepadPlusPlusAutomationTests
{
    [TestFixture]
    public class NotepadTests
    {
        private Application _notepadApp;
        private UIA3Automation _automation;

        [SetUp]
        public void SetUp()
        {
            // Log the start of the test setup
            Console.WriteLine("Attempting to launch Notepad++...");

            try
            {
                // Launch Notepad++
                _notepadApp = FlaUI.Core.Application.Launch("C:\\Program Files\\Notepad++\\notepad++.exe");
                _automation = new UIA3Automation();

                // Wait for the window to load and check if it opens successfully
                var window = _notepadApp.GetMainWindow(_automation);

                if (window != null)
                {
                    Console.WriteLine("Notepad++ launched successfully.");
                }
                else
                {
                    Console.WriteLine("Failed to retrieve the main window of Notepad++.");
                }
            }
            catch (Exception ex)
            {
                // Log any exception that occurs during launching
                Console.WriteLine($"Error launching Notepad++: {ex.Message}");
            }
        }

        [TearDown]
        public void TearDown()
        {
            // Close Notepad++ after the test
            _notepadApp.Close();
            _automation.Dispose();
        }

        [Test]
        public void TestClickSearchAndFind()
        {
            // Attach to the main window
            var window = _notepadApp.GetMainWindow(_automation);

            // Click on the "Search" tab and the "Find" item in the dropdown menu
            window.FindFirstDescendant(cf => cf.ByName("Search")).AsMenuItem().Click();
            Thread.Sleep(3000); // Optional sleep to see the dropdown menu

            // Wait for the "Find" menu item to be available
            window.FindFirstDescendant(cf => cf.ByName("Find...")).AsMenuItem().Click();

            // Wait for the "Find" dialog to appear
            var findDialog = window.FindFirstDescendant(cf => cf.ByName("Find")).AsWindow();
            Thread.Sleep(3000); // Optional sleep to see the dialog

            // Assert that the "Find" dialog has opened
            Assert.IsNotNull(findDialog, "The 'Find' dialog did not open.");
            Console.WriteLine("Test passed: 'Find' dialog opened successfully.");
        }


    }
}
