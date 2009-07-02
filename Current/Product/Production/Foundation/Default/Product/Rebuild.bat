CALL ..\Build\SetEnv.bat
nant.exe -listener:NAnt.Core.XmlLogger -buildfile:Personal.Build.xml UpdateSource
IF NOT %ERRORLEVEL%==0 exit /B %ERRORLEVEL%
nant.exe -listener:NAnt.Core.XmlLogger -buildfile:Personal.Build.xml Rebuild
@echo off
SET /P variable="Hit Enter to exit."