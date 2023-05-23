# Auto Download Mover
.NET 4.8 Windows process to monitor a folder for a filename. When a new file matching the pattern is detected, it is moved and optionally renamed. The file can automatically be executed if it's an exe.

## Config
**The program should be restarted for changes to take effect.**  
`RetryLimit`: How many times to retry the operation upon failure.  
`RetryDelayMS`: How long to wait between retries, or check if the file has finished downloading.  
`TargetFile`: The filename to look for, including the extension. Accepts a regular expression.  
`DestinationDirectory`: The directory where the file will be moved. A file with the same name in this directory will be overwritten.  
`Rename`: New name for the file. Accepts regular expressions enclosed by `{}` which will match against the original filename. For example, `ab12.txt` with `rename = cd{\d*}.txt` should rename to `cd12.txt`.  
`StartExe`: Whether to run a `.exe` file upon moving it.  
`DeleteOld`: Whether to delete files matching `Rename` in `DestinationDirectory`.  
`MonitorDirectory`: The directory to monitor. When not specified or the directory doesn't exist, the `Downloads` folder is used.

## Installation
When running the executable for the first time, the program will be set to start with Windows. If the program is later moved, the executable should be run again to update the startup location.  
To uninstall, run the executable with the `uninstall` argument. This will remove it from startup and stop any instances.
