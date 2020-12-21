module Server.Tests.Markdown


open FsCheck
open Server
open Expecto
open Shared

let properties = testList "String Property Tests" [

    testProperty "Markdown.image: wraps provided url in Markdown image syntax."
    <| fun url ->
        // act
        let result = Markdown.image url

        // assert
        result = (sprintf "![equation](%s)" url)

    testProperty "Markdown.Latex.encodedImage: encodes, prefixes, and wraps in markdown image syntax"
    <| fun (NonNull url) prefix ->
        // act
        let result = Markdown.Latex.encodedImage prefix url

        // assert
        result = (sprintf "![equation](%s%s)" prefix (String.urlEncode url))

    testProperty "Markdown.Latex.mathImage: prefixes CodeCogs URL"
    <| fun (NonNull url) ->
        // act
        let result = Markdown.Latex.displayMathImage url

        // assert
        result = (sprintf "![equation](%s%s)" Markdown.urlStart (String.urlEncode url))

    testProperty "Markdown.Latex.inlineMathImage: prefixes CodeCogs URL with inline prefix"
    <| fun (NonNull url) ->
        // act
        let result = Markdown.Latex.inlineMathImage url

        // assert
        result = (sprintf "![equation](%s%s)" Markdown.inlineUrlStart (String.urlEncode url))

    ]

let all =
    testList
        "Latex Tests"
        [

            properties

            testCase "Markdown.Latex.replaceMath: replaces string of only [MATH]"
            <| fun _ ->
                // arrange
                let expression =
                    @"V_c \equiv V_{trig} M AT [H] \equiv V_{thresh} = L"

                let input =
                    Markdown.displayOpenTag + expression + Markdown.displayCloseTag

                let expected = Markdown.Latex.displayMathImage expression

                // act
                let result = Markdown.Latex.convertDisplayMath input

                // assert
                Expect.equal result expected ""

            testCase "Markdown.Latex.replaceMath: replaces multiples [MATH] tags."
            <| fun _ ->
                // arrange
                let expression1 =
                    @"V_c \equiv V_{trig} \equiv V_{thresh} = L"

                let expression2 = @"X^2 \frac{1}{2} \overline{taco}"

                let template =
                    sprintf "Blah blah blah %s. Blah blah blah %s blah blah!"

                let input =
                    template
                        (Markdown.displayOpenTag + expression1 + Markdown.displayCloseTag)
                        (Markdown.displayOpenTag + expression2 + Markdown.displayCloseTag)

                let expected =
                    template (Markdown.Latex.displayMathImage expression1) (Markdown.Latex.displayMathImage expression2)

                // act
                let result = Markdown.Latex.convertDisplayMath input

                // assert
                Expect.equal result expected ""

            testCase "Markdown.Latex.replaceMath: ignores [iMATH] tags."
            <| fun _ ->
                // arrange
                let expression1 =
                    @"V_c \equiv V_{trig} \equiv V_{thresh} = L"

                let expression2 = @"X^2 \frac{1}{2} \overline{taco}"

                let template =
                    sprintf "Blah blah blah %s. Blah blah blah %s blah blah!"

                let input =
                    template
                        (Markdown.displayOpenTag + expression1 + Markdown.displayCloseTag)
                        (Markdown.inlineOpenTag
                         + expression2
                         + Markdown.inlineOpenTag)

                let expected =
                    template
                        (Markdown.Latex.displayMathImage expression1)
                        (Markdown.inlineOpenTag
                         + expression2
                         + Markdown.inlineOpenTag)

                // act
                let result = Markdown.Latex.convertDisplayMath input

                // assert
                Expect.equal result expected ""

            testCase "Markdown.Latex.replaceInlineMath: replaces string of only [iMATH]"
            <| fun _ ->
                // arrange
                let expression = @"X^2 \frac{1}{2} IMATH [] \overline{taco}"

                let input =
                    Markdown.inlineOpenTag
                    + expression
                    + Markdown.inlineCloseTag

                let expected =
                    Markdown.Latex.inlineMathImage expression

                // act
                let result = Markdown.Latex.convertInlineMath input

                // assert
                Expect.equal result expected ""

            testCase "Markdown.Latex.replaceInlineMath: replaces multiples [iMATH] tags."
            <| fun _ ->
                // arrange
                let expression1 =
                    @"V_c \equiv V_{trig} \equiv V_{thresh} = L"

                let expression2 = @"X^2 \frac{1}{2} \overline{taco}"

                let template =
                    sprintf "Blah blah blah %s. Blah blah blah %s blah blah!"

                let input =
                    template
                        (Markdown.inlineOpenTag
                         + expression1
                         + Markdown.inlineCloseTag)
                        (Markdown.inlineOpenTag
                         + expression2
                         + Markdown.inlineCloseTag)

                let expected =
                    template (Markdown.Latex.inlineMathImage expression1) (Markdown.Latex.inlineMathImage expression2)

                // act
                let result = Markdown.Latex.convertInlineMath input

                // assert
                Expect.equal result expected ""

            testCase "Markdown.Latex.replaceInlineMath: ignores [MATH] tags."
            <| fun _ ->
                // arrange
                let expression1 =
                    @"V_c \equiv V_{trig} \equiv V_{thresh} = L"

                let expression2 = @"X^2 \frac{1}{2} \overline{taco}"

                let template =
                    sprintf "Blah blah blah %s. Blah blah blah %s blah blah!"

                let input =
                    template
                        (Markdown.displayOpenTag + expression1 + Markdown.displayCloseTag)
                        (Markdown.inlineOpenTag
                         + expression2
                         + Markdown.inlineCloseTag)

                let expected =
                    template
                        (Markdown.displayOpenTag + expression1 + Markdown.displayCloseTag)
                        (Markdown.Latex.inlineMathImage expression2)

                // act
                let result = Markdown.Latex.convertInlineMath input

                // assert
                Expect.equal result expected ""

            testCase "Markdown.Latex.replaceAllMath: replaces both inline and normal math tags."
            <| fun _ ->
                // arrange
                let expression1 =
                    @"V_c \equiv V_{trig} \equiv V_{thresh} = L"

                let expression2 = @"X^2 \frac{1}{2} \overline{taco}"

                let template =
                    sprintf "Blah blah blah %s. Blah blah blah %s blah blah!"

                let input =
                    template
                        (Markdown.displayOpenTag + expression1 + Markdown.displayCloseTag)
                        (Markdown.inlineOpenTag
                         + expression2
                         + Markdown.inlineCloseTag)

                let expected =
                    template
                        (Markdown.Latex.displayMathImage expression1)
                        (Markdown.Latex.inlineMathImage expression2)

                // act
                let result = Markdown.Latex.convertMath input

                // assert
                Expect.equal result expected ""

            testCase "Markdown.Latex.replace: replaces all instances of pattern using replacer"
            <| fun _ ->
                // arrange
                let patternText = "dogs are cool"
                let pattern = sprintf "(%s)" patternText
                let converter = String.slugify

                let inputPattern x = sprintf "Some stuff blah %s and other stuff %s" x x
                let input = inputPattern patternText

                // act
                let result = Markdown.Latex.convert pattern converter input

                // assert
                Expect.equal result (inputPattern (String.slugify pattern)) ""

        ]
