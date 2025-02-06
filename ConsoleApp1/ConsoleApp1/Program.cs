using System;
using Newtonsoft.Json;

namespace ConsoleApp1
{
    partial class Program
    {

        static void Main(string[] args)
        {
            //01
            //StringBuilderClass.StringBuilderFunc();

            //02
            //FileClass.FileFunc();

            //03.Compostion
            //var dbMigrator = new DbMigrator(new Logger());
            //var installer = new Installer(new Logger());
            //dbMigrator.Migrate();
            //installer.Install();

            //04.Nuget
            NugetFunc();


        }
    }
}