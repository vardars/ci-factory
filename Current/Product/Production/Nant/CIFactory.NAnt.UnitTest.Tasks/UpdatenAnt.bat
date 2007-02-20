copy bin\Debug\CIFactory.NAnt.UnitTest.Tasks.dll "C:\Projects\CI Factory\Current\Third Party\nAnt\bin"
copy bin\Debug\CIFactory.NAnt.UnitTest.Tasks.pdb "C:\Projects\CI Factory\Current\Third Party\nAnt\bin"
nant -buildfile:"C:\Projects\CI Factory\Current\Product\DevEnv\SetupScripts\SetUp.xml"