module Client.Pages.Music.Types

type Track = { Title: string; Src: string }

type State = { CurrentTrack: Track; Shuffle: bool }

type Msg =
    | TrackEnded
    | UserClickedNext
    | UserClickedPrevious
    | UserClickedShuffleBtn
    | UserClickedTrack of Track
