CALL ..\Build\SetEnv.bat
nant.exe -listener:NAnt.Core.XmlLogger -buildfile:Personal.Build.xml Eclipse.CreateProject %*
@echo off
SET /P variable="Hit Enter to Exit."