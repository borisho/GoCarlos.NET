namespace GoCarlos.NET.Interfaces;

public interface IEgdService
{
    string SearchByPin(string pin);
    string SearchByData(string lastName, string name);
}
