@echo off
dotnet publish epub2cbz.csproj -r win-x64 -c Release -f net9.0-windows7.0 -o bin\ /p:PublishReadyToRun=true /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true /p:SelfContained=true /p:DebugType=None /p:DebugSymbols=false
pause