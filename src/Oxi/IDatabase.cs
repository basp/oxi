namespace Oxi;

public interface IDatabase
{
    // bool Valid(int id);

    int Create();

    void Recycle(int id);

    void AddProperty(int id, string name, IValue value);

    void DeleteProperty(int id, string name);

    IValue GetProperty(int id, string name);

    string[] GetProperties(int id);

    int GetMaxObject();
}