using HMS.Application.Commands.Chat;
using HMS.Application.Queries.Chat;
using HMS.Web.Hubs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace HMS.Web.Controllers
{
    [Authorize]
    public class ChatsController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IHubContext<ChatHub> _hubContext;
        public ChatsController(IMediator mediator, IHubContext<ChatHub> hubContext) {
            _mediator = mediator;
            _hubContext = hubContext;
        }

        public IActionResult Chats() => View();

        [HttpGet]
        public async Task<IActionResult> GetUsers(string query = "")
        {
            var dto = await _mediator.Send(new GetUsersQuery { Query = query });
            return Ok(dto);
        }

        [HttpGet]
        public async Task<IActionResult> GetRecentChats()
        {
            var dto = await _mediator.Send(new GetRecentChatsQuery());
            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> StartChat([FromForm] StartChatCommand cmd)
        {
            try
            {
                var chat = await _mediator.Send(cmd);
                return Ok(chat);
            }
            catch (Exception ex)
            {
                // Log the error in console for debugging
                Console.WriteLine("🚨 Error in StartChat:");
                Console.WriteLine($"Message: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner: {ex.InnerException.Message}");
                }
                Console.WriteLine($"StackTrace: {ex.StackTrace}");

                // You can also log this to your logger if you have DI injected ILogger
                // _logger.LogError(ex, "Error while starting chat");

                // Return a proper JSON error response
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    error = "An error occurred while starting the chat.",
                    message = ex.Message,
                    inner = ex.InnerException?.Message
                });
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetMessages(int chatRoomId, int page = 1, int pageSize = 50)
        {
            var msgs = await _mediator.Send(new GetMessagesQuery { ChatRoomId = chatRoomId, Page = page, PageSize = pageSize });
            return Ok(msgs);
        }

        /*
        [HttpPost]
        public async Task<IActionResult> SendMessage([FromForm] SendMessageCommand cmd)
        {
            var sentMessage = await _mediator.Send(cmd);

            await _hubContext.Clients.Group($"chat-{cmd.ChatRoomId}")
                                     .SendAsync("ReceiveMessage", sentMessage);
            return Ok(sentMessage);
        }
        */
        [HttpGet]
        public async Task<IActionResult> GetUnseenMessages(int chatRoomId)
        {
            var userId = GetCurrentUserId();
            var messages = await _mediator.Send(new GetUnseenMessagesQuery
            {
                ChatRoomId = chatRoomId,
                UserId = userId
            });
            return Ok(messages);
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                throw new UnauthorizedAccessException("User ID not found in claims.");
            }

            return userId;
        }

    [HttpPost]
        public async Task<IActionResult> MarkMessagesDelivered()
        {
            var userId = GetCurrentUserId();

            var count = await _mediator.Send(new MarkMessagesDeliveredCommand
            {
                UserId = userId
            });

            return Ok(new { count });
        }
        [HttpPost]
        public async Task<IActionResult> DeleteMessage(int messageId)
        {
            var userId = GetCurrentUserId();

            await _mediator.Send(new DeleteMessageCommand
            {
                MessageId = messageId,
                UserId = userId
            });

            return Ok(new { success = true, message = "Message deleted successfully" });
        }
        [HttpPost]
        public async Task<IActionResult> EditMessage(int messageId, string newContent)
        {
            var userId = GetCurrentUserId();

            await _mediator.Send(new EditMessageCommand
            {
                MessageId = messageId,
                NewContent = newContent,
                UserId = userId
            });

            return Ok(new { success = true, message = "Message edited successfully" });
        }
    }
}
