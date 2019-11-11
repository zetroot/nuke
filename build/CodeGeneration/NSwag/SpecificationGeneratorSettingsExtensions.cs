// Copyright Sebastian Karasek, Matthias Koch 2018.
// Distributed under the MIT License.
// https://github.com/nuke-build/nswag/blob/master/LICENSE

namespace CodeGeneration.NSwag
{
    public static class SpecificationGeneratorSettingsExtensions
    {
        public static T SetOutputFolder<T> (this T settings, string outputFolder)
                where T : SpecificationGeneratorSettings
        {
            settings.OutputFolder = outputFolder;
            return settings;
        }
    }
}