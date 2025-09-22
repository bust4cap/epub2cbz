@echo off
dotnet publish -r win-x64 -c Release /p:PublishReadyToRun=true /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true /p:SelfContained=true /p:DebugType=None
pause