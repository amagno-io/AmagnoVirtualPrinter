# VirtualPrinterDriver

## Description

The _VirtualPrinterDriver_ is an interface which forwards any print job given from a specified printer, to a specified application that processes the 'print job'.

There are two ways of using the _VirtualPrinterDriver_:

1. If you just want to install the driver build the project in release mode, navigate to the folder _VirtualPrinter.WixSharpinstaller_ and execute the created .msi file. After the installation, a new printer, with the name set in _Defaults.cs_, can be located under _Printer & Scanner_ in the Microsoft® Windows settings.
2. To debug the _VirtualPrinterDriver_, follow step one. After the installation, go to _Windows Services_ and stop _VirtualPrinterService_. In Visual Studio, select VirtualPrinter.Agent.Console as startup project and run in debug mode. To start a test print (and debug the solution) start PowerShell or cmd and navigate to the root folder of the repository. Go to `Files` and run `.\setupdrv.exe test`, which will create a test page and send it to the virtual printer. Or just print any document you want to.

## Table of Contents

1. [Installation](#installation)
    1. [From MSI](#from-msi)
    2. [From Source](#from-source)
2. [Usage](#usage)
    1. [Configure](#configure)
    2. [Debugging](#debugging)
3. [License](#license)

## Installation

### From MSI

If you want to use the official installer, you can download it [here](https://link). Make sure to run the installer with extended rights. After installation, you may need to [configure](#configure) the _VirutalPrinterDriver_.
After installation, a new printer with the name set in _Defaults.cs_ can be located under _Printer & Scanner_ in the Microsoft® Windows settings.

### From Source

#### **_Dependencies_**

To compile the installer, please make sure you Wix Toolset is installed. The [WixSharp library](https://github.com/oleg-shilo/wixsharp) is used to define the package in `VirtualPrinter.WixSharpInstaller`.

#### **_Create MSI package_**

To install the driver from source, build the project in release mode. When the build is run successfully, open up powershell and navigate to `C:\[Git]\VirtualPrinterDriver\Files`. Then run the `VPDInstaller.exe "/MSBUILD:C:\[Git]\VirtualPrinterDriver\Installer\VirtualPrinter.WixSharpInstaller" "/p:C:\[Git]\VirutalPrinterDriver\"` where `/p:` is the output directory for the msi and working directory for the WixSharp project. Make sure the given working directory contains a `Files` folder with all binaries needed for _VirtualPrinterDriver_. This command will use WixSharp to create the msi package right next to the VPDInstaller.exe called **VPDInstaller.msi**.

Alternatively use `create_msi.ps1` which automates the building step above.

## Usage

### Configure

Make sure Ghostscript is installed on your machine.

While installing, several registry entries are inserted into `Computer\HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\vpd\`. The most important ones are those under the key `Application`
- **Pre-Converter:** Contains the exe to be executed (pre convert process) and the `arg` to be processed by the application (for example: `C:\Program Files\MyApp.exe PRINT`). 
- **Post-Converter:** Contains the exe to be executed (post convert process) and the args to be processed by the application (for example: `C:\Program Files\MyApp.exe PRINTCOMPLETE`).

The `Converter` key defines the settings which are needed for the convert process, e.g. server port (9101 by default) and  output directory for the converted prints, which can be processed by the application. If the value is set to an empty string, the default temp path will be used (e.g. `C:\temp\PrinterOuput`). 
### Debugging

To debug the _VirtualPrinterDriver_, run the msi. After installation, go to _Windows Services_ and stop the _VirtualPrinterService_. In your IDE, select the VirtualPrinter.Agent.Console as startup project and run in debug mode. To start a test print (and debug the solution) start  PowerShell or cmd and navigate to the root folder of the repository. Go to `Files` and run `.\setupdrv.exe test`, which will create a test page and send it to the virtual printer. Or just print any document you want to.

## License