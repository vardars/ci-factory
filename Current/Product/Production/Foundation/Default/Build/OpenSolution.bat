FOR /F "delims=" %%v IN ('cd') DO set BuildDirectory=%%v
set PATH=%PATH%;%BuildDirectory%\nAnt\bin
call "%ProgramFiles%\Microsoft Visual Studio 9.0\VC\vcvarsall.bat"
devenv "%ProjectName%BuildScripts.sln"
