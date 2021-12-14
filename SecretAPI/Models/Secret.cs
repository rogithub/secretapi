namespace SecretAPI.Models;

public class Login
{
    public Login()
    {
	this.Username = string.Empty;
	this.Password = string.Empty;
    }
    public string Username { get; set; }
    public string Password { get; set; }
}

public class User
{
    public User()
    {
	this.Username = string.Empty;
	this.Id = Guid.NewGuid();
	this.DateCreated = DateTime.Now;
    }
    public Guid Id { get; set; }
    public string Username { get; set; }
    public DateTime DateCreated { get; set; }    
}

public class Secret
{
    public Secret()
    {
	this.Id = Guid.NewGuid();
	this.UserId = Guid.NewGuid();
	this.Content = String.Empty;
	this.DateCreated = DateTime.Now;
	this.DateModified = DateTime.Now;
    }
    
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime DateModified { get; set; }

    public string Content { get; set; }

}
