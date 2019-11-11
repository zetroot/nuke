// Copyright Sebastian Karasek, Matthias Koch 2018.
// Distributed under the MIT License.
// https://github.com/nuke-build/nswag/blob/master/LICENSE

namespace CodeGeneration.NSwag
{
    public class NSwagSpecificationGeneratorSettings : SpecificationGeneratorSettings
    {
        public string PackageFolder { get; set; }
        public string GitReference { get; set; }
    }
}