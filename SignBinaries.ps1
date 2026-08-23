$signtool = (Get-ChildItem "C:\Program Files (x86)\Windows Kits\10\bin\*\x64\signtool.exe" | Select-Object -Last 1).FullName

if (-not $signtool) {
    Write-Host "signtool.exe not found!"
    exit 1
}

# Recursively unblock all files in the project
Get-ChildItem -Path "E:\STORM FELLOWSHIP" -Recurse | Unblock-File -ErrorAction SilentlyContinue

# Sign executables and libraries
$filesToSign = @(
    "E:\STORM FELLOWSHIP\Assembling\StormFellowship.exe",
    "E:\STORM FELLOWSHIP\Assembling\StormFellowship.dll",
    "E:\STORM FELLOWSHIP\Files\STORM_FELLOWSHIP_0.2.3_setup.exe"
)

foreach ($f in $filesToSign) {
    if (Test-Path $f) {
        $desc = "STORM FELLOWSHIP 0.2.3"
        $argsList = "sign /fd SHA256 /d `"$desc`" /sha1 F8A8D6D6A6954867F08F480210CA0A81F2FEF756 `"$f`""
        
        Write-Host "Running signtool for $f ..."
        Start-Process -FilePath $signtool -ArgumentList $argsList -Wait -NoNewWindow
    }
}

