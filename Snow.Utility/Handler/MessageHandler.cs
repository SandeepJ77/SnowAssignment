using AutoMapper;
using NATS.Client;
using Newtonsoft.Json;
using Snow.Utility.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snow.Utility.Handler
{
    public interface IMessageHandler
    {
        Message GetMessage(string subject, string userName);
        bool PostMessage(Message message);
        IEnumerable<string> GetAllReceivedMessage(string subject, string sender);
    }
    public class MessageHandler : IMessageHandler
    {
        private readonly IMapper _mapper;
        private readonly IConnection _connection;
        private readonly ConcurrentBag<string> subscribedSubject = new ConcurrentBag<string>();
        private readonly ConcurrentDictionary<string, List<Message>> messageQueue = new ConcurrentDictionary<string, List<Message>>();

        public MessageHandler(IMapper mapper, IConnection connection)
        {
            _mapper = mapper;
            _connection = connection;
        }

        /// <summary>
        /// Get Subscribe Message (Used for PART 1 Implementation)
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public Message GetMessage(string subject, string userName)
        {
            DoSubscribe(subject);

            if (!messageQueue.IsEmpty)
            {
                var hasMessage = messageQueue.TryGetValue(subject, out var messageList);
                if (hasMessage)
                {
                    var message = messageList.FirstOrDefault(x => x.IsRead == false && x.Sender.ToLower() != userName.ToLower());
                    if (message != null)
                    {
                        message.IsRead = true;
                        return message;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Publishes Message for the given subject (Used for PART 1 & 3 Implementation)
        /// </summary>
        /// <param name="message"></param>
        public bool PostMessage(Message message)
        {
            bool isMessagePublished = false;

            try
            {
                _connection.Publish(message.Subject, message.Sender, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message.Content)));

                isMessagePublished = true;
                return isMessagePublished;
            }
            catch (NATSBadSubscriptionException ex)
            {
                throw new Exception("PostMessage:" + " subject is null or entirely whitespace. \n" + ex.Message);
            }
            catch (NATSMaxPayloadException ex)
            {
                throw new Exception("PostMessage:" + " Data exceeds the maximum payload size supported by the NATS server \n" + ex.Message);
            }
            catch (NATSConnectionClosedException ex)
            {
                throw new Exception("PostMessage:" + " The Connection is closed.. \n" + ex.Message);
            }
            catch (NATSException ex)
            {
                throw new Exception("PostMessage:" + " There was an unexpected exception performing an internal NATS call while executing the request.See Exception.InnerException for more details. \n" + ex.Message);
            }
        }

        /// <summary>
        /// Get all received messages from Replier (Used for PART 2 Implementation)
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="sender"></param>
        /// <returns></returns>
        public IEnumerable<string> GetAllReceivedMessage(string subject, string sender)
        {
            if (!messageQueue.IsEmpty)
            {
                var hasMessage = messageQueue.TryGetValue(subject, out var messageList);

                if (hasMessage)
                    return messageList.Where(x => x.Sender.ToLower().Equals(sender.ToLower())).Select(x => x.Content).ToList();
            }

            return null;
        }

        /// <summary>
        /// Subscribe on the supplied subject & adding message into messageQueue:
        /// </summary>
        /// <param name="subject"></param>
        private void DoSubscribe(string subject)
        {
            if (!subscribedSubject.Contains(subject))
            {
                try
                {
                    IAsyncSubscription subscription = _connection.SubscribeAsync(subject, (sender, args) =>
                    {
                        var recievedMessage = _mapper.Map<Message>(args.Message);

                        // To implement PART 2, have added received messages into messageQueue to process further....
                        if (messageQueue.TryGetValue(subject, out var messageList))
                            messageQueue[subject].Add(recievedMessage);
                        else
                            messageQueue.TryAdd(subject, new List<Message> { recievedMessage });
                    });

                    subscribedSubject.Add(subject);
                }
                catch (NATSBadSubscriptionException ex)
                {
                    throw new Exception("DoSubscribe:" + " subject is null or entirely whitespace. \n" + ex.Message);
                }
                catch (NATSConnectionClosedException ex)
                {
                    throw new Exception("DoSubscribe:" + " The Connection is closed.. \n" + ex.Message);
                }
                catch (NATSException ex)
                {
                    throw new Exception("DoSubscribe:" + " There was an unexpected exception performing an internal NATS call while executing the request.See Exception.InnerException for more details. \n" + ex.Message);
                }
            }
        }
    }
}
