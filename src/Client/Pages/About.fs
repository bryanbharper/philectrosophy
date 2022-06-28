module Client.Pages.About

open Client.Styles
open Elmish
open Feliz
open Feliz.Bulma

type State = unit

type Msg = unit

let init (): State * Cmd<Msg> = (), Cmd.none

let update (msg: Msg) (state: State): State * Cmd<Msg> = (), Cmd.none

let section (bgColor: string) (children: ReactElement list) =
    Bulma.section [
        prop.classes [
            Bulma.HasTextCentered
            bgColor
        ]
        prop.children children
    ]

let sectionHeader (title: string) (subtitle: string) =
    [
        Bulma.title.h2 [
            prop.className Bulma.HasTextWhite
            prop.text title
        ]
        Bulma.subtitle.h4 [
            prop.classes [
                Bulma.HasTextWhite
                Bulma.Mb5
            ]
            prop.text subtitle
        ]
    ]
    |> Html.div

let sectionBlurb (blurb: string) =
    Bulma.container [
        Bulma.content [
            content.isLarge
            prop.className Bulma.Mb5
            prop.children [
                Html.p [
                    prop.classes [ Bulma.HasTextWhite ]
                    prop.text blurb
                ]
            ]
        ]
    ]

let about =
    [
        Bulma.image [
            prop.className Bulma.IsInlineBlock
            prop.children [
                Html.img [
                    image.isRounded
                    prop.src "img/profile.jpg"
                ]
            ]
        ]
        Bulma.columns [
            Bulma.column [
                column.isOffsetOneThird
                column.isOneThird
                prop.children [
                    Bulma.content [
                        content.isMedium
                        prop.children [
                            Html.p [
                                prop.classes [
                                    Bulma.HasTextWhite
                                    Bulma.Mt3
                                ]
                                prop.text
                                    ("I'm Bryan. Philectrosophy is my blog."
                                     + " I've created it as a place to explore my interests."
                                     + " Hopefully other people will find it interesting too."
                                     + " Topics range from technical to personal.")
                            ]
                        ]
                    ]
                ]
            ]
        ]
    ]
    |> section Style.DarkBg

let progressBar (color: string) percent (label: string) =
    Html.div [
        prop.className Style.ProgressWrapper
        prop.children [
            Bulma.progress [
                progress.isMedium
                progress.max 100
                progress.value percent
                prop.className color
                prop.text label
            ]
            Html.p [
                prop.classes [
                    Style.ProgressValue
                    Bulma.HasTextWhite
                ]
                prop.text label
            ]
        ]
    ]

let programming =
    let languages =
        [
            "C#"
            "F#"
            "Typescript / JavaScript"
            "GraphQL"
            "Haskell"
            "Python"
        ]

    let colors =
        [
            Bulma.IsPrimary
            Bulma.IsLink
            Bulma.IsInfo
            Bulma.IsSuccess
            Bulma.IsWarning
            Bulma.IsDanger
        ]

    let values =
        seq { 0.0 .. 0.15 .. 7.0 }
        |> Seq.map (fun x -> 2.0 ** x)
        |> Seq.filter ((>=) 100.0)
        |> Seq.rev
        |> Seq.take languages.Length
        |> Seq.map (round >> int)
        |> List.ofSeq

    [
        sectionHeader "Programming" "[ˑproʊgræmɪŋ] (noun)"

        sectionBlurb
            ("I work as a Software Engineer."
             + " Interested in functional programming, web development, testing, tooling, 2d games, visualizations, and more...")

        Bulma.columns [
            Bulma.column [
                Bulma.columns [
                    Bulma.column [
                        column.isOffsetOneFifth
                        column.isThreeFifths

                        List.zip3 colors values languages
                        |> List.map (fun (c, v, l) -> progressBar c v l)
                        |> prop.children
                    ]
                ]
            ]
            Bulma.column [
                Bulma.title.h4 [
                    prop.className Bulma.HasTextWhite
                    prop.text "Follow me on:"
                ]

                Html.ul [
                    Html.li [
                        Html.a [
                            prop.className Bulma.HasTextWhite
                            prop.href "https://github.com/bryanbharper"
                            prop.text "https://github.com/bryanbharper"
                        ]
                    ]
                    Html.li [
                        Html.a [
                            prop.className Bulma.HasTextWhite
                            prop.href "https://www.codewars.com/users/bryanbharper"
                            prop.text "www.codewars.com/users/bryanbharper"
                        ]
                    ]
                ]
            ]
        ]

    ]
    |> section Style.DarkPurpleBg

let interestsAndProjects (interests: string list) (projects: (string * string) list) =
    Bulma.columns [
        Bulma.column [
            Bulma.title.h4 [
                prop.classes [
                    Bulma.HasTextWhite
                    Bulma.Mb1
                ]
                prop.text "Areas of Interest:"
            ]

            interests
            |> List.map (fun s ->
                Html.li [
                    prop.className Bulma.HasTextWhite
                    prop.text s
                ])
            |> Html.ul
        ]

        Bulma.column [
            Bulma.title.h4 [
                prop.classes [
                    Bulma.HasTextWhite
                    Bulma.Mb1
                ]
                prop.text "Related Posts / Projects"
            ]

            projects
            |> List.map (fun (label, href) ->
                Html.li [
                    Html.a [
                        prop.classes [
                            Bulma.HasTextWhite
                            Style.IsUnderlined
                        ]
                        prop.href href
                        prop.text label
                    ]
                ])
            |> Html.ul
        ]
    ]

let engineering =
    let interests =
        [
            "Circuits"
            "Digital Systems"
            "Control Systems"
            "Signal Theory"
        ]

    let projects =
        [
            "Digital Clock Design", "/blog/build-a-digital-clock"
            "555 Timer IC", "/blog/the-555-timer-ic"
            "Terminal Talk", "/blog/terminal-talk"
        ]

    [
        sectionHeader "Engineering" "[ˈɛnʤəˈnɪrɪŋ] (noun)"
        sectionBlurb
            "Earned a bachelor's of science in Electrical Engineering. I'm a bit rusty these days, but still tinker."
        interestsAndProjects interests projects
    ]
    |> section Style.PurpleBg

let philosophy =
    let interests =
        [
            "Metaphysics"
            "Logic"
            "Mind"
            "Language"
        ]

    let projects =
        [
            "Gunky Atoms", "/blog/gunky-atoms"
            "Identity of Indiscernibles", "/blog/the-identity-of-indiscernibles"
            "On Plantinga's E.A.A.N", "/blog/on-the-evolutionary-argument-against-naturalism"
        ]

    [
        sectionHeader "Philosophy" "[fəˈlɑsəfi] (noun)"
        sectionBlurb
            "Earned a master's degree in philosophy. Also rusty, but enjoy the occasional omphaloskepsis — yes, that's a fancy word for 'navel-gaze'."
        interestsAndProjects interests projects
    ]
    |> section Style.LightPurpleBg

let resumeBtn =
    Html.a [
        prop.classes [ Bulma.IsHiddenMobile ]
        prop.href "resources/Resume_BryanHarper.pdf"
        prop.target "_blank"
        prop.children [
            Html.div [
                prop.classes [ Style.ResumeTab ]
                prop.children [
                    Html.h4 "View Resume"
                    Html.span [
                        prop.classes [ FA.Fa; FA.FaFilePdf ]
                    ]
                ]
            ]
        ]
    ]

let render (state: State) (dispatch: Msg -> unit): ReactElement =
    [
        resumeBtn
        about
        programming
        engineering
        philosophy
        section Style.LighterPurpleBg []
    ]
    |> Html.div
