using System;
using Microsoft.Data.Sqlite;

namespace Oxi.Tool;

public class SQLiteDatabase : IDatabase
{
    private readonly string connectionString;

    public SQLiteDatabase(string path)
    {
        this.connectionString = $"Data Source={path}";
    }

    public void AddProperty(int id, string name, IValue value)
    {
        throw new System.NotImplementedException();
    }

    public int Create()
    {
        var id = this.GetMaxObject() + 1;
        using var conn = new SqliteConnection(this.connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = $"insert into objects values ({id})";
        cmd.ExecuteNonQuery();
        return id;
    }

    public void DeleteProperty(int id, string name)
    {
        throw new System.NotImplementedException();
    }

    public int GetMaxObject()
    {
        using var conn = new SqliteConnection(this.connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = @"select max(id) from objects";
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public string[] GetProperties(int id)
    {
        throw new System.NotImplementedException();
    }

    public IValue GetProperty(int id, string name)
    {
        throw new System.NotImplementedException();
    }

    public void Recycle(int id)
    {
        throw new System.NotImplementedException();
    }
}
