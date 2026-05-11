using MessagesApi.Models;
using MessagesApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace MessagesApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController(InMemoryMessageStore store) : ControllerBase
{
    [HttpPost("register")]
    public ActionResult<AuthResponse> Register(AuthRequest request)
    {
        try
        {
            return Ok(store.Register(request.UserName, request.Password));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }

    [HttpPost("login")]
    public ActionResult<AuthResponse> Login(AuthRequest request)
    {
        try
        {
            return Ok(store.Login(request.UserName, request.Password));
        }
        catch (InvalidOperationException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
    }
}
