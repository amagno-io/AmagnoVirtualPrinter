$args = '/MSBUILD:..\Installer\AmagnoVirtualPrinter.WixSharpInstaller', '/p:..\AmagnoVirtualPrinter'
Start-Process -FilePath '.\Files\AmagnoPrinterInstaller.exe' -ArgumentList $args