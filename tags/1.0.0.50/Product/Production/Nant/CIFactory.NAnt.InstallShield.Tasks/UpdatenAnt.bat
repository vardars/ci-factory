:: "$(ProjectDir)UpdatenAnt.bat" "$(SolutionDir)" "$(TargetDir)" "$(ProjectName)"
mkdir %1Production\Packages\InstallShield\bin
copy %2%3.dll %1Production\Packages\InstallShield\bin
copy %2%3.pdb %1Production\Packages\InstallShield\bin
cd %1
nant -buildfile:DevEnv\SetupScripts\SetUp.xml