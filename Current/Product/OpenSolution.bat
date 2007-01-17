set PATH=%SystemRoot%\system32;%SystemRoot%;%SystemRoot%\System32\Wbem;C:\Program Files\PVCS\Tracker\nt;C:\Program Files\Subversion\bin;C:\Projects\CI Factory\Current\Third Party\nAnt\bin
call "C:\Program Files\Microsoft Visual Studio 8\VC\vcvarsall.bat"
devenv "CI Factory.sln"
