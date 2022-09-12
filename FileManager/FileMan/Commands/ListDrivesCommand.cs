using FileMan.Commands.Base;

namespace FileMan.Commands;

public class ListDrivesCommand : FileMangerCommand
{
    private readonly IUserInterface _userInterface;
    public ListDrivesCommand(IUserInterface userInterface) => _userInterface = userInterface;

    public override string Description => "Вывод списка дисков в системе.";

    public override void Execute(string[] args)
    {
        _userInterface.ClearWindow();
        var settings =new StartSettings();
        Dictionary<(int, int), string> rec = new();
        var drives = DriveInfo.GetDrives();

        string dsk = drives.Length == 1 ? "диск": (drives.Length >= 2 && drives.Length <= 4) ? "диска": "дисков";
        _userInterface.Clear(settings.message_prompt_left, settings.message_prompt_top, settings.show_window_lines);
        _userInterface.WriteByXY(5, 2, $"В файловой сиситеме всего: {drives.Length} {dsk}");

        string drv = string.Empty;

        foreach (var drive in drives)
        {
            drv += $" {drive.Name}, ";
        }
        _userInterface.WriteByXY(6, 4, $"{drv.TrimEnd(' ').TrimEnd(',')}");
    }

}
