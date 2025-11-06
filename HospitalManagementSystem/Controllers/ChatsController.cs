using HMS.Application.Commands.Chat;
using HMS.Application.Queries.Chat;
using HMS.Web.Hubs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

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

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromForm] SendMessageCommand cmd)
        {
            // 1. Send through CQRS pipeline (Mediate command to handler)
            var sentMessage = await _mediator.Send(cmd);

            // 2. Broadcast via SignalR to all participants in the chat room
            // Inject IHubContext<ChatHub> into your controller via constructor
            await _hubContext.Clients.Group($"chat-{cmd.ChatRoomId}")
                                     .SendAsync("ReceiveMessage", sentMessage);

            // 3. Return the persisted message to the sender
            return Ok(sentMessage);
        }

    }

}
