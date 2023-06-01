using System.ComponentModel.DataAnnotations;

namespace GameAPIServer.ReqResModels
{
    public class BaseRequest
    {
        [Required] public String email { get; set; }
        [Required] public String token { get; set; }
        [Required] public String version { get; set; }
    }
}
