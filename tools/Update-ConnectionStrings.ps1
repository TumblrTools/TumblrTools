Param(
    [String] $configPath,
    [String] $name,
    [String] $connectionString,
    [String] $providerName
)

$scriptPath = Split-Path -parent $MyInvocation.MyCommand.Definition

$configPath = Join-Path $scriptPath $configPath

Write-Host "Updating connectionStrings section..." -foregroundcolor "Cyan"

Write-Host -NoNewline "Loading config file in $configPath... " -ForegroundColor Cyan
[xml]$xml = Get-Content $configPath
Write-Host "Done" -ForegroundColor Green
 
# Check if connectionStrings node exists
$appSettingsNode = $xml.SelectSingleNode("//configuration/connectionStrings")
if ($appSettingsNode -eq $null)
{
    Write-Host -NoNewline "connectionStrings node does not exist. Adding the node... " -ForegroundColor Cyan
 
        # Node does not exist. So create it
    $config = $xml.SelectSingleNode("//configuration");
    $appSettingsNode = $xml.CreateNode('element', "connectionStrings", "")    
    $configurationNode = $config.AppendChild($appSettingsNode)

    Write-Host "Done" -ForegroundColor Green
}
 
# Check for <add> node
$addNode = $xml.SelectSingleNode("//configuration/connectionStrings/add[@name='$name']")
if ($addNode -ne $null)
{
    Write-Host -NoNewline "$name entry found in config file... " -ForegroundColor Cyan
    # Delete node
    $temp = $appSettingsNode.RemoveChild($addNode)
    Write-Host -NoNewline "Node removed" -ForegroundColor Green
}
 
Write-Host -NoNewline "Creating entry... " -ForegroundColor Cyan
$root = $xml.get_DocumentElement();         
$addNode = $xml.CreateNode('element',"add","")    
$addNode.SetAttribute("name", $name)
$addNode.SetAttribute("connectionString", $connectionString)
$addNode.SetAttribute("providerName", $providerName)
$appSettingsNode =  $xml.SelectSingleNode("//configuration/connectionStrings").AppendChild($addNode)
Write-Host "Done" -ForegroundColor Green
 
Write-Host -NoNewline "Saving... " -ForegroundColor Cyan
$xml.Save($configPath)
Write-Host "Done" -ForegroundColor Green
     
Write-Host "connectionStrings section updated successfully." -foregroundcolor "Green"