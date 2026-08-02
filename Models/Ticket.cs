namespace SmartDesk.Api.Models;

public class Ticket
{
    public int Id {get; set;}
    public required string Customer {get; set;}
    public required string Email {get; set;}
    public required string Subject {get; set;}

     // If the request does not include status, it defaults to "open".
     public string Status {get; set;}="open";

     // Claude API will update this. Until then, default is MEDIUM.
     public string Urgency {get; set;}="MEDIUM";

     public DateTime CreatedAt {get; set;}= DateTime.UtcNow;

}