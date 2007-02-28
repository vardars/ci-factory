set PATH=%SystemRoot%\system32;%SystemRoot%;%SystemRoot%\System32\Wbem;C:\Program Files\Subversion\bin;%BuildDirectory%\nAnt\bin

cd "%CCNET.ServerDirectory%"
"%CCNET.ServerExe%" -config:"%CCNET.ProjectFile%" -remoting:on