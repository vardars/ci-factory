:: "$(ProjectDir)UpdatenAnt.bat" "$(SolutionDir)" "$(TargetDir)" "$(ProjectName)"
mkdir %1Production\Packages\CVS\bin
copy %2%3.dll %1Production\Packages\CVS\bin
copy %2%3.pdb %1Production\Packages\CVS\bin
copy %2ICSharpCode.SharpCvsLib.dll %1Production\Packages\CVS\bin
cd %1
nant -buildfile:DevEnv\SetupScripts\SetUp.xml