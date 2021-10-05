namespace Client.Extensions

module List =
    let rand l =
      let rnd = System.Random()
      l |> List.item (rnd.Next(l.Length))
