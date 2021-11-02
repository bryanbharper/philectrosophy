module Shared.Math

module Operators =

    /// <summary>
    /// Returns modulus of positive or negative n values
    /// </summary>
    /// <param name="n">n</param>
    /// <param name="m">m</param>
    let (-%) n m =
        match n % m with
        | i when i >= 0 -> i
        | i -> abs m + i
