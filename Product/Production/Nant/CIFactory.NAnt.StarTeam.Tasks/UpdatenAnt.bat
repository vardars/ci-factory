:: "$(ProjectDir)UpdatenAnt.bat" "$(SolutionDir)" "$(TargetDir)" $(ProjectName)
mkdir %1Production\Packages\StarTeam\bin
copy %2%3.dll %1Production\Packages\StarTeam\bin
copy %2%3.pdb %1Production\Packages\StarTeam\bin

cd %1
cd ..\Third Party\StarTeam
copy Interop.StarTeam.dll %1Production\Packages\StarTeam\bin

cd %1
nant -buildfile:DevEnv\SetupScripts\SetUp.xml