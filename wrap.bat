@echo off
set targetdir=Mkay.Nuget
set jsdir=%targetdir%\content\Scripts
set dlldir=%targetdir%\lib\net40
xcopy Mkay.JS\Scripts\*.js %jsdir% /Y /Q
xcopy Mkay\bin\debug\Mkay.dll %dlldir% /Y /Q
cd Mkay.Nuget
del *.nupkg
7z a -tzip Mkay.0.0.2.nupkg .
cd ..