// Copyright Matthias Koch, Sebastian Karasek 2018.
// Distributed under the MIT License.
// https://github.com/nuke-build/kubernetes/blob/master/LICENSE

using System;
using System.Collections.Generic;

namespace CodeGeneration.Kubernetes.Parsing
{
    internal class TypeResolver
    {
        public Dictionary<Option, string> EnumerationOptionTypes { get; set; }

        public TypeReference GetType(Option option)
        {
            if (EnumerationOptionTypes == null)
                throw new InvalidOperationException($"{nameof(EnumerationOptionTypes)} has to be set before types can be resolved.");

            if (EnumerationOptionTypes.TryGetValue(option, out var enumerationName))
                return new TypeReference { Type = enumerationName };

            var reference = new TypeReference();
            switch (option.Type)
            {
                case "Level":
                    reference.Type = "int";
                    break;
                case "bool":
                    reference.Type = "bool";
                    break;
                case "duration":
                    reference.Type = "TimeSpan?";
                    break;
                case "int":
                    reference.Type = "int";
                    break;
                case "int32":
                    reference.Type = "int";
                    break;
                case "int64":
                    reference.Type = "long?";
                    break;
                case "moduleSpec":
                    reference.Type = "Dictionary<string,string>";
                    reference.ItemFormat = "{key}={value}";
                    reference.Separator = ',';
                    break;
                case "severity":
                    reference.Type = "int";
                    break;
                case "string":
                    reference.Type = "string";
                    break;
                case "stringArray":
                    reference.Type = "List<string>";
                    reference.Separator = null;
                    break;
                case "stringSlice":
                    reference.Type = "List<string>";
                    reference.Separator = ',';
                    break;
                case "traceLocation":
                    reference.Type = "string";
                    break;
                default:
                    throw new NotImplementedException($"Type {option.Type} in {option.Definition.Name}.{option.Name} is not implemented yet.");
            }

            return reference;
        }
    }
}
