Param(
    [String] $configPath,
    [String] $name,
    [String] $connectionString,
    [String] $providerName
)

.\Update-AppSettings ..\TumblrTools.Viewer\Web.config ConnectionString $Env:ConnectionString
.\Update-AppSettings ..\TumblrTools.Viewer\Web.config TableName $Env:PhotosDirectory
.\Update-AppSettings ..\TumblrTools.Viewer\Web.config ConsumerKey $Env:TumblrConsumerKey
.\Update-AppSettings ..\TumblrTools.Viewer\Web.config ConsumerSecret $Env:TumblrConsumerSecret
