# Kubernetes Batch Job Connector

This application is a connector to a Kubernetes Cluster and allows users to run batch processes in containers within a Kubernetes environment. All initalization and cleanup is done by the connector making it easy to run batch processes as part of a workflow. The connector can run on Windows or Linux systems.

# Disclaimer

No Support and No Warranty are provided by SMA Technologies for this project and related material. The use of this project's files is on your own risk.

SMA Technologies assumes no liability for damage caused by the usage of any of the files offered here via this Github repository.

# Prerequisites

- Before using the connector you must have a cloud Kubernetes Service already configured and running. E.g. [Sample for Azure](https://docs.microsoft.com/en-us/azure/aks/kubernetes-walkthrough-portal).
- You should also have connected to it, so you have a Kubernetes config file with the cluster/user/context/token information in it (typically created at /Users/\<userid\>/.kube/config). An example of how to do that for Azure is [here](https://docs.microsoft.com/en-us/cli/azure/aks?view=azure-cli-latest#az_aks_get_credentials).
- You need to save this config file in some place where the connector can access it.
- You must also have a Docker image that contains the job you need to run uploaded to a public registry like Docker Hub, or even a private one, as long as your cloud Kubernetes Service has been granted privileges to pull images from there. For Azure, you can do that while setting up the Kubernetes Cluster.

# Instructions

## Installation

To get the connector, simply browse to the [latest release](https://github.com/smatechnologies/kubernetes-batchjob-connector/releases/latest) of this repository and look for the assets. If running the connector on Windows, download the ZIP asset. If running on Linux, download the GZip asset. Extract the archive and place the extracted folder on any system with an OpCon agent (Windows or Linux based on what is downloaded).

## Configuration

- Create a new Windows or Linux job in OpCon.
- For Windows, the job's command line should be similar to: C:\MyApps\KubeJobs.exe config=".\kubeconfig\config" image="mycompany/my-job-image" command="my-job-command" arguments="arg1 arg2 arg3" name="my-job" container-name="my-container" pods-to-complete="5" parallel-executions="3"
- For Linux, the job's start image should be similar to: /app/KubeJobs and parameters should be like: config="./app/config" image="mycompany/my-job-image" command="my-job-command" arguments="arg1 arg2 arg3" name="my-job" container-name="my-container" pods-to-complete="5" parallel-executions="3"
- **config:** The Kubernetes config file explained in prerequisites. **Required**
- **image:** The image from the Docker registry that has the job to run. **Required**
- **command:** The command to execute within the image.
- **arguments:** The arguments to the command above.
- **name:** Name of the Kubernetes job. **Required**
- **container-name:** Name of the container when the image is run.
- **pods-to-complete:** Number of times the container must be run to consider the OpCon job to be completed.
- **parallel-executions:** The number of containers that are allowed run concurrently, if using multiple container runs.

## Execution

Build and run the job like any other OpCon job. No other information is required to be maintained in OpCon for the container to run. You can view job output like any other job, and it will fetch output from the container job for each container that is run as part of the job execution. Please note that 'kill job' will kill the connector, not the actual jobs running in the containers. NOTE: If the image in the Docker Registry is updated between job reruns (for the tag used), the connector will pull the latest image and run it, so multiple OpCon job runs may have different results in this case.

# License

Copyright 2019 SMA Technologies

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at [apache.org/licenses/LICENSE-2.0](http://www.apache.org/licenses/LICENSE-2.0)

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

# Contributing

We love contributions, please read our [Contribution Guide](CONTRIBUTING.md) to get started!

# Code of Conduct

[![Contributor Covenant](https://img.shields.io/badge/Contributor%20Covenant-v2.0%20adopted-ff69b4.svg)](code-of-conduct.md)
SMA Technologies has adopted the [Contributor Covenant](CODE_OF_CONDUCT.md) as its Code of Conduct, and we expect project participants to adhere to it. Please read the [full text](CODE_OF_CONDUCT.md) so that you can understand what actions will and will not be tolerated.
