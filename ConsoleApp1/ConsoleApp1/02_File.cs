using System.IO;

namespace ConsoleApp1
{
    class FileClass
    {
        public static void FileFunc()
        {
            var filePath = @".\..\..\..\02_SourceFile.txt";
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    Console.WriteLine(line);
                }
            }
        }
    }
}
