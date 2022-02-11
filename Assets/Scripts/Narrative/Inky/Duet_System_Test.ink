VAR duetScore = 0
@back ph_bonfire id:Bonfire
@char STEPHAN.DEFAULT
@char PROTAG.DEFAULT
@printer Partita
STEPHAN: Hello

->AskDuet

=AskDuet
STEPHAN: Do you wanna duet?

    +[Yes.]
    
    ->Duet
    
    +[No.]
    
    STEPHAN: Why the hell did you even play this demo.
    
    ->AskDuet
    
=Duet

STEPHAN: Let's start

@skip false

@hideUI

@spawn duetEasy
@duet easy

STEPHAN: You did...
@despawn duetEasy

->AskDuet

=GoodJob

goodjob
->AskDuet
    
=BadJob

badjob 
->AskDuet