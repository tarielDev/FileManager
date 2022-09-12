using FileMan.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Intrinsics.X86;
using System.Text;

namespace FileMan;

public class ConsoleUserInterface : IUserInterface, IHighlight
{
    private Dictionary<(int, int), string> recc = new();
    public bool attrChanged { get; set; }
    private enum HARS
    {
        H = 1,
        A = 2,
        R = 3,
        S = 4
    }
    private string DHARS { get; set; }
    private string xxxxx { get; set; }
    private string Ext { get; set; }
    private int attNum { get; set; }
    private int attNumStatus { get; set; }
    private int page { get; set; }
    private (int, int) dharsXY { get; set; }
    private (int, int) XY { get; set; }
    private int dharsID { get; set; }
    private string dharsPath { get; set; }


    public List<(int, int)> AttrCoordinates(string atrr)
    {
        List<(int, int)> sss = new();
        if (atrr[0] != 'D')
        {

            FileInfo fi = new FileInfo(GetExt());
            if (fi.Exists)
            {
                if (fi.Extension.Equals(".txt"))
                {
                    sss = AttributesDatas(true);
                }
                else
                    sss = AttributesDatas();
            }
        }
        else
            sss = AttributesDatas();

        return sss;

    }
    public string ReadKey(string? prompt, bool promptNewLine = true, bool _first_time = false)
    {

        StringBuilder command = new();
        ConsoleKeyInfo keyInfo;
        char key; string ccm;

        var settings = new StartSettings();
        Console.SetCursorPosition(settings.command_prompt_left, settings.command_prompt_top);

        WritePrompt(prompt, promptNewLine);

        (int left, int top) = GetCursorPosition();

        if (_first_time)
        {
            command.Append("ls");
            Console.ResetColor();
            Clear(settings.command_prompt_left, settings.command_prompt_top, 1);
            Console.SetCursorPosition(settings.command_prompt_left, settings.command_prompt_top);
            WritePrompt(prompt, promptNewLine);
            return command.ToString();
        }

        do
        {
            keyInfo = Console.ReadKey();
            key = keyInfo.KeyChar;
            
            if (keyInfo.Key != ConsoleKey.Enter
                && keyInfo.Key != ConsoleKey.Escape
                && keyInfo.Key != ConsoleKey.Spacebar
                && keyInfo.Key != ConsoleKey.Tab
                && keyInfo.Key != ConsoleKey.Backspace
                && keyInfo.Key != ConsoleKey.Delete
                && keyInfo.Key != ConsoleKey.Home
                && keyInfo.Key != ConsoleKey.End
                && keyInfo.Key != ConsoleKey.Insert
                && keyInfo.Key != ConsoleKey.PageUp
                && keyInfo.Key != ConsoleKey.PageDown

                && keyInfo.Key != ConsoleKey.UpArrow
                && keyInfo.Key != ConsoleKey.DownArrow
                && keyInfo.Key != ConsoleKey.LeftArrow
                && keyInfo.Key != ConsoleKey.RightArrow)
                command.Append(key);

            (int currentLeft, _) = GetCursorPosition();

            if (command.Length == 0 && (keyInfo.Key == ConsoleKey.UpArrow || keyInfo.Key == ConsoleKey.DownArrow || keyInfo.Key == ConsoleKey.LeftArrow || keyInfo.Key == ConsoleKey.RightArrow))
            {
                var recc = GetPrintData();
                string cmd;
                switch (keyInfo.Key)
                {
                    case ConsoleKey.UpArrow:

                        HighlightRow(recc, true, left, top);
                        cmd = $"ls up";
                        command.Append(cmd);
                        Console.ResetColor();
                        Clear(settings.command_prompt_left, settings.command_prompt_top, 1);
                        Console.SetCursorPosition(settings.command_prompt_left, settings.command_prompt_top);
                        WritePrompt(prompt, promptNewLine);
                        attNum = -1;
                        return command.ToString();

                    case ConsoleKey.DownArrow:

                        HighlightRow(recc, false, left, top);
                        cmd = $"ls dn";
                        command.Append(cmd);
                        Console.ResetColor();
                        Clear(settings.command_prompt_left, settings.command_prompt_top, 1);
                        Console.SetCursorPosition(settings.command_prompt_left, settings.command_prompt_top);
                        WritePrompt(prompt, promptNewLine);
                        attNum = -1;
                        return command.ToString();

                    case ConsoleKey.LeftArrow:

                        cmd = $"ls -";
                        command.Append(cmd);
                        return command.ToString();

                    case ConsoleKey.RightArrow:

                        cmd = $"ls +";
                        command.Append(cmd);
                        return command.ToString();
                }

                Console.ResetColor();
                Clear(settings.command_prompt_left, settings.command_prompt_top, 1);
                Console.SetCursorPosition(settings.command_prompt_left, settings.command_prompt_top);
                WritePrompt(prompt, promptNewLine);
            }

            if (command.Length == 0 && keyInfo.Key == ConsoleKey.Enter)
            {
                ;
                string cmd;
                var recc = GetPrintData();
                string[] xy = recc.Values.Last().Split('#');
                page = xy[2] == null ? 1 : int.Parse(xy[2]);

                _ = Int32.TryParse(xy[0], out Int32 x);
                _ = Int32.TryParse(xy[1], out Int32 y);
                Console.SetCursorPosition(x, y);
                int currentTop;
                (currentLeft, currentTop) = GetCursorPosition();

                string str = recc[(currentLeft, currentTop)];
                if (str.Substring(9, 1) == ".")
                {
                    string str_dir = str.Substring(str.IndexOf('['), str.IndexOf(']') - 6);
                    cmd = $"ls {str_dir}";
                    command.Append(cmd);
                    break;
                }

                if (str.Substring(1, 1) == "D")
                {
                    string str_dir = str.Substring(str.IndexOf('['), str.IndexOf(']') - 6);
                    cmd = $"ls {str_dir}";
                    command.Append(cmd);
                    break;
                }
                else
                {
                    command.Append('f');
                    continue;
                }

            }
            else
            {
                ;
                if (keyInfo.Key == ConsoleKey.Spacebar)
                    command.Append(" ");
            }

            if (keyInfo.Key == ConsoleKey.Backspace)
            {
                if (command.Length > 0)
                {
                    command.Remove(command.Length - 1, 1);
                }

                if (currentLeft >= left)
                {
                    Console.SetCursorPosition(currentLeft, top);
                    Console.Write(" ");
                    Console.SetCursorPosition(currentLeft, top);
                }
                else
                {
                    Console.SetCursorPosition(left, top);
                }
            }

            if (command.Length == 0 && keyInfo.Key == ConsoleKey.Tab)
            {
                #region old code
                //var recx = GetPrintData();
                //string[] xy = recx.Values.Last().Split('#');
                //(int, int) _key = (int.Parse(xy[0]), int.Parse(xy[1]));
                //int pg = int.Parse(xy[2]);
                //string result = recx.ContainsKey(_key) ? recx[_key] : string.Empty;

                //string gaps;
                //if (result.Contains('['))
                //{
                //    var str1 = result.Substring(0, result.IndexOf('['));
                //    var str2 = result.Substring(result.IndexOf('['), result.IndexOf(']') - 6).Replace(' ', '@').Replace("[", "").Replace("]", "");
                //    var str3 = result.Substring(result.IndexOf(']') + 1);
                //    gaps = str1 + str2 + str3;
                //}
                //else
                //{
                //    gaps = result;
                //}

                //string dss = DHARS;

                //string[] tmp = gaps.Split(" ");
                //string atrr = tmp[1];
                //string path = tmp[2];
                //if (path.Replace("@", " ") != string.Empty)
                //{
                //    path = path.Replace("@", " ");
                //}
                //else
                //{
                //    Console.ResetColor();
                //    Clear(settings.command_prompt_left, settings.command_prompt_top, 1);
                //    Console.SetCursorPosition(settings.command_prompt_left, settings.command_prompt_top);
                //    WritePrompt(prompt, promptNewLine);
                //    continue;

                //}


                //string atrr=DHARS;
                //string dhars = string.Empty;
                ////DHARS = atrr;


                //for (int i = 0; i < atrr.Count(); i++)
                //{
                //    if (i == atrr.Count() - 1)
                //    {
                //        if (atrr[i] == '-')
                //            dhars += " ";
                //        else
                //            dhars += "x";
                //    }
                //    else
                //    {
                //        if (atrr[i] == '-')
                //            dhars += " ";
                //        else
                //            dhars += "x";
                //    }
                //}

                //List<(int, int)> sss = new();

                //if (atrr[0] != 'D')
                //{

                //    FileInfo fi = new FileInfo(GetExt());
                //    if (fi.Exists)
                //    {
                //        if (fi.Extension.Equals(".txt"))
                //        {
                //            sss = AttributesDatas(true);
                //        }
                //        else
                //            sss = AttributesDatas();
                //    }
                //}
                //else
                //    sss = AttributesDatas();
                #endregion

                var recc = GetPrintData();
                string[] xy = recc.Values.Last().Split('#');
                _ = Int32.TryParse(xy[0], out Int32 xx);
                _ = Int32.TryParse(xy[1], out Int32 yy);

                string str = recc[(xx, yy)];
                if (str.Substring(9, 1) == ".")
                {
                    command.Append("ls");
                    break;
                }

                string ddhars = GetXXXXX();
                string dDHARS = GetDHARS();

                int x, y, attN;
                List<(int, int)> sss = ShowAttributes(DHARS, out string dhars);
                SetXXXXX(dhars);

                attNum++;

                if (attNum==0)
                    attNum=1;

                attNumStatus = attNum;

                if (attNum > dhars.Length - 1)
                {
                    (x, y) = sss.Last();
                    Console.ResetColor();
                    WriteByXY(x, y, "[" + dhars.Last() + "]");

                    Console.ResetColor();
                    Clear(settings.command_prompt_left, settings.command_prompt_top, 1);
                    Console.SetCursorPosition(settings.command_prompt_left, settings.command_prompt_top);
                    WritePrompt(prompt, promptNewLine);
                    attNum = -1;
                    continue;
                }

                ;

                if (attNum - 1 == -1)
                    attN = dhars.Length - 1; 
                else
                    attN = attNum - 1;


                (x, y) = sss[attN];
                Console.ResetColor();
                WriteByXY(x, y, "[" + dhars[attN] + "]");


                (x, y) = sss[attNum]; 
                SetdharsXY(sss[attNum]); 
                Console.ResetColor();
                ShowSel("[" + dhars[attNum] + "]", x, y); 
                SetdharsID(attNum); 

                Console.ResetColor();
                Clear(settings.command_prompt_left, settings.command_prompt_top, 1);
                Console.SetCursorPosition(settings.command_prompt_left, settings.command_prompt_top);
                WritePrompt(prompt, promptNewLine);

            }

            if (command.Length == 0 && keyInfo.Key == ConsoleKey.Spacebar)
            {
                var recc = GetPrintData();
                string dhars = string.Empty;
                string xxx = GetXXXXX();
                int x, y;

                if (attNumStatus == dhars.Length || attNumStatus==5) 
                {
                    attNum = -1;
                    Console.ResetColor();
                    Clear(settings.command_prompt_left, settings.command_prompt_top, 1);
                    Console.SetCursorPosition(settings.command_prompt_left, settings.command_prompt_top);
                    WritePrompt(prompt, promptNewLine);
                    continue;
                }
                else
                {

                    ManageAttributes(xxx);
                    xxx = GetXXXXX();
                    (int a, int b) = GetXY();
                    ShowSel(" " + GetDHARS(), a, b);
                    attNum = GetdharsID();
                    (x, y) = GetdharsXY();

                    Console.ResetColor();
                    string fi = GetPath();

                    if (xxx[0] == 'x')
                    {
                        if (xxx[attNum] == 'x')
                            ShowSel("[x]", x, y);
                        else
                            ShowSel("[ ]", x, y);

                        DirectoryInfo ff = new DirectoryInfo(fi);
                        FileAttributes fa = new();
                        fa = Str2Attr(GetDHARS());
                        ff.Attributes = fa;
                        SetXXXXX(xxx);
                    }
                    else
                    {
                        if (xxx[attNum] == 'x')
                            ShowSel("[x]", x, y);
                        else
                            ShowSel("[ ]", x, y);

                        FileInfo ff = new FileInfo(fi);
                        FileAttributes fa = new();
                        fa = Str2Attr(GetDHARS());
                        File.SetAttributes(ff.ToString(), fa);
                        SetXXXXX(xxx);

                    }

                    command.Append("ls x");
                    attNum = attNumStatus;
                }

                Console.ResetColor();
                Clear(settings.command_prompt_left, settings.command_prompt_top, 1);
                Console.SetCursorPosition(settings.command_prompt_left, settings.command_prompt_top);
                WritePrompt(prompt, promptNewLine);
                break;
            }

        }
        while (keyInfo.Key != ConsoleKey.Enter);
        return command.ToString();
    }

    private void WritePrompt(string? prompt, bool promptNewLine)
    {
        if (prompt is { Length: > 0 })

            if (promptNewLine)
                WriteLine(prompt);
            else
                Write(prompt);
    }

    public string ReadLine(string? prompt, bool promptNewLine = true)
    {

        WritePrompt(prompt, promptNewLine);

        return Console.ReadLine()!;
    }

    public void Write(string str)
    {
        Console.Write(str);
    }

    public void WriteLine(string str)
    {
        Console.WriteLine(str);
    }

    public static (int left, int top) GetCursorPosition()
    {
        return (Console.CursorLeft, Console.CursorTop);
    }

    public List<(int, int)> AttributesDatas(bool txt = false)
    {
        var settings = new StartSettings();
        List<(int, int)> lst = new();

        if (txt)
        {
            lst.Add((settings.message_window_left + 39, settings.message_window_top + 4));
            lst.Add((settings.message_window_left + 43, settings.message_window_top + 4));
            lst.Add((settings.message_window_left + 47, settings.message_window_top + 4));
            lst.Add((settings.message_window_left + 51, settings.message_window_top + 4));
            lst.Add((settings.message_window_left + 55, settings.message_window_top + 4));
        }
        else
        {
            lst.Add((settings.message_window_left + 49, settings.message_window_top + 4));
            lst.Add((settings.message_window_left + 53, settings.message_window_top + 4));
            lst.Add((settings.message_window_left + 57, settings.message_window_top + 4));
            lst.Add((settings.message_window_left + 61, settings.message_window_top + 4));
            lst.Add((settings.message_window_left + 65, settings.message_window_top + 4));
        }

        return lst;
    }

    public void Clear(int x, int y, int line)
    {
        var settings = new StartSettings();
        for (int i = 0; i < line; i++)
        {
            Console.SetCursorPosition(x, y + i);
            Write(new string(' ', settings.command_prompt_clear_width));
        }
    }

    public void WriteByXY(int x, int y, string str)
    {
        Console.SetCursorPosition(x, y);
        Write(str);
    }

    public Dictionary<(int, int), string> GetPrintData()
    {
        return recc;
    }

    public void SetPrintData(Dictionary<(int, int), string> rec)
    {
        recc = rec;
    }

    public void HighlightRow(Dictionary<(int, int), string> rec, bool move, int left, int top)
    {

        int currentLeft, currentTop;
        string val;

        string[] xy = rec.Values.Last().Split('#');

        _ = Int32.TryParse(xy[0], out Int32 x);
        _ = Int32.TryParse(xy[1], out Int32 y);

        Console.SetCursorPosition(x, y);
        page = xy[2] == null ? 1 : int.Parse(xy[2]);
        (currentLeft, currentTop) = GetCursorPosition();
        val = rec[(currentLeft, currentTop)];
        Console.ResetColor();
        WriteByXY(currentLeft, currentTop, val);

        if (move)
        {
            if (currentTop - 1 >= 1)
            {
                currentTop--;
                ShowSelected(rec, currentLeft, currentTop);
            }
            else
            {
                ShowSelected(rec, currentLeft, currentTop);
                Console.SetCursorPosition(left, top);
            }
        }
        else
        {
            if (currentTop + 1 <= rec.Count - 1)
            {
                currentTop++;
                ShowSelected(rec, currentLeft, currentTop);
            }
            else
            {
                ShowSelected(rec, currentLeft, currentTop);
                Console.SetCursorPosition(left, top);
            }
        }

    }

    public void ShowSelected(Dictionary<(int, int), string> rec, int x, int y)
    {
        Console.SetCursorPosition(x, y);
        string val = rec[(x, y)];
        Console.BackgroundColor = ConsoleColor.White;
        Console.ForegroundColor = ConsoleColor.Black;
        WriteByXY(x, y, val);
        rec[rec.Keys.Last()] = $"{x}#{y}#{page}";
    }

    public void ShowSel(string val, int x, int y)
    {
        Console.SetCursorPosition(x, y);
        Console.BackgroundColor = ConsoleColor.White;
        Console.ForegroundColor = ConsoleColor.Black;
        WriteByXY(x, y, val);
    }

    public void ClearWindow()
    {
        var settings = new StartSettings();
        Clear(settings.message_prompt_left, settings.message_prompt_top, settings.show_window_lines);
        Clear(settings.message_window_left + 2, settings.message_window_top + 1, 6);

    }

    public string Attr2Str(FileAttributes attr)
    {
        string DHARSs = "";
        if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            DHARSs += "D";
        else
            DHARSs += "-";

        if ((attr & FileAttributes.Hidden) == FileAttributes.Hidden)
            DHARSs += "H";
        else
            DHARSs += "-";

        if ((attr & FileAttributes.Archive) == FileAttributes.Archive)
            DHARSs += "A";
        else
            DHARSs += "-";

        if ((attr & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            DHARSs += "R";
        else
            DHARSs += "-";

        if ((attr & FileAttributes.System) == FileAttributes.System)
            DHARSs += "S";
        else
            DHARSs += "-";

        return DHARSs;
    }

    public FileAttributes Str2Attr(string DHARSs)
    {
        FileAttributes attr = new();

        //for (int i = 0; i < 6; i++)
        //{
        if (DHARSs[0] == 'D')
            attr |= FileAttributes.Directory;
        else
            attr &= ~FileAttributes.Directory;
        if (DHARSs[1] == 'H')
            attr |= FileAttributes.Hidden;
        else
            attr &= ~FileAttributes.Hidden;
        if (DHARSs[2] == 'A')
            attr |= FileAttributes.Archive;
        else
            attr &= ~FileAttributes.Archive;
        if (DHARSs[3] == 'R')
            attr |= FileAttributes.ReadOnly;
        else
            attr &= ~FileAttributes.ReadOnly;
        if (DHARSs[4] == 'S')
            attr |= FileAttributes.System;
        else
            attr &= ~FileAttributes.System;
        //}

        return attr;
    }

    public string GetExt() => Ext;

    public void SetPath(string path)
    {
        dharsPath = path;
    }

    public string GetPath() => dharsPath;

    public void SetExt(string ext)
    {
        Ext = ext;
    }

    public string GetDHARS() => DHARS;

    public void SetDHARS(string dhars)
    {
        DHARS = dhars;
    }

    public List<(int, int)> ShowAttributes(string atrr, out string atr)
    {
        List<(int, int)> sss = new();
        string dhars = string.Empty;

        for (int i = 0; i < atrr.Count(); i++)
        {
            if (i == atrr.Count() - 1)
            {
                if (atrr[i] == '-')
                    dhars += " ";
                else
                    dhars += "x";
            }
            else
            {
                if (atrr[i] == '-')
                    dhars += " ";
                else
                    dhars += "x";
            }
        }
        atr = dhars;


        if (atrr[0] != 'D')
        {

            FileInfo fi = new FileInfo(GetExt());
            if (fi.Exists)
            {
                if (fi.Extension.Equals(".txt"))
                {
                    sss = AttributesDatas(true);
                }
                else
                    sss = AttributesDatas();
            }
        }
        else
            sss = AttributesDatas();

        return sss;

    }

    public string GetXXXXX() => xxxxx;

    public void SetXXXXX(string dhars)
    {
        xxxxx = dhars;
    }

    public (int, int) GetXY() => XY;

    public void SetXY((int, int) xy)
    {
        XY = xy;
    }

    public (int, int) GetdharsXY() => dharsXY;

    public void SetdharsXY((int, int) xy)
    {
        dharsXY = xy;
    }

    public int GetdharsID() => dharsID;

    public void SetdharsID(int id)
    {
        dharsID = id;
    }

    public int GetAttNum() => attNum;

    public void SetAttNum(int attnum)
    {
        attNum = attnum;
    }

    public void ManageAttributes(string xxx)
    {
        string dhars = string.Empty;
        string sss = string.Empty;
        //string xxx = GetXXXXX();

        if (xxx[0] == 'x')
        {
            dhars += 'x';
            sss += 'D';
            for (int i = 1; i < xxx.Count(); i++)
            {
                if (i != attNum) //attNumStatus
                {
                    if (xxx[i] == 'x')
                    {
                        dhars += "x";
                        sss += (HARS)i;
                    }
                    else
                    {
                        dhars += " ";
                        sss += '-';
                    }
                }
                else
                {
                    if (xxx[i] == 'x')
                    {
                        dhars += " ";
                        sss += '-';
                    }
                    else
                    {
                        dhars += "x";
                        sss += (HARS)i;
                    }
                }
            }

        }
        else
        {
            dhars += ' ';
            sss += '-';
            for (int i = 1; i < xxx.Count(); i++)
            {
                if (i != attNum) //attNumStatus
                {
                    if (xxx[i] == 'x')
                    {
                        dhars += "x";
                        sss += (HARS)i;
                    }
                    else
                    {
                        dhars += " ";
                        sss += '-';
                    }
                }
                else
                {
                    if (xxx[i] == 'x')
                    {
                        dhars += " ";
                        sss += '-';
                    }
                    else
                    {
                        dhars += "x";
                        sss += (HARS)i;
                    }
                }
            }
        }

        ;

        SetXXXXX(dhars);
        SetDHARS(sss);

    }

    public bool GetAttrChange() => attrChanged;

    public void SetAttrChange(bool ch)
    {
        attrChanged = ch;
    }
}
