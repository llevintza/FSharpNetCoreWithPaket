module KafkaProducer

open Kafunk
open System


let conn = async {
    let! config = KafkaConfig.loadConfig
    
    return 
        match config with
        | Some(cfg) -> 
            let t = cfg.Endpoint.JsonValue.ToString()
            Kafka.connHost t
        | None -> failwith ""
}

/// Configuration.
let producerConfig = 
  ProducerConfig.create (
    
    /// The topic to produce to.
    topic = "absurd-topic", 

    /// The partition function to use.
    partition = Partitioner.roundRobin,

    /// The required acks setting.
    requiredAcks = RequiredAcks.AllInSync,

    /// The per-broker in-memory buffer size, in bytes.
    bufferSizeBytes = ProducerConfig.DefaultBufferSizeBytes,

    /// The maximum size, in bytes, of an individual produce request.
    batchSizeBytes = ProducerConfig.DefaultBatchSizeBytes,
    
    /// The maximum time to wait for a batch.
    batchLingerMs = ProducerConfig.DefaultBatchLingerMs)


/// Create a producer.
let producer(configOption: Option<KafkaConn>) = async {
    let! connection = conn
    let! p = Producer.createAsync connection producerConfig

    return p
  }


/// Create a message.
let m = 
  ProducerMessage.ofString (
    value = "hello value", 
    key = "hello key")


/// Produce a single message.
let prodRes = 
  Producer.produce producer m 
  |> Async.RunSynchronously

printfn "partition=%i offset=%i" prodRes.partition prodRes.offset