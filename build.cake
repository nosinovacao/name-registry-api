#addin nuget:https://www.nuget.org/api/v2/?package=Cake.Docker
#addin nuget:https://www.nuget.org/api/v2/?package=Cake.FileHelpers
#addin nuget:https://www.nuget.org/api/v2/?package=Cake.Docker
#addin nuget:https://www.nuget.org/api/v2/?package=Cake.AssemblyInfoReflector

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Full-Build");
var configuration = Argument("configuration", "Release");

var dockerPushTagSuffix = Argument("dockerTagSuffix", string.Empty);
var dockerPushLatest = HasArgument("dockerPushLatest");


//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var outputDir = Directory("Output/");
var artifactsDir = outputDir + Directory("Artifacts/");

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
    {
        CleanDirectory(artifactsDir);
    });

Task("Restore-NuGet-Packages")
    .Does(() =>
    {
        DotNetCoreRestore("NAME.Registry.API.sln");
    });

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
    {
        var dotNetBuildConfig = new DotNetCoreBuildSettings() {
            Configuration = configuration
        };

        DotNetCoreBuild("NAME.Registry.API.sln", dotNetBuildConfig);
    });
        
Task("Run-Unit-Tests")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
    {
        var files = GetFiles("./unit-tests/**/*.csproj");

        foreach (var file in files) {
            DotNetCoreTool(file, "xunit", "-trait \"TestCategory=\"Unit\"\"");
        }
    });

Task("Publish-Website")
    .IsDependentOn("Build")
    .Does(() => 
    {
        var settings = new DotNetCorePublishSettings {
            Configuration = configuration,
            OutputDirectory = artifactsDir + Directory("NAME.Registry.API/")
        };

        DotNetCorePublish("./src/NAME.Registry.API", settings);
    });

Task("Docker-Build-AND-Push")
    .IsDependentOn("Publish-Website")
    .Does(() => 
    {
        var registryApiLocation = artifactsDir + Directory("NAME.Registry.API/");
        var currentVersion = ReflectAssemblyInfo(registryApiLocation + File("NAME.Registry.API.dll")).AssemblyVersion;
        
        var dockerImage = "nosinovacao/name-registry-api";

        var currentVersionTag = dockerImage + ":" + currentVersion;
        if(!string.IsNullOrEmpty(dockerPushTagSuffix))
            currentVersionTag += ("-" + dockerPushTagSuffix);

        var tags = new List<string>();
        tags.Add(currentVersionTag);

        if(dockerPushLatest)
            tags.Add(dockerImage + ":latest");

        var buildSettings = new DockerBuildSettings() {
            Tag = tags.ToArray()
        };

        DockerBuild(buildSettings, artifactsDir + Directory("NAME.Registry.API/Dockerfile"));
    
        foreach(var tag in tags) {
            DockerPush(tag);
        }
    });


//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////


Task("Build-AND-Test")
    .IsDependentOn("Build")
    .IsDependentOn("Run-Unit-Tests");

Task("AppVeyor")
    .IsDependentOn("Run-Unit-Tests");

Task("TravisCI")
    .IsDependentOn("Run-Unit-Tests");

Task("Default")
    .IsDependentOn("Build-AND-Test");
    
//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);