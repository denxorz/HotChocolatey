$ErrorActionPreference = 'Stop'; # stop on all errors

$packageName = 'hot-chocolatey'
$toolsDir = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"
$url = 'http://hotchocolatey.jjb3.nl/1.0.0.0/Setup%20Hot%20Chocolatey.msi'

$packageArgs = @{
  packageName   = $packageName
  unzipLocation = $toolsDir
  fileType      = 'msi'
  url           = $url
  silentArgs    = "/qn /norestart /l*v '$env:TEMP\chocolatey\$packageName\install.log'"
  validExitCodes= @(0, 3010, 1641)
  registryUninstallerKey = 'hot-chocolatey'
}

Install-ChocolateyPackage @packageArgs