namespace Oxi.Tool;

using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

public class SqliteDatabase : IDatabase
{
    private readonly string connectionString;

    public SqliteDatabase(string path)
    {
        this.connectionString = $"Data Source={path}";
    }

    public void AddProperty(int id, string name)
    {
        using var conn = new SqliteConnection(this.connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = $"insert into properties (object_id, name) values ({id}, '{name}')";
        cmd.ExecuteNonQuery();
    }

    public void DeleteProperty(int id, string name)
    {
        throw new System.NotImplementedException();
    }

    public void SetProperty(int id, string name, byte[] value)
    {
        throw new NotImplementedException();
    }

    public byte[] GetProperty(int id, string name)
    {
        throw new System.NotImplementedException();
    }

    public string[] GetProperties(int id)
    {
        using var conn = new SqliteConnection(this.connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = $"select name from properties where object_id = {id}";
        var reader = cmd.ExecuteReader();
        var names = new List<string>();
        while (reader.Read())
        {
            names.Add(reader.GetString(0));
        }

        return names.ToArray();
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

    public int GetMaxObject()
    {
        using var conn = new SqliteConnection(this.connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "select max(id) from objects";
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public int Recycle(int id)
    {
        using var conn = new SqliteConnection(this.connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = $"delete from objects where id = {id}";
        return cmd.ExecuteNonQuery();
    }
}
