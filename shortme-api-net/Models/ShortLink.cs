namespace shortme_api_net.Models;

public class ShortLink : Base
{
    public int Id { get; set; }
    
    public string Code { get; set; }
    
    public string OriginalUrl { get; set; }
    
    
}