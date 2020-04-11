using Microsoft.AspNetCore.SignalR;
using MediatR;
using System.Threading.Tasks;
using Application.Comments;
using System.Linq;
using System.Security.Claims;
namespace API.SignalR
{
    public class ChatHub : Hub
    {
        private readonly IMediator _mediator;
        public ChatHub(IMediator mediator)
        {
            _mediator = mediator;
        }


        public async Task SendComment(Create.Command command)
        {
            var username = GetUserName();

            command.Username = username;
            var comment = await _mediator.Send(command);
            await Clients.Group(command.ActivityId.ToString()).SendAsync("ReceiveComment", comment);
        }

        private string GetUserName()
        {
            return Context.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        }

        public async Task AddToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            var username = GetUserName();

            await Clients.Group(groupName).SendAsync("send", $"{username} has joined the group");
        }

        public async Task RemoveFromGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            var username = GetUserName();

            await Clients.Group(groupName).SendAsync("send", $"{username} has left the group");
        }
    }
}