## Introduction

I've been on the hunt for a new job. In preparation for technical interviews, I've been going over common interview problems (like the ones in this [list](https://www.teamblind.com/post/New-Year-Gift---Curated-List-of-Top-75-LeetCode-Questions-to-Save-Your-Time-OaM1orEU)).

Unfortunately, the vast majority of explanations and solutions to these problems are written in an imperative style. If you know me at all, you know that I'm a functional programming fanatic. So, I thought it'd be fun and useful to find functional solutions to these problems. This will be the first post in a series doing just that.

## Two Sum
Let's start with a simple one. The "Two Sum" problem (found on LeetCode [here](https://leetcode.com/problems/two-sum/)) is as follows:

> Given a collection of integers called `nums`, and an integer called `target`, find the indices of two numbers in `nums` whose sum is `target`.
> * Every input will have exactly one solution.
> * You may not use the same element twice.
>
> **Example**
>
> ```fsharp
> let nums = [ 6; 5; 19; 7; 3 ]
> let target = 12
> twoSum target nums    // returns (1, 3)
> ```
> **Output:** (1, 3)

## Brute Force Solution
We'll start with a brute force solution as often it's easier to grok. Then we'll take any insight we gain and try to develop a more performant solution.

The first thing that occurs to me is that we could utilize the [Cartesian product](https://en.wikipedia.org/wiki/Cartesian_product) of `nums` with itself to get all possible pairs of integers. Then it's simply a matter of finding which pair sums to `target`. F# has a built in function `List.allPairs` that produces the Cartesian product of two lists. Let's use this and give the problem a first pass:

```fsharp
let twoSum target nums =
    nums
    |> List.allPairs nums
    |> List.find (fun (a, b) -> a + b = target)
```

There are two issues with this attempt. First, we are returning the two summands themselves when we should be returning their indices. Additionally, we are doing nothing to ensure that we don't use the same element twice --- if you consider the example above, the first element, 6, could be used with itself as a solution. So, what if we first created an indexed version of `nums` and _then_ took its product?


```fsharp
let twoSum target nums =
    let indexed =
        List.indexed nums

    indexed
    |> List.allPairs indexed
    |> List.find (fun ((i, a), (j, b)) -> i <> j && a + b = target)
    |> fun ((i, _), (j, _)) -> i, j

```
This is a valid solution to the question prompt. But how performant is it? What if `nums` were very large? Would we be able to find a solution in a reasonable amount of time?

#### Complexity Analysis
Imagine for a second implementing a function that produces a Cartesian product, such as `List.allPairs`. It would look something like this:

```fsharp
let product listA listB =
    [
        for a in listA do
            for b in listB ->
                a, b
    ]
```

If [IMATH]m[/IMATH] is the length of `listA`, and [IMATH]n[/IMATH] is the length of `listB`, then the time complexity of `product` is [IMATH]O(n * m)[/IMATH]. In our case `listA` and `listB` are the same length, so the time complexity is [IMATH]O(n^2)[/IMATH]. Not ideal. Since our brute force solution uses this function, it's worth looking for something better.

## Optimized Solution
What if, for every element in `nums` we computed what integer _would_ sum with the current element to `target`? We could keep track of these _would be_ summands in a `Map` along with the current index. Then, when we encounter this complementary summand later, we know we've found the elements needed for our sum.

```fsharp
let twoSum (target: int) (nums: int list) =
    let rec findComplement map i =
        let comp = target - nums.[i]                            // Complement is target minus current element

        if map |> Map.containsKey nums.[i]                      // Base Case; we've already found the complement
        then
            map |> Map.find nums.[i], i                         // Return answer
        else
            findComplement (map |> Map.add comp i) (i + 1)     // keep searching with updated map and index

    findComplement Map.empty 0

```

With this solution, there are no nested recursions or loops. Instead we simply pass through `nums` once --- in the worst case scenario we don't find our complement until the end. So, the time complexity is [IMATH]O(n)[/IMATH]. Significantly better.
