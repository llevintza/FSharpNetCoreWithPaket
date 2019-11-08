//open System
//open System.Text
//open Consul


[<EntryPoint>]
let main argv =
    
    let config = 
        KafkaConfig.loadConfig
        |> Async.RunSynchronously

    let log = 
        match config with
        | Some validConfig -> sprintf "Kafka configuration is %A" validConfig
        | None -> sprintf "Could not retrieve Kafka configurations from consul"

    printfn "[TRACE] - %s" log
    0 // return an integer exit code
