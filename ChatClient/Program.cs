using Snow.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChatClient
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.Write("Welcome to Chat Client \nEnter your name to start chat: ");

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
