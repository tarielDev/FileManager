
namespace FileMan;

public interface IHighlight
{
    void HighlightRow(Dictionary<(int, int), string> rec, bool move, int left, int top);
    void ShowSelected(Dictionary<(int, int), string> rec, int x, int y);

}
