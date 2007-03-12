:: "$(ProjectDir)UpdatenAnt.bat" "$(SolutionDir)" "$(TargetDir)" "$(ProjectName)"
mkdir %1Production\Packages\Tracker\bin
copy %2%3.dll %1Production\Packages\Tracker\bin
copy %2%3.pdb %1Production\Packages\Tracker\bin
cd %1
nant -buildfile:DevEnv\SetupScripts\SetUp.xml