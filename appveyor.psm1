# Inspired by: https://github.com/PowerShell/PSScriptAnalyzer/blob/master/tools/appveyor.psm1

$ErrorActionPreference = 'Stop'

# Implements the AppVeyor 'install' step and installs the desired .Net Core SDK if needed.
function Invoke-AppVeyorInstall {

    Write-Verbose -Verbose "Installing required .Net CORE SDK"
    # the legacy WMF4 image only has the old preview SDKs of dotnet
    $globalDotJson = Get-Content (Join-Path $PSScriptRoot 'global.json') -Raw | ConvertFrom-Json
    $requiredDotNetCoreSDKVersion = $globalDotJson.sdk.version
    if ($PSVersionTable.PSVersion.Major -gt 4) {
        $requiredDotNetCoreSDKVersionPresent = (dotnet --list-sdks) -match $requiredDotNetCoreSDKVersion
    }
    else {
        # WMF 4 image has old SDK that does not have --list-sdks parameter
        $requiredDotNetCoreSDKVersionPresent = (dotnet --version).StartsWith($requiredDotNetCoreSDKVersion)
    }
    if (-not $requiredDotNetCoreSDKVersionPresent) {
        Write-Verbose -Verbose "Installing required .Net CORE SDK $requiredDotNetCoreSDKVersion"
        $originalSecurityProtocol = [Net.ServicePointManager]::SecurityProtocol
        try {
            [Net.ServicePointManager]::SecurityProtocol = [Net.ServicePointManager]::SecurityProtocol -bor [Net.SecurityProtocolType]::Tls12
            if ($IsLinux -or $isMacOS) {
                Invoke-WebRequest 'https://dot.net/v1/dotnet-install.sh' -OutFile dotnet-install.sh
                bash dotnet-install.sh --version $requiredDotNetCoreSDKVersion
                [System.Environment]::SetEnvironmentVariable('PATH', "/home/appveyor/.dotnet$([System.IO.Path]::PathSeparator)$PATH")
            }
            else {
                Invoke-WebRequest 'https://dot.net/v1/dotnet-install.ps1' -OutFile dotnet-install.ps1
                .\dotnet-install.ps1 -Version $requiredDotNetCoreSDKVersion
            }
        }
        finally {
            [Net.ServicePointManager]::SecurityProtocol = $originalSecurityProtocol
            Remove-Item .\dotnet-install.*
        }
        Write-Verbose -Verbose 'Installed required .Net CORE SDK'
    }
}
