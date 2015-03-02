Param(
    [String] $configPath,
    [String] $name,
    [String] $connectionString,
    [String] $providerName
)


$scriptPath = Split-Path -parent $MyInvocation.MyCommand.Definition
pushd $scriptPath

mv ..\TumblrTools.Viewer\Web.config.example ..\TumblrTools.Viewer\Web.config
.\Update-AppSettings.ps1 ..\TumblrTools.Viewer\Web.config ConnectionString $Env:ConnectionString
.\Update-AppSettings.ps1 ..\TumblrTools.Viewer\Web.config TableName $Env:TableName
.\Update-AppSettings.ps1 ..\TumblrTools.Viewer\Web.config PhotosDirectory $Env:PhotosDirectory
.\Update-AppSettings.ps1 ..\TumblrTools.Viewer\Web.config ConsumerKey $Env:TumblrConsumerKey
.\Update-AppSettings.ps1 ..\TumblrTools.Viewer\Web.config ConsumerSecret $Env:TumblrConsumerSecret

mv ..\TumblrTools.CommandLine\App.config.example ..\TumblrTools.CommandLine\App.config
.\Update-AppSettings.ps1 ..\TumblrTools.CommandLine\App.config ConnectionString $Env:ConnectionString
.\Update-AppSettings.ps1 ..\TumblrTools.CommandLine\App.config TableName $Env:TableName
.\Update-AppSettings.ps1 ..\TumblrTools.CommandLine\App.config PhotosDirectory $Env:PhotosDirectory
.\Update-AppSettings.ps1 ..\TumblrTools.CommandLine\App.config ConsumerKey $Env:TumblrConsumerKey
.\Update-AppSettings.ps1 ..\TumblrTools.CommandLine\App.config ConsumerSecret $Env:TumblrConsumerSecret

popd