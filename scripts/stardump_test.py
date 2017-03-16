import os, glob, sys, subprocess, shutil
from pathlib import Path

##################################################
#
# Script which tests and pusblishes StarDump
# 
##################################################

def main():
	setGlobalVariables()
	restoreStarDumpBase()
	restoreStarDumpCoreTests()
	executeStarDumpCoreTests()
	restoreStarDump()
	publishStarDump()
	copyStardumpToScBin()

	print("-- StarDump tests and publish succeeded!")
	sys.exit(0)

# Getting path of the script.
def get_script_path():
    return os.path.abspath(__file__)
	
# Init function, sets global variables
def setGlobalVariables():
	global rootPath	
	global publishPath

	rootPath = Path(get_script_path()).parent.parent
	publishPath = os.path.join("{0}".format(rootPath), "src/StarDump/bin/publish", "StarDump")

# Restore StarDump Base repository
def restoreStarDumpBase():
	dotnet_cmd = "dotnet restore {0}".format(rootPath)
	print("-- Restore StarDump projects: {0}".format(dotnet_cmd))
	exit_code = subprocess.call(dotnet_cmd, shell=True)
	if 0 != exit_code:
		print("-- Restore StarDump projects exited with error code {0}!".format(exit_code))
		sys.exit(exit_code)
		
# Restore StarDump.Unit.Tests
def restoreStarDumpCoreTests():
	dotnet_cmd = "dotnet restore {0}\\test\\StarDump.Unit.Tests".format(rootPath)
	print("-- Restore StarDump.Unit.Tests project: {0}".format(dotnet_cmd))
	exit_code = subprocess.call(dotnet_cmd, shell=True)
	if 0 != exit_code:
		print("-- Restore StarDump.Unit.Tests project exited with error code {0}!".format(exit_code))
		sys.exit(exit_code)

# Execute tests in StarDump.Unit.Tests
def executeStarDumpCoreTests():
	dotnet_cmd = "dotnet test {0}\\test\\StarDump.Unit.Tests\\StarDump.Unit.Tests.csproj".format(rootPath)
	print("-- Execute tests in StarDump.Unit.Tests project: {0}".format(dotnet_cmd))
	exit_code = subprocess.call(dotnet_cmd, shell=True)
	if 0 != exit_code:
		print("-- Tests in StarDump.Unit.Tests project exited with error code {0}!".format(exit_code))
		sys.exit(exit_code)

# Restore StarDump
def restoreStarDump():
	dotnet_cmd = "dotnet restore {0}\\src\\StarDump".format(rootPath)
	print("-- Restore StarDump project: {0}".format(dotnet_cmd))
	exit_code = subprocess.call(dotnet_cmd, shell=True)
	if 0 != exit_code:
		print("-- Restore StarDump project exited with error code {0}!".format(exit_code))
		sys.exit(exit_code)

# Publish StarDump
def publishStarDump():
	dotnet_cmd = "dotnet publish {0}\\src\\StarDump -o {1} -c release -r win10-x64".format(rootPath, publishPath)
	print("-- Publish StarDump project: {0}".format(dotnet_cmd))
	exit_code = subprocess.call(dotnet_cmd, shell=True)
	if 0 != exit_code:
		print("-- Publish StarDump project exited with error code {0}!".format(exit_code))
		sys.exit(exit_code)

# Copying published binaries to StarcounterBin/StarDump
def copyStardumpToScBin():
	scBin = os.environ["StarcounterBin"]
	dstDir = os.path.join(scBin, "StarDump")
	if os.path.exists(dstDir):
		print("-- Deleting existing StarDump dir: {0}".format(dstDir))
		shutil.rmtree(dstDir)
	print("-- Copying stardump dir from \"{0}\" to \"{1}\"".format(publishPath, dstDir))
	shutil.copytree(publishPath, dstDir)

if __name__ == "__main__":
    main()