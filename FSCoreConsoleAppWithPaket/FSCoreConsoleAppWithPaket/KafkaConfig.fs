module KafkaConfig

open System
open System.Text

open FSharp.Data

open Consul

let consulKey = Environment.GetEnvironmentVariable "CONSUL_KAFKA_CONFIG_KEY"


type KafkaConfiguration = JsonProvider<"""{
"endpoint": "",
"topics": [
  {
    "name": "main-topic",
    "consumerGroups": [
      "consumerGroup1",
      "consumerGroup2"
    ]
  },
  {
    "name": "secondary-topic",
    "consumerGroups": [
      "consumerGroup1",
      "consumerGroup2"
    ]
  }
]
}
""">

let loadKafkaConfigurations (key:string) = async {
    printfn "[TRACE] - Trying to load consul configurations!"
    use consulClient = new ConsulClient()
    let! queryResult = 
        consulClient.KV.Get(key) 
        |> Async.AwaitTask

    let responseResult =
        match queryResult.StatusCode with
        | Net.HttpStatusCode.OK -> 
            match queryResult.Response with
            | (kvPair) -> Some(KafkaConfiguration.Parse(Encoding.UTF8.GetString(kvPair.Value)))
            | _ -> None
        | _ -> None 

    let logMessage = 
        match responseResult with
        | Some(config) -> sprintf "Successfully read the configurations from consul with value %A" config
        | None -> sprintf "Failed to read consul key %s" key

    printfn "[TRACE] - %s" logMessage

    return responseResult
}

let loadConfig  = async {
    return! loadKafkaConfigurations consulKey
}
