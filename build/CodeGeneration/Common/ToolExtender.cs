using System;
using System.Collections.Generic;
using System.Linq;
using Nuke.CodeGeneration.Generators;
using Nuke.CodeGeneration.Model;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Utilities;
using Nuke.Common.Utilities.Collections;

namespace CodeGeneration.Common
{
    public abstract class ToolExtender
    {
        public void Extend(Tool tool)
        {
            FileSystemTasks.EnsureCleanDirectory(WorkingDirectory);

            Prepare();

            tool.Tasks.AddRange(GetTasks());
            tool.Tasks.ForEach(HandleTask);

            tool.DataClasses.AddRange(GetDataClasses());
            tool.DataClasses.ForEach(HandleDataClass);

            tool.Enumerations.AddRange(GetEnumerations());
            tool.Enumerations.ForEach(HandleEnumeration);

            void CheckCount<T>(IReadOnlyCollection<T> collection, Func<T, string> keySelector)
            {
                var lookup = collection.ToLookup(keySelector, x => x);
                var duplicated = lookup.Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                ControlFlow.Assert(duplicated.Count == 0,
                    new[] {$"Found duplicated {typeof(T).Name} entries:"}
                        .Concat($"  - {duplicated}")
                        .JoinNewLine());
            }

            CheckCount(tool.Tasks, x => x.GetTaskMethodName());
            CheckCount(tool.DataClasses, x => x.Name);
            CheckCount(tool.Enumerations, x => x.Name);
        }

        public string WorkingDirectory => NukeBuild.TemporaryDirectory / GetType().Name;

        public virtual void Prepare()
        {
        }

        protected virtual IEnumerable<Task> GetTasks() => new Task[0];
        protected virtual IEnumerable<DataClass> GetDataClasses() => new DataClass[0];
        protected virtual IEnumerable<Enumeration> GetEnumerations() => new Enumeration[0];

        protected virtual void HandleTask(Task task)
        {
        }

        protected virtual void HandleDataClass(DataClass dataClass)
        {
        }

        protected virtual void HandleEnumeration(Enumeration enumeration)
        {
        }
    }
}