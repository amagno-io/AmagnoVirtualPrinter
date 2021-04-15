$args = '/MSBUILD:..\Installer\AmagnoVirtualPrinter.WixSharpInstaller', '/p:..\AmagnoVirtualPrinter'
Remove-Item -Path '.\Files\*' -Filter '*.pdb' -Force
Start-Process -FilePath '.\Files\AmagnoPrinterInstaller.exe' -ArgumentList $args