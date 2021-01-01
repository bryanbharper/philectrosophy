namespace Shared

open System

type BlogEntry =
    {
        Author: string
        CreatedOn: DateTime
        IsPublished: bool
        Slug: string
        Synopsis: string
        Tags: string option
        ThumbNailUrl: string
        Title: string
        Subtitle: string
        UpdatedOn: DateTime option
    }

module BlogEntry =

    let create title =
        {
            Author = "Bryan B. Harper"
            CreatedOn = DateTime.UtcNow
            IsPublished = true
            Slug = String.slugify title
            Synopsis = sprintf "A blog about: %s" title
            Tags = None
            ThumbNailUrl = "https://picsum.photos/100/100"
            Title = title
            Subtitle = "SUB " + title
            UpdatedOn = None
        }

    let setCreatedOn date entry = { entry with CreatedOn = date }
    let setIsPublished isPublished entry = { entry with IsPublished = isPublished }
    let setSubtitle subTitle entry = { entry with Subtitle = subTitle }
    let setSynopsis synopsis entry = { entry with Synopsis = synopsis }
    let setThumbNail url entry = { entry with ThumbNailUrl = url }
    let setUpdatedOn dateOption entry = { entry with UpdatedOn = dateOption }
