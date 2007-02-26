:: "$(ProjectDir)UpdateFoundation.bat" "$(SolutionDir)" "$(TargetDir)"
cd %2
xcopy * %1Production\Foundation\Default\Build\server /S /Y
xcopy * %1Production\Foundation\Default\Build\dashboard\bin /S /Y
