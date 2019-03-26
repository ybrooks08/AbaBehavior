using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace AbaBackend.DataModel
{
  public class Assignment
  {
    public int AssignmentId { get; set; }

    public int ClientId { get; set; }

    public int UserId { get; set; }

    public bool Active { get; set; } = true;

    public Client Client { get; set; }

    public User User { get; set; }
  }
}