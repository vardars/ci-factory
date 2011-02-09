:: "$(ProjectDir)UpdatenAnt.bat" "$(SolutionDir)" "$(TargetDir)" "$(ProjectName)"
mkdir %1Production\Packages\RTC\bin
copy %2%3.dll %1Production\Packages\RTC\bin
copy %2%3.pdb %1Production\Packages\RTC\bin
copy %2%3.dll \\tsclient\C\Projects\Chapter33\Trunk\Build\Packages\RTC\bin
copy %2%3.pdb \\tsclient\C\Projects\Chapter33\Trunk\Build\Packages\RTC\bin
copy C:\Projects\CI Factory\Current\Product\nant.xsd \\tsclient\C\Projects\Chapter33\Trunk\Build
copy %2RTC.Common.dll %1Production\Packages\RTC\bin
copy %2RTC.Common.pdb %1Production\Packages\RTC\bin
cd %1
nant -buildfile:DevEnv\SetupScripts\SetUp.xml