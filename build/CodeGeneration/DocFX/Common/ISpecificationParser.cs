// Copyright Sebastian Karasek, Matthias Koch 2018.
// Distributed under the MIT License.
// https://github.com/nuke-build/docfx/blob/master/LICENSE

using System;
using Nuke.CodeGeneration.Model;

namespace CodeGeneration.DocFX.Common
{
    public interface ISpecificationParser : IDisposable
    {
        void Populate(Tool tool);
    }
}
