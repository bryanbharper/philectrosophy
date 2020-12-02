namespace Shared

open System

type BlogEntry =
    {
        Author: string
        CreatedOn: DateTime
        Slug: string
        Synopsis: string
        Tags: string option
        ThumbNailUrl: string
        Title: string
        UpdatedOn: DateTime option
    }

module BlogEntry =

    let create title =
        {
            Author = "Bryan B. Harper"
            CreatedOn = DateTime.UtcNow
            Slug = String.slugify title
            Synopsis = sprintf "A blog about: %s" title
            Tags = None
            ThumbNailUrl = "https://picsum.photos/100/100"
            Title = title
            UpdatedOn = None
        }
