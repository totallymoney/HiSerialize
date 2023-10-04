module ExampleOne



// Lambda dotnet serialization docs
// https://docs.aws.amazon.com/lambda/latest/dg/csharp-handler.html#csharp-handler-serializer-reflection


open System.IO
open System.Text.Json
open Amazon.Lambda.Core


type MySerializer() =

    // member this.Serialize(res, stream) =
    //     (this :> ILambdaSerializer).Serialize(res, stream)

    // member this.Deserialize(stream) =
    //     (this :> ILambdaSerializer).Deserialize(stream)

    interface ILambdaSerializer with

        member this.Serialize(res, stream: Stream) : unit =
            use writer = new Utf8JsonWriter(stream)
            JsonSerializer.Serialize(writer, res)

        member this.Deserialize<'T>(stream: Stream) : 'T =
            JsonSerializer.Deserialize<'T> stream




type MyDto = { Name: string; Size: int }

[<LambdaSerializer(typeof<MySerializer>)>]
let ExampleOne (event: MyDto) =
    printfn "the event is:\n\n%A" event
    event


(*
yarn gsda-dev-deploy nick
*)

(*
yarn sls invoke -s nick -f ExampleOne -d '{ "Name": "Bob", "Size": 2 }'
*)


// C# interface example
// https://sharplab.io/#v2:CYLg1APgAgzABASwHYBcCmAnAZgQwMZpwCSAsAFADe5cNcUALHABoAUUAjAAxwDOAlAG5yAX3LlYdAExwAwnBDE4VMrTrwGzNl158lossKA=


// F# interface example
// https://sharplab.io/#v2:DYLgZgzgNAJiDUAfALgTwA4FMAEBJbAvALABQ252AhgEYTIBOlAxstgBojYSd30CWAOwDm2ALQA+bAFUBfZKQUk0WbAGEAFAEpCpCtkHJM9MMxz4A7nIAWuvRQC2me9SPZkVvhAB0bddy4MgkLaBNhaukA==


// F# calling interface methods docs
// https://learn.microsoft.com/en-us/dotnet/fsharp/language-reference/interfaces#calling-interface-methods
