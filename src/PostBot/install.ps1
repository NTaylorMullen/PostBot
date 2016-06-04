$env:PATH="$env:PATH;$env:WINDIR\Microsoft.NET\Framework\v4.0.30319"

InstallUtil /u PostBot.exe
InstallUtil /i PostBot.exe

net start PostBot
PAUSE