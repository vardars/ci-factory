copy bin\Debug\NAnt.Contrib.Tasks.dll "C:\Projects\CI Factory\Current\Third Party\nAnt\bin"
copy bin\Debug\NAnt.Contrib.Tasks.pdb "C:\Projects\CI Factory\Current\Third Party\nAnt\bin"
nant -buildfile:"C:\Projects\CI Factory\Current\Product\DevEnv\SetupScripts\SetUp.xml"