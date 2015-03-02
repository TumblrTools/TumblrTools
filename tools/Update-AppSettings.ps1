Param(
    [String] $configPath,
    [String] $key,
    [String] $value
)

$scriptPath = Split-Path -parent $MyInvocation.MyCommand.Definition
pushd $scriptPath

$configPath = Join-Path $scriptPath $configPath | Resolve-Path

Write-Host "Updating AppSettings section..." -foregroundcolor "Cyan"

Write-Host -NoNewline "Loading config file in $configPath... " -ForegroundColor Cyan
[xml]$xml = Get-Content $configPath
Write-Host "Done" -ForegroundColor Green
 
# Check if appSettings node exists
$appSettingsNode = $xml.SelectSingleNode("//configuration/appSettings")
if ($appSettingsNode -eq $null)
{
    Write-Host -NoNewline "appSettings node does not exist. Adding the node... " -ForegroundColor Cyan
 
        # Node does not exist. So create it
    $config = $xml.SelectSingleNode("//configuration");
    $appSettingsNode = $xml.CreateNode('element',"appSettings","")    
    $configurationNode = $config.AppendChild($appSettingsNode)

    Write-Host "Done" -ForegroundColor Green
}
 
# Check for <add> node
$addNode = $xml.SelectSingleNode("//configuration/appSettings/add[@key='$key']")
if ($addNode -ne $null)
{
    Write-Host -NoNewline "$key entry found in config file... " -ForegroundColor Cyan
    # Delete node
    $temp = $appSettingsNode.RemoveChild($addNode)
    Write-Host "Node removed" -ForegroundColor Green
}
 
Write-Host -NoNewline "Creating entry... " -ForegroundColor Cyan
$root = $xml.get_DocumentElement();         
$addNode = $xml.CreateNode('element',"add","")    
$addNode.SetAttribute("key", $key)
$addNode.SetAttribute("value", $value)
$appSettingsNode =  $xml.SelectSingleNode("//configuration/appSettings").AppendChild($addNode)
Write-Host "Done" -ForegroundColor Green
 
Write-Host -NoNewline "Saving... " -ForegroundColor Cyan
$xml.Save($configPath)
Write-Host "Done" -ForegroundColor Green
     
Write-Host "appSettings section updated successfully." -foregroundcolor "Green"

popd