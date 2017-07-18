#addin nuget:https://www.nuget.org/api/v2/?package=Cake.Docker
#addin nuget:https://www.nuget.org/api/v2/?package=Cake.FileHelpers
#addin nuget:https://www.nuget.org/api/v2/?package=Cake.Docker
#addin nuget:https://www.nuget.org/api/v2/?package=Cake.AssemblyInfoReflector

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Full-Build");
var configuration = Argument("configuration", "Release");

var dockerPushCurrentVersion = HasArgument("dockerPushCurrentVersion");
var dockerPushAdditionalTag = Argument<string>("dockerPushAdditionalTag", null);


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
    .IsDependentOn("Clean")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() => 
    {
        var registryApiLocation = artifactsDir + Directory("NAME.Registry.API/");

        var settings = new DotNetCorePublishSettings {
            Configuration = configuration,
            OutputDirectory = registryApiLocation
        };

        DotNetCorePublish("./src/NAME.Registry.API", settings);

        Zip("./Output/Artifacts", "./Output/Artifacts/NAME.Registry.API.zip", "./Output/Artifacts/**/*");
    });

Task("Docker-Build-AND-Push")
    .Does(() => 
    {
        var registryApiLocation = artifactsDir + Directory("NAME.Registry.API/");
        
        var reflectedAssemblyInfo = ReflectAssemblyInfo(registryApiLocation + File("NAME.Registry.API.dll"));
        var currentVersion = reflectedAssemblyInfo.AssemblyFileVersion;
        
        var dockerImage = "nosinovacao/name-registry-api";
        var tags = new List<string>();

        if (dockerPushCurrentVersion)
            tags.Add(dockerImage + ":" + currentVersion);

        if (!string.IsNullOrWhiteSpace(dockerPushAdditionalTag))
            tags.Add(dockerImage + ":" + dockerPushAdditionalTag);

        var buildSettings = new DockerBuildSettings() {
            Tag = tags.ToArray()
        };

        DockerBuild(buildSettings, registryApiLocation);
    
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
    .IsDependentOn("Run-Unit-Tests")
    .IsDependentOn("Publish-Website");

Task("Default")
    .IsDependentOn("Build-AND-Test");
    
//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);