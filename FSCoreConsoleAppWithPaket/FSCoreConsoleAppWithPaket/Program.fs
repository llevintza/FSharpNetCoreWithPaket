open System
open System.Text
open Consul

let consulKey = Environment.GetEnvironmentVariable "CONSUL_KAFKA_CONFIG_KEY"

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
            | (kvPair) -> Some(Encoding.UTF8.GetString(kvPair.Value))
            | _ -> None
        | _ -> None 

    let logMessage = 
        match responseResult with
        | Some(x) -> sprintf "Successfully read the configurations from consul with value %s" x
        | None -> sprintf "Failed to read consul key %s" key

    printfn "[TRACE] - %s" logMessage

    return responseResult
}

[<EntryPoint>]
let main argv =
    
    let config = 
        loadKafkaConfigurations consulKey
        |> Async.RunSynchronously

    let log = 
        match config with
        | Some validConfig -> sprintf "Kafka configuration is %s" validConfig
        | None -> sprintf "Could not retrieve Kafka configurations from consul"

    printfn "[TRACE] - %s" log
    0 // return an integer exit code
