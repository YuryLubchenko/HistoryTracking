namespace WebApp.Models;

public class Contact
{
    public long Id { get; set; }

    public ContactType ContactType { get; set; }

    public string Value { get; set; }
}