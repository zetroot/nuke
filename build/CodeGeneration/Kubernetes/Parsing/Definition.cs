// Copyright Matthias Koch, Sebastian Karasek 2018.
// Distributed under the MIT License.
// https://github.com/nuke-build/kubernetes/blob/master/LICENSE

using System.Diagnostics;
using Newtonsoft.Json;

namespace CodeGeneration.Kubernetes.Parsing
{
    [DebuggerDisplay("{" + nameof(Name) + "}")]
    internal class Definition
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("usage")]
        public string Usage { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("options")]
        public Option[] Options { get; set; } = new Option[0];

        [JsonProperty("inherited_options")]
        public Option[] InheritedOptions { get; set; } = new Option[0];

        [JsonProperty("see_also")]
        public string[] SeeAlso { get; set; } = new string[0];
    }
}
