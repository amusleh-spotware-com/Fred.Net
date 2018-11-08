using Fred.Net;
using System;
using System.Threading.Tasks;

namespace ConsoleTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Task.Run(async () =>
            {
                try
                {
                    Client client = new Client("eaa1d5cae31ccc11b9e5a0e807ffb618");

                    var result = await client.GetSources();

                    Console.WriteLine($"result Count: {result.Count}");
                    Console.WriteLine($"First result is: {result[0].Name}");
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