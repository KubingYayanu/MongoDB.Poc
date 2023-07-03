using MongodB.Poc.Concurrency;

namespace MongodB.Poc.Gifts // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // await DocumentAtomic();
            // await ConnectionPoolExhausted();
            await ConcurrencyTransaction();
            Console.ReadLine();
        }

        /// <summary>
        /// 測試 MongoDB 語法
        /// </summary>
        private static async Task DocumentAtomic()
        {
            var documentAtomic = new DocumentAtomic();
            await documentAtomic.Version1();
            await documentAtomic.Version2();
            await documentAtomic.Version3();
            await documentAtomic.Version4();
            await documentAtomic.Version5();
        }

        /// <summary>
        /// 測試耗盡 Connection Pool 中的 Connection
        /// </summary>
        private static async Task ConnectionPoolExhausted()
        {
            var connections = new ConnectionPoolExhausted();
            await connections.GetMember();
        }

        /// <summary>
        /// 測試 Transaction
        /// </summary>
        private static async Task ConcurrencyTransaction()
        {
            var concurrency = new ConcurrencyTransaction();
            await concurrency.Go();
        }
    }
}