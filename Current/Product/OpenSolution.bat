set PATH=%SystemRoot%\system32;%SystemRoot%;%SystemRoot%\System32\Wbem;C:\Program Files\PVCS\Tracker\nt;C:\Program Files\Subversion\bin;..\Build\nAnt\bin
call "C:\Program Files\Microsoft Visual Studio 8\VC\vcvarsall.bat"
devenv "CI Factory.sln"
