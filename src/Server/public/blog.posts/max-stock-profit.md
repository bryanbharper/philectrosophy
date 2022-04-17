## Introduction

I've been on the hunt for a new job. In preparation for technical interviews, I've been going over common interview problems (like the ones in this [list](https://www.teamblind.com/post/New-Year-Gift---Curated-List-of-Top-75-LeetCode-Questions-to-Save-Your-Time-OaM1orEU)).

Unfortunately, the vast majority of explanations and solutions to these problems are written in an imperative style. If you know me at all, you know that I'm a functional programming fanatic. So, I thought it'd be fun and useful to find functional solutions to these problems. This is the fourth post in a series doing just that.

## Best Time to Buy and Sell Stock
Here's the problem as stated on [here on LeetCode](https://leetcode.com/problems/best-time-to-buy-and-sell-stock/):

> Let `prices` be an array where `prices[i]` is the price of a given stock on the _ith_ day.
>
> Maximize your profit by finding a day to buy the stock and a future day to sell it. Return that profit.
>
> If you cannot make a profit, return zero.
>
> **Examples**
>
> ```fsharp
> maxProfit [7; 1; 5; 3; 6; 4]
> ```
> **Output:** 5
>
>
> ```fsharp
> maxProfit [7; 6; 4; 3; 1]
> ```
> **Output:** 0


## Solution
Normally I'd start with a brute force solution, as those are often easier to grok. However, in this case, I think the optimized solution is simple enough. In fact, I really don't see the need to even explain it, so here it is:

```fsharp
open System

let maxProfit (prices: int list) =
    let folder (minPrice, maxProfit) price =
        match price - minPrice with
        | profit when profit < 0 ->
            price, maxProfit
        | profit when profit > maxProfit ->
            minPrice, profit
        | _ ->
            minPrice, maxProfit

    prices
    |> List.fold folder (Int32.MaxValue, 0)
    |> snd
```

#### Complexity Analysis
As demonstrated in the [previous post](http://www.philectrosophy.com/blog/longest-unique-substring), the time complexity of a `fold` function (with no nested recursion or loops) is simply [IMATH]O(n)[/IMATH].
