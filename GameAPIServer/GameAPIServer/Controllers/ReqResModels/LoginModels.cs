using System.ComponentModel.DataAnnotations;

namespace GameAPIServer.Controllers.ReqResModels;

public class LoginRequest
{
    [Required] public string email { get; set; }
    [Required] public string password { get; set; }
    [Required] public string version { get; set; }
}

public class LoginResponse : BaseResponse
{
    public string token { get; set; }
}
