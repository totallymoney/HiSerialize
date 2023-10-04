module ExampleTwo


open System.IO
open System.Text.Json
open Amazon.Lambda.Core
open Amazon.Lambda.SQSEvents


type TypicalDto = { Name: string; Size: int }

[<LambdaSerializer(typeof<ExampleOne.MySerializer>)>]
let ``A typical SQS event handler`` (event: SQSEvent) =

    let msgs: TypicalDto list =
        event.Records
        |> List.ofSeq
        |> List.map (fun r -> JsonSerializer.Deserialize<TypicalDto> r.Body)

    printfn "the messages are:\n\n%A" msgs

    ()




open type Amazon.Lambda.SQSEvents.SQSEvent



type SQSEventSerializer() =

    member this.Serialize(res, stream) =
        System.Runtime.CompilerServices.Unsafe
            .As<ILambdaSerializer>(this)
            .Serialize(res, stream)

        (this :> ILambdaSerializer).Serialize(res, stream)

    member this.Deserialize(stream) =
        (this :> ILambdaSerializer).Deserialize(stream)

    interface ILambdaSerializer with

        member this.Serialize(res, stream: Stream) : unit =
            use writer = new Utf8JsonWriter(stream)
            JsonSerializer.Serialize(writer, res)

        member this.Deserialize<'T>(stream: Stream) : 'T =
            let sqsEvent: SQSEvent = JsonSerializer.Deserialize<SQSEvent> stream

            use stream = new MemoryStream()
            use writer = new Utf8JsonWriter(stream)
            let parse (m: SQSMessage) = JsonDocument.Parse(m.Body).RootElement
            let write (e: JsonElement) = e.WriteTo writer

            writer.WriteStartArray()
            sqsEvent.Records |> Seq.iter (parse >> write)
            writer.WriteEndArray()

            writer.Flush()
            stream.Position <- 0
            JsonSerializer.Deserialize<'T> stream





type MySqsMsg = { Name: string; Size: int }


[<LambdaSerializer(typeof<SQSEventSerializer>)>]
let ExampleTwo (msgs: MySqsMsg list) =
    printfn "the messages are:\n\n%A" msgs
    msgs




(*
yarn sls invoke -s nick -f ExampleTwo -p ./example-two-input.json
*)
