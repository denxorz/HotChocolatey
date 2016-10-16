$name   = 'hot-chocolatey'
$url = 'http://hotchocolatey.jjb3.nl/releases/__version__/Setup%20Hot%20Chocolatey.msi'
$silent = '/quiet'
$checksum = '__checksum__'
$checksumType = 'sha512'

Install-ChocolateyPackage $name 'msi' $silent $url -checksum $checksum -checksumType $checksumType
