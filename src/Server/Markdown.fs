module Server.Markdown

open System.Text.RegularExpressions
open Shared

let displayPatternTemplate keyword =
    sprintf "\[%s\](.*?)\[\/%s\]" keyword keyword

let displayKeyword = "MATH"
let displayOpenTag = sprintf "[%s]" displayKeyword
let displayCloseTag = sprintf "[/%s]" displayKeyword
let displayPattern = displayPatternTemplate displayKeyword
let urlStart = "https://latex.codecogs.com/gif.latex?"


let inlineKeyword = "IMATH"
let inlineOpenTag = sprintf "[%s]" inlineKeyword
let inlineCloseTag = sprintf "[/%s]" inlineKeyword
let inlinePattern = displayPatternTemplate inlineKeyword
let inlineUrlStart = urlStart + "%5Cinline%20%5Csmall%20"

let image url = sprintf "![equation](%s)" url

module Latex =
    let encodedImage prefix input =
        input |> String.urlEncode |> (+) prefix |> image

    let displayMathImage = encodedImage urlStart

    let inlineMathImage = encodedImage inlineUrlStart

    let convert pattern converter input =
        Regex.Matches(input, pattern)
        |> Seq.map (fun m -> m.Value, m.Groups.[1].Value |> converter)
        |> Seq.fold (fun acc (m, v) -> acc |> String.replace m v) input

    let convertDisplayMath = convert displayPattern displayMathImage

    let convertInlineMath = convert inlinePattern inlineMathImage

    let convertMath = convertDisplayMath >> convertInlineMath
