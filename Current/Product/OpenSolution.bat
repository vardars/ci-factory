cd ..\Third Party\nAnt
FOR /F "TOKENS=1 DELIMS=," %%A IN ('dir bin* /A:D /B') DO set AntBinPath=%%~fA
set PATH=%SystemRoot%\system32;%SystemRoot%;%SystemRoot%\System32\Wbem;%AntBinPath%
call "%ProgramFiles%\Microsoft Visual Studio 9.0\VC\vcvarsall.bat"
cd ..\..\Product
nant -buildfile:Scripts\OpenSolution.Script.xml
FOR /F "TOKENS=1 DELIMS=," %%A IN ('dir /b *.sln') DO SET solutionfilename=%%A
devenv "%solutionfilename%"
