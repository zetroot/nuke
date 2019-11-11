using System;
using System.Linq;
using Nuke.Common.Tools.NuGet;

namespace CodeGeneration.Common
{
    public abstract class NuGetBasedToolExtender : ToolExtender
    {
        public abstract string PackageId { get; }

        public override void Prepare()
        {
            NuGetTasks.NuGet($"install {PackageId} -OutputDirectory {WorkingDirectory} -ExcludeVersion");
        }
    }
}