// Copyright Matthias Koch, Sebastian Karasek 2018.
// Distributed under the MIT License.
// https://github.com/nuke-build/kubernetes/blob/master/LICENSE

using System.Collections.Generic;
using Nuke.CodeGeneration.Model;

namespace CodeGeneration.Helm.Overwrite
{
    internal class Overwrite
    {
        public Dictionary<string, AdditionalProperty[]> AdditionalProperties { get; set; } = new Dictionary<string, AdditionalProperty[]>();
        public Enumeration[] Enumerations { get; set; } = new Enumeration[0];
        public Dictionary<string, PropertyOverwrite> PropertyOverwrites { get; set; } = new Dictionary<string, PropertyOverwrite>();
        public Dictionary<string, TaskOverwrite> TaskOverwrites { get; set; } = new Dictionary<string, TaskOverwrite>();
    }
}
