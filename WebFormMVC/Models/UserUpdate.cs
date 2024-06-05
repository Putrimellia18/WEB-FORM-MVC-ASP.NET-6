namespace ConsumeAPINet6.Models
{
    public class UserUpdate
    {
        public int id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public List<Course> course { get; set; }
    }
}
