// Copyright Matthias Koch, Sebastian Karasek 2018.
// Distributed under the MIT License.
// https://github.com/nuke-build/kubernetes/blob/master/LICENSE

using Nuke.CodeGeneration.Model;

namespace CodeGeneration.Kubernetes.Overwrite
{
    internal class AdditionalProperty : Property
    {
        public Position Position { get; set; }
    }
}
