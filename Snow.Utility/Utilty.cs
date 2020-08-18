using Newtonsoft.Json;
using Snow.Utility.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Snow.Utility
{
    public static class Utilty
    {
        private static readonly string _apiBaseUrl = "http://localhost:52172/api";
        private static string _subject => "snowagent";

        /// <summary>
        /// 1. Run Publish/Subscriber Messages Tasks for ChatClient & ChatSubscriber Console:
        /// 2. To avoid redundant code for ChatClient & ChatSubscriber Console so added at here..
        /// </summary>
        public static void RunPublishSubscriberMessagingTasks()
        {
            var userName = Console.ReadLine();
            try
            {
                // BG Thread for listening subscribe messages:
                Task.Run(() =>
                {
                    while (true)
                        SubscribeMessage(userName, _subject);
                });

                // Main Thread for publishing messages:
                while (true)
                {
                    Console.Write("[" + userName + "]" + " " + DateTime.Now.ToShortTimeString() + ": ");

                    PublishMessage(ConstructMessageToPublish(Console.ReadLine(), _subject, userName));
                }
            }
            catch (AggregateException ex)
            {
                throw new Exception("RunPublishSubscriberMessagingTasks: " + " aggregate exception occured. \n" + ex.Message);
            }            
        }

        /// <summary>
        /// Handles Subscribe Message for User on Subject level:
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="userName"></param>
        private static async void SubscribeMessage(string userName, string subject)
        {
            try
            {
                var uri = new Uri($"{_apiBaseUrl}/message/{userName}/{subject}");
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(uri).Result.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(response))
                    {
                        Message receivedMessage = JsonConvert.DeserializeObject<Message>(response);

                        Console.WriteLine("\n[" + receivedMessage.Sender + "]" + " " + DateTime.Now.ToShortTimeString() + ": " + receivedMessage.Content);
                        Console.Write("[" + userName + "]" + " " + DateTime.Now.ToShortTimeString() + ": ");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Hanldes Publish Message payload
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        private static async void PublishMessage(Message payload)
        {
            try
            {
                HttpResponseMessage responseMessage;
                var httpRequest = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri($"{_apiBaseUrl}/publish/message"),
                    Content = new StringContent(JsonConvert.SerializeObject(payload), System.Text.Encoding.UTF8, "application/json"),
                    Headers =
                            {
                                { HttpRequestHeader.ContentType.ToString(), "application/json" },
                            }
                };

                using (var client = new HttpClient())
                {
                    responseMessage = await client.SendAsync(httpRequest);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Construct a Message To Publish
        /// </summary>
        /// <param name="inputMessage"></param>
        /// <param name="subject"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        private static Message ConstructMessageToPublish(string inputMessage, string subject, string username)
        {
            var message = new Message
            {
                Subject = subject,
                Sender = username,
                Content = inputMessage
            };
            return message;
        }
    }
}
