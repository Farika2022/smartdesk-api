using Microsoft.AspNetCore.Mvc;
using SmartDesk.Api.Models;
using Microsoft.EntityFrameworkCore;
using SmartDesk.Api.Data;
using Microsoft.AspNetCore.Authorization;

namespace SmartDesk.Api.Controllers;

//[ApiController]=> Tells .NET: this is an API controller.
// Enables automatic request validation, model binding,
// and proper error responses. Always add this to API controllers.

[Authorize]
[ApiController]

//[Route("api/[controller]")]=> Sets the URL prefix for all endpoints in this controller.
// [controller] = the class name minus "Controller".
// TicketsController → /api/tickets
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{

     // _context is the database connection.
    // readonly = it is set once in the constructor and never changed.
    private readonly SmartDeskContext _context;

   // .NET automatically passes SmartDeskContext here.
   // This is dependency injection — never create _context manually.
   // .NET creates it, manages the connection, and disposes it after the request.
   public TicketsController(SmartDeskContext context)
   {
    _context = context;
   }


    // [HttpGet]=> Tells .NET: this method handles GET requests to /api/tickets.
    // Returns all tickets as a JSON array.
    // IActionResult = the method can return different response types
[HttpGet]
public async Task< IActionResult >GetAll()
{
    // Fetches all tickets from the database asynchronously.
    // Same as SQL: SELECT * FROM "Tickets"

    var tickets = await _context.Tickets.ToListAsync();
    // Ok() returns HTTP 200 with the data as JSON.
    return Ok(tickets);
}

 // This is a route parameter. /api/tickets/10001 sets id = 10001.
// .NET automatically extracts it and passes it to the method.
[HttpGet("{id}")]
public async Task<IActionResult> GetById (int id)
{

  
    //FirstOrDefault=> Searches the list for a ticket with matching ID.
    // Returns the ticket if found, null if not found.
   // var ticket = _tickets.FirstOrDefault(t=>t.Id==id);

   // Finds a ticket by primary key — O(1) lookup via index.
  // Same as SQL: SELECT * FROM "Tickets" WHERE "Id" = id
    var ticket = await _context.Tickets.FindAsync(id);

    if (ticket==null) return NotFound();
    return Ok(ticket);
}
 
    //  [HttpPost] => Handles POST requests — creating a new ticket.
    // Called when a customer submits the form in React.
    [HttpPost]
    public async Task< IActionResult> Create ([FromBody]Ticket ticket)
    {
        // .NET automatically validates the incoming data.
         // This is the server-side validation layer.
         // React validates first (client-side). .NET validates again (server-side).
         if (!ModelState.IsValid) return BadRequest (ModelState);

         // The server controls these to prevent manipulation.
         //  ticket.Id= _nextId++;
         ticket.CreatedAt =DateTime.UtcNow;

         _context.Tickets.Add(ticket);
 
        // THIS is when data is actually written to PostgreSQL.
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = ticket.Id }, ticket);

   }

     // PATCH /api/tickets/{id}
     // PATCH updates only specific fields — just the status here.
     [HttpPatch("{id}")]
     public async Task<IActionResult> UpdateStatus (int id, [FromBody]UpdateStatusRequest request)
     {
        var ticket= await _context.Tickets.FindAsync(id);
        if (ticket==null) return NotFound();

        ticket.Status=request.Status;
        return Ok(ticket);
     }

}

// PATCH only accepts { status: "resolved" } — not a full Ticket.
// This class defines exactly what the PATCH endpoint accepts.
// Sending extra fields = ignored. Missing status = validation error.

public class UpdateStatusRequest
{
    public required string Status {get;set;}
}