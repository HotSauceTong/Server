namespace GameAPIServer.DatabaseServices.GameDb.Models;

public class UserAccount
{
    public Int64 user_id { get; set; }
    public String email { get; set; }
    public String nickname { get; set; }
    public byte[] salt { get; set; }
    public byte[] hashed_password { get; set; }
}
