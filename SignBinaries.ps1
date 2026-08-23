$cert = Get-Item "Cert:\CurrentUser\My\F8A8D6D6A6954867F08F480210CA0A81F2FEF756" -ErrorAction SilentlyContinue

if (-not $cert) {
    Write-Host "Certificate STORM TEAM not found!"
    exit 1
}

# Recursively unblock all files in the project
Get-ChildItem -Path "E:\STORM FELLOWSHIP" -Recurse | Unblock-File -ErrorAction SilentlyContinue

# Sign executables and libraries
$filesToSign = @(
    "E:\STORM FELLOWSHIP\Assembling\StormFellowship.exe",
    "E:\STORM FELLOWSHIP\Assembling\StormFellowship.dll",
    "E:\STORM FELLOWSHIP\Files\STORM_FELLOWSHIP_0.2.2_setup.exe"
)

foreach ($f in $filesToSign) {
    if (Test-Path $f) {
        $res = Set-AuthenticodeSignature -FilePath $f -Certificate $cert -HashAlgorithm SHA256
        Write-Host "[SIGN] $f -> $($res.Status) ($($res.StatusMessage))"
    }
}
