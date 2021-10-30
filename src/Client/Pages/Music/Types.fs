module Client.Pages.Music.Types

open Shared

type State =
    {
        CurrentSong: Song option
        Shuffle: bool
        Songs: Deferred<Song list>
    }

type Msg =
    | ServerReturnedError of exn
    | ServerReturnedSongs of Song list
    | TrackEnded
    | UserClickedNext
    | UserClickedPrevious
    | UserClickedShuffleBtn
    | UserClickedTrack of Song
