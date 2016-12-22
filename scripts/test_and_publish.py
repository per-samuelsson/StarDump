import os, glob, sys, subprocess
from pathlib import Path

#################################################################################
#
# The script assumes that the callers path is at the same place as this file
# 
#################################################################################

def main():
	setGlobalVariables()
	restoreStarcounterCore()
	restoreStarDumpCoreTests()
	executeStarDumpCoreTests()
	restoreStarDump()
	publishStarDump()

	print("-- StarDump tests and publish succeeded!")
	sys.exit(0)

# Init function, sets global variables
def setGlobalVariables():
	global rootPath	
	global publishBasePath

	rootPath = Path(os.getcwd()).parent
	publishBasePath = "\\src\\StarDump\\bin\\publish"

# Restore Starcounter.Core
def restoreStarcounterCore():
	dotnet_cmd = "dotnet restore {0}".format(Path(rootPath).parent)
	print("-- Restore Starcounter.Core projects: {0}".format(dotnet_cmd))
	exit_code = subprocess.call(dotnet_cmd, shell=True)
	if 0 != exit_code:
		print("-- Restore Starcounter.Core projects exited with error code {0}!".format(exit_code))
		sys.exit(exit_code)
	
# Restore StarDump.Core.Tests
def restoreStarDumpCoreTests():
	dotnet_cmd = "dotnet restore {0}\\test\\StarDump.Core.Tests".format(rootPath)
	print("-- Restore StarDump.Core.Tests project: {0}".format(dotnet_cmd))
	exit_code = subprocess.call(dotnet_cmd, shell=True)
	if 0 != exit_code:
		print("-- Restore StarDump.Core.Tests project exited with error code {0}!".format(exit_code))
		sys.exit(exit_code)

# Excute tests in StarDump.Core.Tests
def executeStarDumpCoreTests():
	dotnet_cmd = "dotnet test {0}\\test\\StarDump.Core.Tests".format(rootPath)
	print("-- Excute tests in StarDump.Core.Tests project: {0}".format(dotnet_cmd))
	exit_code = subprocess.call(dotnet_cmd, shell=True)
	if 0 != exit_code:
		print("-- Tests in StarDump.Core.Tests project exited with error code {0}!".format(exit_code))
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
	dotnet_cmd = "dotnet publish {0}\\src\\StarDump -o {0}{1}\\StarDump".format(rootPath, publishBasePath)
	print("-- Publish StarDump project: {0}".format(dotnet_cmd))
	exit_code = subprocess.call(dotnet_cmd, shell=True)
	if 0 != exit_code:
		print("-- Publish StarDump project exited with error code {0}!".format(exit_code))
		sys.exit(exit_code)

if __name__ == "__main__":
    main()