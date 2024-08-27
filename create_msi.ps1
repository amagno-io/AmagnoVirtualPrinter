Set-Location $PSScriptRoot

# https://github.com/actions/runner-images/issues/9667
choco uninstall wixtoolset -y
choco install wixtoolset --version=3.11.2 --force -y

$arguments = "/MSBUILD:$PSScriptRoot\Installer\AmagnoVirtualPrinter.WixSharpInstaller", "/p:$PSScriptRoot"
Remove-Item -Path "$PSScriptRoot\Files\*" -Filter '*.pdb' -Force
Start-Process -FilePath "$PSScriptRoot\Files\AmagnoPrinterInstaller.exe" -ArgumentList $arguments -wait