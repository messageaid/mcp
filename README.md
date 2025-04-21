# Broker MCP

This is the [Message Aid MCP server](https://messageaid.com/docs/reference/mcp). The goal is to support the three main brokers 
of Message Aid, RabbitMQ, Azure Service Bus, and SQS.

| Action      | Rabbit MQ | Azure Service Bus | SQS     |
|-------------|-----------|-------------------|---------|
| List Queues | Y         | Planned           | Planned |
| Purge Queue | Y         | Planned           | Planned | 


## Usage

### Via Docker

```sh
docker run -i --rm \
  --env 'BROKER_URL=rabbitmq://guest:guest@localhost:15672/' \
  ghcr.io/messageaid/mcp
```

### Sample Json Config for Cursor, etc

```json
{
  "mcpServers": {
    "messageAid": {
      "command": "docker",
      "args": [
        "run", 
        "-i", 
        "--rm", 
        "--env 'BROKER_URL=rabbitmq://guest:guest@localhost:15672/'",
        "docker pull ghcr.io/messageaid/mcp:latest"]
    }
  }
}
```

## Configuration

| Env Var    | ...            |
|------------|----------------|
| BROKER_URL |                |
| MCP_MODE   | ReadOnly       | 
|            | MessageAllowed | 
|            | BrokerAllowed  | 

## Building

```sh
docker build -t ghcr.io/messageaid/mcp -f Dockerfile .
```

## Licence

This MCP server is licensed under the BSL 1.1 License. For more details,
see the LICENSE file in the project repository.