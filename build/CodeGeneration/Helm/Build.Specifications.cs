// // Copyright 2019 Maintainers and Contributors of NUKE.
// // Distributed under the MIT License.
// // https://github.com/nuke-build/nuke/blob/master/LICENSE
//
// using System.IO;
// using Nuke.Common;
// using Nuke.Common.IO;
// using static Nuke.Common.Tools.Git.GitTasks;
//
// namespace CodeGeneration.Helm
// {
//     partial class Build
//     {
//         PathConstruction.AbsolutePath DefinitionRepositoryPath => NukeBuild.TemporaryDirectory / "definition-repository";
//
//         [Parameter("The tag of the Helm release to generate the specifications for")] readonly string HelmReleaseTag;
//
//         Target Specifications => _ => _
//             .DependentFor(Generate)
//             .Requires(() => HelmReleaseTag)
//             .Executes(() =>
//             {
//                 if (Directory.Exists(DefinitionRepositoryPath))
//                     FileSystemTasks.DeleteDirectory(DefinitionRepositoryPath);
//
//                 const string repository = "https://github.com/helm/helm";
//                 Git($"clone --depth 1 --single-branch --branch {HelmReleaseTag} {repository} {DefinitionRepositoryPath}");
//
//                 var settings = new SpecificationGeneratorSettings
//                 {
//                     DefinitionFolder = DefinitionRepositoryPath / "docs" / "helm",
//                     OutputFolder = SourceDirectory / "Nuke.Helm",
//                     OverwriteFile = SourceDirectory / "Nuke.Helm" / "Helm.yml"
//                 };
//                 SpecificationGenerator.GenerateSpecifications(settings);
//             });
//     }
// }
