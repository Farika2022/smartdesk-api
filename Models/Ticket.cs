using System.ComponentModel.DataAnnotations;

namespace SmartDesk.Api.Models;

public class Ticket
{
    public int Id {get; set;}
    public required string Customer {get; set;}

    [EmailAddress]
    public required string Email {get; set;}
    public required string Subject {get; set;}

     // If the request does not include status, it defaults to "open".
     public string Status {get; set;}="open";

     public const string Unclassified = "UNCLASSIFIED";
     public static readonly string[] ValidUrgencies = ["LOW", "MEDIUM", "HIGH"];

     // Set by the AI classifier. Stays UNCLASSIFIED if classification fails,
     // so staff can see the ticket still needs a human to triage it.
     public string Urgency {get; set;}=Unclassified;

     // Filled in by the AI classifier. Null if classification failed.
     public string? Category {get; set;}
     public string? SuggestedReply {get; set;}

     public DateTime CreatedAt {get; set;}= DateTime.UtcNow;

}