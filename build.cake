// Install addins.
#addin "nuget:?package=Polly&version=4.3.0"
#addin "nuget:?package=Cake.Coveralls&version=0.2.0"

// Install tools.
#tool "nuget:?package=GitVersion.CommandLine&version=3.6.4"
#tool "nuget:?package=GitReleaseManager&version=0.6.0"
#tool "nuget:?package=OpenCover&version=4.6.519"
#tool "nuget:?package=ReportGenerator&version=2.5.0"
#tool "nuget:?package=coveralls.io&version=1.3.4"

// Using statements
using Polly;


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

var testCoverageFilter = "+[StrongGrid]* -[StrongGrid]StrongGrid.Properties.* -[StrongGrid]StrongGrid.Model.*";
var testCoverageExcludeByAttribute = "*.ExcludeFromCodeCoverage*";
var testCoverageExcludeByFile = "*/*Designer.cs;*/*AssemblyInfo.cs";

var nuGetApiUrl = EnvironmentVariable("NUGET_API_URL");
var nuGetApiKey = EnvironmentVariable("NUGET_API_KEY");
var gitHubUserName = EnvironmentVariable("GITHUB_USERNAME");
var gitHubPassword = EnvironmentVariable("GITHUB_PASSWORD");

var solution = GetFiles("./Source/" + libraryName + ".sln").First();
var solutionPath = solution.GetDirectory();

var unitTestsPaths = GetDirectories("./Source/*.UnitTests");
var outputDir = "./artifacts/";
var codeCoverageDir = outputDir + "CodeCoverage/";
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

Task("Clean")
	.Does(() =>
{
	// Clean solution directories.
	Information("Cleaning {0}", solutionPath);
	CleanDirectories(solutionPath + "/*/bin/" + configuration);
	CleanDirectories(solutionPath + "/*/obj/" + configuration);

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
	// Restore all NuGet packages.
	var maxRetryCount = 5;
	var toolTimeout = 1d;

	Information("Restoring {0}...", solution);

	Policy
		.Handle<Exception>()
		.Retry(maxRetryCount, (exception, retryCount, context) => {
			if (retryCount == maxRetryCount)
			{
				throw exception;
			}
			else
			{
				Verbose("{0}", exception);
				toolTimeout += 0.5;
			}})
		.Execute(()=> {
			NuGetRestore(solution, new NuGetRestoreSettings {
				Source = new List<string> {
					"https://api.nuget.org/v3/index.json",
					"https://www.myget.org/F/roslyn-nightly/api/v3/index.json"
				},
				ToolTimeout = TimeSpan.FromMinutes(toolTimeout)
			});
		});
});

Task("Update-Asembly-Version")
	.WithCriteria(() => !isLocalBuild)
	.Does(() =>
{
	GitVersion(new GitVersionSettings()
	{
		UpdateAssemblyInfo = true,
		OutputType = GitVersionOutput.BuildServer
	});
});

Task("Build")
	.IsDependentOn("Restore-NuGet-Packages")
	.IsDependentOn("Update-Asembly-Version")
	.Does(() =>
{
	// Build the solution
	Information("Building {0}", solution);
	MSBuild(solution, new MSBuildSettings()
		.SetPlatformTarget(PlatformTarget.MSIL)
		.SetConfiguration(configuration)
		.SetVerbosity(Verbosity.Minimal)
		.SetNodeReuse(false)
		.WithProperty("Windows", "True")
		.WithProperty("TreatWarningsAsErrors", "True")
		.WithTarget("Build")
	);
});

Task("Run-Unit-Tests")
	.IsDependentOn("Build")
	.Does(() =>
{
	var vsTestSettings = new VSTestSettings();
	vsTestSettings.ArgumentCustomization = args => args.Append("/Parallel");

	foreach(var path in unitTestsPaths)
	{
		Information("Running unit tests in {0}...", path);
		VSTest(path + "/bin/" + configuration + "/*.UnitTests.dll", vsTestSettings);
	}
});

Task("Run-Code-Coverage")
	.IsDependentOn("Build")
	.Does(() =>
{
	var testAssemblyPath = string.Format("{2}/bin/{1}/{0}.UnitTests.dll", libraryName, configuration, unitTestsPaths.First());
	
	var testArgs = new List<string>();
	testArgs.Add("/Parallel");
	if (AppVeyor.IsRunningOnAppVeyor) testArgs.Add("/logger:Appveyor");

	var vsTestSettings = new VSTestSettings();
	vsTestSettings.ArgumentCustomization = args =>
	{
		foreach (var arg in testArgs)
		{
			args.Append(arg);
		}
		return args;
	};

	OpenCover(
		tool => { tool.VSTest(testAssemblyPath, vsTestSettings); },
		new FilePath(codeCoverageDir + "coverage.xml"),
		new OpenCoverSettings() { ReturnTargetCodeOffset = 0 }
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
	var settings = new NuGetPackSettings
	{
		Version                 = versionInfo.NuGetVersionV2,
		Symbols                 = false,
		NoPackageAnalysis       = true,
		BasePath                = "./Source/",
		OutputDirectory         = outputDir,
		ArgumentCustomization   = args => args.Append("-Prop Configuration=" + configuration)
	};
			
	NuGetPack("./nuspec/" + libraryName + ".nuspec", settings);
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

Task("Create-Release-Notes")
	.Does(() =>
{
	if(string.IsNullOrEmpty(gitHubUserName)) throw new InvalidOperationException("Could not resolve GitHub user name.");
	if(string.IsNullOrEmpty(gitHubPassword)) throw new InvalidOperationException("Could not resolve GitHub password.");

	GitReleaseManagerCreate(gitHubUserName, gitHubPassword, gitHubUserName, gitHubRepo, new GitReleaseManagerCreateSettings {
		Name              = milestone,
		Milestone         = milestone,
		Prerelease        = true,
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

Task("Package")
	.IsDependentOn("Run-Unit-Tests")
	.IsDependentOn("Create-NuGet-Package");

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
	.IsDependentOn("Publish-NuGet")
	.IsDependentOn("Publish-GitHub-Release");

Task("Default")
	.IsDependentOn("Package");


///////////////////////////////////////////////////////////////////////////////
// EXECUTION
///////////////////////////////////////////////////////////////////////////////

RunTarget(target);
