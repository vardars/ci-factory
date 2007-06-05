set PATH=%SystemRoot%\system32;%SystemRoot%;%SystemRoot%\System32\Wbem;..\Build\nAnt\bin
call "%Compile.DevEnv.vsvars32bat%"
devenv "%ProjectName%.sln"
