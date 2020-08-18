using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Snow.Utility.Handler;
using Snow.Utility.Models;
using System;

namespace Snow.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageHandler _messageHandler;
        public MessageController(IMessageHandler messageHandler)
        {
            _messageHandler = messageHandler;
        }

        [HttpGet("chatclient/received/messages")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetAllReceivedMessage(string sender, string subject = "snowagent")
        {
            try
            {
                var result = _messageHandler.GetAllReceivedMessage(subject, sender);

                if (result == null)
                    return NoContent();

                return Ok(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
               
        [HttpGet("message/{username}/{subject}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult GetMessage(string userName, string subject)
        {
            try
            {
                var result = _messageHandler.GetMessage(subject, userName);

                if (result == null)
                    return NoContent();

                return Ok(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost("publish/message")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult PostMessage([FromBody] Message message)
        {
            try
            {
                if (message == null)
                    return BadRequest();

                var successful = _messageHandler.PostMessage(message);

                return (successful)
                     ? Created("", successful)
                    : throw new Exception("Message published not successful.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}