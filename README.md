# Git change log formatter

Simple git log formatter in C# loosely based on https://github.com/CookPete/auto-changelog

&nbsp;&nbsp;**ChangeLogFormatter -text|-md|-html [outfile]** 



From standard git format:-
```
11/03/23  (HEAD -> master, tag: 1.1.0) Switch to LibGit2Sharp
11/03/23  (tag: 1.0.2) Add usage message. Refactor
09/03/23  (origin/master) Better formatted md / html
03/03/23  (tag: 1.0.1) Simplify regex
01/03/23  Refactor tests
01/03/23  (tag: 1.0.0) Improved 'tag' detection
01/03/23  Initial project files
```

To this:-
#### 1.1.0
> 11 March 2023
- Switch to LibGit2Sharp
#### 1.0.2
> 11 March 2023
- Add usage message. Refactor
- (origin/master) Better formatted md / html
#### 1.0.1
> 03 March 2023
- Simplify regex
- Refactor tests
#### 1.0.0
> 01 March 2023
- Improved 'tag' detection
- Initial project files