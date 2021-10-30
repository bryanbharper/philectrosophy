namespace Shared

open System

type BlogEntry =
    {
        Author: string
        CreatedOn: DateTime
        IsPublished: bool
        Slug: string
        Synopsis: string
        Subtitle: string option
        Tags: string
        ThumbNailUrl: string
        Title: string
        UpdatedOn: DateTime option
        ViewCount: int
    }

module BlogEntry =
    let create title =
        {
            Author = "Bryan B. Harper"
            CreatedOn = DateTime.UtcNow
            IsPublished = true
            Slug = String.slugify title
            Synopsis = sprintf "A blog about: %s" title
            Tags = ""
            ThumbNailUrl = "https://picsum.photos/100/100"
            Title = title
            Subtitle = "SUB " + title |> Some
            UpdatedOn = None
            ViewCount = 0
        }

    let setCreatedOn date entry = { entry with CreatedOn = date }
    let setIsPublished isPublished entry = { entry with IsPublished = isPublished }
    let setSubtitle subTitle entry = { entry with Subtitle = subTitle }
    let setSynopsis synopsis entry = { entry with Synopsis = synopsis }
    let setTags tags entry = { entry with Tags = tags }
    let setThumbNail url entry = { entry with ThumbNailUrl = url }
    let setUpdatedOn dateOption entry = { entry with UpdatedOn = dateOption }

type Song =
    {
        Slug: string
        Title: string
        Placement: int
        Path: string
        CoverOf: string option
        PlayCount: int
    }
