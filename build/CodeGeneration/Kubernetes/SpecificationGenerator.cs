// Copyright Matthias Koch, Sebastian Karasek 2018.
// Distributed under the MIT License.
// https://github.com/nuke-build/kubernetes/blob/master/LICENSE

using System;
using System.IO;
using CodeGeneration.Kubernetes.Overwrite;
using CodeGeneration.Kubernetes.Parsing;
using Nuke.CodeGeneration;
using Nuke.Common.IO;

namespace CodeGeneration.Kubernetes
{
    public class SpecificationGenerator
    {
        public static void GenerateSpecifications(SpecificationGeneratorSettings settings)
        {
            Console.WriteLine("Generating kubectl specifications...");

            var definitions = DefinitionLoader.LoadDefinitions(settings.DefinitionFolder);
            var tool = DefinitionParser.Parse(definitions, settings.Reference);

            var overwrites = settings.OverwriteFile != null ? DefinitionLoader.LoadOverwrites(settings.OverwriteFile) : null;


            SpecificationModifier.Overwrite(tool, overwrites);

            tool.SpecificationFile = PathConstruction.Combine(settings.OutputFolder, "Kubernetes.json");
            Directory.CreateDirectory(settings.OutputFolder);
            ToolSerializer.Save(tool, tool.SpecificationFile);

            Console.WriteLine();
            Console.WriteLine("Generation finished.");
            Console.WriteLine($"Created Tasks: {tool.Tasks.Count}");
            Console.WriteLine($"Created Data Classes: {tool.DataClasses.Count}");
            Console.WriteLine($"Created Enumerations: {tool.Enumerations.Count}");
            Console.WriteLine($"Created Common Task Properties: {tool.CommonTaskProperties.Count}");
            Console.WriteLine($"Created Common Task Property Sets: {tool.CommonTaskPropertySets.Count}");
        }
    }
}
