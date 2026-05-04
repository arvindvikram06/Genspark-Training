using UnderstandingOOPSApp.Interfaces;
using UnderstandingOOPSApp.Services;

namespace UnderstandingOOPSApp
{
    internal class Program
    {
        ICustomerInteract customerInteract;

        public Program()
        {
            customerInteract = new CustomerService();
        }

        void DoBanking()
        {
            while (true)
            {
                Console.WriteLine("Please select the service\n1. Open Account\n2. Print account by account number\n3. Print account by phone number\n4. Exit");

                int serviceChoice;
                while (!int.TryParse(Console.ReadLine(), out serviceChoice) || serviceChoice < 1 || serviceChoice > 4)
                {
                    Console.WriteLine("Invalid input. Please enter a number between 1 and 4.");
                }

                switch (serviceChoice)
                {
                    case 1:
                        var account = customerInteract.OpensAccount();
                        Console.WriteLine(account);
                        break;

                    case 2:
                        Console.WriteLine("Enter account number:");
                        string accNum = Console.ReadLine() ?? "";
                        customerInteract.PrintAccountDetails(accNum);
                        break;

                    case 3:
                        Console.WriteLine("Enter phone number:");
                        string phoneNum = Console.ReadLine() ?? "";
                        customerInteract.PrintAccountWithPhone(phoneNum);
                        break;

                    case 4:
                        Console.WriteLine("Exiting the Application!");
                        return;
                }
            }
        }

        static void Main(string[] args)
        {
            new Program().DoBanking();
        }
    }
}
