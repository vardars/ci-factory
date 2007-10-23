set PATH=%SystemRoot%\system32;%SystemRoot%;%SystemRoot%\System32\Wbem;%ProgramFiles%\Subversion\bin;%BuildDirectory%\nAnt\bin
call "%ProgramFiles%\Microsoft Visual Studio 8\VC\vcvarsall.bat"
devenv "%ProjectName%BuildScripts.sln"
