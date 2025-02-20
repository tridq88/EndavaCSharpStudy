
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Input;
using FlaUI.Core.WindowsAPI;
using FlaUI.UIA3;
using NUnit.Framework;
using System;
using System.Drawing.Imaging;
using System.Drawing;
using System.Threading;
using System.Runtime.InteropServices;
using Tesseract;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace NotepadPlusPlusAutomationTests
{
    class NotepadHelpers
    {
        public static Application NotepadApp { get; private set; }
        public static UIA3Automation Automation { get; private set; }

        public static void OpenNotepad()
        {
            // Log the start of the test setup
            Console.WriteLine("Attempting to launch Notepad++...");

            try
            {
                // Launch Notepad++
                NotepadApp = FlaUI.Core.Application.Launch("C:\\Program Files\\Notepad++\\notepad++.exe");
                Automation = new UIA3Automation();

                // Wait for the window to load and check if it opens successfully
                var window = NotepadApp.GetMainWindow(Automation);

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

        public static void CloseNotepad()
        {
            // Close Notepad++ after the test
            NotepadApp.Close();
            Automation.Dispose();
        }

        public static void OpenFindDialog()
        {
            // Attach to the main window
            var window = NotepadApp.GetMainWindow(Automation);

            // Click on the "Search" tab and the "Find" item in the dropdown menu
            window.FindFirstDescendant(cf => cf.ByName("Search")).AsMenuItem().Click();

            // Wait for the "Find" menu item to be available
            window.FindFirstDescendant(cf => cf.ByName("Find...")).AsMenuItem().Click();

            // Wait for the "Find" dialog to appear
            var findDialog = window.FindFirstDescendant(cf => cf.ByName("Find")).AsWindow();
            Thread.Sleep(3000); // Optional sleep to see the dialog

            // Assert that the "Find" dialog has opened
            Assert.IsNotNull(findDialog, "The 'Find' dialog did not open.");
        }
    }
}
