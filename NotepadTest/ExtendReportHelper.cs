using System;
using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;

namespace NotepadPlusPlusAutomationTests 
{
    public class ExtentReportHelper
    {
        private static ExtentReports _extent;
        private static ExtentTest _test;
        // ✅ Save report in the project root folder instead of "bin/Debug"
        private static string reportPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "TestReport.html");

        public static void InitializeReport()
        {
            try
            {
                var sparkReporter = new ExtentSparkReporter(reportPath);
                _extent = new ExtentReports();
                _extent.AttachReporter(sparkReporter);
                Console.WriteLine($"✅ ExtentReports initialized. Report will be saved at: {reportPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error initializing ExtentReports: {ex.Message}");
            }
        }

        public static void StartTest(string testName)
        {
            _test = _extent.CreateTest(testName);
            Console.WriteLine($"✅ Test started: {testName}");
        }

        public static void LogPass(string message)
        {
            _test.Pass(message);
            Console.WriteLine($"✅ PASS: {message}");
        }

        public static void LogFail(string message)
        {
            _test.Fail(message);
            Console.WriteLine($"❌ FAIL: {message}");
        }

        public static void FinalizeReport()
        {
            try
            {
                _extent.Flush();
                Console.WriteLine($"✅ ExtentReports flushed. Report saved at: {reportPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error flushing ExtentReports: {ex.Message}");
            }
        }
}
}