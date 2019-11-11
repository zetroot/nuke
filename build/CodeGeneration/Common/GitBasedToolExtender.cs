// Copyright 2019 Maintainers of NUKE.
// Distributed under the MIT License.
// https://github.com/nuke-build/nuke/blob/master/LICENSE

using System;
using System.Linq;
using Nuke.CodeGeneration.Model;
using static Nuke.Common.Tools.Git.GitTasks;
using static Nuke.Common.Tools.NuGet.NuGetTasks;

namespace CodeGeneration.Common
{
    public abstract class GitBasedToolExtender : ToolExtender
    {
        public override void Prepare()
        {
            Git($"clone --depth 1 --single-branch --branch {CommitSha} {GitRepository} {WorkingDirectory}");
        }

        public abstract string GitRepository { get; }

        public abstract string CommitSha { get; }

        public abstract void Extend(Tool tool, string repositoryDirectory);
    }
}
