copy bin\Debug\NAnt.Core.dll "C:\Projects\CI Factory\Current\Third Party\nAnt\bin"
copy bin\Debug\NAnt.Core.pdb "C:\Projects\CI Factory\Current\Third Party\nAnt\bin"
nant -buildfile:"C:\Projects\CI Factory\Current\Product\DevEnv\SetupScripts\SetUp.xml"