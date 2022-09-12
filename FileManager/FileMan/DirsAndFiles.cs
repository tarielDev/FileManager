using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace FileMan;

public class DirsAndFiles
{
    private readonly IUserInterface _userInterface;
    public DirectoryInfo CurrentDirectory { get; set; } = new(new StartSettings().start_path_prompt);
    public DirsAndFiles(IUserInterface userInterface) => _userInterface = userInterface;
    
    public void GetDirsAndFiles(StringBuilder dirData, string dirc = "")
    {
        var settings = new StartSettings();
        string dirStr;

        if (dirc == "")
        {
            if (CurrentDirectory.Parent is null)
                dirStr = Path.Combine(CurrentDirectory.FullName).ToString();
            else
                dirStr = Path.Combine(CurrentDirectory.FullName, dirc).ToString();
        }
        else
        {
            if (dirc.Replace("@", " ") != " .. ")
            {
                dirStr = Path.Combine(CurrentDirectory.FullName, dirc.Replace("@", " ")).ToString();
            }
            else
            {
                if (CurrentDirectory.Parent is null)
                {
                    _userInterface.Clear(settings.message_window_left + 2, settings.message_window_top + 1, 6);
                    _userInterface.WriteByXY(settings.message_window_left + 2, settings.message_window_top + 2, $"Невозможно подняться выше по дереву директорий.");
                    DrawDirsAndFiles();
                    dirStr = Path.Combine(CurrentDirectory.FullName).ToString();

                }
                else
                    dirStr = CurrentDirectory.Parent.ToString();
            }

        }

        DirectoryInfo directory = new DirectoryInfo(dirStr);

        int first = 0;
        var dirs_count = 0;
        string DHARSs;

        dirData.Append($"       [ .. ] {new string(' ', settings.line_indent - 10)}\n");
        foreach (var dir in directory.EnumerateDirectories())
        {
            DHARSs = _userInterface.Attr2Str(dir.Attributes);
            int fl = "<Директория>".Length;
            string dr = new string(' ', settings.line_indent - fl - dir.Name.Length - (DHARSs.Length + 1)) + "<Директория>";
            dirData.Append($" {DHARSs} [{dir.Name}] {dr}\n");

            dirs_count++;
            first++;
        }

        var files_count = 0;
        long total = 0;
        string shortName, shortDirName;

        foreach (var file in directory.EnumerateFiles())
        {

            int sss = file.Name.Length;
            if (file.Name.Length >= settings.short_filename_length)
            {
                shortName = file.Name.Substring(0, settings.short_filename_length) + "...";
            }
            else
            {
                shortName = file.Name;
            }

            DHARSs = _userInterface.Attr2Str(file.Attributes);
            int fl = file.Length.ToString("### ### ### ###").Length + 5;
            string dr;
            if (file.Length > 0)
                dr = new string(' ', settings.line_indent - fl - shortName.Length - (DHARSs.Length + 1)) + file.Length.ToString("### ### ### ###") + " байт";
            else
                dr = new string(' ', settings.line_indent - fl - shortName.Length - (DHARSs.Length + 1)) + file.Length.ToString("  0") + " байт";

            dirData.Append($" {DHARSs} {shortName}   {dr}\n");
            files_count++;
            total += file.Length;
        }

        CurrentDirectory = directory;
        if (CurrentDirectory.ToString().Length >= settings.short_dirname_length)
        {
            shortDirName = Path.Combine(CurrentDirectory.FullName.Substring(0, settings.short_dirname_length) + "...").ToString();
        }
        else
        {
            shortDirName = Path.Combine(CurrentDirectory.FullName).ToString();
        }
        _userInterface.Clear(settings.path_window_left + 2, settings.path_window_top + 1, 1);
        _userInterface.WriteByXY(settings.path_window_left + 2, settings.path_window_top + 1, $"Путь: {shortDirName}");

        Directory.SetCurrentDirectory(directory.FullName);
    }
    
    public void DrawDirsAndFiles(string dirc = "", int page = 1)
    {
        var settings = new StartSettings();

        var rec = new Dictionary<(int, int), string>();
        int firstX, firstY;
        string first = "";

        StringBuilder tree = new StringBuilder();

        if (dirc == "x")
        {
            _userInterface.SetAttrChange(true);
            var reccc = _userInterface.GetPrintData();
            string[] xy = reccc.Values.Last().Split('#');
            _ = Int32.TryParse(xy[2], out Int32 z);
            page = z;
            dirc = "";
        }

        if (dirc == "+")
        {
            var reccc = _userInterface.GetPrintData();
            string[] xy = reccc.Values.Last().Split('#');
            _ = Int32.TryParse(xy[2], out Int32 z);

            page = z + 1;
            dirc = "";
        }

        if (dirc == "-")
        {
            var reccc = _userInterface.GetPrintData();
            string[] xy = reccc.Values.Last().Split('#');
            _ = Int32.TryParse(xy[2], out Int32 z);

            page = z - 1;
            dirc = "";
        }

        GetDirsAndFiles(tree, dirc);
        Console.SetCursorPosition(2, 0);

        int currentLeft = Console.CursorLeft;
        int currentTop = Console.CursorTop;

        int pageLines = settings.show_window_lines;
        string[] lines = tree.ToString().Split('\n');
        int pageTotal = (lines.Length + pageLines - 1) / pageLines;

        if (page > pageTotal)
            page = pageTotal;

        if (page < 1)
            page = 1;

        for (int i = (page - 1) * pageLines, counter = 0; i < page * pageLines; i++, counter++)
        {
            if (lines.Length - 1 > i)
            {
                Console.ResetColor();

                firstX = currentLeft + 1;
                firstY = currentTop + 1 + counter;

                if (!_userInterface.GetAttrChange())
                {
                    if (counter == 0)
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Black;
                        firstX = currentLeft + 1;
                        firstY = currentTop + 1 + counter;
                        first = $"{firstX}#{firstY}#{page}";
                    }
                }
                else
                {
                    (int a, int b) = _userInterface.GetXY();
                    if (firstX == a && firstY == b)
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Black;
                        first = $"{a.ToString()}#{b.ToString()}#{page}";
                    }
                }

                Console.SetCursorPosition(currentLeft + 1, currentTop + 1 + counter);
                Console.WriteLine(lines[i]);
                rec.Add((currentLeft + 1, currentTop + 1 + counter), lines[i]);
            }
        }

        Console.ResetColor();
        rec.Add((3, lines.Length), first);

        if (_userInterface.GetAttrChange())
        {
            string xxx = _userInterface.GetXXXXX();
            (int x, int y) = _userInterface.GetdharsXY();
            int id=_userInterface.GetdharsID();
            GetDirsAndFilesCount();
            _userInterface.ShowSel("[" + xxx[id] + "]", x, y);
            _userInterface.SetAttrChange(false);
        }

        Console.ResetColor();
        string footer = $"╣ {page} из {pageTotal} ╠";
        Console.SetCursorPosition(settings.command_prompt_left, settings.show_page_line);
        Console.Write(new string('═', settings.line_indent));
        Console.SetCursorPosition(settings.win_width / 2 - footer.Length / 2 - 1, settings.show_page_line);
        Console.WriteLine(footer);

        _userInterface.SetPrintData(rec);

    }
    
    public void GetDirsAndFilesCount()
    {
        var settings = new StartSettings();
        string dirStr;

        var recx = _userInterface.GetPrintData();
        string[] xy = recx.Values.Last().Split('#');
        (int, int) key = (int.Parse(xy[0]), int.Parse(xy[1]));
        _userInterface.SetXY(key);
        int pg = int.Parse(xy[2]);
        string result = recx.ContainsKey(key) ? recx[key] : string.Empty;

        string gaps;
        if (result.Contains('['))
        {
            var str1 = result.Substring(0, result.IndexOf('['));
            var str2 = result.Substring(result.IndexOf('['), result.IndexOf(']') - 6).Replace(' ', '@').Replace("[", "").Replace("]", "");
            var str3 = result.Substring(result.IndexOf(']') + 1);
            gaps = str1 + str2 + str3;
        }
        else
        {
            gaps = result;
        }

        string[] tmp = gaps.Split(" ");
        string atrr=tmp[1];

        if (!_userInterface.GetAttrChange())
        {
            _userInterface.SetDHARS(tmp[1]);
        }

        string path =tmp[2];
        
        string razm= " " + tmp[tmp.Length - 5] + " " + tmp[tmp.Length - 4] + " " + tmp[tmp.Length-3] + " " + tmp[tmp.Length - 2] + " " + tmp[tmp.Length - 1];
        
        if (atrr == string.Empty)
            return;

        if (atrr.Substring(0, 1) == "D")
        {
            if (path == "")
            {
                if (CurrentDirectory.Parent is null)
                    dirStr = Path.Combine(CurrentDirectory.FullName).ToString();
                else
                    dirStr = Path.Combine(CurrentDirectory.FullName, path).ToString();
            }
            else
            {
                if (path.Replace("@", " ") != " .. ")
                {
                    dirStr = Path.Combine(CurrentDirectory.FullName, path.Replace("@", " ")).ToString();
                }
                else
                {
                    if (CurrentDirectory.Parent is null)
                    {
                        GetDirsAndFilesCount();
                        dirStr = Path.Combine(CurrentDirectory.FullName).ToString();
                    }
                    else
                        dirStr = CurrentDirectory.Parent.ToString();
                }

            }

            _userInterface.SetPath(dirStr);

            DirectoryInfo directory = new DirectoryInfo(dirStr);

            var dirs_count = 0;
            var files_count = 0;

            try
            {
                foreach (var dir in directory.EnumerateDirectories())
                {
                    dirs_count++;
                }

                foreach (var file in directory.EnumerateFiles())
                {
                    files_count++;
                }

                string dhars = string.Empty;
                
                for (int i = 0; i < atrr.Count(); i++)
                {
                    if (i== atrr.Count() - 1)
                    {
                        if (atrr[i] == '-')
                            dhars += "[ ]";
                        else
                            dhars += "[x]";
                    }
                    else
                    {
                        if (atrr[i] == '-')
                            dhars += "[ ] ";
                        else
                            dhars += "[x] ";
                    }
                }

                _userInterface.Clear(settings.message_window_left + 2, settings.message_window_top + 1, 6);
                _userInterface.WriteByXY(settings.message_window_left + 2, settings.message_window_top + 2, $"Директорий:   {dirs_count} шт");
                _userInterface.WriteByXY(settings.message_window_left + 2, settings.message_window_top + 4, $"Файлов:       {files_count} шт");
                _userInterface.WriteByXY(settings.message_window_left + 49, settings.message_window_top + 2, $" D   H   A   R   S ");
                _userInterface.WriteByXY(settings.message_window_left + 49, settings.message_window_top + 4, $"{dhars}");
                _userInterface.WriteByXY(settings.message_window_left + 23, settings.message_window_top + 6, $"D - Директория, H - Невидимый, A - Архивный, R - Только чтение, S - Системный");
            }
            catch
            {

                string dhars = string.Empty;
                
                for (int i = 0; i < atrr.Count(); i++)
                {
                    if (i == atrr.Count() - 1)
                    {
                        if (atrr[i] == '-')
                            dhars += "[ ]";
                        else
                            dhars += "[x]";
                    }
                    else
                    {
                        if (atrr[i] == '-')
                            dhars += "[ ] ";
                        else
                            dhars += "[x] ";
                    }
                }
                _userInterface.Clear(settings.message_window_left + 2, settings.message_window_top + 1, 6);
                _userInterface.WriteByXY(settings.message_window_left + 2, settings.message_window_top + 2, $"При выполнении команды произошла ошибка:");
                _userInterface.WriteByXY(settings.message_window_left + 2, settings.message_window_top + 4, $"У вас нет прав доступа в директорию! ");
                _userInterface.WriteByXY(settings.message_window_left + 49, settings.message_window_top + 2, $" D   H   A   R   S ");
                _userInterface.WriteByXY(settings.message_window_left + 49, settings.message_window_top + 4, $"{dhars}");
                _userInterface.WriteByXY(settings.message_window_left + 23, settings.message_window_top + 6, $"D - Директория, H - Невидимый, A - Архивный, R - Только чтение, S - Системный");

            }
        }
        else
        {

            string dhars = string.Empty;
            for (int i = 0; i < atrr.Count(); i++)
            {
                if (i == atrr.Count()-1)
                {
                    if (atrr[i] == '-')
                        dhars += "[ ]";
                    else
                        dhars += "[x]";
                }
                else
                {
                    if (atrr[i] == '-')
                        dhars += "[ ] ";
                    else
                        dhars += "[x] ";
                }
            }

            FileInfo fi = new FileInfo(Path.Combine(CurrentDirectory.FullName, path));
            _userInterface.SetPath(fi.ToString());
            if (fi.Exists)
            {
                _userInterface.SetExt(fi.FullName.ToString());

                if (fi.Extension.Equals(".txt"))
                {
                    Dictionary<string, int> txtFileData = GetDatasFromTxtFile(fi.ToString());
                    _userInterface.Clear(settings.message_window_left + 2, settings.message_window_top + 1, 6);
                    _userInterface.WriteByXY(settings.message_window_left + 2, settings.message_window_top + 2, $"Размер файла:{razm} ");
                    _userInterface.WriteByXY(settings.message_window_left + 39, settings.message_window_top + 2, $" D   H   A   R   S ");
                    _userInterface.WriteByXY(settings.message_window_left + 39, settings.message_window_top + 4, $"{dhars}");
                    _userInterface.WriteByXY(settings.message_window_left + 23, settings.message_window_top + 6, $"D - Директория, H - Невидимый, A - Архивный, R - Только чтение, S - Системный");

                    _userInterface.WriteByXY(settings.message_window_left + 65, settings.message_window_top + 2, $"Слов: {txtFileData["Words"]}");
                    _userInterface.WriteByXY(settings.message_window_left + 65, settings.message_window_top + 4, $"Букв: {txtFileData["Characters"]}");
                    _userInterface.WriteByXY(settings.message_window_left + 81, settings.message_window_top + 2, $"Строк: {txtFileData["Lines"]}");
                    _userInterface.WriteByXY(settings.message_window_left + 81, settings.message_window_top + 4, $"Параграфов: {txtFileData["Paragraphs"]}");

                }
                else
                {
                    _userInterface.Clear(settings.message_window_left + 2, settings.message_window_top + 1, 6);
                    _userInterface.WriteByXY(settings.message_window_left + 2, settings.message_window_top + 2, $"Размер файла:{razm} ");
                    _userInterface.WriteByXY(settings.message_window_left + 49, settings.message_window_top + 2, $" D   H   A   R   S ");
                    _userInterface.WriteByXY(settings.message_window_left + 49, settings.message_window_top + 4, $"{dhars}");
                    _userInterface.WriteByXY(settings.message_window_left + 23, settings.message_window_top + 6, $"D - Директория, H - Невидимый, A - Архивный, R - Только чтение, S - Системный");
                }

            }

        }

    }
    
    public Dictionary<string,int> GetDatasFromTxtFile(string FilePath)
    {
        Dictionary<string, int> datas = new();
        Regex nonWhitespace = new Regex(@"\S", RegexOptions.Compiled);
        Regex word = new Regex(@"\b\W+\b", RegexOptions.Compiled);
        Regex emptyString = new Regex("^$", RegexOptions.Compiled);

        int characterCount = 0,
            wordCount = 0,
            lineCount = 0,
            paragraphCount = 0;

        bool paragraphStartFound = false;

        using (FileStream theFile = File.Open(FilePath, FileMode.Open, FileAccess.Read))
        using (StreamReader reader = new(theFile))
        {
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();

                lineCount++;

                if (emptyString.IsMatch(line))
                {
                    if (paragraphStartFound)
                    {
                        paragraphCount++;
                        paragraphStartFound = false;
                    }
                }
                else
                {
                    paragraphStartFound = true;

                    MatchCollection characterMatches = nonWhitespace.Matches(line);
                    characterCount += characterMatches.Count;

                    MatchCollection wordMatches = word.Matches(line);
                    wordCount += wordMatches.Count;
                }
            }
        }

        if (paragraphStartFound)
            paragraphCount++;

        datas.Add("Words", wordCount);
        datas.Add("Characters", characterCount);
        datas.Add("Lines", lineCount);
        datas.Add("Paragraphs", paragraphCount);

        return datas;
    }

}
