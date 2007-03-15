:: "$(ProjectDir)UpdatenAnt.bat" "$(SolutionDir)" "$(TargetDir)" "$(TargetName)"
mkdir %1Production\Packages\Tracker\Install\CCNet
copy %2%3.dll %1Production\Packages\Tracker\Install\CCNet
copy %2%3.pdb %1Production\Packages\Tracker\Install\CCNet
::Tracker.Common
copy %2Tracker.Common.dll %1Production\Packages\Tracker\Install\CCNet
copy %2Tracker.Common.pdb %1Production\Packages\Tracker\Install\CCNet