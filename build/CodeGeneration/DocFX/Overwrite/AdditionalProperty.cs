// Copyright Sebastian Karasek, Matthias Koch 2018.
// Distributed under the MIT License.
// https://github.com/nuke-build/docfx/blob/master/LICENSE

using Nuke.CodeGeneration.Model;

namespace CodeGeneration.DocFX.Overwrite
{
    internal class AdditionalProperty : Property
    {
        public Position Position { get; set; }
    }
}
