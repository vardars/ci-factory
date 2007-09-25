set PATH=%SystemRoot%\system32;%SystemRoot%;%SystemRoot%\System32\Wbem;.\nAnt\bin
call "%ProgramFiles%\Microsoft Visual Studio 8\VC\vcvarsall.bat"
devenv "CI FactoryBuild.sln"
