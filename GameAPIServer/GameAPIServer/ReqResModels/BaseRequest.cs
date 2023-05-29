using System.ComponentModel.DataAnnotations;

namespace GameAPIServer.ReqResModels
{
    public class RequestBase
    {
        [Required] public String userEmail { get; set; }
        [Required] public String token { get; set; }
        [Required] public String clientVersion { get; set; }
        //[Required] public String masterDataVersion { get; set; }
    }
}
