namespace SecretAPI.Models;

public class ChangePassword
{
    public ChangePassword()
    {        
        this.Password = string.Empty;
        this.PasswordConfirm = string.Empty;
    }
    public string Password { get; set; }
    public string PasswordConfirm { get; set; }

    public bool IsValid()
    {
        if (string.IsNullOrWhiteSpace(this.Password) ||
            string.IsNullOrWhiteSpace(this.PasswordConfirm))
            return false;

        if (this.Password != this.PasswordConfirm)
            return false;

        return true;
    }
}

public class Login
{
    public Login()
    {
        this.Username = string.Empty;
        this.Password = string.Empty;
    }
    public string Username { get; set; }
    public string Password { get; set; }

    public bool IsValid()
    {
        int maxLength = 30;
        if (string.IsNullOrWhiteSpace(this.Username) ||
            string.IsNullOrWhiteSpace(this.Password))
            return false;

        if (this.Username.Length > maxLength || 
            this.Password.Length > maxLength )
            return false;

        return true;
    }
}

public class User
{
    public User()
    {
        this.Username = string.Empty;
        this.Id = Guid.NewGuid();
        this.DateCreated = DateTime.Now;
        this.IsActive = false;
    }
    public Guid Id { get; set; }
    public string Username { get; set; }
    public DateTime DateCreated { get; set; }
    public bool IsActive { get; set; }
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

    public Secret(SecretUpload m) : this()
    {
        this.Id = m.Id;
        this.Content = m.Content;
    }
    
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime DateModified { get; set; }

    public string Content { get; set; }

}

public class SecretUpload
{
    public SecretUpload()
    {
        this.Id = Guid.NewGuid();
        this.Content = String.Empty;
    }
    
    public Guid Id { get; set; }
    public string Content { get; set; }
}
