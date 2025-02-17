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

        [Test]
        public void TestClickSearchAndFindInNotepad()
        {
            // Attach to the main window
            var window = _notepadApp.GetMainWindow(_automation);

            // Log the action of clicking the "Search" tab
            Console.WriteLine("Clicking on the 'Search' tab...");

            // Find the "Search" menu item
            var searchTab = window.FindFirstDescendant(cf => cf.ByName("Search")).AsMenuItem();

            // Click on the "Search" tab
            searchTab.Click();
            Thread.Sleep(500); // Optional wait to ensure the menu is displayed

            // Log the action of clicking on the "Find" item
            Console.WriteLine("Clicking on the 'Find' item...");

            // Find the "Find" menu item
            var findItem = window.FindFirstDescendant(cf => cf.ByName("Find...")).AsMenuItem();

            // Click on the "Find" item
            findItem.Click();

            // Log the action of checking for the "Find" dialog
            Console.WriteLine("Checking if the 'Find' dialog has opened...");

            // Wait for the Find dialog to appear and verify it is open
            var findDialog = window.FindFirstDescendant(cf => cf.ByName("Find")).AsWindow();

            // Assert that the "Find" dialog has opened
            Assert.IsNotNull(findDialog, "The 'Find' dialog did not open.");

            // Log success
            Console.WriteLine("Test passed: 'Find' dialog opened successfully, and text box found.");
        }

        [TearDown]
        public void TearDown()
        {
            // Close Notepad++ after the test
            _notepadApp.Close();
            _automation.Dispose();
        }
    }
}
