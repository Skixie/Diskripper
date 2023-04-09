# Diskripper
## Analyze and extract files from a disk file

![N|Solid](https://github.com/Skixie/Diskripper/blob/main/diskripper.png)

![Build Status](https://travis-ci.org/joemccann/dillinger.svg?branch=master)

Diskripper is a tool which can analyze and extract files from a disk file (currently supports only vmdk). What makes this tool special, is the fact that it can do this without needing to download the whole disk file. This also means that Diskripper accepts network paths as to where the disk file is located. During pentests for example, you could find a disk file on a share that could be as big as a couple hunderd gigabytes.

The challenge with this is that downloading the disk file takes time and storage, both of which that can be quite limited when performing a pentest. This tool has been made during an internship to solve that problem.

## Features

- Check whether the given vmdk file is encrypted with Bitlocker or not
- Output a directory listing of the disk file
- Output the contents of Program Files and Program Files (x86) directories with just one command
- Copy a file from the disk file
- Copy file(s) from a provided txt file with the full paths of the desired files to be copied
- Copy the SAM, SYSTEM and SECURITY files in one command

## Usage
Diskripper can be used with the following commands

- ``--command dir``, which can make a directory listing of the provided disk file.<br>
Example:
    ```sh
    diskripper --command dir --source "C:\VM\Win10\Win10.vmdk" --directory
    ```
- ``--command pf``, which can show the contents of the directories Program Files and Program FIles (x86) with one command.<br>
Example:
    ```sh
    diskripper.exe --command pf --source "C:\VM\Win10\Win10.vmdk"
    ```
- ``--command cp``, which can copy a specified file from the provided disk file. A destination has to be provided.<br>
Example:
    ```sh
    diskripper.exe --command cp --source "C:\VM\Win10\Win10.vmdk" --file2copy \Windows\System32\calc.exe --destination "C:\Users\Public\calc.exe"
    ```
- ``--command cpfile``, which can copy files that are specified in a provided txt file.<br>
Example:
    ```sh
    diskripper.exe --command cpfile --source "C:\VM\Win10\Win10.vmdk" --file "C:\Users\Public\\filelist.txt" --destinationdir "C:\Users\Public\Output"
    ```
- ``--command sam``, which will copy the SAM, SYSTEM and SECURITY files, that can be used to extract password hashes to crack them.<br>
Example:
    ```sh
    diskripper.exe --command sam --source "C:\VM\Win10\Win10.vmdk"
    ```
###### Credits
The base of this tool has been written by [leftp](https://github.com/leftp). The original tool can be found [here](https://github.com/leftp/VmdkReader). When I tried to use the original tool it initially didn't work, so I fixed it and added the previously mentioned functions.

NOTE:
Diskripper has been tested with a Windows 10 vmdk file that has been created in VMware. Other disk file formats are currently not yet supported. Use at your own risk!

Code is pretty messy, because my programmingskills arent't the best, but I tried to make it as 'readable' as possible and, most importantly, it works
¯\\__(ツ)__/¯

