:: "$(ProjectDir)UpdatenAnt.bat" "$(SolutionDir)" "$(TargetDir)" "$(ProjectName)"
mkdir %1Production\Packages\CVS\bin
copy %2%3.dll %1Production\Packages\CVS\bin
copy %2%3.pdb %1Production\Packages\CVS\bin
copy %2ICSharpCode.SharpCvsLib.dll %1Production\Packages\CVS\bin
copy %2ICSharpCode.SharpZipLib.dll %1Production\Packages\CVS\bin

copy "..\..\..\..\..\..\Third Party\ICSharpCode\ICSharpCode.SharpCvsLib.Console.dll" %1Production\Packages\CVS\bin
copy "..\..\..\..\..\..\Third Party\ICSharpCode\log4net.dll" %1Production\Packages\CVS\bin
copy "..\..\..\..\..\..\Third Party\ICSharpCode\scvs.exe" %1Production\Packages\CVS\bin

cd %1
nant -buildfile:DevEnv\SetupScripts\SetUp.xml