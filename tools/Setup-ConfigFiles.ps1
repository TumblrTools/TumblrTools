Param(
    [String] $configPath,
    [String] $name,
    [String] $connectionString,
    [String] $providerName
)

.\Update-AppSettings ..\TumblrTools.Viewer\Web.config ConnectionString $Env:ConnectionString
.\Update-AppSettings ..\TumblrTools.Viewer\Web.config TableName $Env:TableName
.\Update-AppSettings ..\TumblrTools.Viewer\Web.config PhotosDirectory $Env:PhotosDirectory
.\Update-AppSettings ..\TumblrTools.Viewer\Web.config ConsumerKey $Env:TumblrConsumerKey
.\Update-AppSettings ..\TumblrTools.Viewer\Web.config ConsumerSecret $Env:TumblrConsumerSecret
