module Client.Pages.NotFound

open Client.Styles

let render =
    BaseError.render
        Bulma.IsWarning
        "We lost you there..."
        "Sorry, whatever it is you're looking for -- meaning, love, a snack -- it's not here. But keep on searching! We won't stop you :)"
        "https://media.giphy.com/media/12zV7u6Bh0vHpu/giphy.gif"
