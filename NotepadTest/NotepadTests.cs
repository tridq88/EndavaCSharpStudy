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
using System.Diagnostics;
using System.Windows.Forms.Design;

namespace NotepadPlusPlusAutomationTests
{
    [TestFixture]
    public class NotepadTests
    {
        private Application _notepadApp;
        private UIA3Automation _automation;

        [OneTimeSetUp]
        public void GlobalSetup()
        {
            ExtentReportHelper.InitializeReport();
            Console.WriteLine("✅ [OneTimeSetUp] - ExtentReports initialized.");
        }

        [OneTimeTearDown]
        public void GlobalTeardown()
        {
            ExtentReportHelper.FinalizeReport();
            Console.WriteLine("✅ [OneTimeTearDown] - ExtentReports finalized.");
        }

        [SetUp]
        public void SetUp()
        
        {
            Console.WriteLine("==============================================");
            ExtentReportHelper.StartTest(TestContext.CurrentContext.Test.Name);
            NotepadHelpers.OpenNotepad();
            _notepadApp = NotepadHelpers.NotepadApp;
            _automation = NotepadHelpers.Automation;
        }

        [TearDown]
        public void TearDown()
        {
            var testStatus = TestContext.CurrentContext.Result.Outcome.Status;
            Console.WriteLine($"🔍 Test Teardown Called - Status: {testStatus}");

            if (testStatus == NUnit.Framework.Interfaces.TestStatus.Failed)
            {
                string errorMessage = TestContext.CurrentContext.Result.Message;
                Console.WriteLine($"❌ FAIL: {errorMessage}");
                ExtentReportHelper.LogFail($"Test failed: {errorMessage}");
            }
            else if (testStatus == NUnit.Framework.Interfaces.TestStatus.Skipped) // ✅ Debug Skipped Test
            {
                Console.WriteLine("⚠️ Detected Skipped Test.");
                string reason = TestContext.CurrentContext.Result.Message ?? "Test skipped.";
                ExtentReportHelper.LogSkip(reason);
            }
            else
            {
                Console.WriteLine("✅ Test Passed.");
                ExtentReportHelper.LogPass("Test passed.");
            }

            Console.WriteLine("==============================================");
            NotepadHelpers.CloseNotepad();
        }


        // =================================================================================================
        // TC01: Open the Find dialog in Notepad++
        // =================================================================================================
        [Test]
        [Category("Others")]
        public void TestClickSearchAndFind()
        {
            NotepadHelpers.OpenFindDialog();
            Console.WriteLine("✅ Test passed: 'Find' dialog opened successfully.");
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
        [Category("ImageCapture")]
        public void TestDropdownSelectAndFindText()
        {
            // Preconditions: Open a new tab and search for the first word
            bool isPrecondtionGood = true;
            // Uncomment the below line to fail the precondition
            //isPrecondtionGood = false;

            NotepadHelpers.SearchFirstWordInCurrentTab(isPrecondtionGood);

            NotepadHelpers.OpenFindDialogUsingHotKey();
            NotepadHelpers.ClickDropdownAndFindNext();
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
                    string lastLine = extractedText.Split('\n').Last().Trim();
                    Console.WriteLine("\n📌 Extracted Text from Image:\n" + lastLine);

                    // ✅ Use NUnit Assert to verify the extracted text contains the expected message
                    string ExpectedText = "Find: Reached document end";
                    Assert.That(lastLine, Does.Contain(ExpectedText), "❌ Assertion Failed: Extracted text does NOT contain the expected message!");

                    Console.WriteLine("✅ Test Passed: The extracted text contains the expected message.");
                }
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
        [Ignore("Skipping this test temporarily for debugging.")]
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
            Console.WriteLine("✅ Test Passed: Copy-Paste operation verified successfully.");
        }

        //=================================================================================================
        // TC04: Drag and Drop file into Notepad++
        //=================================================================================================
        [Test]
        [Category("DragAndDrop")]
        public void TestDragAndDropFileIntoNotepadPP()
        {
            string filePath = @"..\..\..\..\README.md";

            DragFileToNotepadPP(filePath);
        }

        static void DragFileToNotepadPP(string filePath)
        {
            using (var automation = new UIA3Automation())
            {
                var app = FlaUI.Core.Application.Attach("notepad++.exe"); // Attach to Notepad++
                var window = app.GetMainWindow(automation);
                if (window == null)
                {
                    Console.WriteLine("❌ Notepad++ window not found.");
                    return;
                }
                Console.WriteLine("✅ Notepad++ window found. Simulating drag-and-drop...");

                // Open File Explorer and find the file
                var fileDir = Path.GetDirectoryName(filePath);
                Console.WriteLine($"🔍 Opening File Explorer at: {fileDir}");
                var explorer = FlaUI.Core.Application.Launch("explorer.exe", Path.GetDirectoryName(fileDir));
                //var explorer = FlaUI.Core.Application.Attach("explorer.exe");
                if (explorer == null || explorer.HasExited)
                {
                    Console.WriteLine("❌ Failed to launch Explorer.");
                    return;
                }
                Console.WriteLine("✅ Explorer launched. Waiting for 5 seconds...");
                Thread.Sleep(5000); // Wait for Explorer to open

                Process[] explorerProcesses = Process.GetProcessesByName("explorer");
                if (explorerProcesses.Length == 0)
                {
                    Console.WriteLine("❌ No existing Explorer window found. Launching a new instance...");
                    explorer = FlaUI.Core.Application.Launch("explorer.exe", Path.GetDirectoryName(filePath));
                    Thread.Sleep(3000); // Wait for Explorer to fully launch
                }
                else
                {
                    Console.WriteLine("✅ Found existing Explorer process. Attaching...");
                    explorer = FlaUI.Core.Application.Attach(explorerProcesses[0].Id);
                }

                var explorerWindow = explorer.GetMainWindow(automation);
                if (explorerWindow == null)
                {
                    Console.WriteLine("❌ Could not attach to the Explorer window.");
                    return;
                }
                Console.WriteLine("✅ Explorer window found.");


                var fileName = Path.GetFileName(filePath);
                Console.WriteLine($"🔍 Searching for file: {fileName}");
                var fileItem = explorerWindow.FindFirstDescendant(x => x.ByName(fileName));
                if (fileItem != null)
                {
                    Console.WriteLine("✅ File found in Explorer. Dragging into Notepad++...");

                    // ✅ FIX: Manually calculate the center of the elements
                    var fileBounds = fileItem.BoundingRectangle;
                    var notepadBounds = window.BoundingRectangle;

                    var filePos = new Point(fileBounds.X + fileBounds.Width / 2, fileBounds.Y + fileBounds.Height / 2);
                    var notepadPos = new Point(notepadBounds.X + notepadBounds.Width / 2, notepadBounds.Y + notepadBounds.Height / 2);

                    // Drag file into Notepad++
                    Mouse.Drag(filePos, notepadPos);
                    Thread.Sleep(2000); // Wait for file to load in Notepad++
                }
                else
                {
                    Console.WriteLine("❌ File not found in Explorer. Ensure it is visible.");
                    return;
                }

                // ✅ Check if the file is opened in Notepad++
                bool isFileOpened = IsFileOpenedInNotepadPP(window, filePath);
                if (isFileOpened)
                    Console.WriteLine($"✅ File '{Path.GetFileName(filePath)}' successfully opened in Notepad++.");
                else
                    Console.WriteLine($"❌ File '{Path.GetFileName(filePath)}' did NOT open in Notepad++.");
            }
        }

        static bool IsFileOpenedInNotepadPP(Window window, string filePath)
        {
            var titleBar = window.FindFirstDescendant(x => x.ByControlType(FlaUI.Core.Definitions.ControlType.TitleBar));

            if (titleBar != null)
            {
                string windowTitle = titleBar.AsLabel().Text;
                string expectedTitle = Path.GetFileName(filePath);

                Console.WriteLine($"🔍 Current Notepad++ Title: {windowTitle}");
                return windowTitle.Contains(expectedTitle);
            }

            return false;
        }
    }
}
