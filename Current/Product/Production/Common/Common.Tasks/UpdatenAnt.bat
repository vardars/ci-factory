"..\..\..\..\Third Party\nAnt\bin\nant.exe" -buildfile:UpdatenAnt.xml
copy "bin\.NET 1.1\Release\Common.Tasks.dll" "..\..\..\..\Third Party\nAnt\bin"
"..\..\..\..\Third Party\nAnt\bin\nant.exe" -buildfile:..\..\..\DevEnv\SetupScripts\SetUp.xml
exit