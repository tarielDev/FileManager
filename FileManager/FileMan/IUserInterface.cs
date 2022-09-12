
using System;

namespace FileMan;

public interface IUserInterface
{
    List<(int, int)> AttributesDatas(bool txt=false);
    void Write(string str);
    void ShowSel(string val, int x, int y);
    void Clear(int x, int y, int line);
    void ClearWindow();
    void WriteByXY(int x, int y, string str);
    void WriteLine(string str);
    string ReadLine(string? prompt, bool promptNewLine=true);
    string ReadKey(string? prompt, bool promptNewLine = true, bool _first_time=false);
    Dictionary<(int, int), string> GetPrintData();
    void SetPrintData(Dictionary<(int, int), string> rec);
    string Attr2Str(FileAttributes attr);
    FileAttributes Str2Attr(string DHARS);
    string GetExt();
    void SetExt(string ext);
    void SetPath(string path);
    string GetPath();
    string GetDHARS();
    void SetDHARS(string dhars);
    string GetXXXXX();
    void SetXXXXX(string dhars);
    List<(int, int)> ShowAttributes(string dhars, out string atr);
    void ManageAttributes(string xxx);
    (int, int) GetXY();
    void SetXY((int, int) xy);
    (int, int) GetdharsXY();
    void SetdharsXY((int, int) xy);
    int GetdharsID();
    void SetdharsID(int id); 
    bool GetAttrChange();
    void SetAttrChange(bool ch);
    int GetAttNum();
    void SetAttNum(int attnum);

}
