
using System.Security.Cryptography.X509Certificates;

namespace AsyncaAwait
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Ex1(); // does not complete the entire call
            // await Ex1(); // await ensures the call completes
            


            // int result = 0; // default value
            // Task<int> task = Ex2(); // not yet assigned
            // Console.WriteLine(result);
            // result = await Ex2();
            // Console.WriteLine(result);



            // string data = await GetData();
            // Console.WriteLine(data);

            // Task data = GetData(); 
            // Console.WriteLine(data.Status); // without await it returns only task object
            // await data;

            // Console.WriteLine("after await " + data.Status); 

            // // sequential await
            // int t1 = await Ex2();
            // int t2 = await Ex2();
            // Console.WriteLine("afterCalls");
            // Console.WriteLine(t1);
            // Console.WriteLine(t2);

            // //parallel execution
            // Task<int> tt1 = Ex2();
            // Task<int> tt2 = Ex2();
            // Console.WriteLine("afterCalls");

            // int []results = await Task.WhenAll(tt1,tt2);  // wait to finish together

            // Console.WriteLine(results[0]);
            // Console.WriteLine(results[1]);

            // await foreach(int n in Numbers()) async for each
            // {
            //     Console.WriteLine(n);
            // }

            string value = await GetCachedNameAsync();
            Console.WriteLine(value);
            
        }

        // IAsyncEnumerable:
        // Used when data arrives over time or is too large to load at once.
        // Returns items one-by-one asynchronously.
        // Task<List<T>>:
        // Waits until ALL data is ready, then returns everything together.
        public static async IAsyncEnumerable<int> Numbers()
        {
            for (int i = 1; i<= 5; i++)
            {
                await Task.Delay(1000);
                yield return i;
            }
        }

        public static async Task Ex1()
        {
            Console.WriteLine("call1");
            await Task.Delay(5000);
            Console.WriteLine("call2");
        }

        //  public static async Task<int> Ex2()
        // {
        //     await Task.Delay(5000);
        //     return 100;
        // }

        public static async Task<int> Ex2()
        {
            Console.WriteLine($"Started : {DateTime.Now:HH:mm:ss}");

             await Task.Delay(2000);

            Console.WriteLine($"Completed : {DateTime.Now:HH:mm:ss}");

            return 10;
        }


        public static async Task<string> GetData()
        {
            HttpClient client = new HttpClient();

            string data = await client.GetStringAsync("https://dummyjson.com/test");

            return data;
        }




        /*Performance Considerations:
        Async programming has some overhead like 
        task object creation,
        thread switching, 
        state machine generation

        ValueTask:
            ValueTask<T> reduces allocations when result is often ready immediately

            eg: cached data already exist in the memory using value task avoids unecesarry allocation
        */

        public static ValueTask<string> GetCachedNameAsync()
        {
            string name = "Max Verstappen";

            return ValueTask.FromResult(name);
        }



    }
}