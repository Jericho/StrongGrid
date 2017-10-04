// Install addins.
#addin "nuget:?package=Cake.Coveralls&version=0.7.0"

// Install tools.
#tool "nuget:?package=GitVersion.CommandLine&version=4.0.0-beta0012"
#tool "nuget:?package=GitReleaseManager&version=0.6.0"
#tool "nuget:?package=OpenCover&version=4.6.519"
#tool "nuget:?package=ReportGenerator&version=3.0.0"
#tool "nuget:?package=coveralls.io&version=1.3.4"
#tool "nuget:?package=xunit.runner.console&version=2.2.0"


///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument<string>("target", "Default");
var configuration = Argument<string>("configuration", "Release");


///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////

var libraryName = "StrongGrid";
var gitHubRepo = "StrongGrid";

var testCoverageFilter = "+[StrongGrid]* -[StrongGrid]StrongGrid.Properties.* -[StrongGrid]StrongGrid.Models.* -[StrongGrid]StrongGrid.Logging.*";
var testCoverageExcludeByAttribute = "*.ExcludeFromCodeCoverage*";
var testCoverageExcludeByFile = "*/*Designer.cs;*/*AssemblyInfo.cs";

var nuGetApiUrl = EnvironmentVariable("NUGET_API_URL");
var nuGetApiKey = EnvironmentVariable("NUGET_API_KEY");

var myGetApiUrl = EnvironmentVariable("MYGET_API_URL");
var myGetApiKey = EnvironmentVariable("MYGET_API_KEY");

var gitHubUserName = EnvironmentVariable("GITHUB_USERNAME");
var gitHubPassword = EnvironmentVariable("GITHUB_PASSWORD");

var sourceFolder = "./Source/";

var outputDir = "./artifacts/";
var codeCoverageDir = outputDir + "CodeCoverage/";
var unitTestsProject = sourceFolder + libraryName + ".UnitTests/" + libraryName + ".UnitTests.csproj";

var versionInfo = GitVersion(new GitVersionSettings() { OutputType = GitVersionOutput.Json });
var milestone = string.Concat("v", versionInfo.MajorMinorPatch);
var cakeVersion = typeof(ICakeContext).Assembly.GetName().Version.ToString();
var isLocalBuild = BuildSystem.IsLocalBuild;
var isMainBranch = StringComparer.OrdinalIgnoreCase.Equals("master", BuildSystem.AppVeyor.Environment.Repository.Branch);
var	isMainRepo = StringComparer.OrdinalIgnoreCase.Equals(gitHubUserName + "/" + gitHubRepo, BuildSystem.AppVeyor.Environment.Repository.Name);
var	isPullRequest = BuildSystem.AppVeyor.Environment.PullRequest.IsPullRequest;
var	isTagged = (
	BuildSystem.AppVeyor.Environment.Repository.Tag.IsTag &&
	!string.IsNullOrWhiteSpace(BuildSystem.AppVeyor.Environment.Repository.Tag.Name)
);


///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(context =>
{
	if (isMainBranch && (context.Log.Verbosity != Verbosity.Diagnostic))
	{
		Information("Increasing verbosity to diagnostic.");
		context.Log.Verbosity = Verbosity.Diagnostic;
	}

	Information("Building version {0} of {1} ({2}, {3}) using version {4} of Cake",
		versionInfo.LegacySemVerPadded,
		libraryName,
		configuration,
		target,
		cakeVersion
	);

	Information("Variables:\r\n\tLocalBuild: {0}\r\n\tIsMainBranch: {1}\r\n\tIsMainRepo: {2}\r\n\tIsPullRequest: {3}\r\n\tIsTagged: {4}",
		isLocalBuild,
		isMainBranch,
		isMainRepo,
		isPullRequest,
		isTagged
	);

	Information("Myget Info:\r\n\tApi Url: {0}\r\n\tApi Key: {1}",
		myGetApiUrl,
		string.IsNullOrEmpty(myGetApiKey) ? "[NULL]" : new string('*', myGetApiKey.Length)
	);

	Information("Nuget Info:\r\n\tApi Url: {0}\r\n\tApi Key: {1}",
		nuGetApiUrl,
		string.IsNullOrEmpty(nuGetApiKey) ? "[NULL]" : new string('*', nuGetApiKey.Length)
	);

	Information("GitHub Info:\r\n\tRepo: {0}\r\n\tUserName: {1}\r\n\tPassword: {2}",
		gitHubRepo,
		gitHubUserName,
		string.IsNullOrEmpty(gitHubPassword) ? "[NULL]" : new string('*', gitHubPassword.Length)
	);
});

Teardown(context =>
{
	// Executed AFTER the last task.
	Information("Finished running tasks.");
});


///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

Task("AppVeyor-Build_Number")
	.WithCriteria(() => AppVeyor.IsRunningOnAppVeyor)
	.Does(() =>
{
	GitVersion(new GitVersionSettings()
	{
		UpdateAssemblyInfo = false,
		OutputType = GitVersionOutput.BuildServer
	});
});

Task("Clean")
	.IsDependentOn("AppVeyor-Build_Number")
	.Does(() =>
{
	// Clean solution directories.
	Information("Cleaning {0}", sourceFolder);
	CleanDirectories(sourceFolder + "*/bin/" + configuration);
	CleanDirectories(sourceFolder + "*/obj/" + configuration);

	// Clean previous artifacts
	Information("Cleaning {0}", outputDir);
	if (DirectoryExists(outputDir)) CleanDirectories(MakeAbsolute(Directory(outputDir)).FullPath);
	else CreateDirectory(outputDir);

	// Create folder for code coverage report
	CreateDirectory(codeCoverageDir);
});

Task("Restore-NuGet-Packages")
	.IsDependentOn("Clean")
	.Does(() =>
{
	DotNetCoreRestore("./Source/", new DotNetCoreRestoreSettings
	{
		Sources = new [] {
			"https://www.myget.org/F/xunit/api/v3/index.json",
			"https://dotnet.myget.org/F/dotnet-core/api/v3/index.json",
			"https://dotnet.myget.org/F/cli-deps/api/v3/index.json",
			"https://api.nuget.org/v3/index.json",
		}
	});
});

Task("Build")
	.IsDependentOn("Restore-NuGet-Packages")
	.Does(() =>
{
	DotNetCoreBuild(sourceFolder + libraryName + ".sln", new DotNetCoreBuildSettings
	{
		Configuration = configuration,
		ArgumentCustomization = args => args.Append("/p:SemVer=" + versionInfo.LegacySemVerPadded)
	});
});

Task("Run-Unit-Tests")
	.IsDependentOn("Build")
	.Does(() =>
{
	DotNetCoreTest(unitTestsProject, new DotNetCoreTestSettings
	{
		NoBuild = true,
		Configuration = configuration
	});
});

Task("Run-Code-Coverage")
	.IsDependentOn("Build")
	.Does(() =>
{
	Action<ICakeContext> testAction = ctx => ctx.DotNetCoreTest(unitTestsProject, new DotNetCoreTestSettings
	{
		NoBuild = true,
		Configuration = configuration
	});

	OpenCover(testAction,
		codeCoverageDir + "coverage.xml",
		new OpenCoverSettings
		{
			OldStyle = true,
			MergeOutput = true,
			ArgumentCustomization = args => args.Append("-returntargetcode")
		}
		.WithFilter(testCoverageFilter)
		.ExcludeByAttribute(testCoverageExcludeByAttribute)
		.ExcludeByFile(testCoverageExcludeByFile)
	);
});

Task("Upload-Coverage-Result")
	.Does(() =>
{
	CoverallsIo(codeCoverageDir + "coverage.xml");
});

Task("Generate-Code-Coverage-Report")
	.IsDependentOn("Run-Code-Coverage")
	.Does(() =>
{
	ReportGenerator(
		codeCoverageDir + "coverage.xml",
		codeCoverageDir,
		new ReportGeneratorSettings() {
			ClassFilters = new[] { "*.UnitTests*" }
		}
	);
});

Task("Create-NuGet-Package")
	.IsDependentOn("Build")
	.Does(() =>
{
	var settings = new DotNetCorePackSettings
	{
		Configuration = configuration,
		IncludeSource = false,
		IncludeSymbols = false,
		NoBuild = true,
		OutputDirectory = outputDir,
        ArgumentCustomization = (args) =>
		{
			return args
				.Append("/p:Version={0}", versionInfo.LegacySemVerPadded)
				.Append("/p:AssemblyVersion={0}", versionInfo.MajorMinorPatch)
				.Append("/p:FileVersion={0}", versionInfo.MajorMinorPatch)
				.Append("/p:AssemblyInformationalVersion={0}", versionInfo.InformationalVersion);
		}
	};

	DotNetCorePack(sourceFolder + libraryName + "/" + libraryName + ".csproj", settings);
});

Task("Upload-AppVeyor-Artifacts")
	.WithCriteria(() => AppVeyor.IsRunningOnAppVeyor)
	.Does(() =>
{
	foreach (var file in GetFiles(outputDir + "*.*"))
	{
		AppVeyor.UploadArtifact(file.FullPath);
	}
});

Task("Publish-NuGet")
	.IsDependentOn("Create-NuGet-Package")
	.WithCriteria(() => !isLocalBuild)
	.WithCriteria(() => !isPullRequest)
	.WithCriteria(() => isMainRepo)
	.WithCriteria(() => isMainBranch)
	.WithCriteria(() => isTagged)
	.Does(() =>
{
	if(string.IsNullOrEmpty(nuGetApiKey)) throw new InvalidOperationException("Could not resolve NuGet API key.");
	if(string.IsNullOrEmpty(nuGetApiUrl)) throw new InvalidOperationException("Could not resolve NuGet API url.");

	foreach(var package in GetFiles(outputDir + "*.nupkg"))
	{
		// Push the package.
		NuGetPush(package, new NuGetPushSettings {
			ApiKey = nuGetApiKey,
			Source = nuGetApiUrl
		});
	}
});

Task("Publish-MyGet")
	.IsDependentOn("Create-NuGet-Package")
	.WithCriteria(() => !isLocalBuild)
	.WithCriteria(() => !isPullRequest)
	.WithCriteria(() => isMainRepo)
	.Does(() =>
{
	if(string.IsNullOrEmpty(nuGetApiKey)) throw new InvalidOperationException("Could not resolve MyGet API key.");
	if(string.IsNullOrEmpty(nuGetApiUrl)) throw new InvalidOperationException("Could not resolve MyGet API url.");

	foreach(var package in GetFiles(outputDir + "*.nupkg"))
	{
		// Push the package.
		NuGetPush(package, new NuGetPushSettings {
			ApiKey = myGetApiKey,
			Source = myGetApiUrl
		});
	}
});

Task("Create-Release-Notes")
	.Does(() =>
{
	if(string.IsNullOrEmpty(gitHubUserName)) throw new InvalidOperationException("Could not resolve GitHub user name.");
	if(string.IsNullOrEmpty(gitHubPassword)) throw new InvalidOperationException("Could not resolve GitHub password.");

	GitReleaseManagerCreate(gitHubUserName, gitHubPassword, gitHubUserName, gitHubRepo, new GitReleaseManagerCreateSettings {
		Name              = milestone,
		Milestone         = milestone,
		Prerelease        = false,
		TargetCommitish   = "master"
	});
});

Task("Publish-GitHub-Release")
	.WithCriteria(() => !isLocalBuild)
	.WithCriteria(() => !isPullRequest)
	.WithCriteria(() => isMainRepo)
	.WithCriteria(() => isMainBranch)
	.WithCriteria(() => isTagged)
	.Does(() =>
{
	if(string.IsNullOrEmpty(gitHubUserName)) throw new InvalidOperationException("Could not resolve GitHub user name.");
	if(string.IsNullOrEmpty(gitHubPassword)) throw new InvalidOperationException("Could not resolve GitHub password.");

	GitReleaseManagerClose(gitHubUserName, gitHubPassword, gitHubUserName, gitHubRepo, milestone);
});


///////////////////////////////////////////////////////////////////////////////
// TARGETS
///////////////////////////////////////////////////////////////////////////////

Task("Coverage")
	.IsDependentOn("Generate-Code-Coverage-Report")
	.Does(() =>
{
	StartProcess("cmd", "/c start " + codeCoverageDir + "index.htm");
});

Task("ReleaseNotes")
	.IsDependentOn("Create-Release-Notes"); 

Task("AppVeyor")
	.IsDependentOn("Run-Code-Coverage")
	.IsDependentOn("Upload-Coverage-Result")
	.IsDependentOn("Create-NuGet-Package")
	.IsDependentOn("Upload-AppVeyor-Artifacts")
	.IsDependentOn("Publish-MyGet")
	.IsDependentOn("Publish-NuGet")
	.IsDependentOn("Publish-GitHub-Release");

Task("Default")
	.IsDependentOn("Run-Unit-Tests")
	.IsDependentOn("Create-NuGet-Package");


///////////////////////////////////////////////////////////////////////////////
// EXECUTION
///////////////////////////////////////////////////////////////////////////////

RunTarget(target);
