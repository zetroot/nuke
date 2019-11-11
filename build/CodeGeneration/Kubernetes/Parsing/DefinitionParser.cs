// Copyright Matthias Koch, Sebastian Karasek 2018.
// Distributed under the MIT License.
// https://github.com/nuke-build/kubernetes/blob/master/LICENSE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CodeGeneration.Kubernetes.Utilities;
using JetBrains.Annotations;
using Nuke.CodeGeneration.Model;

namespace CodeGeneration.Kubernetes.Parsing
{
    internal class DefinitionParser
    {
        private readonly IReadOnlyCollection<Definition> _definitions;
        private readonly string _reference;
        private readonly TypeResolver _typeResolver;
        private readonly Tool _tool;

        private DefinitionParser(IReadOnlyCollection<Definition> definitions, string reference)
        {
            _definitions = definitions;
            _reference = reference;
            _tool = CreateTool();
            _typeResolver = new TypeResolver();
        }

        public static Tool Parse(IReadOnlyCollection<Definition> definitions, string reference)
        {
            var parser = new DefinitionParser(definitions, reference);
            parser.PopulateEnumerations();
            parser.PopulateCommonSettings();
            parser.PopulateTasks();
            return parser._tool;
        }

        private Task ParseTask(Definition definition)
        {
            var dataClass = CreateSettingsClass();
            dataClass.Properties = definition.InheritedOptions.Length != 0
                ? definition.Options.Select(x => CreateProperty(x, dataClass)).ToList()
                : new List<Property>();
            var task = CreateTask(definition, dataClass);

            return task;
        }

        private Property ParseProperty(Option option)
        {
            var typeReference = _typeResolver.GetType(option);
            return new Property
                   {
                       Name = option.Name.ToPascalCase(separator: '-'),
                       Type = typeReference.Type,
                       Help = option.Usage.FormatForXmlDoc(),
                       Format = $"--{option.Name}={{value}}",
                       ItemFormat = typeReference.ItemFormat,
                       Separator = typeReference.Separator
                   };
        }

        private void PopulateEnumerations()
        {
            var regex = new Regex(@"[Oo]ne of:? (?>([\w-]+(?>\|[\w-]+)+)|\[([\w-]+(?> [\w-]+)+)\])");
            var enumerationOptionPairs = _definitions.SelectMany(x => x.Options.Concat(x.InheritedOptions))
                .Select(x => new { Option = x, Match = regex.Match(x.Usage) })
                .Where(x => x.Match.Success)
                .Select(x =>
                {
                    var value = x.Match.Groups[groupnum: 1].Value;
                    if (string.IsNullOrWhiteSpace(value))
                        value = x.Match.Groups[groupnum: 2].Value;

                    var values = value.Split(new[] { '|', ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    var name = "Kubernetes" + x.Option.Definition.Name.ToPascalCase('_', '-') + x.Option.Name.ToPascalCase('_', '-');
                    var enumeration = new Enumeration
                                      {
                                          Name = name,
                                          Tool = _tool,
                                          Values = values
                                      };
                    return new
                           {
                               Enumeration = enumeration,
                               x.Option
                           };
                })
                .ToArray();
            _typeResolver.EnumerationOptionTypes = enumerationOptionPairs.ToDictionary(x => x.Option, x => x.Enumeration.Name);

            _tool.Enumerations = enumerationOptionPairs
                .Select(x => x.Enumeration)
                .ToList();
        }

        private void PopulateTasks()
        {
            _tool.Tasks = _definitions
                .Select(ParseTask)
                .ToList();
        }

        private void PopulateCommonSettings()
        {
            var dataClass = new DataClass
                            {
                                Name = "KubernetesCommonSettings",
                                Tool = _tool,
                                ExtensionMethods = true
                            };
            dataClass.Properties = _definitions
                .Single(x => !x.InheritedOptions.Any())
                .Options.Select(x => CreateProperty(x, dataClass))
                .ToList();
            _tool.DataClasses.Add(dataClass);
        }

        private Tool CreateTool()
        {
            return new Tool
                   {
                       Name = "Kubernetes",
                       PathExecutable = "kubectl",
                       OfficialUrl = "https://kubernetes.io/",
                       References = _definitions
                           .Select(x => x.Name)
                           .Where(x => x != "kubectl").Concat(new[] { "help" })
                           .Select(x => $"https://raw.githubusercontent.com/kubernetes/kubernetes/{_reference}/pkg/kubectl/cmd/{x}.go")
                           .OrderBy(x => x)
                           .ToList()
                   };
        }

        private SettingsClass CreateSettingsClass()
        {
            return new SettingsClass
                   {
                       Tool = _tool,
                       BaseClass = "KubernetesToolSettings"
                   };
        }

        private Task CreateTask(Definition definition, SettingsClass settingsClass)
        {
            var isRootTask = definition.Usage == "kubectl";
            var task = new Task
                       {
                           Postfix = isRootTask ? null : definition.Name.ToPascalCase(separator: '-'),
                           Help = definition.Description.FormatForXmlDoc(),
                           DefiniteArgument = definition.Name,
                           Tool = _tool,
                           SettingsClass = settingsClass
                       };
            settingsClass.Task = task;
            return task;
        }

        private Property CreateProperty(Option option, [CanBeNull] DataClass dataClass)
        {
            var property = ParseProperty(option);
            property.DataClass = dataClass;
            return property;
        }
    }
}
