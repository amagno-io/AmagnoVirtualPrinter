[CmdletBinding()]
param (
    [Parameter(Mandatory=$true)]
    [String]
    $certPath,
    [Parameter(Mandatory=$true)]
    [String]
    $certPwd
)

$cert = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2($certPath, $certPwd)

$file = "AmagnoPrinterInstaller.msi"

Write-Host "Signing... $file"
Set-AuthenticodeSignature $file -Certificate $cert -TimestampServer "http://timestamp.digicert.com"
