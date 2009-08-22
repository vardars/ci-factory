FOR /F "delims=" %%v IN ('cd') DO set Common.Directory.Build.Path=%%v
set PATH=%SystemRoot%\system32;%SystemRoot%;%SystemRoot%\System32\Wbem;%ProgramFiles%\Subversion\bin;%Common.Directory.Build.Path%\nAnt\bin
call "%ProgramFiles%\Microsoft Visual Studio 9.0\VC\vcvarsall.bat"
devenv "CI FactoryBuildScripts.sln"
