using System.ComponentModel.DataAnnotations;

namespace GameAPIServer.ReqResModels
{
    public class RegisteRequest
    {
        [Required] public String email { get; set; }
        [Required] public String nickname { get; set; }
        [Required] public String password { get; set; }
        [Required] public String version { get; set; }
    }

    public class RegistResponse : BaseResponse
    {
    }
}
