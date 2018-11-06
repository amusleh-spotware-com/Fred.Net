using Fred.Net;
using Fred.Net.Types;
using System;
using System.Collections.Generic;
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
                    List<VintageDate> result = await client.GetSeriesVintageDates("GNPCA");

                    Console.WriteLine($"Count: {result.Count}");

                    Console.WriteLine($"Id: {result[0].Date}");
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