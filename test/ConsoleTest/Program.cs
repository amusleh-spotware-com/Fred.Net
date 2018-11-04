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
                    List<Tag> tags = await client.GetCategoryRelatedTags(125, new List<string> { "services", "quarterly" });

                    Console.WriteLine($"Count: {tags.Count}");
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