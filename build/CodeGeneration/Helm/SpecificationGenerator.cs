// Copyright Matthias Koch, Sebastian Karasek 2018.
// Distributed under the MIT License.
// https://github.com/nuke-build/kubernetes/blob/master/LICENSE

using System;
using System.IO;
using System.Linq;
using CodeGeneration.Helm.Overwrite;
using CodeGeneration.Helm.Parsing;
using Nuke.CodeGeneration;
using Nuke.Common.IO;
using Nuke.Common.Utilities.Collections;

namespace CodeGeneration.Helm
{
    public class SpecificationGenerator
    {
        public static void GenerateSpecifications(SpecificationGeneratorSettings settings)
        {
            Console.WriteLine("Generating Helm specifications...");

            var definitions = DefinitionLoader.LoadDefinitions(settings.DefinitionFolder);

            definitions.SelectMany(x => x.Options).Concat(definitions.SelectMany(x => x.InheritedOptions)).Select(x => x.Type).ToHashSet()
                .OrderBy(x => x).ForEach(x => Console.WriteLine(x));
            var tool = DefinitionParser.Parse(definitions, settings.Reference);

            var overwrites = settings.OverwriteFile != null ? DefinitionLoader.LoadOverwrites(settings.OverwriteFile) : null;
            if (overwrites != null)
                SpecificationModifier.Overwrite(tool, overwrites);

            tool.SpecificationFile = PathConstruction.Combine(settings.OutputFolder, "Helm.json");
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
