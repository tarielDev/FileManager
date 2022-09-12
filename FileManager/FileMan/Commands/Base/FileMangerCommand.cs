
namespace FileMan.Commands.Base;

public abstract class FileMangerCommand
{
    public abstract void Execute(string[] args);
    public abstract string Description { get; }
}
