## Introduction

I've been on the hunt for a new job. In preparation for technical interviews, I've been going over common interview problems (like the ones in this [list](https://www.teamblind.com/post/New-Year-Gift---Curated-List-of-Top-75-LeetCode-Questions-to-Save-Your-Time-OaM1orEU)).

Unfortunately, the vast majority of explanations and solutions to these problems are written in an imperative style. If you know me at all, you know that I'm a functional programming fanatic. So, I thought it'd be fun and useful to find functional solutions to these problems. This will be the first post in a series doing just that.

## Longest Unique Substring
Here's the problem as stated on [here on LeetCode](https://leetcode.com/problems/longest-substring-without-repeating-characters/):

> Given a string, `s`, find the length of the longest substring without repeating characters.
>
> **Examples**
>
> ```fsharp
> let s = "abcabcbb"
> longestSubstring s    // returns 3
> ```
> **Output:** 3
>
>
> ```fsharp
> let s = ""
> longestSubstring s    // returns 0
> ```
> **Output:** 0
>
>
> ```fsharp
> let s = "bbbbb"
> longestSubstring s    // returns 1
> ```
> **Output:** 1

## Brute Force Solution
A straight forward approach is to create windows out of the string, from a window size of 1 to `String.length s`. Then, with all the possible sub-windows in hand, filter for windows with all distinct elements. Finally, find the largest sub-window remaining.

First we'll want to create a function that determines whether a collection contains all unique elements. For this we can make use of a `Set`:
```fsharp
let allDistinct (collection: 'a seq) =
    collection |> Set.ofSeq |> Set.count = Seq.length collection
```

We'll utilize a `fold` function to iterate through the string and build up a collection of sub-windows:
```fsharp
let longestSubstring (s: string): int =
    let distinctWindows (prev: char list list) winSize =
        let cur =
            s
            |> Seq.toList
            |> List.windowed winSize
            |> List.filter allDistinct

        cur @ prev

    if String.length s <= 0
    then 0
    else
        [ 1 .. String.length s ]
        |> List.fold distinctWindows [  ]
        |> List.maxBy List.length
        |> List.length
```

#### Complexity Analysis
Implementing the `windowed` function would look something like this:[POP]This is not the actual implementation.[/POP]
```fsharp
let windowed size list =
    [
        for i in 0 .. List.length list - size ->
            list.[i .. i + size - 1]
    ]
```

Getting a slice of a list has a time complexity of [IMATH]O(k)[/IMATH] where [IMATH]k[/IMATH] is the size of the slice.[POP]At least, in Python, according to [this stack overflow question](https://stackoverflow.com/questions/13203601/big-o-of-list-slicing). [/POP]. Since these slices are produced in a loop, the time complexity of the `windowed` as a whole is [IMATH]O(n * k)[/IMATH] where [IMATH]n[/IMATH] is the size of `list`. And in our worst case scenario the window and list are the same size, resulting in [IMATH]O(n^2)[/IMATH].

But we're not done.

An implementation of the fold function would look something like this:[POP]This is not the actual implementation.[/POP]
```fsharp
let rec fold folder state list =
    match list with
    | [] -> state
    | head::tail ->
        let newState = folder state head
        fold folder newState tail
```

Which is essentially equivalent to a `for` loop through `list`. So, the time complexity of `fold` is  [IMATH]O(n)[/IMATH].

Putting this all together, since the `windowed` function is called inside of `fold`, the time complexity of `longestSubstring` is [IMATH]O(n^3)[/IMATH]. Pretty terrible...

## Optimized Solution

The optimized solution will also make use of a window, but we'll manage it ourselves by keeping track of the `top` and `bot` (bottom) of the window. Each iteration we'll increment `top`. We'll also keep track of each character we've seen (along with its index) in a `map`. When we encounter a character we've seen before, we'll increase `bot` so that our next window won't contain the repeated character. Finally, we'll keep track of the longest unique sequence so far, `curMax`, which by the end should be our answer.

```fsharp
let longestSubstring (s: string) =
    let chars = s |> Seq.toList

    let rec findMax bot top curMax map =
        if top >= List.length chars
        then
            curMax
        else

            if map |> Map.containsKey chars.[top]
            then
                let bot' = (map |> Map.find chars.[top]) + 1
                let map' = map |> Map.remove chars.[top] |> Map.add chars.[top] top

                findMax bot' (top + 1) curMax map'
            else
                let map' = map |> Map.add chars.[top] top
                let newMax = max curMax (top - bot + 1)
                findMax bot (top + 1) newMax map'

    findMax 0 0 0 Map.empty
```

#### Complexity Analysis
Since we only need to go through the list once, the time complexity is [IMATH]O(n)[/IMATH].
