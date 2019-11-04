// Learn more about F# at http://fsharp.org

open System

let consulEnvironment = 
    Environment.GetEnvironmentVariables()
    |> Seq.cast<System.Collections.DictionaryEntry>
    |> Seq.map (fun d -> d.Key :?> string, d.Value :?> string)
    |> Seq.where (fun (k,_) -> k.StartsWith("CONSUL_"))
    |> dict

[<EntryPoint>]
let main argv =
    
    for configurationKey in consulEnvironment do
        printfn "Key %A Value %A" configurationKey.Key configurationKey.Value
    printfn "Hello World from F#!"
    0 // return an integer exit code
