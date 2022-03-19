namespace Shared.Extensions

open System

type MonthInt =
    private
    | MInt of int
    static member create n =
        n % 12
        |> Math.Abs
        |> (fun i -> if i = 0 then 12 else i)
        |> MInt

    member m.ToInt32() = let (MInt i) = m in i

type DayInt =
    private
    | DInt of int
    static member create n =
        n % 31
        |> Math.Abs
        |> (fun i -> if i = 0 then 31 else i)
        |> DInt

    member d.ToInt32() = let (DInt i) = d in i

module Date =
    let monthName (mInt: MonthInt): string =
        match mInt with
        | MInt 1 -> "January"
        | MInt 2 -> "February "
        | MInt 3 -> "March"
        | MInt 4 -> "April"
        | MInt 5 -> "May"
        | MInt 6 -> "June"
        | MInt 7 -> "July"
        | MInt 8 -> "August"
        | MInt 9 -> "September"
        | MInt 10 -> "October"
        | MInt 11 -> "November"
        | MInt 12 -> "December"
        | _ -> failwith "mInt must be a value from 1-12"

    let daySuffix (dInt: DayInt): string =
        let (DInt i) = dInt

        match i with
        | 1
        | 21
        | 31 -> "st"
        | 2
        | 22 -> "nd"
        | 3
        | 23 -> "rd"
        | _ -> "th"

    let format (date: DateTime): string =
        let month =
            date.Month |> MonthInt.create |> monthName

        let daySuffix = date.Day |> DayInt.create |> daySuffix

        sprintf "%i%s %s %i" date.Day daySuffix month date.Year
