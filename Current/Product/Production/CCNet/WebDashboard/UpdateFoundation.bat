:: "$(ProjectDir)UpdateFoundation.bat" "$(SolutionDir)" "$(TargetDir)"
cd %2
xcopy * %1Production\Foundation\Default\Build\dashboard /S /Y
mkdir %1Production\Foundation\Default\Build\dashboard\bin
move /Y %1Production\Foundation\Default\Build\dashboard\*.dll %1Production\Foundation\Default\Build\dashboard\bin
move /Y %1Production\Foundation\Default\Build\dashboard\*.pdb %1Production\Foundation\Default\Build\dashboard\bin
move /Y %1Production\Foundation\Default\Build\dashboard\de %1Production\Foundation\Default\Build\dashboard\bin
move /Y %1Production\Foundation\Default\Build\dashboard\fr %1Production\Foundation\Default\Build\dashboard\bin
move /Y %1Production\Foundation\Default\Build\dashboard\ja %1Production\Foundation\Default\Build\dashboard\bin
