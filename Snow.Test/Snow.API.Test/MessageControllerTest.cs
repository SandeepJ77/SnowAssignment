using AutoFixture;
using AutoFixture.Xunit2;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Snow.API.Controllers;
using Snow.Utility.Handler;
using Snow.Utility.Models;
using System;
using System.Collections.Generic;
using System.Net;
using Xunit;

namespace Snow.Test.Snow.API.Test
{
    public class MessageControllerTest
    {
        private readonly Mock<IMessageHandler> _mockMessageHandler;
        private readonly Fixture _fixture;
        private MessageController _sut;

        public MessageControllerTest()
        {
            _mockMessageHandler = new Mock<IMessageHandler>();
            _fixture = new Fixture();

            _sut = new MessageController(_mockMessageHandler.Object);
        }

        [Fact]
        public void GetAllReceivedMessage_Returns_NoContent()
        {
            _mockMessageHandler.Setup(x => x.GetAllReceivedMessage(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(() => null);

            var response = _sut.GetAllReceivedMessage(It.IsAny<string>(), It.IsAny<string>());

            Assert.Equal((int)HttpStatusCode.NoContent, ((StatusCodeResult)response).StatusCode);
        }

        [Theory, AutoData]
        public void GetAllReceivedMessage_Returns_Success(IEnumerable<string> data)
        {
            _mockMessageHandler.Setup(x => x.GetAllReceivedMessage(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(() => data);

            var response = _sut.GetAllReceivedMessage(It.IsAny<string>(), It.IsAny<string>());
            var result = ((ObjectResult)response).Value as IEnumerable<string>;

            Assert.IsType<OkObjectResult>(response);
            Assert.NotNull(result);
        }

        [Fact]
        public void GetMessage_Returns_NoContent()
        {
            _mockMessageHandler.Setup(x => x.GetMessage(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(() => null);

            var response = _sut.GetMessage(It.IsAny<string>(), It.IsAny<string>());

            Assert.Equal((int)HttpStatusCode.NoContent, ((StatusCodeResult)response).StatusCode);
        }

        [Theory, AutoData]
        public void GetMessage_Returns_Success(Message message)
        {
            _mockMessageHandler.Setup(x => x.GetMessage(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(() => message);

            var response = _sut.GetMessage(It.IsAny<string>(), It.IsAny<string>());
            var result = ((ObjectResult)response).Value as Message;

            Assert.IsType<OkObjectResult>(response);
            Assert.NotNull(result);
        }

        [Fact]
        public void PostMessage_Returns_BadRequest()
        {
            var response = _sut.PostMessage(null);

            Assert.Equal((int)HttpStatusCode.BadRequest, ((StatusCodeResult)response).StatusCode);
        }

        [Theory, AutoData]
        public void PostMessage_Return_Success(Message message)
        {
            _mockMessageHandler.Setup(x => x.PostMessage(message))
               .Returns(() => true);

            var response = _sut.PostMessage(message);

            Assert.NotNull(response);
            Assert.Equal((int)HttpStatusCode.Created, ((ObjectResult)response).StatusCode);
        }

        [Theory, AutoData]
        public void PostMessage_Return_Falied_Exception(Message message)
        {
            _mockMessageHandler.Setup(x => x.PostMessage(message))
                .Returns(() => false);

            var ex = Assert.Throws<Exception>(() => _sut.PostMessage(message));

            Assert.Equal("Message published not successful.", ex.Message);
        }
    }
}
