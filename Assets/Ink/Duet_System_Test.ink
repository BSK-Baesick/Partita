VAR duetScore = 0

@char Stephan
@char Protag
Hello
->AskDuet

=AskDuet

Do you wanna duet?
    +[Yes.]
    ->Duet
    +[No.]
    Why the hell did you even play this demo.
    ->AskDuet
    
=Duet
please find bugs and send them our way at the game-tech channel
duet starts
@hideUI
@spawn duet
@duet
You just finished the duet!
@despawn duet
{duetScore>50: ->GoodJob}
{duetScore<50: ->BadJob}
->DONE

=GoodJob
    You did good!
    ->AskDuet
=BadJob
    I'm afraid you did terrible lmao.
    Maybe try again?
    ->AskDuet