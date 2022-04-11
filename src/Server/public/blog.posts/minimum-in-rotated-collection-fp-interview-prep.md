## Introduction

I've been on the hunt for a new job. In preparation for technical interviews, I've been going over common interview problems (like the ones in this [list](https://www.teamblind.com/post/New-Year-Gift---Curated-List-of-Top-75-LeetCode-Questions-to-Save-Your-Time-OaM1orEU)).

Unfortunately, the vast majority of explanations and solutions to these problems are written in an imperative style. If you know me at all, you know that I'm a functional programming fanatic. So, I thought it'd be fun and useful to find functional solutions to these problems. This is the second post in a series doing just that.

## Find Minimum of Rotated, Sorted Collection
Here's the problem as stated on [here on LeetCode](https://leetcode.com/problems/find-minimum-in-rotated-sorted-array/):

> Suppose you have a collection of integers called `nums` of length `n` that is **rotated** between 1 and `n` times. For example:
> ```fsharp
> let unrotated = [ 0; 1; 2; 3; 4; 5 ]
> let nums      = [ 3; 4; 5; 0; 1; 2 ]
> ```
> Additionally you know:
> * The original collection was sorted (ascending)
> * The members of the collection are all distinct
>
> Write a function that finds the minimum element in `nums` (e.g., in the example above return 0). **The function must run in** [IMATH] O\left(\log{(n)}\right) [/IMATH] **time**.

# Solution
There are a few things that stick out about this problem:
* The collection is (kinda) sorted
* The task is to _search_ for a certain element
* The function must have a time complexity of [IMATH] O\left(\log{(n)}\right) [/IMATH]

This sounds a whole lot like [binary search](https://en.wikipedia.org/wiki/Binary_search_algorithm)! Let's look at an implementation of _binary search_ and see how we can modify it to solve our problem.

#### Binary Search
```fsharp
let binSearch (target: int) (source: int list) =
    let rec search lo hi =
        if lo > hi
        then None
        else
            let mid = (lo + hi) / 2

            match compare target source.[mid] with
            | 0 -> Some mid
            | 1 -> search (mid + 1) hi
            | _ -> search lo (mid - 1)

    search 0 (List.length source - 1)
```

There are a few key differences between binary search and our function:
* We are guaranteed to have a minimum element, so we don't need to return an `option`
* Instead of searching for an element _equal_ to the target, we need to find an element that meets one of two conditions
  * The element to the left is greater than it
  * The element to the right is less than it
* Our function returns the minimum itself, not its index

With all that in mind let's try to modify `binSearch` to meet our needs. Here's what we'll do differently:
* Instead of comparing `mid` to `target`, we'll compare `mid` to the elements to the `left` and `right` of it.
* Since we don't care about the index, instead of recursing with new indices, we'll simply recurse with a `slice` of the collection.
* When neither `mid` nor `right` is the minimum, to determine whether to search the upper or lower half of the collection, we'll compare `mid` to the last element of `slice`. If the last element is larger, then we know the point of rotation happened before `mid`. Otherwise it happened after.

```fsharp
let findMin nums =
    let rec search slice =
        match slice with
        | [ singleton ] -> singleton
        | _ ->
            let midIdx = (List.length slice - 1) / 2

            match slice.[midIdx - 1], slice.[midIdx], slice.[midIdx + 1] with
            | left, mid, _  when   mid < left -> mid
            | _, mid, right when right < mid  -> right
            | _, mid, _ ->
                if (slice |> List.last) > mid
                then search slice.[..midIdx - 1]
                else search slice.[midIdx + 1..]

    search nums
```

While the details of the implementation differ, this solution is essentially a _binary search_, and so we know it will run in [IMATH] O\left(\log{(n)}\right) [/IMATH] time.
