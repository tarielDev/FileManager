using FileMan.Commands.Base;

namespace FileMan.Commands;

public class ChangeDirectoryCommand : FileMangerCommand
{
    private readonly DirsAndFiles _dirAndFiles;
    private readonly IUserInterface _userInterface;
    public override string Description => "Изменение текущей директории.";
    
    public ChangeDirectoryCommand(DirsAndFiles dirAndFiles, IUserInterface userInterface)
    {
        _userInterface = userInterface;
        _dirAndFiles = dirAndFiles;
    }

    public override void Execute(string[] args)
    {
        _userInterface.ClearWindow();
        var settings=new StartSettings();
        string str;

        if (args[0] == "cd..")
        {
            str = "..";
        }
        else
        {
            if (args.Length != 2 || string.IsNullOrWhiteSpace(args[1]))
            {
                _userInterface.Clear(settings.message_window_left + 2, settings.message_window_top + 1, 6);
                _userInterface.WriteByXY(settings.message_window_left + 2, settings.message_window_top + 2, $"Для команды изменения директории необходимо указать один параметр - целевую директорию.");
                return;
            }

            if (args[1].Contains("@"))
            {
                str = args[1].Replace("@", " ");
            }
            else
            {
                str = args[1];
            }
        }

        var dir_path=str;

        DirectoryInfo? directory;

        if (dir_path=="..")
        {
            directory = _dirAndFiles.CurrentDirectory.Parent;

            if (directory is null)
            {
                _userInterface.Clear(settings.message_window_left + 2, settings.message_window_top + 1, 6);
                _userInterface.WriteByXY(settings.message_window_left + 2, settings.message_window_top + 2, $"Невозможно подняться выше по дереву директорий.");
                return;
            }
            else
                dir_path = directory.FullName;

        }
        else if (!Path.IsPathRooted(dir_path))
        {
            dir_path = Path.Combine(_dirAndFiles.CurrentDirectory.FullName, dir_path);

        }

        directory = new DirectoryInfo(dir_path);

        if (!directory.Exists)
        {
            _userInterface.Clear(settings.message_window_left + 2, settings.message_window_top + 1, 6);
            _userInterface.WriteByXY(settings.message_window_left + 2, settings.message_window_top + 2, $"Директория {directory} не существует.");
            return;
        }

        _dirAndFiles.CurrentDirectory = directory;
        _userInterface.Clear(settings.path_window_left + 2, settings.path_window_top + 1, 1);
        _userInterface.WriteByXY(settings.path_window_left + 2, settings.path_window_top + 1, $"Путь: {directory.FullName}");

        Directory.SetCurrentDirectory(directory.FullName);
    }


}
