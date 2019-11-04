module KafkaConsumer

open FSharp.Control
open Kafunk
open System

let conn = Kafka.connHost "existential-host"

/// Configuration.
let consumerConfig = 
  ConsumerConfig.create (

    /// The name of the consumer group.
    groupId = "consumer-group", 

    /// The topic to consume.
    topic = "absurd-topic")


/// This creates a consumer and joins it to the group.
let consumer =
  Consumer.create conn consumerConfig