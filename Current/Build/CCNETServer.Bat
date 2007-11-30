set PATH=%SystemRoot%\system32;%SystemRoot%;%SystemRoot%\System32\Wbem

cd server
CCNET.exe -config:"..\ccnetproject.xml" -remoting:on