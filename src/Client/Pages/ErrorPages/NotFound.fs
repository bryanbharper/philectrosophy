module Client.Pages.NotFound

open Client.Styles

let private headerMsg = "We lost you there..."

let private bodyMsg =
    "Sorry, whatever it is you're looking for -- meaning, love, a snack -- it's not here. But keep on searching! We won't stop you :)"

let render =
    BaseError.render Bulma.IsWarning headerMsg bodyMsg "https://media.giphy.com/media/12zV7u6Bh0vHpu/giphy.gif"
