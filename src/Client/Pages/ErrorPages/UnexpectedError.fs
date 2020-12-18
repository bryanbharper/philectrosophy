module Client.Pages.UnexpectedError

open Client.Styles

let private headerMsg = "Well this is embarrassing..."

let private bodyMsg = """
Looks like we've had a bit of a hiccup. Perhaps the internet gnomes are on strike again...
Feel free to let us know and we'll try to wrangle them up.
"""

let render =
    BaseError.render Bulma.IsDanger headerMsg bodyMsg "https://media.giphy.com/media/xT0GqsnPlTt0OvKati/giphy.gif"
