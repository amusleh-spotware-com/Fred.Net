using Fred.Net;
using System;
using System.Threading.Tasks;
using Fred.Net.Parameters;

namespace ConsoleTest
{
    internal class Program
    {
        static string _apiKey = "";

        private static void Main(string[] args)
        {
            Task.Run(async () =>
            {
                try
                {
                    Client client = new Client(_apiKey);

                    var parameters = new SourcesParameters();

                    var result = await client.GetSources(parameters);

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