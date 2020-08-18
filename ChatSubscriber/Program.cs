using Snow.Utility;
using System;

namespace ChatSubscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.Write("Welcome to Replier Chat \nEnter your name to start chat: ");

                Utilty.RunPublishSubscriberMessagingTasks();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                Console.ReadLine();
            }
        }
    }
}
