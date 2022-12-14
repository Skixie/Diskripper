# Diskripper
## Analyze and extract files from an image file

![N|Solid](https://github.com/Skixie/Diskripper/blob/main/diskripper.png)

[![Build Status](https://travis-ci.org/joemccann/dillinger.svg?branch=master)](https://travis-ci.org/joemccann/dillinger)

Diskripper is a tool which can analyze and extract images from an image file (tool supports currently only vmdk). What makes this tool special, is the fact that it can do this without needing to download the whole image file. This also means that Diskripper accepts network paths as to where the image file is located. During pentests for example, you could find an image file on a share that could be as big as a couple hunderd gigabytes. The problem with this is that downloading the image file takes time and storage, both of which that can be quite scarce. This tool has been made during an internship to solve that problem.

## Features

- Output a directory listing of the image file
- Output the contents of Program Files and Program Files (x86) directories with just one command
- Copy a file from the image file
- Copy file(s) from a provided txt file with the full paths of the desired files to be copied
- Copy the SAM, SYSTEM and SECURITY hive files in one command

## Usage
Diskripper can be used with te following commands

- --command dir, which can make a directory listing of the provided image file.
Example:
    ```sh
    diskripper --command dir --source "C:\VM\Win10\Win10.vmdk" --directory
    ```
- --command pf, which can show the contents of the directories Program Files and Program FIles (x86) in just one command.
Example:
    ```sh
    diskripper.exe --command pf --source "C:\VM\Win10\Win10.vmdk"
    ```
- --command cp, which can copy a specified file from the provided image. A destination has to be provided.
Example:
    ```sh
    diskripper.exe --command cp --source "C:\VM\Win10\Win10.vmdk" --file2copy \Windows\System32\calc.exe --destination "C:\Users\Publicalc.exe"
    ```
- --command cpfile, which can copy files that are specified in a provided txt file.
Example:
    ```sh
    diskripper.exe --command cpfile --source "C:\VM\Win10\Win10.vmdk" --file "C:\Users\Public\\filelist.txt" --destinationdir "C:\Users\Public\Output"
    ```
- --command sam, which will copy the SAM, SYSTEM and SECURITY files, that can be used to extract password hashes to crack them.
Example:
    ```sh
    diskripper.exe --command sam --source "C:\VM\Win10\Win10.vmdk"
    ```
###### Credits
The base of this tool has been written by [leftp](https://github.com/leftp). The original tool can be found [here](https://github.com/leftp/VmdkReader). When I tried to use the original tool it initially didn't work, so I fixed it and added the previously mentioned functions.

