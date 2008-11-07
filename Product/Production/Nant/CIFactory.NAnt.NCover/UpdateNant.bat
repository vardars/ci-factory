:: "$(ProjectDir)UpdatenAnt.bat" "$(SolutionDir)" "$(TargetDir)" $(TargetName)
mkdir %1Production\Packages\NCover\bin
copy %2%3.dll %1Production\Packages\NCover\bin
copy %2%3.pdb %1Production\Packages\NCover\bin
copy %2%3.xml %1Production\Packages\NCover\bin
cd %1
nant -buildfile:DevEnv\SetupScripts\SetUp.xml