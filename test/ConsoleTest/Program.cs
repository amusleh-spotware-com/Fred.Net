using Fred.Net;
using System;
using System.Threading.Tasks;

namespace ConsoleTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Client client = new Client("eaa1d5cae31ccc11b9e5a0e807ffb618");

            Task.Run(async () =>
            {
                try
                {
                    var result = await client.GetTagsSeries(new System.Collections.Generic.List<string> { "slovenia", "food", "oecd" });

                    Console.WriteLine($"Count: {result.Count}");

                    Console.WriteLine($"Id: {result[0].Id}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(ex.Message);
                }
            });

            Console.ReadLine();
        }
    }
}