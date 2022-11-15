namespace Oxi;

public interface IDatabase
{
    int Create();

    int Recycle(int id);

    void AddProperty(int id, string name);

    void DeleteProperty(int id, string name);

    byte[] GetProperty(int id, string name);

    string[] GetProperties(int id);

    int GetMaxObject();
}