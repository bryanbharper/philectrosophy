module Server.Tests.Markdown


open FsCheck
open Server
open Expecto
open Shared

let properties = testList "Markdown (Property)" [

    testProperty "image: wraps provided url in Markdown image syntax."
    <| fun url ->
        // act
        let result = Markdown.image url

        // assert
        result = (sprintf "![equation](%s)" url)

    testProperty "Latex.encodedImage: encodes, prefixes, and wraps in markdown image syntax"
    <| fun (NonNull url) prefix ->
        // act
        let result = Markdown.Latex.encodedImage prefix url

        // assert
        result = (sprintf "![equation](%s%s)" prefix (String.urlEncode url))

    testProperty "Latex.mathImage: prefixes CodeCogs URL"
    <| fun (NonNull url) ->
        // act
        let result = Markdown.Latex.displayMathImage url

        // assert
        result = (sprintf "![equation](%s%s)" Markdown.codeCogsUrl (String.urlEncode url))

    testProperty "Latex.inlineMathImage: prefixes CodeCogs URL with inline prefix"
    <| fun (NonNull url) ->
        // act
        let result = Markdown.Latex.inlineMathImage url

        // assert
        result = (sprintf "![equation](%s%s)" Markdown.codeCogsUrlInline (String.urlEncode url))

    ]

let all =
    testList
        "Markdown"
        [

            properties

            testCase "convert: converts all instances of pattern using converter"
            <| fun _ ->
                // arrange
                let patternText = "dogs are cool"
                let pattern = sprintf "(%s)" patternText
                let converter = String.slugify

                let inputPattern x = sprintf "Some stuff blah %s and other stuff %s" x x
                let input = inputPattern patternText

                // act
                let result = Markdown.convert pattern converter input

                // assert
                Expect.equal result (inputPattern (String.slugify pattern)) ""

            testCase "Latex.convertDisplayMath: replaces string of only [MATH]"
            <| fun _ ->
                // arrange
                let expression =
                    @"V_c \equiv V_{trig} M AT [H] \equiv V_{thresh} = L"

                let input =
                    Markdown.displayMathOpenTag + expression + Markdown.displayMathCloseTag

                let expected = Markdown.Latex.displayMathImage expression

                // act
                let result = Markdown.Latex.convertDisplayMath input

                // assert
                Expect.equal result expected ""

            testCase "Latex.convertDisplayMath: replaces multiples [MATH] tags."
            <| fun _ ->
                // arrange
                let expression1 =
                    @"V_c \equiv V_{trig} \equiv V_{thresh} = L"

                let expression2 = @"X^2 \frac{1}{2} \overline{taco}"

                let template =
                    sprintf "Blah blah blah %s. Blah blah blah %s blah blah!"

                let input =
                    template
                        (Markdown.displayMathOpenTag + expression1 + Markdown.displayMathCloseTag)
                        (Markdown.displayMathOpenTag + expression2 + Markdown.displayMathCloseTag)

                let expected =
                    template (Markdown.Latex.displayMathImage expression1) (Markdown.Latex.displayMathImage expression2)

                // act
                let result = Markdown.Latex.convertDisplayMath input

                // assert
                Expect.equal result expected ""

            testCase "Latex.convertDisplayMath: ignores [IMATH] tags."
            <| fun _ ->
                // arrange
                let expression1 =
                    @"V_c \equiv V_{trig} \equiv V_{thresh} = L"

                let expression2 = @"X^2 \frac{1}{2} \overline{taco}"

                let template =
                    sprintf "Blah blah blah %s. Blah blah blah %s blah blah!"

                let input =
                    template
                        (Markdown.displayMathOpenTag + expression1 + Markdown.displayMathCloseTag)
                        (Markdown.inlineMathOpenTag
                         + expression2
                         + Markdown.inlineMathOpenTag)

                let expected =
                    template
                        (Markdown.Latex.displayMathImage expression1)
                        (Markdown.inlineMathOpenTag
                         + expression2
                         + Markdown.inlineMathOpenTag)

                // act
                let result = Markdown.Latex.convertDisplayMath input

                // assert
                Expect.equal result expected ""

            testCase "Latex.convertInlineMath: replaces string of only [IMATH]"
            <| fun _ ->
                // arrange
                let expression = @"X^2 \frac{1}{2} IMATH [] \overline{taco}"

                let input =
                    Markdown.inlineMathOpenTag
                    + expression
                    + Markdown.inlineMathCloseTag

                let expected =
                    Markdown.Latex.inlineMathImage expression

                // act
                let result = Markdown.Latex.convertInlineMath input

                // assert
                Expect.equal result expected ""

            testCase "Latex.convertInlineMath: replaces multiples [IMATH] tags."
            <| fun _ ->
                // arrange
                let expression1 =
                    @"V_c \equiv V_{trig} \equiv V_{thresh} = L"

                let expression2 = @"X^2 \frac{1}{2} \overline{taco}"

                let template =
                    sprintf "Blah blah blah %s. Blah blah blah %s blah blah!"

                let input =
                    template
                        (Markdown.inlineMathOpenTag
                         + expression1
                         + Markdown.inlineMathCloseTag)
                        (Markdown.inlineMathOpenTag
                         + expression2
                         + Markdown.inlineMathCloseTag)

                let expected =
                    template (Markdown.Latex.inlineMathImage expression1) (Markdown.Latex.inlineMathImage expression2)

                // act
                let result = Markdown.Latex.convertInlineMath input

                // assert
                Expect.equal result expected ""

            testCase "Latex.convertInlineMath: ignores [MATH] tags."
            <| fun _ ->
                // arrange
                let expression1 =
                    @"V_c \equiv V_{trig} \equiv V_{thresh} = L"

                let expression2 = @"X^2 \frac{1}{2} \overline{taco}"

                let template =
                    sprintf "Blah blah blah %s. Blah blah blah %s blah blah!"

                let input =
                    template
                        (Markdown.displayMathOpenTag + expression1 + Markdown.displayMathCloseTag)
                        (Markdown.inlineMathOpenTag
                         + expression2
                         + Markdown.inlineMathCloseTag)

                let expected =
                    template
                        (Markdown.displayMathOpenTag + expression1 + Markdown.displayMathCloseTag)
                        (Markdown.Latex.inlineMathImage expression2)

                // act
                let result = Markdown.Latex.convertInlineMath input

                // assert
                Expect.equal result expected ""

            testCase "Latex.convertMath: replaces both inline and display math tags."
            <| fun _ ->
                // arrange
                let expression1 =
                    @"V_c \equiv V_{trig} \equiv V_{thresh} = L"

                let expression2 = @"X^2 \frac{1}{2} \overline{taco}"

                let template =
                    sprintf "Blah blah blah %s. Blah blah blah %s blah blah!"

                let input =
                    template
                        (Markdown.displayMathOpenTag + expression1 + Markdown.displayMathCloseTag)
                        (Markdown.inlineMathOpenTag
                         + expression2
                         + Markdown.inlineMathCloseTag)

                let expected =
                    template
                        (Markdown.Latex.displayMathImage expression1)
                        (Markdown.Latex.inlineMathImage expression2)

                // act
                let result = Markdown.Latex.convertMath input

                // assert
                Expect.equal result expected ""

            testCase "Bulma.wrapInPopover: puts input inside popover"
            <| fun _ ->
                // arrange
                let input =
                    @"![555-ic-comparator-1](img/555-ic-comparator-1.png)"

                let expected = """<sup class="popover"><span class="icon is-small"><i class="fas fa-window-restore"></i></span><span class="popover-content">"""
                                + input
                                + "</span></sup>"

                // act
                let result = Markdown.Bulma.wrapInPopover input

                // assert
                Expect.equal result expected ""

            testCase "Bulma.convertPopovers: replaces string of only [POP]"
            <| fun _ ->
                // arrange
                let expression =
                    @"![555-ic-comparator-1](img/555-ic-comparator-1.png)"

                let input =
                    Markdown.popoverOpenTag + expression + Markdown.popoverCloseTag

                let expected = Markdown.Bulma.wrapInPopover expression

                // act
                let result = Markdown.Bulma.convertPopovers input

                // assert
                Expect.equal result expected ""

        ]
