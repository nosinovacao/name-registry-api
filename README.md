# NAME Registry API: Central Registration Server for [NAME](https://github.com/nosinovacao/name-sdk)
[![Travis Build Status](https://travis-ci.org/nosinovacao/name-registry-api.svg?branch=master)](https://travis-ci.org/nosinovacao/name-registry-api)
[![AppVeyor Build status](https://ci.appveyor.com/api/projects/status/urejv3w2ir1l8rkh/branch/master?svg=true)](https://ci.appveyor.com/project/nosinovacao/name-registry-api/branch/master)

NAME Registry is the service designed to provide Central Registration for applications running the NAME SDK. It fully implements the [Central Registration protocol](https://github.com/nosinovacao/name-sdk/wiki/Central-Registration).

Its goal is to provide users with information regarding the lifecycle of services and its dependencies in a queriable, machine-friendly way.

The Registry API keeps the services in three fundamental entities:

* **Registered Service:** Unique representation of an application in a specific machine and version. This is kept across service restarts.
* **Service Session:** Represents the state of a service since it was started. E.g. when a service restarts a new session is created.
* **Manifest Snapshot:** Represents the manifest of a service session at a certain time. 

### Main Features
* Zero dependencies
* Services storage
* Service lifecycle information
* Manifest snapshots
* Cross-platform
* Query APIs

You can use our [existing Web UI solution](https://github.com/nosinovacao/name-registry-ui) to access the data in a human-friendly way.

## Getting Started
### Running on docker
The Registry API is provided as a docker image, to spin up a container listening on the port 80 use the following command.
```bash
docker run -d -p 80:5000 nosinovacao/name-registry-api
```

Since the API uses [LiteDB](http://www.litedb.org/) to keep state, when the container is deleted the data will be lost, to keep the data consider mapping the database file in a volume.
```bash
docker run -d \
    -e "ConnectionStrings:RegistryLiteDB=/data/db/name-registry-api/Registry.db" \
    -v /data/name-registry-api/db/:/data/name-registry-api/db/ \
    -p 80:5000 \
    nosinovacao/name-registry-api
```

The environment variable `ConnectionStrings:RegistryLiteDB` sets the path of the database file.

## Building and testing
We define our build using [Cake](https://github.com/cake-build/cake/), this allows us to define a common ground for developers on different operating systems, but it requires .Net 4.5 or Mono 4.2.3, so make sure you have those dependencies setup.

Bootstrap scripts are provided for both Windows and Linux environments.

To build and run unit tests on Windows execute the command:

```cmd
powershell ./build.ps1
```

To build and run unit tests on Linux execute the command:

```bash
./build.sh
```

## Contributing
We really appreciate your interest in contributing to the NAME Registry API. 👍

All we ask is that you follow some simple guidelines, so please read the [CONTRIBUTING.md](CONTRIBUTING.md) for details on our code of conduct, and the process for submitting pull requests.

Thank you, [contributors](https://github.com/nosinovacao/name-registry-api/graphs/contributors)!

## License
Copyright © NOS Inovação.

This project is licensed under the BSD 3-Clause License - see the [LICENSE](LICENSE) file for details.