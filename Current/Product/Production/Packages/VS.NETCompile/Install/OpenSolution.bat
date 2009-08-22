cd ..\%Common.Directory.Build.Name%\nAnt
FOR /F "TOKENS=1 DELIMS=," %%A IN ('dir bin* /A:D /B') DO set AntBinPath=%%~fA
set PATH=%SystemRoot%\system32;%SystemRoot%;%SystemRoot%\System32\Wbem;%AntBinPath%

set ENVBAT="%ProgramFiles%\Microsoft Visual Studio 8\VC\vcvarsall.bat"
IF DEFINED ProgramFiles(x86) set ENVBAT="%ProgramFiles(x86)%\Microsoft Visual Studio 8\VC\vcvarsall.bat"
call %ENVBAT%

cd ..\..\%Common.Directory.Product.Name%
nant -buildfile:OpenSolution.Script.xml
FOR /F "TOKENS=1 DELIMS=," %%A IN ('dir /b *.sln') DO SET solutionfilename=%%A
devenv "%solutionfilename%"
