using MongodB.Poc.Concurrency;

namespace MongodB.Poc.Gifts // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            await OptimisticConcurrency();
            Console.ReadLine();
        }

        private static async Task DocumentAtomic()
        {
            var documentAtomic = new DocumentAtomic();
            // await documentAtomic.Version1();
            // await documentAtomic.Version2();
            // await documentAtomic.Version3();
            // await documentAtomic.Version4();
            await documentAtomic.Version5();
        }

        private static async Task ConnectionPoolExhausted()
        {
            var connections = new ConnectionPoolExhausted();
            await connections.GetMember();
        }

        private static async Task OptimisticConcurrency()
        {
            var concurrency = new OptimisticConcurrency();
            await concurrency.Go();
        }
    }
}