using System;
using System.Threading.Tasks;

namespace MerchantClient.src
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var menu = new Menu();
            await menu.RunAsync();
        }
    }
}
