using System.ComponentModel.DataAnnotations;

namespace GameAPIServer.Controllers.ReqResModels;

public class RegisteRequest
{
    [Required] public string email { get; set; }
    [Required] public string nickname { get; set; }
    [Required] public string password { get; set; }
    [Required] public string version { get; set; }
}

public class RegistResponse : BaseResponse
{
}
