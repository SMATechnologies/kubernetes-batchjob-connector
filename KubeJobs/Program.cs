using System;
using System.Threading;
using Microsoft.Extensions.Configuration;

namespace KubeJobs
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var config = GetConfig(args);

            try
            {
                var jobSettings = new JobSettingsBuilder(config).Build();

                var jobStarter = new JobStarter(jobSettings);

                var kubeJob = jobStarter.StartAsync().Result;

                WaitForJobCompletion(kubeJob);

                GetJobOutput(jobSettings, kubeJob);

                var exitCode = kubeJob.HasSucceeded() ? 0 : 1;

                kubeJob.Delete();

                return exitCode;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return -1;
            }
        }

        private static void GetJobOutput(IJobSettings jobSettings, IKubeJob kubeJob)
        {
            Console.WriteLine(jobSettings.ToString());
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine(kubeJob.GetOutputAsync().Result);
        }

        private static void WaitForJobCompletion(IKubeJob kubeJob)
        {
            var count = 60;
            while (count > 0 && !kubeJob.IsCompleted())
            {
                Thread.Sleep(5000);
                count--;
            }
            if (count == 0) Console.WriteLine("Pods still running after 5 minutes. Terminating them...");
        }

        private static IConfiguration GetConfig(string[] args)
        {
            var builder = new ConfigurationBuilder();
            builder.AddCommandLine(args);
            return builder.Build();
        }
    }
}
