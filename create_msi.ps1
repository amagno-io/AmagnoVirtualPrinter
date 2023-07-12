Set-Location $PSScriptRoot

$arguments = "/MSBUILD:$PSScriptRoot\Installer\AmagnoVirtualPrinter.WixSharpInstaller", "/p:$PSScriptRoot"
Remove-Item -Path "$PSScriptRoot\Files\*" -Filter '*.pdb' -Force
Start-Process -FilePath "$PSScriptRoot\Files\AmagnoPrinterInstaller.exe" -ArgumentList $arguments -wait