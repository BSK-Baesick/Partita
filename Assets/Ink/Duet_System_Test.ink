VAR duetScore = 0
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
Let's start
@skip false
@hideUI
@spawn duet
@duet
You just finished the duet!
@despawn duet
->AskDuet

=GoodJob
    You did good!
    ->AskDuet
=BadJob
    I'm afraid you did terrible lmao.
    Maybe try again?
    ->AskDuet