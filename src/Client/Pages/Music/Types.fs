module Client.Pages.Music.Types

open Shared

type PlayerState =
    | Paused
    | Playing
    | Stopped

type State =
    {
        CurrentTrack: Song option
        PlayerState: PlayerState
        Shuffle: bool
        Playlist: Deferred<Song list>
    }

type Msg =
    | PathUpdated of unit
    | ServerReturnedError of exn
    | ServerReturnedTracks of Song list
    | ServerUpdatedTrackPlayCount of Song option
    | TrackEnded
    | UserClickedNext
    | UserClickedPause
    | UserClickedPlay
    | UserClickedPrevious
    | UserClickedShuffleBtn
    | UserClickedTrack of Song
