using FileMan.Commands;
using FileMan.Commands.Base;

namespace FileMan;
public class FileManagerLogic
{
    private readonly IUserInterface _userInterface;
    private readonly DirsAndFiles _dirsAndFiles;
    private bool _canWork=true, _first_time=false;
    private Dictionary<int, Dictionary<(int, int), string>> atr = new();
    private FileMangerCommand? command;
    public IReadOnlyDictionary<string, FileMangerCommand> Commands { get; }

    public FileManagerLogic(IUserInterface userInterface, DirsAndFiles dirsAndFiles)
    {
        _userInterface = userInterface;
        _dirsAndFiles = dirsAndFiles;

        var dir_command = new PrintDirectoryFilesCommand(_userInterface, _dirsAndFiles);
        var help_command = new HelpCommand(_userInterface, this);
        var quit_command = new QuitCommand(_userInterface, this);

        Commands = new Dictionary<string, FileMangerCommand>
        {
            {"drives", new ListDrivesCommand(_userInterface) },
            {"dir", dir_command },
            {"ls", dir_command },
            {"help", help_command },
            {"?", help_command },
            {"quit", quit_command },
            {"exit", quit_command },
            {"cd", new ChangeDirectoryCommand(_dirsAndFiles, _userInterface) },
            {"cd..", new ChangeDirectoryCommand(_dirsAndFiles, _userInterface) }
            // Продолжение следует...
            // Максимально сделал то что хотел...
            // Остальное тут просто команды добавить
            // и логику...
        };

    }

    public void Start()
    {

        _first_time = true;
        Dictionary<(int, int), string> rec=new();
        var settings = new StartSettings();
        
        do
        {
            Console.ResetColor();
            _userInterface.Clear(settings.command_prompt_left,settings.command_prompt_top,1);
            var inputKey = _userInterface.ReadKey("> ", false, _first_time);
            _userInterface.Clear(settings.message_window_left + 2, settings.message_window_top + 1, 6);
            _first_time = false;

            if (inputKey == "ls up" || inputKey == "ls dn" || inputKey == "f")
            {
                _dirsAndFiles.GetDirsAndFilesCount();
            }

            string gaps;
            if (inputKey.Contains('['))
            {
                var str1 = inputKey.Substring(0, inputKey.IndexOf('['));
                var str2 = inputKey.Substring(inputKey.IndexOf('['), inputKey.IndexOf(']') - 2).Replace(' ', '@').Replace("[", "").Replace("]", "");
                var str3 = inputKey.Substring(inputKey.IndexOf(']') + 1);
                gaps = str1 + str2 + str3;
            }
            else
            {
                if (inputKey == "ls up" || inputKey == "ls dn" || inputKey == "f")
                    continue;
                else
                    gaps = inputKey;
            }

            var args = gaps.Split(' ');

            var command_name = args[0];

            if (!Commands.TryGetValue(command_name, out command))
            {
                if (command_name != "f")
                {
                    ;
                    _userInterface.Clear(settings.message_window_left + 2, settings.message_window_top + 1, 6);
                    _userInterface.WriteByXY(settings.message_window_left + 2, settings.message_window_top + 2, $"Неизвестная команда \"{command_name}\".");
                    _userInterface.WriteByXY(settings.message_window_left + 2, settings.message_window_top + 4, $"Для справки введите help, для выхода quit.");
                }
                continue;
            }

            try
            {
                command.Execute(args);
            }
            catch (Exception error)
            {
                _userInterface.Clear(settings.message_window_left + 2, settings.message_window_top + 1, 6);
                _userInterface.WriteByXY(settings.message_window_left + 2, settings.message_window_top + 2, $"При выполнении команды произошла ошибка:");
                _userInterface.WriteByXY(settings.message_window_left + 2, settings.message_window_top + 4, $"{error.Message}");
                _userInterface.WriteByXY(settings.message_window_left + 23, settings.message_window_top + 6, new String(' ', 75));

                _dirsAndFiles.DrawDirsAndFiles();
            }

        }
        while (_canWork);
    }
    
    public void Stop()
    {
        _canWork=false;
    }

}
