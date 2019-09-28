namespace RegistryHelper
{
    public interface IRegistryHelper
    {
        bool CheckRegistry(string path);
        bool DeleteRegistry(string path);
        bool EditRegistry(string path, string key, string value);
        string ReadRegistry(string path, string key);
        bool WriteRegistry(string path, string key, string value);
    }
}