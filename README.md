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

**List of available parameters**

| Name                           | Type       | Default value                               | Description                                   | Notes                                               |
|--------------------------------|------------|---------------------------------------------|-----------------------------------------------|-----------------------------------------------------|
| `-db`, `--database`            | `string`   | `default`                                   | Database name to unload                       |                                                     |
| `-f`, `--file`                 | `string`   | `%TEMP%\stardump-<database>-<date>.sqlite3` | Output file path with name                    |                                                     |
| `-b`, `--buffersize`           | `int`      | `500`                                       | Number of rows in a single `INSERT` operation |                                                     |
| `-scp`, `--skipcolumnprefixes` | `string[]` | `__`                                        | Column prefixes to skip                       |                                                     |
| `-stp`, `--skiptableprefixes`  | `string[]` |                                             | Table prefixes to skip                        | `cs`                                                |
| `-st`, `--skiptables`          | `string[]` |                                             | Table names to skip                           | `cs`                                                |
| `-ut`, `--unloadtables`        | `string[]` |                                             | Table names to unlaod                         | `cs`, disables `-stp` and `-st` parameters when set |

**Notes**

- `string[]` values should be space and/or comma separated
- `cs` - case sensitive value

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