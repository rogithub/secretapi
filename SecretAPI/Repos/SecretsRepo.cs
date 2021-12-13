using Ro.SQLite.Data;
using SecretAPI.Models;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace SecretAPI.Repos;

public interface ISecretsRepo
{
    Task<Secret> GetOne(Guid userId, Guid id);
    Task<IEnumerable<Secret>> GetAll(Guid userId);
    Task<int> Create(Secret model);
    Task<int> Update(Secret model);
    Task<int> Delete(Guid userId, Guid id);    
}

public class SecretsRepo : ISecretsRepo
{
    private IDbAsync Db { get; set; }   
    public SecretsRepo(IDbAsync db)
    {
	this.Db = db; 
    }

    public Task<Secret> GetOne(Guid userId, Guid id)
    {
	string sql = "SELECT Id,UserId,Content,DateCreated,DateModified FROM Secretes WHERE Id=@Id AND UserId=@UserId;";
	var cmd = sql.ToCmd
	(
	    "@Id".ToParam(DbType.String, id.ToString()),
	    "@UserId".ToParam(DbType.String, userId.ToString())
	);

	return Db.GetOneRow(cmd, GetData);
    }

    public Task<IEnumerable<Secret>> GetAll(Guid userId)
    {
	string sql = "SELECT Id,UserId,Content,DateCreated,DateModified FROM Secretes WHERE UserId=@UserId;";
	var cmd = sql.ToCmd
	(
	    "@UserId".ToParam(DbType.String, userId.ToString())
	);

	return Db.GetRows(cmd, GetData);
    }

    private Secret GetData(IDataReader dr)
    {
	return new Secret()
	{
	    Id = Guid.Parse(dr.GetString("Id")),
	    UserId = Guid.Parse(dr.GetString("UserId")),
	    Content = dr.GetString("Content"),
	    DateCreated = DateTime.Parse(dr.GetString("DateCreated")),
	    DateModified = DateTime.Parse(dr.GetString("DateModified"))
	};
    }

    public Task<int> Create(Secret model)
    {
	string sql = "INSERT INTO Secrets (Id,UserId,Content,DateCreated,DateModified) VALUES (@Id,@UserId,@Content,@DateCreated,@DateModified);";
	DateTime stamp = DateTime.Now;
	var cmd = sql.ToCmd(
	    "@Id".ToParam(DbType.String, model.Id.ToString()),
	    "@UserId".ToParam(DbType.String, model.UserId.ToString()),
	    "@Content".ToParam(DbType.String, model.Content),
	    "@DateCreated".ToParam(DbType.String, stamp.ToString("yyyy-MM-dd HH:mm:ss.fff")),
	    "@DateModified".ToParam(DbType.String, stamp.ToString("yyyy-MM-dd HH:mm:ss.fff"))
	);

	return Db.ExecuteNonQuery(cmd);	 
    }

    public Task<int> Update(Secret model)
    {
	string sql = "UPDATE Secrets SET Content=@Content,DateModified=@DateModified WHERE Id=@Id AND UserId=@UserId;";
	var cmd = sql.ToCmd(
	    "@Id".ToParam(DbType.String, model.Id.ToString()),
	    "@UserId".ToParam(DbType.String, model.UserId.ToString()),
	    "@Content".ToParam(DbType.String, model.Content),	    
	    "@DateModified".ToParam(DbType.String, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"))
	);

	return Db.ExecuteNonQuery(cmd);	 
    }

    public Task<int> Delete(Guid userId, Guid id)
    {
	string sql = "DELETE FROM Secrets WHERE Id=@Id AND UserId=@UserId;";

	var cmd = sql.ToCmd(
	    "@Id".ToParam(DbType.String, id.ToString()),
	    "@UserId".ToParam(DbType.String, userId.ToString())
	);

	return Db.ExecuteNonQuery(cmd);	 
    }

    
}
