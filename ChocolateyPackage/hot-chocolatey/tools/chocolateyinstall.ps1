$name   = 'hot-chocolatey'
$url = 'http://hotchocolatey.jjb3.nl/releases/__version__/Setup%20Hot%20Chocolatey.msi'
$silent = '/quiet'

Install-ChocolateyPackage $name 'msi' $silent $url
