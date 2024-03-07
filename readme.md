
Demo Repo for [discussion 4994](https://github.com/MassTransit/MassTransit/discussions/4994)

Project is entirely hosted in docker containers and run from docker compose.
State Machines are configured to leave the sagas in the Redis repository after finalization, to demonstrate behavior.

The project demonstrates that when each State Machine (including the Job State Machine) is defined with a separate Redis repository instance, only the first defined instance is used.

While the behavior is known when dependency injecting an `IConnectionMultiplexer` the thought was that overriding `ConnectionFactory` would allow for multiple Redis instances.

To prove that all Redis instance are reachable by the bus, simply alternate the first configured Redis connection with any of the three connection strings.

Steps to reproduce:

``` bash
# run the project; a single message will be passed to primary saga, branch sage, and to an IJobConsumer
docker compose --project-name discussion_4994 up --detach --build

# list the keys found in each saga repository
# only one repository will have all the sagas for primary, branch and job
docker run -i --rm redis:7.0 /usr/local/bin/redis-cli -h host.docker.internal -p 7001 KEYS '*'
docker run -i --rm redis:7.0 /usr/local/bin/redis-cli -h host.docker.internal -p 7002 KEYS '*'
docker run -i --rm redis:7.0 /usr/local/bin/redis-cli -h host.docker.internal -p 7003 KEYS '*'

# since sagas are left in-place, delete them after each run
docker run -i --rm redis:7.0 /usr/local/bin/redis-cli -h host.docker.internal -p 7001 FLUSHALL && \
docker run -i --rm redis:7.0 /usr/local/bin/redis-cli -h host.docker.internal -p 7002 FLUSHALL && \
docker run -i --rm redis:7.0 /usr/local/bin/redis-cli -h host.docker.internal -p 7003 FLUSHALL

# bring down the project
docker compose --project-name discussion_4994 down --volumes
```
