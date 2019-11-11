using System;
using System.Linq;

namespace CodeGeneration.Common
{
    public class NSwagExtender : NuGetBasedToolExtender
    {
        public override string PackageId => "Nswag.Commands";
    }
}