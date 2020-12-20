module Shared.String

let contains (contained: string) (container: string) =
    container.IndexOf contained <> -1

let trim (str: string) =
    str.Trim()

let ofChars (chars: seq<char>) =
    chars |> Seq.map string |> Seq.fold (+) ""

let split (delimiter: char) (str: string) = str.Split delimiter

let replace (oldVal: string) (newVal: string) (str: string) =
    str.Replace(oldVal, newVal)

let strip (stripChars: string) (str: string): string =
    let removeChar (str: string) (char': char) =
        str.Replace(char' |> string, "")

    Seq.fold removeChar str stripChars

let slugify (phrase: string) =
    phrase.ToLower()
    |> trim
    |> strip "!\"#$%&'()*+,./:;?@[\]^_`{|}~"
    |> replace " " "-"
    |> replace "--" "-"

let urlEncode value =
    System.Uri.EscapeDataString value
