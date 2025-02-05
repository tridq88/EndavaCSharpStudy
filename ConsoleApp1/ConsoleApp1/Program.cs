using System;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            //01
            //StringBuilderClass.StringBuilderFunc();
            
            //02
            //FileClass.FileFunc();

            //03.Compostion
            var dbMigrator = new DbMigrator(new Logger());
            var installer = new Installer(new Logger());
            dbMigrator.Migrate();
            installer.Install();

        }
    }
}