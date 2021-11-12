# Inspired by: https://github.com/PowerShell/PSScriptAnalyzer/blob/master/tools/appveyor.psm1

$ErrorActionPreference = 'Stop'

# Implements the AppVeyor 'install' step and installs the desired .Net Core SDK if not already installed.
function Invoke-AppVeyorInstall {

    Write-Verbose -Verbose "Checking availability of desired .Net CORE SDK"

    # the legacy WMF4 image only has the old preview SDKs of dotnet
    $globalDotJson = Get-Content (Join-Path $PSScriptRoot 'global.json') -Raw | ConvertFrom-Json
    $desiredDotNetCoreSDKVersion = $globalDotJson.sdk.version
    if ($PSVersionTable.PSVersion.Major -gt 4) {
        $desiredDotNetCoreSDKVersionPresent = (dotnet --list-sdks) -match $desiredDotNetCoreSDKVersion
    }
    else {
        # WMF 4 image has old SDK that does not have --list-sdks parameter
        $desiredDotNetCoreSDKVersionPresent = (dotnet --version).StartsWith($desiredDotNetCoreSDKVersion)
    }

    if (-not $desiredDotNetCoreSDKVersionPresent) {
        Write-Verbose -Verbose "Installing desired .Net CORE SDK $desiredDotNetCoreSDKVersion"
        $originalSecurityProtocol = [Net.ServicePointManager]::SecurityProtocol
        try {
            [Net.ServicePointManager]::SecurityProtocol = [Net.ServicePointManager]::SecurityProtocol -bor [Net.SecurityProtocolType]::Tls12
            if ($IsLinux -or $isMacOS) {
                Invoke-WebRequest 'https://dot.net/v1/dotnet-install.sh' -OutFile dotnet-install.sh
                sudo bash dotnet-install.sh --version $desiredDotNetCoreSDKVersion --install-dir /usr/share
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
