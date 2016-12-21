import os, glob, sys, subprocess
import shutil # used to zip published folder


def main():
	if len(sys.argv) != 2:
		print("Base path to StarDump repository is needed as argument")
		sys.exit(1)
	
	global rootPath	
	rootPath = sys.argv[1]

	restoreStarDumpCoreTests()
	executeStarDumpCoreTests()
	restoreStarDump()
	publishStarDump()
	archiveStarDump()

	print("-- StarDump tests and publish succeeded!")
	sys.exit(0)

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
	dotnet_cmd = "dotnet publish {0}\\src\\StarDump -o {0}\\src\\StarDump\\publish\\StarDump".format(rootPath)
	print("-- Publish StarDump project: {0}".format(dotnet_cmd))
	exit_code = subprocess.call(dotnet_cmd, shell=True)
	if 0 != exit_code:
		print("-- Publish StarDump project exited with error code {0}!".format(exit_code))
		sys.exit(exit_code)

# Archive published directory (StarDump.zip)
def archiveStarDump():
	try:
		archive_name = shutil.make_archive("{0}\\StarDump".format(rootPath), "zip", "{0}\\src\\StarDump\\publish".format(rootPath), "StarDump", 1)
	except:
		print("Archive failed")
		sys.exit(1)
	
	print("StarDump has been published at {0}".format(archive_name))



if __name__ == "__main__":
    main()