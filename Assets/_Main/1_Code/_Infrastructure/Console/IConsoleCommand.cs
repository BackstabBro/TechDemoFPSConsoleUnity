public interface IConsoleCommand
{
    string Name { get; }
    string Description { get; }
    string Execute(string[] args);
}
