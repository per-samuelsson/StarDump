# StarDump

## Usage

**Unload database**

```
>StarDump.exe unload --database [DatabaseName] --file [FilePath]
```

Example

```
>StarDump.exe unload --database default --file C:\Temp\default.sqlite3
```

**Reload database**

```
>StarDump.exe reload --database [DatabaseName] --file [FilePath]
```

Example

```
>StarDump.exe reload --database default --file C:\Temp\default.sqlite3
```

**Note:** the database should be dropped and created prior to reload.

```
>staradmin -d=default delete --force db
>staradmin -d=default new db DefaultUserHttpPort=8080
```

## Requires Starcounter.Core

The [Starcounter.Core](https://github.com/Starcounter/Starcounter.Core/) solution is required to run this project.
Clone it into the same parent directory to get it work.

```
|Root
|-Starcounter.Core
|-StarDump  
```

## Supported systems

- `win10-x64`
- `osx.10.10-x64`
- `ubuntu.14.04-x64`

[.NET Core Runtime IDentifier (RID) catalog](https://docs.microsoft.com/en-us/dotnet/articles/core/rid-catalog).