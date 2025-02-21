
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
            if (findDialog == null)
            {
                throw new Exception("The 'Find' dialog did not open.");
            }
        }

        public static void OpenFindDialogUsingHotKey()
        {
            // Attach to the main window
            var window = NotepadApp.GetMainWindow(Automation);

            // ✅ Press Ctrl + F to open the Find dialog
            Keyboard.Press(VirtualKeyShort.CONTROL);
            Keyboard.Press(VirtualKeyShort.KEY_F);
            Keyboard.Release(VirtualKeyShort.KEY_F);
            Keyboard.Release(VirtualKeyShort.CONTROL);

            // ✅ Wait for the "Find" dialog to appear
            Thread.Sleep(3000);  // Small delay to allow UI to update

            // ✅ Locate the "Find" dialog
            var findDialog = window.FindFirstDescendant(cf => cf.ByName("Find"))?.AsWindow();

            // ✅ Assert that the "Find" dialog has opened
            Assert.IsNotNull(findDialog, "❌ The 'Find' dialog did not open.");
            Console.WriteLine("✅ The 'Find' dialog successfully opened using Ctrl + F.");
        }

        public static void SearchFirstWordInCurrentTab(bool isPrecondtionGood)
        {
            var window = NotepadApp.GetMainWindow(Automation);
            if (window == null)
            {
                throw new Exception("❌ Notepad++ main window not found.");
            }

            // ✅ Locate the Notepad++ text editor
            var textEditor = window.FindFirstDescendant(cf => cf.ByClassName("Scintilla"));
            if (textEditor == null)
            {
                throw new Exception("❌ Text editor not found.");
            }

            // ✅ Get the current text from Notepad++
            string text = textEditor.AsTextBox().Text;
            if (string.IsNullOrEmpty(text))
            {
                throw new Exception("❌ No text found in the editor.");
            }

            // ✅ Extract the first word
            string firstWord = text.Split(new[] { ' ', '\t', '\n' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            if (string.IsNullOrEmpty(firstWord))
            {
                throw new Exception("❌ No first word found.");
            }
            Console.WriteLine($"✅ First word found: {firstWord}");

            // ✅ Open "Find" dialog using Ctrl + F
            Keyboard.Press(VirtualKeyShort.CONTROL);
            Keyboard.Press(VirtualKeyShort.KEY_F);
            Keyboard.Release(VirtualKeyShort.KEY_F);
            Keyboard.Release(VirtualKeyShort.CONTROL);
            Thread.Sleep(500);

            // ✅ Locate the "Find what:" text box
            var findDialog = window.FindFirstDescendant(cf => cf.ByName("Find"));
            if (findDialog == null)
            {
                throw new Exception("❌ Find dialog not found.");
            }

            var findTextBox = findDialog.FindFirstDescendant(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.Edit));
            if (findTextBox == null)
            {
                throw new Exception("❌ Find input box not found.");
            }

            // ✅ Enter the first word into the search box
            if (isPrecondtionGood)
            {
                findTextBox.AsTextBox().Enter(firstWord);
            }
            else
            {
                findTextBox.AsTextBox().Enter("InvalidWord");
            }
            Thread.Sleep(500);

            // ✅ Click "Find Next" button
            var findNextButton = findDialog.FindFirstDescendant(cf => cf.ByName("Find Next"));
            if (findNextButton == null)
            {
                throw new Exception("❌ 'Find Next' button not found.");
            }
            findNextButton.AsButton().Invoke();
            Thread.Sleep(500);

            Console.WriteLine($"✅ Search completed for the word: {firstWord}");
        }

        public static void ClickDropdownAndFindNext()
        {
            using (var automation = new UIA3Automation())
            {
                // ✅ Step 1: Attach to Notepad++'s "Find" Dialog
                var app = FlaUI.Core.Application.Attach("notepad++.exe"); // Attach to Notepad++
                var window = app.GetMainWindow(automation).FindFirstDescendant(x => x.ByName("Find"));

                Assert.IsNotNull(window, "❌ Find dialog not found! Make sure it is open.");
                Console.WriteLine("✅ Find dialog detected.");

                // ✅ Step 2: Find and Click the "Dropdown" Button
                var dropdownButton = window.FindFirstDescendant(x => x.ByAutomationId("DropDown"));

                Assert.IsNotNull(dropdownButton, "❌ Dropdown button not found!");
                dropdownButton.AsButton().Invoke();
                Console.WriteLine("✅ Clicked 'Dropdown' button.");

                // ✅ Step 3: Wait for UI to update
                Thread.Sleep(500);

                // ✅ Step 4: Find and Click the "Find Next" Button
                var findNextButton = window.FindFirstDescendant(x => x.ByName("Find Next"));

                Assert.IsNotNull(findNextButton, "❌ 'Find Next' button not found!");
                findNextButton.AsButton().Invoke();
                Console.WriteLine("✅ Clicked 'Find Next' button.");
            }
        }
    }
}
