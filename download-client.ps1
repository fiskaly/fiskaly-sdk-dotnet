$url = "https://storage.googleapis.com/download/storage/v1/b/fiskaly-cdn/o/clients%2Fcom.fiskaly.client-windows-amd64-v1.1.400.dll?generation=1587652227126443&alt=media"
$output = "bin/Release/netcoreapp.3.1/com.fiskaly.client-windows-amd64-v1.1.400.dll"
$start_time = Get-Date
echo "Starting Download at $start_time"
Invoke-WebRequest -Uri $url -OutFile $output
Write-Output "Time taken: $((Get-Date).Subtract($start_time).Seconds) second(s)"