using System.Collections.Generic;
using System.Text;

namespace KubeJobs
{
    public class JobSettings : IJobSettings
    {
        public string KubeConfig { get; set; }

        public int? Completions { get; set; }

        public int? Parallelism { get; set; }

        public string Image { get; set; }

        public IList<string> Command { get; set; }

        public IList<string> Args { get; set; }

        public string Name { get; set; }

        public string ContainerName { get; set; }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine($"Using Docker Image: {Image}");
            builder.AppendLine($"Kubernetes Job Name: {Name}");
            builder.AppendLine($"Container Name: {ContainerName}");
            builder.AppendLine($"Total Number of Pods to Run: {Completions ?? Parallelism ?? 1}");
            builder.AppendLine($"Max Parallel Pods Execution Count: {Parallelism ?? 1}");
            return builder.ToString();
        }
    }

    public interface IJobSettings
    {
        string KubeConfig { get; set; }

        int? Completions { get; set; }

        int? Parallelism { get; set; }

        string Image { get; set; }

        IList<string> Command { get; set; }

        IList<string> Args { get; set; }

        string Name { get; set; }

        string ContainerName { get; set; }
    }
}
