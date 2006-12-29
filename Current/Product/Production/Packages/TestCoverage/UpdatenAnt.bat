"..\..\..\..\Third Party\nAnt\bin\nant.exe" -buildfile:UpdatenAnt.xml
copy "bin\.NET 1.1\Release\TestCoverage.Tasks.dll" "Package\TestCoverage\bin"
copy "bin\.NET 1.1\Release\TestCoverage.dll" "Package\TestCoverage\bin"
"..\..\..\..\Third Party\nAnt\bin\nant.exe" -buildfile:..\..\..\DevEnv\SetupScripts\SetUp.xml
exit