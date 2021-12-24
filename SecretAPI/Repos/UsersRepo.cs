using Ro.SQLite.Data;
using SecretAPI.Models;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Cryptography;
namespace SecretAPI.Repos;

public interface IUsersRepo
{
    Task<User> GetOne(string username);
    Task<Guid> Create(Login model);
    Task<int> Delete(Guid id);
    Task<bool> HasAccess(Login model);
	Task<int> ChangePassword(Guid userId, ChangePassword model);
}

public class UsersRepo : IUsersRepo
{
    private IDbAsync Db { get; set; }   
    public UsersRepo(IDbAsync db)
    {
		this.Db = db; 
    }

    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
		using (var hmac = new HMACSHA512())
		{
			passwordSalt = hmac.Key;
			passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
		}
    }

    private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
		using (var hmac = new HMACSHA512(passwordSalt))
		{
			var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
			return computedHash.SequenceEqual(passwordHash);
		}
    }    	    

    public Task<User> GetOne(string username)
    {
		string sql = "SELECT Id,Username,DateCreated FROM Users WHERE Username=@Username;";
		var cmd = sql.ToCmd
		(
			"@Username".ToParam(DbType.String, username)
		);

		return Db.GetOneRow(cmd, GetData);
    }

    public async Task<bool> HasAccess(Login model)
    {
		var user = await GetOne(model.Username);
		if (user == null || user.IsActive == false)
		{
			return false;
		}

		string sql = "SELECT PasswordHash, PasswordSalt FROM Users WHERE Id=@Id";
		var cmd = sql.ToCmd("@Id".ToParam(DbType.String, user.Id.ToString()));
		var tuple = await Db.GetOneRow(cmd, (dr) => {
			(byte[] hash, byte[] salt) t =
			(
				(byte[])dr["PasswordHash"],
				(byte[])dr["PasswordSalt"]
			);
			return t;
		});

		return VerifyPasswordHash(model.Password, tuple.hash, tuple.salt);
    }

    private User GetData(IDataReader dr)
    {
		return new User()
		{
			Id = Guid.Parse(dr.GetString("Id")),
			Username = dr.GetString("Username"),
			DateCreated = DateTime.Parse(dr.GetString("DateCreated")),
			IsActive = dr.GetInt("IsActive") == 1
		};
    }

    public async Task<Guid> Create(Login model)
    {
		string sql = "INSERT INTO Users (Id,Username,PasswordHash,PasswordSalt,DateCreated) VALUES (@Id,@Username,@PasswordHash, @PasswordSalt,@DateCreated);";
		DateTime stamp = DateTime.Now;
		Guid id = Guid.NewGuid();

		CreatePasswordHash(model.Password, out byte[] hash, out byte[] salt);
		
		var cmd = sql.ToCmd(
			"@Id".ToParam(DbType.String, id.ToString()),
			"@Username".ToParam(DbType.String, model.Username),
			"@PasswordHash".ToParam(DbType.Binary, hash),
			"@PasswordSalt".ToParam(DbType.Binary, salt),
			"@DateCreated".ToParam(DbType.String, stamp.ToString("yyyy-MM-dd HH:mm:ss.fff"))	    
		);

		await Db.ExecuteNonQuery(cmd);
		return id;
    }

	public Task<int> ChangePassword(Guid userId, ChangePassword model)
    {
		string sql = "UPDATE Users SET PasswordHash=@PasswordHash,PasswordSalt=@PasswordSalt WHERE UserId=@UserId;";
		CreatePasswordHash(model.Password, out byte[] hash, out byte[] salt);
		var cmd = sql.ToCmd(			
			"@UserId".ToParam(DbType.String, userId.ToString()),
			"@PasswordHash".ToParam(DbType.Binary, hash),
			"@PasswordSalt".ToParam(DbType.Binary, salt)
		);
		
		return Db.ExecuteNonQuery(cmd);	 
    }

    public Task<int> Delete(Guid id)
    {
		string sql = "DELETE FROM Users WHERE Id=@Id;";

		var cmd = sql.ToCmd(
			"@Id".ToParam(DbType.String, id.ToString())
		);

		return Db.ExecuteNonQuery(cmd);	 
    }

    
}
