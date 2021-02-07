using System.Collections.Generic;
using System.Threading.Tasks;
using k8s;
using k8s.Models;

namespace KubeJobs
{
    public class JobStarter : IKubeJobStarter
    {
        public IJobSettings Settings { get; set; }

        private IKubernetes _client;

        public JobStarter(IJobSettings settings)
        {
            Settings = settings;
            var kubeConfig = KubernetesClientConfiguration.BuildConfigFromConfigFile(Settings.KubeConfig);
            _client = new Kubernetes(kubeConfig);
        }

        public async Task<IKubeJob> StartAsync()
        {
            var job = GetJobObject();
            job = await _client.CreateNamespacedJobAsync(job, "default");
            return new KubeJob(_client) {Name = job.Name(), ControllerUid = job.Metadata.Labels["controller-uid"]};
        }

        private V1Job GetJobObject()
        {
            return new V1Job
            {
                ApiVersion = "batch/v1",
                Kind = "Job",
                Metadata = new V1ObjectMeta
                {
                    Name = Settings.Name
                },
                Spec = new V1JobSpec
                {
                    Template = new V1PodTemplateSpec
                    {
                        Spec = new V1PodSpec
                        {
                            Containers = new List<V1Container>
                            {
                                new V1Container
                                {
                                    Name = Settings.ContainerName,
                                    Image = Settings.Image,
                                    Command = Settings.Command,
                                    Args = Settings.Args
                                }
                            },
                            RestartPolicy = "OnFailure"
                        }
                    },
                    Parallelism = Settings.Parallelism,
                    Completions = Settings.Completions
                }
            };
        }
    }

    public interface IKubeJobStarter
    {
        IJobSettings Settings { get; set; }

        Task<IKubeJob> StartAsync();
    }
}
