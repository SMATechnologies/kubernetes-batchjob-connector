using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using k8s;
using k8s.Models;

namespace KubeJobs
{
    public class KubeJob : IKubeJob
    {
        private IKubernetes _client;

        public string Name { get; set; }

        public string ControllerUid { get; set; }

        public IList<V1Pod> Pods { get; set; }

        public KubeJob(IKubernetes client)
        {
            _client = client;
        }

        public bool IsCompleted()
        {
            var jobs = _client.ListNamespacedJob("default");
            var job = jobs.Items.FirstOrDefault(x => x.Metadata.Name == Name);
            if (job == null) return false;

            var succeeded = job.Status.Succeeded ?? 0;
            var failed = job.Status.Failed ?? 0;
            var completions = job.Spec.Completions ?? job.Spec.Parallelism ?? 1;

            return succeeded + failed == completions;
        }

        public bool HasSucceeded()
        {
            var jobs = _client.ListNamespacedJob("default");
            var job = jobs.Items.FirstOrDefault(x => x.Metadata.Name == Name);
            if (job == null) return false;

            var succeeded = job.Status.Succeeded ?? 0;
            var completions = job.Spec.Completions ?? job.Spec.Parallelism ?? 1;
            return succeeded == completions;
        }

        public async Task<string> GetOutputAsync()
        {
            Stream output;
            var builder = new StringBuilder();
            GetPods();
            foreach (var pod in Pods)
            {
                output = await _client.ReadNamespacedPodLogAsync(pod.Name(), "default");
                builder.AppendLine($"Output from pod: {pod.Name()}");
                builder.AppendLine();
                builder.AppendLine(await new StreamReader(output).ReadToEndAsync());
                builder.AppendLine();
                builder.AppendLine();
            }
            return builder.ToString();
        }

        public void Delete()
        {
            _client.DeleteNamespacedJob(Name, "default", propagationPolicy:"Foreground");
        }

        private void GetPods()
        {
            var podsCollection = _client.ListNamespacedPod("default", labelSelector: $"controller-uid={ControllerUid}");
            Pods = podsCollection.Items;
        }
    }

    public interface IKubeJob
    {
        string Name { get; set; }

        bool IsCompleted();

        bool HasSucceeded();

        Task<string> GetOutputAsync();

        void Delete();
    }
}
