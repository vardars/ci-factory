set PATH=%SystemRoot%\system32;%SystemRoot%;%SystemRoot%\System32\Wbem;%ProgramFiles%\PVCS\Tracker\nt;%ProgramFiles%\Subversion\bin;C:\Projects\CI Factory\Current\Third Party\nAnt\bin
call "%ProgramFiles%\Microsoft Visual Studio 8\VC\vcvarsall.bat"
nant -buildfile:OpenSolution.Script.xml
devenv "CI Factory.sln"
