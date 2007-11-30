set PATH=%SystemRoot%\system32;%SystemRoot%;%SystemRoot%\System32\Wbem;%ProgramFiles%\Subversion\bin;C:\Projects\CI Factory\Current\Third Party\nAnt\bin
call "%ProgramFiles%\Microsoft Visual Studio 8\VC\vcvarsall.bat"
cd "c:\Projects\CI Factory\Current\Build\server"
"c:\Projects\CI Factory\Current\Build\server\CCNET.exe" -config:"c:\Projects\CI Factory\Current\Build\ccnetproject.xml" -remoting:on
