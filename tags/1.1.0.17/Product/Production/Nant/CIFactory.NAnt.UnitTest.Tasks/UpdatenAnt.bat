:: "$(ProjectDir)UpdatenAnt.bat" "$(SolutionDir)" "$(TargetDir)" $(ProjectName)
cd %1
mkdir "Integration Tests\bin"
cd "Integration Tests\bin"
FOR /F "TOKENS=1 DELIMS=," %%A IN ('cd') DO SET Dest=%%A

cd %2
copy %3.dll "%Dest%"
copy %3.pdb "%Dest%"

cd "C:\Projects\CI Factory\Current\Third Party\MbUnit"
xcopy *.* "%Dest%" /S /Y

cd %1
mkdir "NAnt Tests\bin"
cd "NAnt Tests\bin"
FOR /F "TOKENS=1 DELIMS=," %%A IN ('cd') DO SET Dest=%%A

cd %2
copy %3.dll "%Dest%"
copy %3.pdb "%Dest%"

cd "C:\Projects\CI Factory\Current\Third Party\MbUnit"
xcopy *.* "%Dest%" /S /Y

cd %1
nant -buildfile:DevEnv\SetupScripts\SetUp.xml