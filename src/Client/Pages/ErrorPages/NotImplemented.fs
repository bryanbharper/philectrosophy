module Client.Pages.NotImplemented

open Client.Styles

let private headerMsg = "Oh hi... I wasn't expecting you."

let private bodyMsg =
    "Yea, I haven't quite finished this yet so... if you don't mind just turning around and pretending you didn't see this, I'd really appreciate it."

let render =
    BaseError.render Bulma.IsPrimary headerMsg bodyMsg "https://media.giphy.com/media/OMeGDxdAsMPzW/giphy.gif"
