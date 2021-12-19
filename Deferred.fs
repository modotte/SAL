namespace SAL

open DomainModel

[<RequireQualifiedAccess>]
/// Contains utility functions to work with value of the type `Deferred<'T>`.
module Deferred =
    
    /// Returns whether the `Deferred<'T>` value has been resolved or not.
    let resolved = function
        | HasNotStartedYet -> false
        | InProgress -> false
        | Resolved _ -> true

    /// Returns whether the `Deferred<'T>` value is in progress or not.
    let inProgress = function
        | HasNotStartedYet -> false
        | InProgress -> true
        | Resolved _ -> false

    /// Transforms the underlying value of the input deferred value when it exists from type to another
    let map (transform: 'a -> 'b) (deferred: Deferred<'a>) : Deferred<'b> =
        match deferred with
        | HasNotStartedYet -> HasNotStartedYet
        | InProgress -> InProgress
        | Resolved value -> Resolved (transform value)

    /// Verifies that a `Deferred<'T>` value is resolved and the resolved data satisfies a given requirement.
    let exists (predicate: 'a -> bool) = function
        | HasNotStartedYet -> false
        | InProgress -> false
        | Resolved value -> predicate value

    /// Like `map` but instead of transforming just the value into another type in the `Resolved` case, it will transform the value into potentially a different case of the the `Deferred<'T>` type.
    let bind (transform: 'a -> Deferred<'b>) (deferred: Deferred<'a>) : Deferred<'b> =
        match deferred with
        | HasNotStartedYet -> HasNotStartedYet
        | InProgress -> InProgress
        | Resolved value -> transform value

