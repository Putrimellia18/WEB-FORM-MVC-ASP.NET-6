namespace ConsumeAPINet6.Models
{
    public class User
    {
        public int id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public int courseid {  get; set; }
        public IFormFile image { get; set; }
        public List<Course> course { get; set; }
    }
}
