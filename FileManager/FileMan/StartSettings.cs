using System.Text;

namespace FileMan;
public class StartSettings
{
    const int WINDOW_HEIGHT = 50;
    const int LINE_INDENT = 110;
    const int WINDOW_WIDTH = 121;
    const int COMMAND_PROMPT_LEFT = 3;
    const int COMMAND_PROMPT_TOP = 47;
    const int MESSAGE_PROMPT_LEFT = 3;
    const int MESSAGE_PROMPT_TOP = 1;
    const int BASE_WINDOW_LINES = 35;
    const int MESSAGE_WINDOW_LINES = 8;
    const int PATH_WINDOW_LINES = 3;
    const int COMMAND_PROMPT_LINES = 3;
    const int COMMAND_PROMPT_CLEAR_WIDTH = 115;
    const int SHOW_WINDOW_LINES = 33;
    const int SHOW_PAGES_LINE = 34;
    const int SHORT_FILENAME_LENGTH = 80;
    const int SHORT_DIRNAME_LENGTH = 106;

    const int BASE_WINDOW_TOP = 0;
    const int MASAGE_WINDOW_TOP = 35;
    const int PATH_WINDOW_TOP = 43;
    const int PROMPT_WINDOW_TOP = 46;

    const int BASE_WINDOW_LEFT = 1;
    const int MASAGE_WINDOW_LEFT = 1;
    const int PATH_WINDOW_LEFT = 1;
    const int PROMPT_WINDOW_LEFT = 1;
    const string START_PATH_PROMPT = "c:\\";

    public string start_path_prompt => START_PATH_PROMPT;
    public int win_width => WINDOW_WIDTH;
    public int win_height => WINDOW_HEIGHT;
    public int line_indent => LINE_INDENT;
    public int command_prompt_left => COMMAND_PROMPT_LEFT;
    public int command_prompt_top => COMMAND_PROMPT_TOP;
    public int command_prompt_clear_width => COMMAND_PROMPT_CLEAR_WIDTH;
    public int show_window_lines => SHOW_WINDOW_LINES;
    public int show_page_line => SHOW_PAGES_LINE;
    public int short_filename_length => SHORT_FILENAME_LENGTH;
    public int short_dirname_length => SHORT_DIRNAME_LENGTH;
    public int message_prompt_left => MESSAGE_PROMPT_LEFT;
    public int message_prompt_top => MESSAGE_PROMPT_TOP;
    public int message_window_top => MASAGE_WINDOW_TOP;
    public int message_window_left => MASAGE_WINDOW_LEFT;
    public int path_window_top => PATH_WINDOW_TOP;
    public int path_window_left => PATH_WINDOW_LEFT;
    public void SetWin()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        Console.OutputEncoding = Encoding.UTF8;
        Console.Title = "Файл менеджер 1.0";
        Console.SetWindowSize(WINDOW_WIDTH, WINDOW_HEIGHT);
        Console.SetBufferSize(WINDOW_WIDTH, WINDOW_HEIGHT);
        ConsoleWindow.DisableConsoleSizingAndMinimizing();
        ConsoleWindow.DrawWindow(BASE_WINDOW_LEFT, BASE_WINDOW_TOP, WINDOW_WIDTH - 1, BASE_WINDOW_LINES);
        ConsoleWindow.DrawWindow(MASAGE_WINDOW_LEFT, MASAGE_WINDOW_TOP, WINDOW_WIDTH - 1, MESSAGE_WINDOW_LINES);
        ConsoleWindow.DrawConsole(@"Путь: " + START_PATH_PROMPT, PATH_WINDOW_LEFT, PATH_WINDOW_TOP, WINDOW_WIDTH - 1, PATH_WINDOW_LINES);
        ConsoleWindow.DrawWindow(PROMPT_WINDOW_LEFT, PROMPT_WINDOW_TOP, WINDOW_WIDTH - 1, COMMAND_PROMPT_LINES);
    }
}
