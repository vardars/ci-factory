copy bin\Debug\TF.Tasks.dll "C:\Projects\CI Factory\Current\Third Party\nAnt\bin"
copy bin\Debug\TF.Tasks.pdb "C:\Projects\CI Factory\Current\Third Party\nAnt\bin"
nant -buildfile:"C:\Projects\CI Factory\Current\Product\DevEnv\SetupScripts\SetUp.xml"