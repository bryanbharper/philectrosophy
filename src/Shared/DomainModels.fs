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
        Subtitle: string
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
            Subtitle = "SUB " + title
            UpdatedOn = None
        }

    let setSubtitle subTitle entry = { entry with Subtitle = subTitle }
    let setThumbNail url entry = { entry with ThumbNailUrl = url }
    let setCreatedOn date entry = { entry with CreatedOn = date }
    let setSynopsis synopsis entry = { entry with Synopsis = synopsis }
    let setUpdatedOn dateOption entry = { entry with UpdatedOn = dateOption }
