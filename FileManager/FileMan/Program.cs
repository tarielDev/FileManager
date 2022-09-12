using FileMan;

var conUI = new ConsoleUserInterface();
new StartSettings().SetWin();
new FileManagerLogic(conUI, new DirsAndFiles(conUI)).Start();


