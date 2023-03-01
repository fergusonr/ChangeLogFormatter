# Git change log formatter

Simple git log formatter in C# loosely based on https://github.com/CookPete/auto-changelog

&nbsp;&nbsp;**ChangeLogFormatter [-html] [-md] [-text]** 

&nbsp;&nbsp;**% git log --pretty=format:"%cd %d %s" --date=format:"%d/%m/%y" | ChangeLogFormatter > changelog.txt**


Converts this:-
```24/02/23  (HEAD -> master, origin/master, origin/main, origin/HEAD) A few more topics post 2.40-rc0
24/02/23  Merge branch 'ps/free-island-marks'
24/02/23  Merge branch 'jk/http-proxy-tests'
24/02/23  Merge branch 'ma/fetch-parallel-use-online-cpus'
24/02/23  (tag: v2.40.0-rc0) Git 2.40-rc0
24/02/23  Merge branch 'jc/genzeros-avoid-raw-write'
24/02/23  Merge branch 'rd/doc-default-date-format'
24/02/23  Merge branch 'js/gpg-errors'
06/02/23  (tag: v2.39.2) Git 2.39.2
06/02/23  Sync with 2.38.4
06/02/23  (tag: v2.38.4) Git 2.38.4
06/02/23  Sync with 2.37.6
06/02/23  (tag: v2.37.6) Git 2.37.6
06/02/23  Sync with 2.36.5
```

To this:-
<html>
<table>
<tr><td style="background-color:darkblue;color:white;"><b>v2.40.0-rc0 24 February 2023</b></td></tr>
<tr><td>&nbsp;&nbsp;Git 2.40-rc0</td></tr>
<tr><td>&nbsp;&nbsp;Merge branch 'jc/genzeros-avoid-raw-write'</td></tr>
<tr><td>&nbsp;&nbsp;Merge branch 'rd/doc-default-date-format'</td></tr>
<tr><td>&nbsp;&nbsp;Merge branch 'js/gpg-errors'</td></tr>
</table></br>
<table>
<tr><td style="background-color:darkblue;color:white;"><b>v2.39.2 06 February 2023</b></td></tr>
<tr><td>&nbsp;&nbsp;Git 2.39.2</td></tr>
<tr><td>&nbsp;&nbsp;Sync with 2.38.4</td></tr>
</table></br>
<table>
<tr><td style="background-color:darkblue;color:white;"><b>v2.38.4 06 February 2023</b></td></tr>
<tr><td>&nbsp;&nbsp;Git 2.38.4</td></tr>
<tr><td>&nbsp;&nbsp;Sync with 2.37.6</td></tr>
</table></br>
<table>
<tr><td style="background-color:darkblue;color:white;"><b>v2.37.6 06 February 2023</b></td></tr>
<tr><td>&nbsp;&nbsp;Git 2.37.6</td></tr>
<tr><td>&nbsp;&nbsp;Sync with 2.36.5</td></tr>
</table></br>
<table>
<html>
