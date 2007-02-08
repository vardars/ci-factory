attrib -R "C:\Projects\CI Factory\Current\Third Party\nAnt\bin\Common.Tasks.dll"

copy "bin\Common.Tasks.dll" "C:\Projects\CI Factory\Current\Third Party\nAnt\bin"
copy "bin\Common.Tasks.pdb" "C:\Projects\CI Factory\Current\Third Party\nAnt\bin"
nant -buildfile:"C:\Projects\CI Factory\Current\Product\DevEnv\SetupScripts\SetUp.xml"