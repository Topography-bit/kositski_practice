using MessagesApi.Models;
using MessagesApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace MessagesApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class MessagesController(InMemoryMessageStore store) : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<MessageDto>> Get()
    {
        return Ok(store.GetMessages());
    }

    [HttpPost]
    public ActionResult<MessageDto> Post(CreateMessageRequest request)
    {
        try
        {
            return Ok(store.AddLegacyMessage(request.UserName, request.Text));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("secure")]
    public ActionResult<MessageDto> PostSecure(SecureMessageRequest request)
    {
        try
        {
            return Ok(store.AddSecureMessage(request.Token, request.Message));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
