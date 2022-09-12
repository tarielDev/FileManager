
using FileMan.Commands.Base;

namespace FileMan.Commands;

public class PrintDirectoryFilesCommand : FileMangerCommand
{
    private readonly DirsAndFiles _dirsAndFiles;
    private readonly IUserInterface _userInterface;
    
    public PrintDirectoryFilesCommand(IUserInterface userInterface, DirsAndFiles dirsAndFiles)
    { 
        _userInterface = userInterface;
        _dirsAndFiles = dirsAndFiles;
    }

    public override string Description => "Вывод списка директорий и файлов.";

    public override void Execute(string[] args)
    {
        var settings = new StartSettings();
        Dictionary<(int, int), string> rec = new();
        _userInterface.Clear(settings.message_prompt_left, settings.message_prompt_top, settings.show_window_lines);

        if (args.Length>1)
        {
            switch (args.Length)
            {

                case 2:
                    _dirsAndFiles.DrawDirsAndFiles(args[1]);
                    break;

                case 3:
                    _dirsAndFiles.DrawDirsAndFiles(args[1]);
                    break;

                case 4:
                    _ = int.TryParse(args[3], out int page);
                    _dirsAndFiles.DrawDirsAndFiles(args[1], page);
                    break;

            }
        }
        else
        {
            _dirsAndFiles.DrawDirsAndFiles();
        }
    }

}
