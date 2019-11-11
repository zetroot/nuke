// Copyright Sebastian Karasek, Matthias Koch 2018.
// Distributed under the MIT License.
// https://github.com/nuke-build/docker/blob/master/LICENSE

namespace CodeGeneration.Docker
{
    public class SpecificationGeneratorSettings
    {
        public string OutputFolder { get; set; }
        public string Reference { get; set; } = "master";
        public string[] CommandsToSkip { get; set; }
        public string DefinitionFolder { get; set; }
    }
}