namespace JC_PTTLogin.Models
{
    public class CrawlerRequest
    {
        public string AccountId { get; set; }

        public string Password { get; set; }
        public bool ReLogin { get; set; }
    }
}
