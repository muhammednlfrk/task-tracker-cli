$ProjectPath = "./src/TaskTracker.CLI/TaskTracker.CLI.csproj"
$OutputBasePath = "./publish"
$Runtimes = @("win-x64", "linux-x64", "osx-x64", "osx-arm64")

foreach ($Runtime in $Runtimes) {
    $OutputPath = "$OutputBasePath/$Runtime"
    Write-Host "Publishing .NET application for $Runtime..."
    dotnet publish $ProjectPath -c Release -r $Runtime -o $OutputPath `
        -p:PublishSingleFile=true `
        -p:IncludeNativeLibraries=true `
        -p:PublishReadyToRun=true `
        -p:DebugType=embedded `
        --self-contained=true
    Write-Host "Publish completed for $Runtime! Output directory: $OutputPath"
}
