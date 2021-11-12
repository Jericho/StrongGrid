# Inspired by: https://github.com/PowerShell/PSScriptAnalyzer/blob/master/tools/appveyor.psm1

$ErrorActionPreference = 'Stop'

# Implements the AppVeyor 'install' step and installs the desired .Net Core SDK if not already installed.
function Invoke-AppVeyorInstall {

    Write-Verbose -Verbose "Checking availability of desired .Net CORE SDK"

    $globalDotJson = Get-Content (Join-Path $PSScriptRoot 'global.json') -Raw | ConvertFrom-Json
    $desiredDotNetCoreSDKVersion = $globalDotJson.sdk.version
    $desiredDotNetCoreSDKVersionPresent = (dotnet --list-sdks) -match $desiredDotNetCoreSDKVersion

    if (-not $desiredDotNetCoreSDKVersionPresent) {
        Write-Verbose -Verbose "Installing desired .Net CORE SDK $desiredDotNetCoreSDKVersion"
        $originalSecurityProtocol = [Net.ServicePointManager]::SecurityProtocol
        try {
            [Net.ServicePointManager]::SecurityProtocol = [Net.ServicePointManager]::SecurityProtocol -bor [Net.SecurityProtocolType]::Tls12
            if ($IsLinux -or $isMacOS) {
                Invoke-WebRequest 'https://dot.net/v1/dotnet-install.sh' -OutFile dotnet-install.sh
                bash dotnet-install.sh --version $desiredDotNetCoreSDKVersion

                Write-Verbose -Verbose "BEFORE"
                Write-Verbose -Verbose "PATH: $([Environment]::GetEnvironmentVariable("PATH"))"

                $OLDPATH = [System.Environment]::GetEnvironmentVariable("PATH")
                $NEWPATH = "/home/appveyor/.dotnet$([System.IO.Path]::PathSeparator)$OLDPATH"
                [Environment]::SetEnvironmentVariable("PATH", "$NEWPATH")

                Write-Verbose -Verbose "AFTER"
                Write-Verbose -Verbose "PATH: $([Environment]::GetEnvironmentVariable("PATH"))"
            }
            else {
                Invoke-WebRequest 'https://dot.net/v1/dotnet-install.ps1' -OutFile dotnet-install.ps1
                .\dotnet-install.ps1 -Version $desiredDotNetCoreSDKVersion
            }
        }
        finally {
            [Net.ServicePointManager]::SecurityProtocol = $originalSecurityProtocol
            Remove-Item .\dotnet-install.*
        }
        Write-Verbose -Verbose 'Installed desired .Net CORE SDK'
    }
}
