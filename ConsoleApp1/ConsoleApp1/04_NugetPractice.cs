using Newtonsoft.Json;

namespace ConsoleApp1
{
    partial class Program
    {
        class Person()
        {
            public String name { get; set; }
            public int age { get; set; }
        }

        static void NugetFunc()
        {
            var person = new Person()
            {
                name = "TRI",
                age = 36
            };
            string json = JsonConvert.SerializeObject(person);
            Console.WriteLine(json);
            var desJson = JsonConvert.DeserializeObject<Person>(json);
            Console.WriteLine(desJson.name + "-" + desJson.age);
        }
    }
}