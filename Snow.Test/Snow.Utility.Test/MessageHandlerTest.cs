using AutoFixture;
using AutoFixture.Xunit2;
using AutoMapper;
using Moq;
using NATS.Client;
using Newtonsoft.Json;
using Snow.Utility.Handler;
using Snow.Utility.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Snow.Test.Snow.Utility.Test
{
    public class MessageHandlerTest
    {
        private readonly Mock<IConnection> _mockConnection;
        private readonly Fixture _fixture;
        private MessageHandler _sut;
        private readonly ConcurrentDictionary<string, List<Message>> messageQueue = new ConcurrentDictionary<string, List<Message>>();
        public MessageHandlerTest()
        {
            _mockConnection = new Mock<IConnection>();
            _fixture = new Fixture();

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MessageMapperProfile());
            });
            var _mockMapper = mockMapper.CreateMapper();

            _sut = new MessageHandler(_mockMapper, _mockConnection.Object);
        }

        [Fact]
        public void GetMessage_Returns_Null_Response()
        {
            var response = _sut.GetMessage(It.IsAny<string>(), It.IsAny<string>());

            Assert.Null(response);
        }

        [Fact]
        public void GetAllReceivedMessage_Returns_Null_Response()
        {
            var response = _sut.GetAllReceivedMessage(It.IsAny<string>(), It.IsAny<string>());

            Assert.Null(response);
        }

        [Theory, AutoData]
        public void PostMessage_Return_Success(Message message)
        {
            _mockConnection.Setup(x => x.Publish(message.Subject, message.Sender,
                Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message.Content)))).Verifiable();

            var response = _sut.PostMessage(message);

            Assert.True(response);           
        }
    }
}
