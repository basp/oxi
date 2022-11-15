using System;
using Microsoft.Data.Sqlite;

namespace Oxi.Tool;

public class SQLiteDatabase : IDatabase
{
    private readonly string path;

    public SQLiteDatabase(string path)
    {
        this.path = path;
    }

    public void AddProperty(int id, string name, IValue value)
    {
        throw new System.NotImplementedException();
    }

    public int Create()
    {
        throw new System.NotImplementedException();
    }

    public void DeleteProperty(int id, string name)
    {
        throw new System.NotImplementedException();
    }

    public int GetMaxObject()
    {
        var connStr = $"Data Source={this.path}";
        using var conn = new SqliteConnection(connStr);
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