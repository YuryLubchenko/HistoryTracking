namespace WebApp.Models;

public class CreateClientRequest
{
    public string Name { get; set; }
    public List<Contact> Contacts { get; set; }
}
