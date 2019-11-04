// Learn more about F# at http://fsharp.org

open System
open System.Text
open Consul

let consulEnvironmentKeys = 
    Environment.GetEnvironmentVariables()
    |> Seq.cast<System.Collections.DictionaryEntry>
    |> Seq.map (fun d -> d.Key :?> string, d.Value :?> string)
    |> Seq.where (fun (k,_) -> k.StartsWith("CONSUL_"))
    |> dict

(** Unique identifier for this service **)
let serviceGuid = Guid.NewGuid().ToString()
let consulClient = new ConsulClient()
let consulKey = consulEnvironmentKeys.Item("CONSUL_KEY")
(** The Consul session we'll use for setting and releasing locks **)
let session = consulClient.Session.Create(new SessionEntry()) |> Async.AwaitTask |> Async.RunSynchronously 

(** Checks to see if the current node is the leader **)
let getLeaderGuid () = consulClient.KV.Get(consulKey) |> Async.AwaitTask |> Async.RunSynchronously |> fun myKv -> myKv.Response.Value |> Encoding.Default.GetString


let loadKafkaConfigurations () = async {
    (** Console **)
    Console.WriteLine("Trying to load consul config!")

    //(** Handle for the leadership key **)
    let targetpair = new KVPair(consulKey)
    
    (** Did we acquire the lock? **)
    let result = consulClient.KV.Get(consulKey) |> Async.AwaitTask |> Async.RunSynchronously 
    
    
    (** Set the session generated earlier **)
    targetpair.Session <- session.Response
    //targetpair.Value <- System.Text.Encoding.UTF8.GetString(result.Response.Value)
    System.Text.Encoding.UTF8.GetString(result.Response.Value)

    (** Print the result of the election **)
    //match result.Response with 
    //| true  -> Console.WriteLine("I succeeded :o)")
    //| false -> Console.WriteLine("I failed :o(")  
    //targetpair
}

[<EntryPoint>]
let main argv =
    
    loadKafkaConfigurations()
        //|> Async.AwaitTask
        |> Async.RunSynchronously
    printfn "Hello World from F#!"
    0 // return an integer exit code
