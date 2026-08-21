$certSubject = "CN=STORM FELLOWSHIP Security Trust, O=STORM FELLOWSHIP, C=RU"
$cert = Get-ChildItem Cert:\CurrentUser\My -CodeSigningCert | Where-Object { $_.Subject -like "*STORM FELLOWSHIP*" } | Select-Object -First 1

if (-not $cert) {
    Write-Host "[CERT] Создание доверенного сертификата подписи кода..."
    $cert = New-SelfSignedCertificate -Type CodeSigningCert -Subject $certSubject -CertStoreLocation "Cert:\CurrentUser\My" -NotAfter (Get-Date).AddYears(10) -HashAlgorithm "SHA256"
}

# Add to CurrentUser Trusted Root & Trusted Publisher stores
$rootStore = New-Object System.Security.Cryptography.X509Certificates.X509Store("Root", "CurrentUser")
$rootStore.Open([System.Security.Cryptography.X509Certificates.OpenFlags]::ReadWrite)
$rootStore.Add($cert)
$rootStore.Close()

$pubStore = New-Object System.Security.Cryptography.X509Certificates.X509Store("TrustedPublisher", "CurrentUser")
$pubStore.Open([System.Security.Cryptography.X509Certificates.OpenFlags]::ReadWrite)
$pubStore.Add($cert)
$pubStore.Close()

Write-Host "[CERT] Сертификат успешно зарегистрирован в Trusted Root & Trusted Publisher."

# Recursively unblock all files in the project
Get-ChildItem -Path "E:\STORM FELLOWSHIP" -Recurse | Unblock-File -ErrorAction SilentlyContinue

# Sign executables and libraries
$filesToSign = @(
    "E:\STORM FELLOWSHIP\Assembling\StormFellowship.exe",
    "E:\STORM FELLOWSHIP\Assembling\StormFellowship.dll",
    "E:\STORM FELLOWSHIP\Files\STORM_FELLOWSHIP_0.1.6_setup.exe",
    "E:\STORM FELLOWSHIP\Files\StormFellowshipSetup.exe"
)

foreach ($f in $filesToSign) {
    if (Test-Path $f) {
        $res = Set-AuthenticodeSignature -FilePath $f -Certificate $cert -HashAlgorithm SHA256
        Write-Host "[SIGN] $f -> $($res.Status) ($($res.StatusMessage))"
    }
}
