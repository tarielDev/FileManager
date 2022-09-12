using FileMan.Commands.Base;

namespace FileMan.Commands;

public class HelpCommand : FileMangerCommand
{
    private readonly IUserInterface _userInterface;
    private readonly FileManagerLogic _fileManagerLogic;

    public HelpCommand(IUserInterface userInterface, FileManagerLogic fileManagerLogic)
    {
        _userInterface = userInterface;
        _fileManagerLogic = fileManagerLogic;
    }

    public override string Description => "Помощь.";

    public override void Execute(string[] args)
    {
        _userInterface.ClearWindow();
        var settings = new StartSettings();
        Dictionary<(int, int), string> rec = new();
        _userInterface.Clear(settings.message_prompt_left, settings.message_prompt_top, settings.show_window_lines);
        _userInterface.WriteByXY(5, 2, $"Файловый менеджер поддерживает следующие команды:");

        int i = 0;
        foreach (var (name, command) in _fileManagerLogic.Commands)
        {
            _userInterface.WriteByXY(3, 5+i, $"    {name}\t{command.Description}");
            i += 2;
        }
        _userInterface.WriteByXY(7, 5 + i, $"▲ ▼\tCтрелки вверх и вниз:\tПеремещение вверх и вниз по директориям и файлам на активной странице");
        i += 2;
        _userInterface.WriteByXY(7, 5 + i, $"◄ ►\tCтрелки влево и вправо:\tПеремещение по страницам и изменение активной страницы");
        i += 2;
        _userInterface.WriteByXY(7, 5 + i, $"Enter\tEnter на [ .. ] возврат назад, Enter на директории - вход в нее");
        i += 2;
        _userInterface.WriteByXY(7, 5 + i, $"Tab\tПередвижение по атрибутам");

    }


}
