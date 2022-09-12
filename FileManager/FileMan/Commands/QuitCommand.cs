using FileMan.Commands.Base;

namespace FileMan.Commands;

public class QuitCommand : FileMangerCommand
{
    private readonly IUserInterface _userInterface;
    private readonly FileManagerLogic _fileManagerLogic;
    public override string Description => "Выход.";
    
    public QuitCommand(IUserInterface userInterface, FileManagerLogic fileManagerLogic)
    {
        _userInterface = userInterface;
        _fileManagerLogic = fileManagerLogic;
    }

    public override void Execute(string[] args)
    {
        _userInterface.ClearWindow();
        _fileManagerLogic.Stop();
    }
}

