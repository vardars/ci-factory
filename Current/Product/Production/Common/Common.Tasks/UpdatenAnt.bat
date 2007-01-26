attrib -R "C:\Projects\CI Factory\Current\Third Party\nAnt\bin\Common.Tasks.dll"

copy "bin\.NET 1.1\Debug\Common.Tasks.dll" "C:\Projects\CI Factory\Current\Third Party\nAnt\bin"
copy "bin\.NET 1.1\Debug\Common.Tasks.pdb" "C:\Projects\CI Factory\Current\Third Party\nAnt\bin"
nant -buildfile:"C:\Projects\CI Factory\Current\Product\DevEnv\SetupScripts\SetUp.xml"