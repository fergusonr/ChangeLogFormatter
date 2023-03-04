# Git change log formatter

Simple git log formatter in C# loosely based on https://github.com/CookPete/auto-changelog

&nbsp;&nbsp;**ChangeLogFormatter [-html] [-md] [-text]** 

&nbsp;&nbsp;**% git log --pretty=format:"%cd %d %s" --date=format:"%d/%m/%y" | ChangeLogFormatter -md > changelog.md**


Converts this:-
```
08/03/23  (HEAD -> master, tag: 1.0.2) Better formatted html
03/03/23  (tag: 1.0.1, origin/master) Simplify regex
01/03/23  Refactor tests
01/03/23  (tag: 1.0.0) Improved 'tag' detection
01/03/23  Initial project files
```

To this:-
#### 1.0.2
> 08 March 2023
- Better formatted html
#### 1.0.1
> 03 March 2023
- Simplify regex
- Refactor tests
#### 1.0.0
> 01 March 2023
- Improved 'tag' detection
- Initial project files