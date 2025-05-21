$rootPath = "C:\rpc\AvaTerminal3"
$zipFile = "C:\tmp\AvaTerminal3.zip"
# Zip file
if (Test-Path -Path $zipFile) {
	Remove-Item -Path $zipFile -Force -Confirm:$false
}
Compress-Archive -Path (Get-ChildItem -Recurse $rootPath | Where-Object { $_.FullName -notmatch '\\(bin|obj)(\\|$)' }) -DestinationPath $zipFile

explorer.exe C:\tmp
