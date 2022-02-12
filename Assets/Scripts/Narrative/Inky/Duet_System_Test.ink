VAR duetScore = 0
@back ph_bonfire id:Bonfire
@char STEPHAN.DEFAULT scale:1.2,1.2
@char PROTAG.DEFAULT scale:1.2,1.2
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

@startTrans
@skip false
@hideUI
@spawn duet
@duet hard
@finishTrans

STEPHAN: You did...
@despawn duet

->AskDuet

=GoodJob

goodjob
->AskDuet
    
=BadJob

badjob 
->AskDuet