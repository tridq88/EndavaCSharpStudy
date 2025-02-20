
using FlaUI.UIA3;
using FlaUI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
