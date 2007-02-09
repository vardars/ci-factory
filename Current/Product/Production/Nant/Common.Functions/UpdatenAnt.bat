copy bin\Debug\Common.Functions.dll "C:\Projects\CI Factory\Current\Third Party\nAnt\bin"
copy bin\Debug\Common.Functions.pdb "C:\Projects\CI Factory\Current\Third Party\nAnt\bin"
nant -buildfile:"C:\Projects\CI Factory\Current\Product\DevEnv\SetupScripts\SetUp.xml"