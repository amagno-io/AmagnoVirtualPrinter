$args = '/MSBUILD:..\Installer\VirtualPrinter.WixSharpInstaller', '/p:..\VirtualPrinterDriver'
Start-Process -FilePath '.\Files\VPDInstaller.exe' -ArgumentList $args