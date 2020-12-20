module Server.Markdown

open System.Text.RegularExpressions
open Shared

let keyword = "MATH"
let inlineKeyword = "IMATH"
let openTag = sprintf "[%s]" keyword
let closeTag = sprintf "[/%s]" keyword

let patternTemplate keyword =
    sprintf "\[%s\](.*?)\[\/%s\]" keyword keyword

let pattern = patternTemplate keyword
let inlineOpenTag = sprintf "[%s]" inlineKeyword
let inlineCloseTag = sprintf "[/%s]" inlineKeyword
let inlinePattern = patternTemplate inlineKeyword

let urlStart = "https://latex.codecogs.com/gif.latex?"
let inlineUrlStart = urlStart + "%5Cinline%20%5Csmall%20"

let image url = sprintf "![equation](%s)" url

module Latex =
    let encodedImage prefix input =
        input |> String.urlEncode |> (+) prefix |> image

    let mathImage input = encodedImage urlStart input

    let inlineMathImage input = encodedImage inlineUrlStart input

    let replace pattern converter input =
        Regex.Matches(input, pattern)
        |> Seq.map (fun m -> (m.Value, converter m.Groups.[1].Value))
        |> Seq.fold (fun acc (m, v) -> acc |> String.replace m v) input

    let replaceMath input = replace pattern mathImage input

    let replaceInlineMath input =
        replace inlinePattern inlineMathImage input

    let replaceALlMath = replaceMath >> replaceInlineMath
