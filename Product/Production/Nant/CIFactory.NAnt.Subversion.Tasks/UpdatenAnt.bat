:: "$(ProjectDir)UpdatenAnt.bat" "$(SolutionDir)" "$(TargetDir)" $(ProjectName)
mkdir %1Production\Packages\Subversion\bin
copy %2%3.dll %1Production\Packages\Subversion\bin
copy %2%3.pdb %1Production\Packages\Subversion\bin

cd %1
cd ..\Third Party\Svn.Net
copy *.dll %1Production\Packages\Subversion\bin
copy *.pdb %1Production\Packages\Subversion\bin

cd %1
cd ..\Third Party\That
copy *.dll %1Production\Packages\Subversion\bin

cd %1
nant -buildfile:DevEnv\SetupScripts\SetUp.xml