// Copyright Matthias Koch, Sebastian Karasek 2018.
// Distributed under the MIT License.
// https://github.com/nuke-build/kubernetes/blob/master/LICENSE

namespace CodeGeneration.Helm
{
    public class SpecificationGeneratorSettings
    {
        public string DefinitionFolder { get; set; }
        public string OutputFolder { get; set; }
        public string Reference { get; set; } = "master";
        public string OverwriteFile { get; set; }
    }
}
