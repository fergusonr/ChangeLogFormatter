# Git change log formatter

Simple git log formatter in C# loosely based on https://github.com/CookPete/auto-changelog

&nbsp;&nbsp;**ChangeLogFormatter -text | -md | -rtf | -html [-nocredit] [-repo path] [outfile]** 



From standard git output:-
```
% git log --pretty=format:"%cd %d %s" --date=format:"%d/%m/%y"

30/03/23  (HEAD -> master, tag: 1.1.3, origin/master) DotNet7.0
20/03/23  (tag: 1.1.2) Optimise rtf output, fix text duplicates
13/03/23  (tag: 1.1.1) Support Rtf output. -repo path argument
11/03/23  (tag: 1.1.0) Switch to LibGit2Sharp
11/03/23  (tag: 1.0.2) Add usage message. Refactor
09/03/23  Better formatted md / html
03/03/23  (tag: 1.0.1) Simplify regex
01/03/23  Refactor tests
01/03/23  (tag: 1.0.0) Improved 'tag' detection
01/03/23  Initial project files
```

To this:-
#### <span style="background-color:rgb(0,100,0);color:rgb(255,255,255)">1.1.3</span>
30 March 2023
- DotNet7.0
#### <span style="background-color:rgb(0,100,0);color:rgb(255,255,255)">1.1.2</span>
13 March 2023
- Optimise rtf output, fix text duplicates
#### <span style="background-color:rgb(0,100,0);color:rgb(255,255,255)">1.1.1</span>
11 March 2023
- Support Rtf output. -repo path argument
#### <span style="background-color:rgb(0,100,0);color:rgb(255,255,255)">1.1.0</span>
11 March 2023
- Switch to LibGit2Sharp
#### <span style="background-color:rgb(0,100,0);color:rgb(255,255,255)">1.0.2</span>
09 March 2023
- Add usage message. Refactor
- Better formatted md / html
#### <span style="background-color:rgb(0,100,0);color:rgb(255,255,255)">1.0.1</span>
03 March 2023
- Simplify regex
- Refactor tests
#### <span style="background-color:rgb(0,100,0);color:rgb(255,255,255)">1.0.0</span>
01 March 2023
- Improved 'tag' detection
- Initial project files