//open System
//open System.Text
//open Consul


[<EntryPoint>]
let main argv =
    
    let config = 
        KafkaConfig.loadConfig
        |> Async.RunSynchronously

    printfn "[TRACE] - %A" config
    0 // return an integer exit code
