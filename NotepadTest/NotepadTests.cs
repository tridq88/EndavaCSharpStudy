using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.WindowsAPI;
using FlaUI.UIA3;
using NUnit.Framework;
using System;
using System.Drawing.Imaging;
using System.Drawing;
using System.Threading;
using System.Runtime.InteropServices;
using Tesseract;
using FlaUI.Core.Conditions;
using FlaUI.UIA3.Patterns;

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


        // =================================================================================================
        // TC01: Open the Find dialog in Notepad++
        // =================================================================================================
        [Test]
        public void TestClickSearchAndFind()
        {
            OpenFindDialog();
            Console.WriteLine("Test passed: 'Find' dialog opened successfully.");
        }

        private void OpenFindDialog()
        {
            // Attach to the main window
            var window = _notepadApp.GetMainWindow(_automation);

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

        // =================================================================================================
        // TC02: Find text in Notepad++
        // =================================================================================================
        // Import Windows API to find and get window position
        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left, Top, Right, Bottom;
        }

        [Test]
        public void TestDropdownSelectAndFindText()
        {
            OpenFindDialog();
            ClickDropdownAndFindNext();
            Thread.Sleep(3000); // Optional sleep to see the find action result

            string windowTitle = "Find";  // Change this to the exact window title
            IntPtr hWnd = FindWindow(null, windowTitle);

            if (hWnd == IntPtr.Zero)
            {
                Console.WriteLine("❌ Find dialog not found!");
                return;
            }

            if (GetWindowRect(hWnd, out RECT rect))
            {
                int width = rect.Right - rect.Left;
                int height = rect.Bottom - rect.Top;

                using (Bitmap bitmap = new Bitmap(width, height))
                {
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.CopyFromScreen(rect.Left, rect.Top, 0, 0, new Size(width, height));
                    }

                    string imagePath = @"C:\Temp\FindDialogScreenshot.png";
                    bitmap.Save(imagePath, System.Drawing.Imaging.ImageFormat.Png);
                    Console.WriteLine($"✅ Screenshot saved at: {imagePath}");

                    // Extract text using Tesseract OCR
                    ExtractTextFromImage(imagePath);
                }
            }
        }

        private void ClickDropdownAndFindNext()
        {
            using (var automation = new UIA3Automation())
            {
                // ✅ Step 1: Attach to Notepad++'s "Find" Dialog
                var app = FlaUI.Core.Application.Attach("notepad++.exe"); // Attach to Notepad++
                var window = app.GetMainWindow(automation).FindFirstDescendant(x => x.ByName("Find"));

                if (window == null)
                {
                    Console.WriteLine("❌ Find dialog not found! Make sure it is open.");
                    return;
                }

                Console.WriteLine("✅ Find dialog detected.");

                // ✅ Step 2: Find and Click the "Dropdown" Button
                var dropdownButton = window.FindFirstDescendant(x => x.ByAutomationId("DropDown"));

                if (dropdownButton == null)
                {
                    Console.WriteLine("❌ Dropdown button not found!");
                    return;
                }

                dropdownButton.AsButton().Invoke();
                Console.WriteLine("✅ Clicked 'Dropdown' button.");

                // ✅ Step 3: Wait for UI to update
                Thread.Sleep(500);

                // ✅ Step 4: Find and Click the "Find Next" Button
                var findNextButton = window.FindFirstDescendant(x => x.ByName("Find Next"));

                if (findNextButton == null)
                {
                    Console.WriteLine("❌ 'Find Next' button not found!");
                    return;
                }

                findNextButton.AsButton().Invoke();
                Console.WriteLine("✅ Clicked 'Find Next' button.");
            }
        }

        static void ExtractTextFromImage(string imagePath)
        {
            string tessDataPath = @"C:\Program Files\Tesseract-OCR\tessdata";  // Update this to your Tesseract path

            try
            {
                using (var engine = new TesseractEngine(tessDataPath, "eng", EngineMode.Default))
                {
                    using (var img = Pix.LoadFromFile(imagePath))
                    {
                        using (var page = engine.Process(img))
                        {
                            string extractedText = page.GetText();
                            Console.WriteLine("\n📌 Extracted Text from Image:\n" + extractedText);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error: " + ex.Message);
            }
        }
    }
}
