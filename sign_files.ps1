[CmdletBinding()]
param (
    [Parameter(Mandatory=$true)]
    [String]
    $path,
    [Parameter(Mandatory=$true)]
    [String]
    $certPath,
    [Parameter(Mandatory=$true)]
    [String]
    $certPwd
)

$cert = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2($certPath, $certPwd)

$files = Get-ChildItem -Path $path | 
    Where-Object { $_.Extension -in '.dll', '.exe' } |
    Select-Object -ExpandProperty FullName | 
    Get-AuthenticodeSignature | 
    Where-Object { $_.Status -eq "NotSigned" } | 
    Select-Object -ExpandProperty Path

foreach($file in $files){
    Write-Host "Signing... $file"
    Set-AuthenticodeSignature $file -Certificate $cert -TimestampServer "http://timestamp.digicert.com"
}
