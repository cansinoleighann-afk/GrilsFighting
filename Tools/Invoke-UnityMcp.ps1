param([string]$Tool, [string]$ArgumentsJson = '{}', [string]$CodeFile)
$arguments = $ArgumentsJson | ConvertFrom-Json -AsHashtable
if ($CodeFile) { $arguments.code = Get-Content -Raw -LiteralPath $CodeFile }
$payload = @{jsonrpc='2.0';id=1;method='tools/call';params=@{name=$Tool;arguments=$arguments}} | ConvertTo-Json -Depth 30 -Compress
$reply = Invoke-RestMethod -TimeoutSec 55 -Uri 'http://127.0.0.1:22846/mcp' -Method Post -ContentType 'application/json' -Body $payload
if ($reply.error) { $reply.error | ConvertTo-Json -Depth 20; exit 1 }
foreach ($entry in $reply.result.content) { if ($entry.type -eq 'text') { $entry.text } }
