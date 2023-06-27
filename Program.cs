namespace MongodB.Poc // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var documentAtomic = new DocumentAtomic();
            // await documentAtomic.Version1();
            // await documentAtomic.Version2();
            // await documentAtomic.Version3();
            // await documentAtomic.Version4();
            await documentAtomic.Version5();

            Console.ReadLine();
        }
    }
}