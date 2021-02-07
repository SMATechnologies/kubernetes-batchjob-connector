using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace KubeJobs
{
    public class JobSettingsBuilder : IJobSettingsBuilder
    {
        private JobSettings _settings;

        public IConfiguration Config { get; set; }

        public JobSettingsBuilder(IConfiguration config)
        {
            Config = config;
        }

        public IJobSettings Build()
        {
            _settings = new JobSettings
            {
                KubeConfig = Config["config"],
                Completions = int.TryParse(Config["pods-to-complete"], out var comp) ? comp : (int?) null,
                Parallelism = int.TryParse(Config["parallel-executions"], out var parallel) ? parallel : (int?) null,
                Image = Config["image"],
                Command = Config["command"]?.Split(' ').ToList(),
                Args = Config["arguments"] == null ? null : new List<string> {Config["arguments"]},
                Name = Config["name"],
                ContainerName = Config["container-name"]
            };

            if (string.IsNullOrWhiteSpace(_settings.KubeConfig) || string.IsNullOrWhiteSpace(_settings.Image)
                || string.IsNullOrWhiteSpace(_settings.Name))
            {
                throw new ArgumentNullException("config-or-image-or-name", "Kube config, image and job name can't be null");
            }
            return _settings;
        }
    }

    public interface IJobSettingsBuilder
    {
        IJobSettings Build();
    }
}
