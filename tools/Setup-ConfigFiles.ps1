Param(
    [String] $configPath,
    [String] $name,
    [String] $connectionString,
    [String] $providerName
)

.\Update-AppSettings.ps1 ..\TumblrTools.Viewer\Web.config ConnectionString $Env:ConnectionString
.\Update-AppSettings.ps1 ..\TumblrTools.Viewer\Web.config TableName $Env:TableName
.\Update-AppSettings.ps1 ..\TumblrTools.Viewer\Web.config PhotosDirectory $Env:PhotosDirectory
.\Update-AppSettings.ps1 ..\TumblrTools.Viewer\Web.config ConsumerKey $Env:TumblrConsumerKey
.\Update-AppSettings.ps1 ..\TumblrTools.Viewer\Web.config ConsumerSecret $Env:TumblrConsumerSecret
