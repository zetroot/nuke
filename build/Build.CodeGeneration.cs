// Copyright 2019 Maintainers of NUKE.
// Distributed under the MIT License.
// https://github.com/nuke-build/nuke/blob/master/LICENSE

using System.IO;
using System.Linq;
using CodeGeneration.DocFX;
using CodeGeneration.DocFX.Common;
using CodeGeneration.NSwag;
using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.NuGet;
using static Nuke.CodeGeneration.CodeGenerator;
using static Nuke.CodeGeneration.ReferenceUpdater;
using static Nuke.CodeGeneration.SchemaGenerator;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.Git.GitTasks;

partial class Build
{
    string SpecificationsDirectory => BuildProjectDirectory / "specifications";
    string ReferencesDirectory => BuildProjectDirectory / "references";
    AbsolutePath GenerationDirectory => RootDirectory / "source" / "Nuke.Common" / "Tools";
    string ToolSchemaFile => SourceDirectory / "Nuke.CodeGeneration" / "schema.json";

    Target References => _ => _
        .Requires(() => GitHasCleanWorkingCopy())
        .Executes(() =>
        {
            EnsureCleanDirectory(ReferencesDirectory);

            UpdateReferences(SpecificationsDirectory, ReferencesDirectory);
        });

    Target GenerateNSwag => _ => _
        .Executes(() =>
        {
            var directory = TemporaryDirectory / "nswag";
            DeleteDirectory(directory);
            NuGetTasks.NuGet($"install Nswag.Commands -OutputDirectory {directory} -ExcludeVersion -Verbosity detailed");
            NSwagSpecificationGenerator.WriteSpecifications(x => x
                .SetOutputFolder(SpecificationsDirectory)
                .SetPackageFolder(directory));
        });

    Target GenerateDocFX => _ => _
        .Executes(() =>
        {
            NuGetTasks.NuGet(
                $"install docfx.console -OutputDirectory {TemporaryDirectory / "docfx"} -ExcludeVersion -DependencyVersion Ignore -Verbosity detailed");
            DocFXSpecificationGenerator.WriteSpecifications(x => x
                .SetOutputFolder(SpecificationsDirectory)
                .SetOverwriteFile(BuildProjectDirectory / "CodeGeneration" / "DocFX" / "DocFX.yml")
                .SetPackageFolder(TemporaryDirectory / ""));
        });

    Target GenerateDocker => _ => _
        .Executes(() =>
        {
            var directory = TemporaryDirectory / "docker";
            DeleteDirectory(directory);
            var repository = "https://github.com/docker/docker.github.io.git";
            var branch = "master";
            Git($"clone {repository} -b {branch} --single-branch --depth 1 {directory}");

            var reference = Git($"rev-parse --short {branch}", directory).Single().Text;

            var settings = new CodeGeneration.Docker.SpecificationGeneratorSettings
                           {
                               CommandsToSkip = new[]
                                                {
                                                    "docker_container_cp",
                                                    "docker_cp"
                                                },
                               OutputFolder = SpecificationsDirectory,
                               Reference = reference,
                               DefinitionFolder = directory / "_data" / "engine-cli"
                           };

            CodeGeneration.Docker.SpecificationGenerator.GenerateSpecifications(settings);
        });

    Target GenerateKubernetes => _ => _
        .Executes(() =>
        {
            var directory = TemporaryDirectory / "kubernetes";
            DeleteDirectory(directory);

            var branch = "master";
            Git($"clone --depth 1 --single-branch --branch {branch} https://github.com/kubernetes/kubernetes {directory}");
            Git($"apply {BuildProjectDirectory / "CodeGeneration" / "Kubernetes" / "yaml-kubernetes.patch"}", directory);

            ProcessTasks.StartProcess("bash", directory / "hack" / "generate-docs.sh", directory);

            var settings = new CodeGeneration.Kubernetes.SpecificationGeneratorSettings
                           {
                               DefinitionFolder = directory / "docs" / "yaml" / "kubectl",
                               OutputFolder = SpecificationsDirectory,
                               OverwriteFile = BuildProjectDirectory / "CodeGeneration" / "Kubernetes" / "Kubernetes.yml"
                           };
            CodeGeneration.Kubernetes.SpecificationGenerator.GenerateSpecifications(settings);
        });

    Target GenerateHelm => _ => _
        .Executes(() =>
        {
            var DefinitionRepositoryPath = TemporaryDirectory / "helm";
            if (Directory.Exists(DefinitionRepositoryPath))
                FileSystemTasks.DeleteDirectory(DefinitionRepositoryPath);

            const string repository = "https://github.com/helm/helm";
            Git($"clone --depth 1 --single-branch --branch {HelmReleaseTag} {repository} {DefinitionRepositoryPath}");

            var settings = new SpecificationGeneratorSettings
                           {
                               DefinitionFolder = DefinitionRepositoryPath / "docs" / "helm",
                               OutputFolder = SourceDirectory / "Nuke.Helm",
                               OverwriteFile = SourceDirectory / "Nuke.Helm" / "Helm.yml"
                           };
            SpecificationGenerator.GenerateSpecifications(settings);
        });

    Target GenerateSpecifications => _ => _
        .Executes(() =>
        {
        });

    Target Generate => _ => _
        .Executes(() =>
        {
            GenerateSchema<Nuke.CodeGeneration.Model.Tool>(
                ToolSchemaFile,
                GitRepository.GetGitHubDownloadUrl(ToolSchemaFile, MasterBranch),
                "Tool specification schema file by NUKE");

            GenerateCode(
                SpecificationsDirectory,
                outputFileProvider: x => GenerationDirectory / x.Name / x.DefaultOutputFileName,
                namespaceProvider: x => $"Nuke.Common.Tools.{x.Name}",
                sourceFileProvider: x => GitRepository.SetBranch(MasterBranch).GetGitHubBrowseUrl(x.SpecificationFile));
        });
}
