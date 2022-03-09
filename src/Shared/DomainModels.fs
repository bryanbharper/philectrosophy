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
    let empty =
        {
            Author = ""
            CreatedOn = DateTime.UtcNow
            IsPublished = false
            Slug = ""
            Synopsis = ""
            Tags = ""
            ThumbNailUrl = ""
            Title = ""
            Subtitle = "" |> Some
            UpdatedOn = None
            ViewCount = 0
        }

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
    let setTitle title entry = { entry with Title = title }
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
        Note: string option
        PlayCount: int
        IsPublished: bool
    }

module Song =
    let create title =
        {
           Slug = sprintf "slug-%s" title
           Title = title
           Placement = 0
           Path = sprintf "songs/%s.mp3" title
           Note = None
           PlayCount = 0
           IsPublished = false
        }
