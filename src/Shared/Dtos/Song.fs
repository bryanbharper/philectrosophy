namespace Shared.Dtos

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

