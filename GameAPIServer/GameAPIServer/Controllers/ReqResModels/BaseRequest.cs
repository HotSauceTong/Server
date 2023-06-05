using System.ComponentModel.DataAnnotations;

namespace GameAPIServer.Controllers.ReqResModels;

public class BaseRequest
{
    [Required] public string email { get; set; }
    [Required] public string token { get; set; }
    [Required] public string version { get; set; }
}
