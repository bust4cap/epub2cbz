@echo off
dotnet publish epub2cbz.csproj -c Release -f net9.0-windows7.0 -o bin\epub2cbz\ /p:SelfContained=false /p:DebugType=None /p:DebugSymbols=false
pause