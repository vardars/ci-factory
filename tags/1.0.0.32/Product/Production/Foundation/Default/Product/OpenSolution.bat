cd ..\%BuildDirectoryName%\nAnt
FOR /F "TOKENS=1 DELIMS=," %%A IN ('dir /A:D /B') DO set AntBinPath=%%~fA
set PATH=%SystemRoot%\system32;%SystemRoot%;%SystemRoot%\System32\Wbem;%AntBinPath%
call "%ProgramFiles%\Microsoft Visual Studio 8\VC\vcvarsall.bat"
cd ..\..\%ProductDirectoryName%
nant -buildfile:OpenSolution.Script.xml
FOR /F "TOKENS=1 DELIMS=," %%A IN ('dir /b *.sln') DO SET solutionfilename=%%A
devenv "%solutionfilename%"
