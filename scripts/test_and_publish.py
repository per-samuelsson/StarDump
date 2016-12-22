import os, glob, sys, subprocess, shutil

#################################################################################
#
# The script assumes that the callers path is the root of StarDump repository
#
# Optional argument:
#	Name of archive zip file, Default is "StarDump"
# 
#################################################################################

def main():
	setGlobalVariables()
	restoreStarDumpCoreTests()
	executeStarDumpCoreTests()
	restoreStarDump()
	publishStarDump()
	archiveStarDump()

	print("-- StarDump tests and publish succeeded!")
	sys.exit(0)

# Init function, sets global variables
def setGlobalVariables():
	global rootPath	
	global publishBasePath
	global fileName

	rootPath = os.getcwd()
	publishBasePath = "\\src\\StarDump\\bin\\publish"
	if len(sys.argv) < 2:
		fileName = "StarDump"
	else:
		fileName = sys.argv[1]

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
	dotnet_cmd = "dotnet publish {0}\\src\\StarDump -o {0}\\{1}\\StarDump".format(rootPath, publishBasePath)
	print("-- Publish StarDump project: {0}".format(dotnet_cmd))
	exit_code = subprocess.call(dotnet_cmd, shell=True)
	if 0 != exit_code:
		print("-- Publish StarDump project exited with error code {0}!".format(exit_code))
		sys.exit(exit_code)

# Archive published directory
def archiveStarDump():
	try:
		archive_name = shutil.make_archive("{0}\\scripts\\publish\\{1}".format(rootPath, fileName), "zip", "{0}\\{1}".format(rootPath, publishBasePath), "StarDump")
		#TODO: Call TeamCity-Whitelist-Tool with archive_name as argument when it is done
	except:
		print("Archive failed")
		sys.exit(1)
	
	print("-- StarDump has been published at {0}".format(archive_name))


if __name__ == "__main__":
    main()