Set-Location $PSScriptRoot

$args = "/MSBUILD:$PSScriptRoot\Installer\AmagnoVirtualPrinter.WixSharpInstaller", "/p:$PSScriptRoot"
Remove-Item -Path "$PSScriptRoot\Files\*" -Filter '*.pdb' -Force
Start-Process -FilePath "$PSScriptRoot\Files\AmagnoPrinterInstaller.exe" -ArgumentList $args -wait