# Auto Download Mover

Windows Service to monitor the Downloads folder for a filename. When a new file matching the pattern is detected, it is moved to another location. The file can optionally be renamed and/or executed if it's an exe. If a file with the same name or matching the rename pattern exists in the destination, it will be deleted.

## Config
`ServiceName`: The name of the Windows Service when installed.  
`ServiceDescription`: The description of the Windows Service when installed.  
`RetryLimit`: How many times to retry the operation upon failure.  
`RetryDelayMS`: How long to wait between retries, or check if the file has finished downloading.  
`TargetFile`: The file to look for in the Downloads folder. Accepts a regular expression.  
`DestinationDirectory`: The directory where the file will be moved. A file with the same name in this directory will be deleted.  
`Rename`: New name for the file. Accepts regular expressions enclosed by `{}` which will match against the original filename. For example, `ab12.txt` with `rename = cd{\d*}.txt` should rename to `cd12.txt`. Files matching the pattern in `DestinationDirectory` will be deleted.  
`StartExe`: Whether to start the file upon moving it. Only works if the file is a `.exe`.

## Installation
Move the application folder to a directory you like, such as `C:\Program Files (x86)`, and run `install.bat` as administrator. The service name and description can be specified in the config. The service will be started automatically.  
To uninstall, run `uninstall.bat` as administrator. If you changed the service name in the config since installation, the script will not work. Make sure the installed service's name matches that in the config. If it fails, you can try running it through command line and/or stopping the service manually first.
