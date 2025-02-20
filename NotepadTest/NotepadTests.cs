﻿using FlaUI.Core;
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
            OpenFindDialogUsingHotKey();
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
                    string extractedText = ExtractTextFromImage(imagePath);

                    Console.WriteLine("\n📌 Extracted Text from Image:\n" + extractedText);

                    // ✅ Use NUnit Assert to verify the extracted text contains the expected message
                    string ExpectedText = "Find: Reached document end";
                    Assert.That(extractedText, Does.Contain(ExpectedText), "❌ Assertion Failed: Extracted text does NOT contain the expected message!");

                    Console.WriteLine("✅ Test Passed: The extracted text contains the expected message.");
                }
            }
        }

        private void OpenFindDialogUsingHotKey()
        {
            // Attach to the main window
            var window = _notepadApp.GetMainWindow(_automation);

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

        private string ExtractTextFromImage(string imagePath)
        {
            string tessDataPath = @"C:\Program Files\Tesseract-OCR\tessdata";
            try
            {
                using (var engine = new TesseractEngine(tessDataPath, "eng", EngineMode.Default))
                {
                    using (var img = Pix.LoadFromFile(imagePath))
                    {
                        using (var page = engine.Process(img))
                        {
                            return page.GetText().Trim();  // Trim extra spaces/newlines
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Assert.Fail($"❌ OCR extraction failed: {ex.Message}");
                return string.Empty;  // This will never be reached due to Assert.Fail()
            }
        }

        //=================================================================================================
        // TC03: Copy text from one file to another
        //=================================================================================================
        [Test]
        public void TestCopyTextFromOneFileToAnother()
        {
            // Attach to the main window
            var window = _notepadApp.GetMainWindow(_automation);

            // ✅ Locate the text editor (Using Scintilla ClassName)
            var textEditor = window.FindFirstDescendant(cf => cf.ByClassName("Scintilla"));
            Assert.IsNotNull(textEditor, "❌ Text editor not found.");

            // ✅ Ensure Notepad++ text editor is focused
            textEditor.Focus();
            Thread.Sleep(500);

            // ✅ Step 1: Open a new tab (Ctrl + N)
            Keyboard.Press(VirtualKeyShort.CONTROL);
            Keyboard.Press(VirtualKeyShort.KEY_N);
            Keyboard.Release(VirtualKeyShort.KEY_N);
            Keyboard.Release(VirtualKeyShort.CONTROL);
            Thread.Sleep(2000);

            // ✅ Step 2: Enter new text into the first tab
            string textToCopy = "Hello, this is a test for copying text!";
            textEditor.AsTextBox().Enter(textToCopy);
            Thread.Sleep(2000);

            // ✅ Step 3: Copy text (Ctrl + A, Ctrl + C)
            Keyboard.Press(VirtualKeyShort.CONTROL);
            Keyboard.Press(VirtualKeyShort.KEY_A);
            Keyboard.Release(VirtualKeyShort.KEY_A);
            Thread.Sleep(200);
            Keyboard.Press(VirtualKeyShort.KEY_C);
            Keyboard.Release(VirtualKeyShort.KEY_C);
            Keyboard.Release(VirtualKeyShort.CONTROL);
            Console.WriteLine("✅ Text copied successfully.");

            // ✅ Step 4: Open a new tab (Ctrl + N)
            Keyboard.Press(VirtualKeyShort.CONTROL);
            Keyboard.Press(VirtualKeyShort.KEY_N);
            Keyboard.Release(VirtualKeyShort.KEY_N);
            Keyboard.Release(VirtualKeyShort.CONTROL);
            Thread.Sleep(2000);

            // ✅ Step 5: Paste text (Ctrl + V)
            Keyboard.Press(VirtualKeyShort.CONTROL);
            Keyboard.Press(VirtualKeyShort.KEY_V);
            Keyboard.Release(VirtualKeyShort.KEY_V);
            Keyboard.Release(VirtualKeyShort.CONTROL);
            Thread.Sleep(2000);
            Console.WriteLine("✅ Text pasted into the second file.");

            // ✅ Step 6: Verify copied text
            var newTextEditor = window.FindFirstDescendant(cf => cf.ByClassName("Scintilla"));
            string pastedText = newTextEditor.AsTextBox().Text;
            Assert.That(pastedText, Is.EqualTo(textToCopy), "❌ Copied text does not match the original.");
            Console.WriteLine("✅ Copy-Paste operation verified successfully.");
        }
    }
}
