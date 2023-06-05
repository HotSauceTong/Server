namespace GameAPIServer.DatabaseServices.GameDb.Models
{
    public class UserAttendance
    {
        public Int64 user_id { get; set; }
        public Int16 attendences_stack { get; set; }
        public DateTime last_login_date { get; set; }
        public String reward_version { get; set; }
    }
}
