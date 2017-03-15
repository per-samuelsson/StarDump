import os, glob, sys, subprocess
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

	print("-- StarDump tests and publish succeeded!")
	sys.exit(0)

# Getting path of the script.
def get_script_path():
    return os.path.dirname(__file__)
	
# Init function, sets global variables
def setGlobalVariables():
	global rootPath	
	global publishBasePath

	rootPath = Path(get_script_path()).parent
	publishBasePath = "\\src\\StarDump\\bin\\publish"

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
	dotnet_cmd = "dotnet test {0}\\test\\StarDump.Unit.Tests\\StarDump.Unit.Tests.csproj -teamcity".format(rootPath)
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
	dotnet_cmd = "dotnet publish {0}\\src\\StarDump -o {0}{1}\\StarDump".format(rootPath, publishBasePath)
	print("-- Publish StarDump project: {0}".format(dotnet_cmd))
	exit_code = subprocess.call(dotnet_cmd, shell=True)
	if 0 != exit_code:
		print("-- Publish StarDump project exited with error code {0}!".format(exit_code))
		sys.exit(exit_code)

if __name__ == "__main__":
    main()