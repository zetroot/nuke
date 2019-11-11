// Copyright Matthias Koch, Sebastian Karasek 2018.
// Distributed under the MIT License.
// https://github.com/nuke-build/kubernetes/blob/master/LICENSE

using System.Diagnostics;
using Newtonsoft.Json;

namespace CodeGeneration.Kubernetes.Parsing
{
    [DebuggerDisplay("{" + nameof(FullName) + "}")]
    internal class Option
    {
        [JsonProperty("default_value")]
        public string DefaultValue { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("usage")]
        public string Usage { get; set; }

        [JsonIgnore]
        public Definition Definition { get; set; }

        [JsonIgnore] public string FullName => Definition.Name + Name;
    }
}
