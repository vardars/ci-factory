:: "$(ProjectDir)UpdatenAnt.bat" "$(SolutionDir)" "$(TargetDir)" "$(TargetName)"
mkdir %1Production\Packages\VSTSVersionControl\Install\CCNet
copy %2%3.dll %1Production\Packages\VSTSVersionControl\Install\CCNet
copy %2%3.pdb %1Production\Packages\VSTSVersionControl\Install\CCNet