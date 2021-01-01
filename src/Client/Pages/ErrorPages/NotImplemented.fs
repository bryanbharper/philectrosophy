module Client.Pages.NotImplemented

open Client.Styles

let render =
    BaseError.render
        Bulma.IsPrimary
        "Oh hi... I wasn't expecting you."
        "Yea, I haven't quite finished this yet so... if you don't mind just turning around and pretending you didn't see this, I'd really appreciate it."
        "https://media.giphy.com/media/OMeGDxdAsMPzW/giphy.gif"
