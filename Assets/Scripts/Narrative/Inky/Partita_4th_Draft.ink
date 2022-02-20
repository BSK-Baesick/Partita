->Chapter1

=== Chapter1

@hideAll

@setDuetCharacter MILLIA

@startTrans

@bgm approaching_bus fade:1

@wait 2

@sfx open_bus_door volume:0.5 time:4

@back id:BusInterior

@playBusStopSoundscape

@finishTrans Crossfade time:3

@hide BusInterior

@playWorldMusic

@back bs_7 id:Chapter1BusStop

@spawn Snow

@wait 1

@char PROTAG.DEFAULT scale:1.2,1.2

// step on snow sfx

As your feet touch soft powder, weary eyes pierce through glaring snow. 

An ancient woman sits, alert, back pressed against the cracked concrete wall.

In her arms she cradles a rifle, trained delicately on your approach.

Her voice is backed with a surprising vigor.
    
@char MILLIA.DEFAULT look:left pos:75,0 scale:1.2,1.2

@char PROTAG.DEFAULT look:right pos:25,0 scale:1.2,1.2

MILLIA: "No."

@resetText


    + [-] -> SilentType
    
    + [Yes.] -> Scene1




=Scene1

@sfx Mosin_Reload

She scoffs and raises the rifle to her eyeline.

@char MILLIA.DEFAULT look:left pos:75,0 scale:1.2,1.2

@spawn DepthOfField params:MILLIA

MILLIA: "Say that again?"

@resetText


    + [Stay Silent.] -> SilentType
        
    + [No.] -> Boring
    
    + [Yes.] -> FirstGunshot



=Boring
    
Her eyes roll as she bemoans on an exasperated sigh.
    
MILLIA: "Boring."

@sfx pick_up_gun
    
She flips the rifle back towards herself, where it settles harmlessly in her lap.

-> WhereFrom



=FirstGunshot

@despawn DepthOfField

@sfx Mosin_Shot_And_Reload
    
BANG!

@wait 1

@spawn ShakeCharacter params:PROTAG
    
A bullet flies towards you, grazing the outside of your neck. 

@spawn ShakeCharacter params:MILLIA
    
She laughs.

@spawn ShakeCharacter params:PROTAG
    
You feel a trickle of blood.
    
@resetText
    
    + [Take a step towards her.] -> SecondGunshot


    
=SecondGunshot

@char PROTAG.DEFAULT look:right pos:35,0 scale:1.2,1.2

@sfx Mosin_Shot_And_Reload
        
BANG!

@wait 1

@spawn ShakeCharacter params:PROTAG
        
Another bullet. Cutting delicately into the skin of your cheek

@spawn ShakeCharacter params:MILLIA
        
Another laugh. 

@spawn ShakeCharacter params:PROTAG
        
Another trickle of blood.

Then silence.

She stares. 

Curled lips smirk in amusement.

@resetText


    + [If you wanted me dead, I'd be dead.] -> IfYouWantedMeDead
        
    + [What the fuck!?] -> Repetition1
    
    + [Run at her.] -> RunAtHer



=RunAtHer

Are you sure?

@resetText


    + [Yes.] -> MilliaDeath
        
    + [No.] -> WhatTheFuck



=Repetition1

@resetText
        
        
    + [What the fuck!?] -> ThirdGunshot
    
    + [Take a step towards her.] -> ThirdGunshot


    
=ThirdGunshot

@char PROTAG.DEFAULT look:right pos:45,0 scale:1.2,1.2

@char MILLIA.DEFAULT look:left pos:80,0 scale:1.2,1.2

@sfx Mosin_Shot_And_Reload

BANG!

@wait 1

@despawn DepthOfField

@spawn ShakeCharacter params:PROTAG
    
A bullet flies towards you, grazing the outside of your neck. 

@spawn ShakeCharacter params:MILLIA
    
She laughs.

@spawn ShakeCharacter params:PROTAG
    
You feel a trickle of blood.

@resetText


    + [Take another step towards her.] -> FourthGunshot



=FourthGunshot
    
@char PROTAG.DEFAULT look:right pos:55,0 scale:1.2,1.2

@char MILLIA.DEFAULT look:left pos:85,0 scale:1.2,1.2

@sfx Mosin_Shot_And_Reload
        
BANG!

@spawn ShakeCharacter params:PROTAG
        
Another bullet. Cutting delicately into the skin of your cheek

@spawn ShakeCharacter params:MILLIA
        
Another laugh. 

@spawn ShakeCharacter params:PROTAG
        
Another trickle of blood.

Then silence.

She stares. 

Curled lips smirk in amusement.

@resetText


    + [If you wanted me dead, I'd be dead.] -> IfYouWantedMeDead
    
    + [What the fuck!?] -> WhatTheFuck
    
    + [Run towards her.] -> RunAtHer2



=RunAtHer2

MILLIA: Are you sure?

@resetText
   
   
    + [Yes.] -> MilliaDeath
        
    + [No.] -> WhatTheFuck
        
    + [What the fuck!?] -> WhatTheFuck



=MilliaDeath

@char PROTAG.DEFAULT look:right pos:65,0 scale:1.2,1.2

@char MILLIA.DEFAULT look:left pos:95,0 scale:1.2,1.2

You press one foot down at an angle into the snow, propelling yourself forward as the woman's eyes shift from bemusement to a terrifying focus. 

She tilts her scope toward you, and before you can bring your next leg forward, a bullet flies clean through your sk-

@sfx Mosin_Oneshot

@wait 2

@despawn ShakePrinter

@spawn ShakeCharacter params:PROTAG

Everything.

@char PROTAG.DEFAULT look:right transition:Crossfade pos:65,0 scale:1.2,1.2

Ends.

@resetText


    + [Try again.]

-> Chapter1



=WhatTheFuck

@spawn ShakeCharacter params:MILLIA,8,5,,1,,false,true

She howls with laughter, her head reeling back to the sky as she splutters for a moment before it returns.

@despawn ShakeCharacter params:MILLIA

MILLIA: "You play with fire."

The rifle remains poised, and she steadies her aim.

Her finger delicately tensing against the trigger.

She smirks.

@char PROTAG.DEFAULT look:right pos:35,0 scale:1.2,1.2

@char MILLIA.DEFAULT look:right pos:75,0 scale:1.2,1.2

@sfx pick_up_gun

And flips it back over her shoulder, as her lips give way to a wry smile.

MILLIA: "I'm entertained."

-> WhereFrom


        
=IfYouWantedMeDead

@char PROTAG.DEFAULT look:right pos:35,0 scale:1.2,1.2

@char MILLIA.DEFAULT look:right pos:75,0 scale:1.2,1.2

Her smirk gives way to laughter. 

She releases the rifle and raises her hands as it falls into her lap.

@char MILLIA.DEFAULT look:left pos:75,0 scale:1.2,1.2

MILLIA: "You got me."

-> WhereFrom



=SilentType

@despawn DepthOfField

MILLIA:"Silent type?"

@resetText
        
    + [-] -> Stare1
    
    + [Just scared.] -> ScaredProtag



=ScaredProtag

MILLIA:"Smart."

-> WhereFrom



=Stare1

MILLIA:"-"

@resetText
            
    + [-] -> Stare2
        
    + [This is ridiculous.] -> ThisIsRidiculous



=Stare2
                
The woman stares into your soul, her eyes refuse to blink as they start to water in the snow. 
                
Tiny teardrops freezing over wrinkled skin. 

@resetText

                
    + [Are you ok?] -> MilliaWins
        
    + [-] -> BothGiveUp
        
    + [Are we done?] -> CuriousProtag



=MilliaWins
                    
@char Millia look:left pos:75,0 scale:1.2,1.2
                    
@spawn ShakeCharacter params:MILLIA

MILLIA: "HAH! I WIN!" 

She grins with childlike ambition, as one hand wipes the frozen salt from her eyes.

->WhereFrom



=ThisIsRidiculous

MILLIA: "You started it."

@resetText
            
    + [What are you, a child?] -> YouStartedIt
    
    + [True.] -> TrueMillia
    
    + [You're holding a rifle.] -> CorrectMillia



=YouStartedIt

MILLIA: "Just a very bored old woman."

->WhereFrom



=TrueMillia

MILLIA: "Well aren't you agreeable?"

->WhereFrom



=CorrectMillia

MILLIA: "Correct. Mosin's, to be exact."

->WhereFrom



=BothGiveUp

You both remain unblinking, tears forming on the boundaries of your vision in some ridiculous pissing contest. 

You lock gazes against the snow, frozen salt blinding you both until your bodies give way and force your hands to wipe your eyes.

@spawn ShakeCharacter params:MILLIA,0

@spawn ShakeCharacter params:PROTAG,0

As your vision returns, the sound of spluttered laughter carries across the way. A smoker's lung.

@despawn ShakeCharacter params:MILLIA

@despawn ShakeCharacter params:PROTAG

->WhereFrom



=CuriousProtag
                
MILLIA: "I don't know. Are we?"

@resetText


    + [You're a child.]

MILLIA: "Just a very bored old woman."

->WhereFrom

@resetText

+[Fine.]

 MILLIA: "Well aren't you agreeable?"
 
 ->WhereFrom
 
 @resetText
 
+[You're holding a rifle.]

MILLIA: "Correct. A Mosins, to be precise."

->WhereFrom
                    



                
=WhereFrom 

@camera offset:4,0.25 zoom:0.35

Old eyes slowly scan you.

@camera offset:-3,-0.3 zoom:0.35

Your face

@camera offset:-2.75,-1.5 zoom:0.25

Your body. 

@spawn DepthOfField

Your posture. 

@despawn DepthOfField

@camera offset:0,0 zoom:0 rotation:0,0,0

Your response.

@char PROTAG.DEFAULT look:right pos:25,0 scale:1.2,1.2

MILLIA: "Where are you from?"

@resetText

    + [Moscow.] -> ILiveAtMoscow
            
    + [None of your business.] -> NoneOfYourBusiness
    
    + [Where are you from?] -> AskMilliaFirstWhereSheLives
    
    
    
=ILiveAtMoscow

@spawn ShakeCharacter params:MILLIA
    
Her gaze converges on yours, she's intrigued.
    
MILLIA: "And before that?

@resetText
    
    
    + [Not Moscow.] -> NotLivingInMoscow
            
    + [None of your business.] -> NoneOfYourBusiness
            
    + [Where are you from?] -> AskMilliaFirstWhereSheLives
        
        
        
=NoneOfYourBusiness

She squints, digging deeper for a moment. 
    
MILLIA: "Well. Okay then."

-> MilliaFire



=AskMilliaFirstWhereSheLives

MILLIA: Moscow nowadays. 

MILLIA: Born in Bila Tserkva. 

MILLIA: But who knows what we'll be calling either of those a year from now.

-> MilliaFire



=NotLivingInMoscow

She scoffs.
        
MILLIA: "Well I suppose you're not lying at least."
        
-> MilliaFire


        
=MilliaFire

Her eyes slightly soften. The rifle lies harmlessly in her lap.

@stopBusStopSoundscape

MILLIA: "You must be cold. I'll start a fire.

@despawn Snow

@startTrans

@playBonfireSoundscape

@hideChars

@back ph_bonfire id:Bonfire

@finishTrans RadialBlur time:3

After a moment of wariness, the woman's shifting demeanor seems genuine. 

You help her gather firewood, and as you reach for a pile of kindling...

@spawn Envelope

A thin red envelope catches your eye amidst the shrubbery.

@sfx pick_up_letter

You know that this was meant for you.

@despawn Envelope

@sfx open_envelope

You open it and familiar handwriting sends warmth wrapping round your body.

@spawn Letter_001

@wait i

@despawn Letter_001

@char MILLIA.DEFAULT scale:1.2,1.2

MILLIA: "Letter from a lover?"

@resetText


    + [Yes.] -> SmilesGently
        
    + [No.] -> SmilesKnowingly
            
    + [-] -> SmilesSolemnly
    


=SmilesGently

@char PROTAG.DEFAULT look:right pos:25,0 scale:1.2,1.2

@char MILLIA.DEFAULT look:left pos:75,0 scale:1.2,1.2

She smiles, gently. 
        
MILLIA: "You should burn it."

-> YouShouldBurnIt



=SmilesKnowingly

@char PROTAG.DEFAULT look:right pos:25,0 scale:1.2,1.2

@char MILLIA.DEFAULT look:left pos:75,0 scale:1.2,1.2

She smiles, knowingly. 
        
MILLIA: "You should burn it."

-> YouShouldBurnIt



=SmilesSolemnly

@char PROTAG.DEFAULT look:right pos:25,0 scale:1.2,1.2

@char MILLIA.DEFAULT look:left pos:75,0 scale:1.2,1.2

She smiles, solemnly. 
        
MILLIA: "You should burn it."

-> YouShouldBurnIt


    
=YouShouldBurnIt

@resetText


    + [Can't.] -> CannotBurnLetter
    
    + [Why?] -> WhyBurnTheLetter
    
    + [I know.] -> IKnowThat
    
    
    
=CannotBurnLetter

@hidePrinter

@wait 2

MILLIA: "If you're finding letters stashed in bus stops, they're worth burning. Memorise, then burn"  

->YouCouldHaveKilledMe



=WhyBurnTheLetter

@hidePrinter

@wait 2

MILLIA: "If you're finding letters stashed in bus stops, they're worth burning. Memorise, then burn"  

->YouCouldHaveKilledMe



=IKnowThat

@hidePrinter

@wait 2

MILLIA: "I'm sure you do. Take the time to memorise."

->YouCouldHaveKilledMe

    

=YouCouldHaveKilledMe

@hideChars

@char PROTAG.DEFAULT look:right pos:25,0 scale:1.2,1.2

As you sit in silence against the coming night, the woman cradles the fire, eyeing off the letter in your hands with unbridled curiosity.

@char MILLIA.DEFAULT look:right pos:75,0 scale:1.2,1.2

@char PROTAG.DEFAULT look:left pos:25,0 scale:1.2,1.2
    
After some time, her eyes turn back to the flame. She seems lost in the sight, some strange nostalgia gripping at her mind. 

@char PROTAG.DEFAULT look:right pos:25,0 scale:1.2,1.2
    
For a second you think you see a tear, but she catches your stare with a smirk and a wiped eye before you can look closer.

@resetText
    
    
+ [Why didn't you kill me?] -> WhyYouDidntKillMe
    
+ [Do you like music?]  -> PlayHerASong
    
+ [We should sleep.] -> Sleep1
    
    
    
=WhyYouDidntKillMe

@wait 2

@char MILLIA.DEFAULT look:left pos:25,0 scale:1.2,1.2

MILLIA: "I've killed thousands for a country that's about to implode anyway. 
    
MILLIA: "It's no fun anymore. Nothing to be won."
    
MILLIA: "No grand prize."
    
MILLIA: "No medal."
    
MILLIA: "No accolades."
    
MILLIA: "Just a world wasting away on shriveled ambition."
    
Her eyes rise to laze on the horizon.
        
MILLIA: "Besides. You seemed harmless enough."

@resetText
        
        
    + [Play her a song.] -> PlayHerASong
            
    + [We should sleep.] -> Sleep2

VAR duetScore = 0
    
=PlayHerASong

You gently curl your backpack towards the ground, pulling delicate red timber to your chin.

@skip false

@hideUI

@spawn duetEasy pos:50,50

@duet easy

// If duetScore is greater than or equal to x, then go to MilliaDUET. Else, go to MilliaNODUET.


{ 
    - duetScore >= 50: 
    
    -> MilliaDUET
    
    - else:
    
    -> MilliaNODUET
}
        





=AskMilliaIfSheLikesMusic

MILLIA: "I adore it"
    
You gently curl your backpack towards the ground, pulling delicate red timber to your chin.

@skip false

@stopBonfireSoundscape

@stopWorldMusic

@hideUI

@spawn duetEasy

@duet easy
    
// If duetScore is greater than or equal to x, then go to MilliaDUET. Else, go to MilliaNODUET.


{ 
    - duetScore >= 50: 
    
    -> MilliaDUET
    
    - else:
    
    -> MilliaNODUET
}



=Sleep1

MILLIA: I'll keep first watch.
    
-> MilliaNoSong



=Sleep2

MILLIA: "True. I'll keep first watch."

-> MilliaNoSong



//IF DUET IS SUCCESFUL

=MilliaDUET

@playMusicDuet

@despawn duetEasy

@char PROTAG.DEFAULT look:right pos:25,0 scale:1.2,1.2

@char MILLIA.DEFAULT look:left pos:75,0 scale:1.2,1.2

@camera offset:-2.75,-1.5 zoom:0.25

As you raise a bow to match with string, you can't help but feel a little more alive

@camera offset:4.5,2 zoom:0.25

Matching your melody, the woman plays her own. 

@camera offset:0,0 zoom:0 rotation:0,0,0

@spawn SunShafts

A once dull brass sheen refracts moon and flame into the sky above. 

At first, it feels like a rising challenge, but you both settle into an uncanny unity 

The bonfire dances in your wake. Tiny ripples of sound from the woman to you and you to her, each carving a path through the fire. 

Smoke ebbs and flows as it rises into the open air.

You play long into the night. But eventually, weariness does draw you back to reality.

As the music comes to a close and your eyes give way to sleep, you turn your attention to the fire, letter dangling in your lap.

@despawn SunShafts

@stopMusicDuet

-> 1stLetter



//IF DUET IS NOT SUCCESFUL

=MilliaNODUET

As you raise a bow to matchstring, your arm curves against cold wind. You press forward, but the chill freezes you. The sound is naught but a screech.

MILLIA: "I appreciate the sentiment, but now's the time for rest."

-> MilliaNoSong



=MilliaNoSong

@hideChars

@despawn duet

@spawn Envelope

@char PROTAG.DEFAULT look:right scale:1.2,1.2

As your eyes give way to sleep, you turn your attention to the fire, letter dangling in your lap.

-> 1stLetter



=1stLetter

@resetText

    + [Burn it.] -> BurnTheLetter
    
    + [Keep it.] -> KeepTheLetter



=BurnTheLetter

You take one last glance. 

Breathe. 

@despawn Envelope

@sfx burn_letter

Then press the paper into the fire. 

It burns a little brighter, and you feel a little warmer as your eyes succumb to the night.

@stopBonfireSoundscape

@wait 2

-> WakeUpProtag



=KeepTheLetter

@despawn Envelope

You press the letter to your chest. Holding it tightly in place against the winter winds. 

There's a cold chill that sways you to sleep against the warmth of the fire.

@stopBonfireSoundscape

@wait 2

-> WakeUpProtag



=WakeUpProtag

@despawn Snow

@playBusStopSoundscape

You wake before the birds, your eyes pressing open against the weight of the world. 

@spawn Mosin_Icon

Beside you, the sleeping woman's rifle lies loose. There should be a bus arriving soon.

@resetText


    + [Take it.] -> TakeTheRifle
    
    + [Leave it.] -> LeaveTheRifle



=TakeTheRifle

@despawn Mosin_Icon

You tread carefully on fresh snow, sidling your way longside the first birdcall.

She mumbles.

You freeze.

You wait.

@sfx approaching_bus volume:0.5 time:4

You hear the rumbling of an engine in the distance.

@sfx pick_up_gun

Your hands stretch out against the dying night, pulling the rifle towards you.

She sleeps.

@sfx open_bus_door
    
You turn towards the coming bus.
    
-> Chapter2



=LeaveTheRifle
    
You consider the thought.
    
And let it pass.

@sfx approaching_bus volume:0.5 time:4
    
In the distance, an engine rumbles on approach.

@sfx open_bus_door
    
You turn back to the roadside and press on into morning.
    
-> Chapter2



=Chapter2

@setDuetCharacter STEPHAN

@startTrans

@hideAll remove:true

@finishTrans Crossfade time:3

@back id:BusInterior

@playBusStopSoundscape

The road is long and monotonous. 

Grey snow falls against a dull backdrop. 

@sfx approaching_bus volume:0.5 time:4

Faint hints of color from passing towns splash amidst the skyline and before you know it, the ride is over.

@back id:Chapter2BusStop

@spawn Snow

@char PROTAG.DEFAULT look:right pos:25,0 scale:1.2,1.2

@char STEPHAN.DEFAULT look:left pos:75,0 scale:1.2,1.2

As you press on to sleeted ground, a man waits nearby. 

Older, but not elder, he's frail if not determined in demeanor.

@sfx open_bus_door

Grey eyes seem to glance towards you, then behind, towards the closing door. 

@sfx bus_departure

As the bus departs, his gaze follows it. 

In one hand he holds a slowly burning cigar. 

In the other, a letter.

Your letter.

@resetText

    + [I think that might be for me.] -> StephanSmilesAtYou
    
    + [Stare at him.] -> StareAtStephan
    
    + {TakeTheRifle} [Train your rifle on him.] -> TrainYourRifleAtStephan



=StephanSmilesAtYou

Weary eyes smile at you.

STEPHAN: "I think you might be correct."

@char STEPHAN.DEFAULT look:left pos:55,0 scale:1.2,1.2

He extends the letter out towards you. You take it, nestling it inside your coat.

@char STEPHAN.DEFAULT look:left pos:75,0 scale:1.2,1.2

The man watches with warm intrigue.

->MindIfIAskWho



=StareAtStephan

As the bus dips behind a cradle of trees, the man's eyes return to lock with yours.

He smiles. It's surprisingly warm against the cold air.

STEPHAN: "I'd thought this was for me. Seems I was wrong."

He raises the letter towards you.

@char PROTAG.DEFAULT look:right pos:55,0 scale:1.2,1.2

You take it. Nestling it inside your coat.

@char PROTAG.DEFAULT look:right pos:25,0 scale:1.2,1.2

->MindIfIAskWho



=TrainYourRifleAtStephan

@char STEPHAN.DEFAULT look:left pos:80,0 scale:1.2,1.2

@spawn ShakeCharacter params:STEPHAN,2,0.15,0.01

His arms fly upwards, eyes full of fear. 

@despawn ShakeCharacter

The letter dangles gently from one hand as smoke rises from the cigar in the other.

@resetText

    + [Drop it.] -> DropTheLetter1
    
    + [Move closer.] -> MoveCloserToStephan



=DropTheLetter1

@sfx Mosin_Reload

STEPHAN: "I haven't opened it."
    
His voice is tired.
    
STEPHAN: "I thought it was for me."

@resetText
    
    + [It's not.] -> TheLetterisNotForStephan
                
    + [Drop. It.] -> DropTheLetter2



=TheLetterisNotForStephan

A wry, forced smile. 
        
STEPHAN: "I know."

@resetText
        
    + [Drop. It.] -> DropTheLetter3
                
    + [Grab the letter.] -> GrabTheLetter1



=DropTheLetter2

He drops the letter. His hands remain pressed to the sky.

->IDontWantTrouble



=GrabTheLetter1

@char PROTAG.DEFAULT look:right pos:45,0 scale:1.2,1.2

You rush forward and swipe the letter from his hands.

->IDontWantTrouble



=DropTheLetter3

He drops the letter.

His hands remain pressed toward the sky.

@char PROTAG.DEFAULT look:right pos:45,0 scale:1.2,1.2

You rush forward and swipe it from the snow.

->IDontWantTrouble



=MoveCloserToStephan

@char PROTAG.DEFAULT look:right pos:45,0 scale:1.2,1.2

@char STEPHAN.DEFAULT look:left pos:85,0 scale:1.2,1.2

He winces at the approach.

@resetText
    
    + [Grab the letter.] -> GrabTheLetter2
            
    + [Give me the letter.] -> GrabTheLetter3



=GrabTheLetter2

@char PROTAG.DEFAULT look:right pos:45,0 scale:1.2,1.2

You rush forward and swipe the letter from his hands.

->IDontWantTrouble



=GrabTheLetter3

He holds out a palm towards you. It trembles in the cold.

@char STEPHAN.DEFAULT look:left pos:75,0 scale:1.2,1.2

@char PROTAG.DEFAULT look:right pos:45,0 scale:1.2,1.2

You walk forward, rifle still trained as you swipe the letter from his palms.

->IDontWantTrouble


=IDontWantTrouble

@char PROTAG.DEFAULT look:right pos:25,0 scale:1.2,1.2

@char STEPHAN.DEFAULT look:left pos:75,0 scale:1.2,1.2

STEPHAN: "I don't want trouble."

@resetText

    + [Sorry. Tense.] -> SorryTense
    
    + [Neither do I.] -> NeitherDoI
    
    + [Look away.] ->LookAway



=SorryTense

STEPHAN: Tense times. Can hardly blame you.

-> MindIfIAskWho



=NeitherDoI

STEPHAN: "And so we've reached an accord. No bullets then?"

He chuckles, vaguely masking a residual fear.

-> MindIfIAskWho



=LookAway

STEPHAN: "I really didn't read it, if that helps? Seemed pretty well hidden too so I doubt anyone else has."

-> MindIfIAskWho



=MindIfIAskWho

His eyes laze towards the letter.

@wait 2

STEPHAN: "Mind if I ask who?"

@resetText

    + [...] -> SilentAnswer
    
    + [I shouldn't say.] -> DoNotTellStephan
    
    + [A lover.] ->TellStephanItIsYourLover



=SilentAnswer

STEPHAN: "I understand."

->AndYou



=DoNotTellStephan

STEPHAN: "I understand."

->AndYou



=TellStephanItIsYourLover

@spawn ShakeCharacter params:STEPHAN,1,0.15,0.01

STEPHAN: "A secret lover!"

@despawn ShakeCharacter

@resetText  
    
    + [That or dead.] -> IsLoverDead
            
    + [I suppose you could say that.] ->StephanHunch
        
        

=IsLoverDead

His tone softens.
        
STEPHAN: "Ah."

His face hardens.

STEPHAN: "Heading south then?"

->South



=StephanHunch

STEPHAN: "Heading south amidst the chaos?"
        
->South



=South  

@resetText

    + [...] ->YouSmoke
    
    + [I shouldn't..."] ->YouSmoke 
    
    + [Through Abkhazia. Then on to Turkey. I have family there.] -> TellStephanYourDestination



=TellStephanYourDestination

STEPHAN: "Rough road. Suppose they all are right now."

->AndYou


        
=YouSmoke

The man glances towards your face for a moment. He nods and smiles. 

STEPHAN: "Sorry. I'll drop it."

@char STEPHAN.DEFAULT look:left pos:80,0 scale:1.2,1.2

He leans back, head to the sky, dragging in a mound of smoke off the end of his cigar before exhaling. 

You watch it dance against the fog.

STEPHAN: "You smoke?"

@resetText

    + [No.] -> ProtagDoNotSmoke
    
    + [No. Thank You.] ->NoThankYouStephan
    
    + [I do.] -> ProtagSmokes



=ProtagDoNotSmoke

STEPHAN: "Ah."

@char STEPHAN.DEFAULT look:left pos:75,0 scale:1.2,1.2

He presses the cigar butt down against his coat.

STEPHAN: "Sorry about that."

->AndYou



=NoThankYouStephan

STEPHAN: "Ah."

@char STEPHAN.DEFAULT look:left pos:75,0 scale:1.2,1.2

He presses the cigar butt down against his coat.

STEPHAN: "Sorry about that."

->AndYou



=ProtagSmokes

Cigar in mouth, he flicks open a small box from his trouser pocket. 

@sfx strike_lighter

From inside his coat he removes a lighter and strikes a light. 
@char STEPHAN.DEFAULT look:left pos:55,0 scale:1.2,1.2

"Take the edge off."

@char STEPHAN.DEFAULT look:left pos:75,0 scale:1.2,1.2

@sfx smoking

You take the cigar and inhale. There's comfort in the heat.

->AndYou


        
=AndYou

@resetText

    + [Who are you waiting for?] ->AskStephanWhoIsSheWaiting
    
    + [Why wait in the cold? That was the only bus for hours.] -> AskStephanWhySheWaitsInTheCold



=AskStephanWhoIsSheWaiting

STEPHAN: "My son."

-> TheyAreNotComingthrough



=AskStephanWhySheWaitsInTheCold

STEPHAN: "Just tradition."

@spawn ShakeCharacter params:STEPHAN,2,0.15,0.01,0.25

He laughs at himself.

STEPHAN: "My son."

-> TheyAreNotComingthrough



=TheyAreNotComingthrough

STEPHAN: "They're not coming through."

@resetText

    + [Sorry for your loss.] -> SorryForYourLossStephan
    
    + [Sorry.] -> SorryStephan



=SorryForYourLossStephan

STEPHAN: "Oh they're not dead. Least I hope not."

-> StephanTellsHerStory



=SorryStephan

STEPHAN: "Don't be. My own damn fault."

-> StephanTellsHerStory



=StephanTellsHerStory

STEPHAN: You have a child that outruns the world and sometimes you just can't keep up.

STEPHAN: Sometimes you say terrible things. 

STEPHAN: I don't blame them for running off.  

STEPHAN: Moscow was probably better for them, find their own crowd and all that. 

STEPHAN: Play in a band. 

STEPHAN: Not a lot of those around here.

@resetText

    + [City for the strays.] -> CityForTheStrays
    
    + [I lived there. Before all this.] -> ILivedThere


 
 =CityForTheStrays
 
 STEPHAN: "Sounds like them."
 
 -> StephanBreak
 
 
 
 =ILivedThere
 
 STEPHAN: "Ah! Wouldn't have guessed."
 
 -> StephanBreak
 
 
 
 =StephanBreak
 
 A cold breeze rushes past you and wind whips powdered snow around your feet.
 
 STEPHAN: "Ahhh. I guess we're done for the day."
 
 @resetText
 
     + [I've got a bus to catch.] -> IHaveABusToCatch
     
     + [It was nice to meet you.] -> ItWasNiceToMeetYouStephan



=IHaveABusToCatch

STEPHAN: You'll freeze before it comes. 

STEPHAN: Stay with my wife and I. 

STEPHAN: Rest. 

STEPHAN: Recover.

-> BreezeReturns



=ItWasNiceToMeetYouStephan

STEPHAN: "You'll freeze before the bus comes.

STEPHAN: Stay with my wife and I. 

STEPHAN: Rest. 

STEPHAN: Recover.

-> BreezeReturns



=BreezeReturns

The breeze returns with harsher ambition.

@resetText

     + [I appreciate the concern. I'll be ok.] -> AppreciateStephanConcern
     
     + [As long as I'm not imposing.] -> AsLongAsWeAreNotImposing
 
 
 
=AppreciateStephanConcern
 
He frowns with furrowed brow.
 
@char STEPHAN.DEFAULT look:right pos:85,0 scale:1.2,1.2
 
STEPHAN: Yellow house. 
 
STEPHAN: Just behind the bend due south. 
 
STEPHAN: Don't die stupid.
 
@hide STEPHAN remove:true
 
As he goes to leave, cigar in one hand, he waves with the other, and you spot fractured stubs on the tips of his fingers. 

Frostbite.
 
 ->SoloSnow
 
 
 
 =AsLongAsWeAreNotImposing
 
 The man grins. 
 
 @char STEPHAN.DEFAULT look:right pos:75,0 scale:1.2,1.2
 
 STEPHAN: "Impossible. Come on then."
 
 @char PROTAG.DEFAULT look:right pos:45,0 scale:1.2,1.2
 
 ->ThroughTheSnow
 
 
 
 =SoloSnow
 
 @startTrans
 
 @hideChars remove:true
 
 @back id:SnowWild
 
 @despawn Snow
 
 @char PROTAG.DEFAULT scale:1.2,1.2
 
 @spawn SnowHeavy
 
 @finishTrans
 
 You wait in silence, shattered by whistling wind. The storm rises and you pull your scarf to cover your face.
 
 @resetText
 
     + [Stay.] -> Stay1
        
     + [Leave.] -> Leave
 
 
 
 =Stay1
 
 @spawn ShakeCamera params:,0,,0.01,0.01
 
 @spawn DepthOfField params:PROTAG,,1,3
 
 Your vision starts to cloud in blanket white, snow no longer merely bites at your ankles but rises to surround you on all sides. 
 
 The cold cuts at your skin.
 
 @resetText
 
    + [Stay.] -> Stay2
        
    + [Leave.] -> Leave
 
 
 
 =Stay2
 
 @spawn ShakeCamera params:,0,,0.02,0.02
 
 @spawn DepthOfField params:PROTAG,,2,3
 
 You can feel your blood start to freeze, your skin frosting over. 
 
 There's a dangerous illusion of warmth as you begin to lose perception of your ligaments.
 
 @resetText
 
    + [Stay] -> Stay3
                
    + [Leave] -> Leave
 
 
 
 =Stay3
 
@spawn ShakeCamera params:,0,,0.02,0.02

@spawn DepthOfField params:PROTAG,,5,3

Are you sure?

@resetText
          
            
    + [Stay] -> Death2
                
    + [Leave] -> Leave
 
 
 
=Leave
 
@despawn DepthOfField

@despawn ShakeCamera

 -> ThroughTheSnow
  
  
  
=Death2

@despawn ShakeCamera

@spawn DepthOfField params:null,,5,3

Your eyes give way to pure white. 

Cold gives way to warmth. 

Light gives way to dark. 

The pain.

@hideChars remove:true

Ends.

@resetText

+Try again.

@despawn SnowHeavy

@despawn DepthOfField

->Chapter2


 
 =ThroughTheSnow
 
@startTrans

@hideAll remove:true

@despawn SnowHeavy

@despawn DepthOfField

@back ph_bonfire id:Bonfire

@char PROTAG.DEFAULT look:right pos:25,0 scale:1.2,1.2

@finishTrans


 You trudge against the storm. 
 
 Yellow paint cuts through the white like a knife, glistening in the distance. 
 
 The walk is hard, but short. 
 
 @char STEPHAN.DEFAULT look:left pos:75,0 scale:1.2,1.2
 
 The man and his wife brighten in your presence.
 
 The house is warm. 
 
 The food is warm. 
 
 You feel both full and light. 
 
 He eyes the case on your back.
 
 His eyes brighten.
 
 STEPHAN: "Play with me."
 @resetText
 
 + [Why not?] -> StartStephanDuet
 
 + [I should sleep.] -> RestWithStephan
 
 
 
  =StartStephanDuet
  
  @skip false
  
  @hideUI
  
  @spawn duetEasy
    
  @duet easy
  
 { 
    - duetScore >= 50: 
    
    -> StephanDuet
    
    - else:
    
    -> StephanNODUET
}
 
 
 
  =StephanNODUET
  
 @despawn duet
 
 As his wife watches from nearby, the man delicately places fingers on his accordion. 
 
 It seems almost like a reunion of sorts. 
 
 Long overdue. 
 
 You raise your bow as the wind howls outside. 
 
 The man starts to play, but the sounds are wrong. 
 
 You can barely make eachother out over the wind outside.
 
 Eventually, you both give up. He offers you a rassuring smile.
 
STEPHAN: You must be tired. 
 
STEPHAN: We should let you rest.
 
 The man and his wife thank you, showing you a bed before retiring to their own.
 
 ->Chapter2Epilogue
 
 
 
 =RestWithStephan
 
 The man face betrays his dissapointment, but he smiles regardless.
 
 STEPHAN: "I understand. Goodnight."
 
 ->Chapter2Epilogue
 
 
  
 =StephanDuet
 
@despawn duet

@playMusicDuet

As his wife watches from nearby, the man delicately places fingers on his accordion. 

It seems almost like a reunion of sorts. 

Long overdue. 

He waits for you, for some kind of strange permission, and you give it by stretching bow across silver string. 

The world, their world, brightens against the night and you see heavy smiles turn light. 

The woman laughs.

The man laughs.

You laugh.

You play on against the storm outside and long into the eveing. 

In this reprieve, music mutes the outside air.

You play until the storm recedes. 

It's remarkably simple, and beautifully melancholic.

Eventually, the man and his wife thank you, before showing you a bed and retiring to their own.

@stopMusicDuet

 ->Chapter2Epilogue
 
 
 
 =Chapter2Epilogue
 
 @hide STEPHAN remove:true
 
 @char PROTAG.DEFAULT look:right pos:0,0 scale:1.2,1.2
 
 You welcome the propect of sleep, but not before you remove the letter from your coat, reading secret words bathed in firelight.
 
 @spawn Letter_002
 
 @wait i
 
 @resetText
 
 
    + [Burn it.] -> YouShouldBurnIt2
    
    + [Keep it.] -> KeepTheLetter2
    


=YouShouldBurnIt2

@despawn Letter_002

You take one last glance. 

Breathe. 

@sfx burn_letter

Then press the paper into the fire. 

It burns a little brighter and you feel a little warmer as your eyes give way to sleep.

@title

-> Chapter3



=KeepTheLetter2

@despawn Letter_002
 
You press the letter to your chest. 

Holding it tightly in place against the winter winds. 

There's a cold chill that sways you to sleep against the warmth of the fire.

@title

-> Chapter3
 
 
 
=Chapter3
@setDuetCharacter VERA
@hideAll remove:true
The bus rails on agains/
 
/"OI STRINGS!"
A voice cries out from across the way.
You look around for a moment, but find only a single figure besides yourself and the driver.
A teenage girl treading the boundaries of adulthood. She squints as you stare.
VERA: "YEAH YOU, HORSEHAIR!"
+"What?"
->YouComeFromUpNorth
+"I don't appreciate the tone."
VERA: "YEAH WELL I DON'T APPRECIATE YOUR LONG ASS FACE, I GOT A QUESTION FOR YA!"
+"Neigh."
VERA: "YEAH TROT ON BITCH I GOT A QUESTION FOR YOU!"
- +"What a great way to get an answer."
    +"Ask it then"
    - ->YouComeFromUpNorth
=YouComeFromUpNorth
Her face scrunches as she observes you for a moment, taking it all in.
VERA: "You come from up North?"
+"Yes and no."
VERA: "The fuck does that mean?"
    ++"Wasn't born there. Worked there a couple years. Moscow Orchestra."
    VERA: "Huh. Cool."
    She catches herself in a moment of sincerity.
    Then her face twists.
    ->YouSeeAnOldBitch
    ++"Means yes and no."
    Her distaste is palpable. Her face scrunches again.
    ->YouSeeAnOldBitch
+"Yes."
->YouSeeAnOldBitch
+"No."
She groans, stretching out her fingers and throwing her head back in frustration.
VERA: "FUUUUUUUUCK!"
She sighs.
VERA: "Fuck it."
->YouSeeAnOldBitch
+"Neigh."
She groans, stretching out her fingers and throwing her head back in frustration.
VERA: "FUUUUUUUCK!"
She sighs.
VERA: "Fuck it."
->YouSeeAnOldBitch
=YouSeeAnOldBitch
VERA: "You see an old bitch on your trip? Seventies? Big ass nose? Good shot? Apparently she's got a Mosins. {TakeTheRifle:  Just like yours.}
{TakeTheRifle: She points to the rifle on your back}.
+"Yes."
Her face gives way to shock as simmering eyes converge on yours.
VERA: "Where?"
    ++[Tell the truth.] "Donetsk."
    ->VeraTruth
    ++[Lie] "Kursk."
    ->VeraLie
    
+"No."
->NoOrNeigh
+"Neigh."
->NoOrNeigh

=NoOrNeigh
She groans into the back of the headrest.
->WhatsYourBeef

=VeraTruth
Her eyes widen. 
She pulls out a notepad and presses it against the headrest, scribbling letters on the page.
->WhatsYourBeef

=VeraLie
Her eyes widen. 
She pulls out a notepad and presses it against the headrest, scribbling letters on the page.
->WhatsYourBeef

=WhatsYourBeef
+"What's your beef, Drumsticks?"
"Fuck off Horseface."
But you've caught her in a moment of weakness.
+"Why do you ask?"
- VERA: "None of your fuckin business."
She pauses. 
Frustrated.
VERA: "I just wanna learn.
+"How to kill?"
VERA: "How to survive."
+"Better lessons out there."
VERA: "I'm not a coward."
- VERA: "Everythings falling apart and everyone's pretending it's not. I just want to do something. That's all."
Embarrassed by her own honesty, she turns away to stare against the horizon. 
The rage is still there, resting behind the eyes, but resting none-the-less.
+"I get it."
VERA: "Fuck off Shoestring."
    ++"That one's not even a pun."
    VERA: "You're not even a fucking pun."
    She smirks, stifling laughter.
    ->Silence
    ++"Aight, Whackstick."
    She smirks, stifling laughter.
    ->Silence
+Sit in silence.
->Silence

=Silence
Eventually, you settle into silence against the hum of the engine, your gazes drawn to the endless white horizon. Out of the corner of your eye, you see her soften at the sight. She's still a kid, tapping gently at the drum on her side.
+Play alongside her.
->VeraDuet
  @skip false
  
  @hideUI
  
  @spawn duetEasy
    
  @duet easy
  
 { 
    - duetScore >= 50: 
    
    -> VeraDuet
    
    - else:
    
    -> VeraNoDuet
}
+Sit in silence.
->SitInSilence

=VeraDuet

@playMusicDuet

@despawn duetEasy

@stopMusicDuet

You draw bow to string, expecting some grand rebuke. She offers nothing more than a petty scowl and a louder beat. An attempt to dissuade your communion. 

Drum beats back against melody. An uncommon pairing for uncommon times. She no longer fights against, but sets the lead and pace. You follow willingly.

You slowly start to dance against each other's sound, watching as the world you both once knew floats by on a sea of snow. 

It's still beautiful. 

You think you see her smile. 

Maybe. 

Probably not though.

@stopMusicDuet

->FarewellVera

=VeraNoDuet
You draw bow to string, expecting some grand rebuke. She offers nothing more than a petty scowl and a louder beat. An attempt to dissuade your communion. 

You play with sound and ryhthm but she sticks to her own. Tapping restlessly against the snare and the side of the bus.

She continues for the rest of the trip, fiery eyes gazing out over a sea of snow, refusing to meet your own.
->FarewellVera

=SitInSilence
She taps against the snare and the side of the bus, matching wardrum rhythm to the engine below. She continues for the rest of the trip, fiery eyes gazing out over a sea of snow.
->FarewellVera

=FarewellVera
Eventually, the bus starts to slow against the final bend of the road. Her fire returns as she pulls out a map from her coat pocket, scouring dogears and scribbled notes. This is your stop. Not hers. You weigh the weight of truth.
    + {VeraLie or NoOrNeigh} [Tell here the truth about Millia's location.] 
    "I lied before. I met her in Donetsk."
    ->VeraFinalTruth
    + {VeraTruth} [Lie about Millia's location.] 
    "I lied, before. I met her in Kursk."
    ->VeraFinalLie
    + {NoOrNeigh} "Good luck."
    VERA: "Cheers, Strings."
    + {VeraTruth} "Good luck in Donetsk."
    VERA: "Cheers, Strings."
    + {VeraLie} "Good luck in Kursk."
    VERA: "Cheers, Strings."
    - ->GoodbyeVera
=VeraFinalTruth
She stares you up and down as you gather your belongings.
VERA: "Appreciated, Strings."
->GoodbyeVera
=VeraFinalLie
She stares you up and down as you gather your belongings. 
VERA: "Appreciated, Strings."
->GoodbyeVera
      
=GoodbyeVera  
You move past her on your way to the front of the bus. A determined bundle of energy fumbling through pages of poorly kept notes.
+ "Don't die stupid."
VERA: "Go fuck yourself on a spindle, Strings!" 
+ "Go change the world, whack-a-mole!"
->Chapter3Epilogue
VERA: "Go fuck yourself on a spindle Strings!"
->Chapter3Epilogue
+ {TakeTheRifle} "You'll need this more than me." Give her the rifle. 
+ {TakeTheRifle and VeraFinalTruth} "Tell her thanks for the loan." Give her the rifle. 
+ {TakeTheRifle and VeraFinalTruth} "Tell her I'm sorry." Give her the rifle.
+ {TakeTheRifle and VeraTruth and not VeraFinalLie} "Tell her thanks for the loan." Give her the rifle. 
+ {TakeTheRifle and VeraTruth and not VeraFinalLie} "Tell her I'm sorry." Give her the rifle.
- ->GiveVeraRifle

=GiveVeraRifle
Her eyes widen and her mouth hangs agape. You feel like she hasn't been speechless in some time.
VERA: "Thankyou."
She smiles.
VERA: "Good luck.
->Chapter3EpilogueHijack

@back id:Bus_stop2  

@playBusStopSoundscape

@sfx  // Rock the Volume if you want

@sfx open_bus_door volume:0.5 time:4  //

@char PROTAG.DEFAULT look:right pos:25,0 scale:1.2,1.2

@sfx open_bus_door volume:0.5 time:4//Bus Door Closing Sound

=Chapter3Epilogue
You step back onto solid ground once more as the doors behind you hiss to a close. You turn back to face the window, where you're met by the sight of two middle fingers and two fiery eyes. She offers one last smirk as the bus curves around the trees and back out onto the empty road. 
Nothing to be done. You turn back to your own journey.

@back //Bus Exit Art

->Chapter4

=Chapter3EpilogueHijack

@back id:Bus_stop2 

@playBusStopSoundscape

@char PROTAG.DEFAULT look:right pos:25,0 scale:1.2,1.2

You step back onto solid ground once more as the doors behind you hiss to a close.

@sfx open_bus_door volume:0.5 time:4  //Bus door Volume

As you turn back to face the window, you're met by the sound of a cocking rifle from inside the bus.

@sfx Mosin_Pop_Clip  //Bus door Volume

VERA: "ALRIGHT FUCKBUCKET! LET'S TURN THIS STEEL TURD AROUND. WE'RE HEADING NORTH BABY!"
Grimacing as the driver's desperate eyes catch your own, you watch as the bus slowly reverses back the way it came.

@back bus_departure // Bus Exits

From a nearby window, a terrifying child offers a friendly wink before the bus disappears behind the bend.
Nothing to be done. You turn back to your own journey.

@bgm approaching_bus fade:1 //Background Music

@wait 2


->Chapter4

=Chapter4

@setDuetCharacter PASCHA

@startTrans

@finishTrans Crossfade time:3

@playWorldMusic

@wait 1

@char PROTAG.DEFAULT scale:1.2,1.2


As evening settles in, you cross the road to the next weary stop. It's empty. A strange, lonely reprieve from the recent norms of peculiar company. You scour the surroundings, until a familiar red hue pops out from the stump of a nearby tree. Warmth rushes through you as you pull the letter from it's hidey hole.

PASCHA: "Drugs?"

    @char PASCHA scale:1.2,1.2


    @bgm Pascha.Intro volume:0.5 time:4//If We have Intro


    You rise with a start against the sudden new presence. Turning back towards the stop, you see a figure where there was none before. Sharp eyes. Hair too soft. Skin too shiny. A perfect smile: charming and yet full of grit. They are, in a word, "eccentric".
    +[Nothing that interesting.]
    PASCHA: "I beg to differ. In fact I'd be more interested if it wasn't drugs. So now, I am thus, "more interested"."
    +[Just a letter.]
    PASCHA: "Well that is much more interesting."
    - They siddle slowly towards you.
    PASCHA: "Albeit, I wouldn't say no to drugs. I am desperately bored."
    Sharp eyes turn down towards the envelope in your hands.
    PASCHA: "Read it to me?"
    +[No.]
    Their eyes turn soft as they plead with you.
    PASCHA: "Pleaaaase?"
    ->AreYouHigh
    +[Why?]
    PASCHA: "I told you, I'm bored.
    ->AreYouHigh
    +[Fine.]
    ->PublicReading
    
=AreYouHigh

@startTrans

++[Are you high?]
        PASCHA: "I fucking wish, my dear. Now come, you sweet summer thing. Why not spill secrets with a stranger? 
            +++[Fine.]
            ->PublicReading
            +++[...]
            PASCHA: "Fine. I'll start." 
            They raise a dramatic hand against their forehead.
            "I'll be dead within a week." 
            ->IllBeDead

++[I don't even know you.]
        PASCHA: "Nor I you, you sweet summer thing. Why not spill secrets with a stranger?
            +++[Fine.]
            ->PublicReading
            +++[...]
            PASCHA: "Fine. I'll start." 
            They raise a dramatic hand against their forehead.
            "I'll be dead within a week."  
            ->IllBeDead
            
=IllBeDead



+[Sure you will.]

PASCHA: "Swear it on my grave. Dragged there by stupid, stupid queer love for stupid stupid queers."

+[How?]
PASCHA: "Stupid, stupid queer love for stupid stupid queers."
+[How dramatic.]
PASCHA: "Death is pure drama. But it's love that pulls me there. Stupid, stupid queer love for stupid stupid queers."
- They twirl towards you. Honest eyes stare into your soul.
PASCHA: "I travel weeks from Moscow. I get half a day's walk from the border and my fuckwit of a lover gets caught home. Now me and my moronic bleeding heart are stuck racing back into the dark."

They sigh.

PASCHA: "Anyway. Your turn!"

Before you can respond, they slip the letter from your grasp, laughing as they race back into the snow.

@spawn snow 

+[Chase them.]
You lumber through thick powder, but they dash away with a tedious ease, tearing open the envelope as they bound through the snow.

@sfx  // Snow Feet Music

+[Let them go.]

They settle a few feet away in the snow.

@sfx  //Feet In the Snow

Grounding themselves for some kind of performance as they rip the envelope open.


-PASCHA: "I'm about to cross into Abkhaz1a and I've just realised I've never heard back from you. It makes sense of course, me traveling ahead and all. But it's scary, terrifying, to never really know where you are. I w1sh I'd never asked you to stay in Moscow. I wish we'd left when you said we should. I'm 3orry. Please let me say that in person. Please let me meet you at the next stop. Please let us cross the rest together. I love you. I love you. I love you."

->PostPerformance


=PublicReading

    PASCHA: "Huh. That was easy."
    
    You sigh as you rip open the envelope, eyes greeting familiar, comforting handwriting.

    @spawn Envelope
    
    "I'm about to cross into Abkhaz1a and I've just realised I've never heard back from you. It makes sense of course, me traveling ahead and all. But it's scary, terrifying, to never really know where you are. I w1sh I'd never asked you to stay in Moscow. I wish we'd left when you said we should. I'm 3orry. Please let me say that in person. Please let me meet you at the next stop. Please let us cross the rest together. I love you. I love you. I love you."
    
    ->PostPerformance
    
    =PostPerformance

    @despawn Envelope

    They sigh and stare at you with sad eyes.

    PASCHA: "Truly, we are kin both bound by dumb love. You for freedom, and I for the fire. Guess I should've chosen a smarter lover."
    +[I'm sorry.]
    PASCHA: "Don't be. Jealousy among outcasts is a tedious thing. I am happy for you."
    They smile.
    PASCHA: "Truly."
    +[You don't owe them your life.]
    PASCHA: "Oh but I do. Love's a fickle fiend. Take comfort that we'll never truly die, I suppose. Long as they keep breeding, we'll be being bred. Duality of man and all that."
    They chuckle.
    - PASCHA: "Play a dirge with me? Some battle song to feed me fake courage?"
    +[I won't entertain a deathwish.]
    PASCHA: "You are a cruel thing, my dear."
    ->PaschaPlaysAlone
    +[Of course.]
    PASCHA: "Then let's dance the night away."
    ->PaschaDuet

    @spawn duet


    @back PASCHA.DUETLOOP //Pasha DUET art.

    @sfx //pasha duet audio if any
     //START DUET SYSTEM - IF SUCCESUL GO TO =PaschaDuet - IF UNSUCCESFUL GO TO =PaschaNoDuet
    

    {
    - duetScore >= 50: 
    
    -> PaschaDuet
    
    - else:
    
    -> PaschaNoDuet


    }

    =PaschaDuet
    
    @playMusicDuet

    @despawn duet

    They take you by the hand and out into thick snow.

    @spawn snow //Snow spawn to make it look preeetty

    As you press forward with bow, They strum aginst string and twirl in the night.
    
    They exude joy. Not as a facade, but as someone who's found it in all the joyless places.
    
    It's inspiring.
    
    And so you dance as you play.
    
    @wait 1

    For hours.
    
    @wait 1

    And hours.
    
    @wait 1

    You twist angels in the fresh laid snow and music in the clear moon sky.
    
    It's no dirge, no battle song, but rather a ballad for all the queers that ever did queer.
    
    You could swear for a moment you felt the earth shift, before evening takes hold and you both collapse harmlessly into a long night's sleep.
    
    @stopMusicDuet
    
    ->Chapter4Epilogue
    
    =PaschaNoDuet

    @despawn duet


    They take you by the hand, leading out into the cold snow.

    @spawn snow //Spawning snow
    
    You raise bow to string as they strum gently on their own.
    
    The intent is clear and true, but the weight of their tomorrow lies heavy upon them.
    
    They stop. They smile gently towards you.
    
    PASCHA: "Thankyou for trying. Seems I need solo stage tonight. Drama queen that I am."

    ->PaschaPlaysAlone
    
   =PaschaPlaysAlone

   @playMusicDuet
    Pascha plays alone. 
    
    You see faint tears catching moonlight. 

    @back //Moonlight Art if any
    
    You stay awake beside them until the grasp of exhaustion drags you both into slumber.
    
    @stopMusicDuet
    ->Chapter4Epilogue
    
=Chapter4Epilogue

@bgm approaching_bus fade:1

@playBusStopSoundscape

@back id:Bus_stop2 //Bus Stop 3 Art Replacement

Their bus arriving earlier than your own, your momentary companion rises early against the dawn. Their skin is still too shiny and their hair is still too soft, but at least their sharp eyes seem somewhat weary. They smile as you wake to bid them farewell.

+ {TakeTheRifle and not GiveVeraRifle} "Here you go Rambo." Give them the rifle.

@back PASHA.rifle // Pasha With the rifel.

->GivePaschaRifle
+[Don't die stupid.]
PASCHA: "Oofy doofy. That is a tall order my dear."
They chuckle.
PASCHA: "I'll try."
+[Good luck.]
PASCHA: "I'm sure your well wishings will make all the difference, my dear.
They chuckle, warmly.

- As their bus presses round the corner, they step off the platform and into the snow leading to the roadside.

@sfx //Footsteps in the snow

->Father

=GivePaschaRifle
They take the rifle with both hands, toying with it for a moment before nodding graciously and strapping it to their back.
PASCHA: "S'pose I'll be getting more in touch with my Masculine side."
As their bus presses round the corner, they step off the platform and into the snow leading to the roadside. ->Father

=Father
+[I think I met your father.]
They don't stop walking, but a voice responds.
PASCHA: Truly, a real cunt of a man.
    ++[I've never seen so much regret.]

    @sfx open_bus_door volume:0.5 time:4  //Bus door Volume

    They stop. 
    The bus waits. 
    They wait.
    PASCHA: "Thank-you."

    @char PASHA.DEFAULT fade: 1 //Pasha Enters Bus and Fades

    And press forth once more, rising on steel steps and disappearing behind glass doors.
    
    @sfx open_bus_door volume:0.5 time:4  //Bus door Volume

    @back id:bus_departure fade: 1 // Bus Departure Scene

    ->Chapter4StephanEnd
    ++[Yeah...]
    ++[...]

+[Let them go.]

-And they press on through the snow.

@sfx //Footsteps In the snow

@sfx open_bus_door volume:0.5 time:4  //Bus door Volume


Up the stairs.


Disappearing behind glass doors.

@sfx open_bus_door volume:0.5 time:4  //Bus door Volume

@back id:bus_departure //Bus Departs and Exist to infity

@camera zoom:out 

->Chapter4MoscowEnd

=Chapter4MoscowEnd

An hour later, your bus arrives: a quiet, lonely shuttle to the edge of the Union's remains. 

@bgm approaching_bus fade:1 // Bus approaches

Abkhazia awaits.

And so do they.

@sfx open_bus_door volume:0.5 time:4  //Bus door Volume

@back id:bus_departure //Bus Departs and Exist to infity

@camera zoom:out 

->Chapter5

=Chapter4StephanEnd

An hour later, your bus arrives: a quiet, lonely shuttle nearing the edge of the Union's remains. 

@sfx open_bus_door volume:0.5 time:4  //Bus door Volume

@camera zoom:out 

Abkhazia awaits.

@wait 1

And so do they.

@back id:bus_departure //Bus Departs and Exist to infity

->Chapter5

=Chapter5

@setDuetCharacter ZURAB

@startTrans

@bgm approaching_bus fade:1

@back id:BusInterior // When Travelling in a bus

@char PROTAG.DEFAULT scale:1.2,1.2 // Scaled to normal.

Abkhazia rolls in on the horizon. The Dead Sea breeze stings your nostrils. You stare out down the road ahead. They should be here.

They should.

They will.

...

They aren't.

As the bus pulls up and you stumble out into the street, the stark absence of them cuts you deeper than any winter wind.

+[Look for anyone. Anyone at all.]
Your eyes dash out against the cold horizon, marked only by the bus stop before you. They dart to and from every corner of the world, before resting on a figure hidden behind a bus stop pillar.

@char PROTAG.DEFAULT look:right pos:25,0 scale:1.2,1.2

+[Call out their name.]
You cry out into the wind. You could swear it picks up a chill in response. 
Silence.

Then.
"Hello?"

Your eyes flash towards the source of the voice, a figure resting behind a pillar of the bus stop.

- Your smile widens as you race towards them.

@char PROTAG.DEFAULT pos:25,0 scale:1.2,1.2 // Protags Default Sprite

The figure emerges.

You pause in your tracks.

@wait 2

It's not them.

@spawn DepthOfField

ZURAB: "Ah. You're not looking for me, are you?"

An old man, not so much disappointed as embarrassed. 

@char ZURAB.sad look:back pos:25,0 scale:1.2,1.2

A slightly curved back and open eyes. They carry with them a pointed stick and a rubbish bag half full to the brim. 
As they wait for your response, they prick a metal can and shake it into the bag.

@wait 2

+[No. Sorry.]
ZURAB: "Haven't seen anyone else yet, I'm afraid."

+[Have you seen anyone else here today?]
ZURAB: "No. Sorry."


-He examines you. 
ZURAB: "Come take a seat. You look weary."

@spawn BreezeReturns
The invitation is as alluring as the seat itself.

@spawn seat 
You take him up on the offer, grateful even for flimsy shelter from the cold sea breeze.

@despawn BreezeReturns

The man gently careers around the perimeter of the station, pricking cans of steel and plastic wrapping from the ground.
ZURAB: "You've come a long way?"

+[And then some]
ZURAB: "And still some yet, I'm sure."
+[Not really.]

ZURAB: "Eyes say otherwise."
-He taps the side of the station.
ZURAB: "Seen many of these?"
+[Too many.]
+[Not enough yet.]
- His eyes enquire of yours with a strange sincerity.
ZURAB: "Do you like them?"

+[Yes.]
He smiles.
ZURAB: "So do I."
+[No.]
ZURAB: "Ahh. I'm sorry to hear that."
@wait 2


    ++[Not their fault. Not yours. Just want to leave them all behind.]
    ZURAB: "Understandable.
    ++[No one's at fault for that.]
    ZURAB: "Perhaps the artist is?"
+[I hadn't thought about it.]
ZURAB: "Who could blame you? They're just bus stops."

- His eyes drift to the top of the station. Cracked, weathered paint withers away on the concrete.
ZURAB: "I just hope they remain a little longer than the world that built them. The absurdity. The color. Everything else is falling apart and everyone else is itching to paint our soon to be history in black and white and grey and anything but color. But we were red and gold and green and blue and anything and everything."
@char ZURAB.SmilesGently
He smiles.

ZURAB: "I want us to outlive the fall. Language, art, music, silly bus stops. That might be all we have against a world that wants to sees us mute and all a wash. I want them to know that we were here, and we were wonderful. To see us as we were and are."

@char ZURAB.smile
The smile fades to embarrassment, he pricks another can on the ground.
ZURAB: "Fickle artsy fartsy fancy, I know."

@resetText 

+[What isn't?]
ZURAB: "Who can say?"
+[I understand.] -> ZurabNoDuet
He smiles again.
+[A little.] -> ZurabDUET
ZURAB: "Only a little?"
He chuckles.
- His eyes wander back towards you.
ZURAB: "Play a song with me? Just until the end times?"
+[Haven't the energy.] ->HisEyes
ZURAB: Then I'll play for you.
->HisEyes
+"Sure."
->ZurabDUET

     //START DUET SYSTEM - IF SUCCESUL GO TO =PaschaDuet - IF UNSUCCESFUL GO TO =PaschaNoDuet
{
    - duetScore <= 5:
    -> ZurabNoDuet
    - else:
    -> ZurabDUET

}

@skip false

@stopBonfireSoundscape

@stopWorldMusic

@hideUI

@spawn duet

@duet
    

=ZurabDUET

@playMusicDuet

@despawn duet

@char PROTAG.DEFAULT look:right pos:25,0 scale:1.2,1.2

@char  Zurab.DEFAULT look:left pos:75,0 scale:1.2,1.2

@camera offset:-2.75,-1.5 zoom:0.25

His smile widens into a childlike grin, and you lose yourselves in the nothingness of fancificul sound.

He strums with weary hands abound with energy.

The world becomes more vibrant, color dances on the rooftop.

Concrete cracks heal over and grass grows through the morning snow.

You think you see a sun behind the sky, and for a time, the world is brighter, if only in your minds.

@stopMusicDuet

->Them

=ZurabNoDuet
He inspects you, the bags under eyes, the damp on your clothes.

ZURAB: "You've come so far. Rest. I'll play for you."

->HisEyes



=HisEyes

@despawn duet //Aftermath of Duet

He eyes you from his periphery as he plays, matching tune to meet your face.

But you can barely hear the tune.

Your mind is stuck in questions of where they are.

If they're still coming.

If waiting is worth the wait. 
They should be here.


They should.
++[They will].->Them

=Them
Footsteps meet the beat of the song, faint crunching of snow in the distance. Your eyes rise instantly to face a silhouette in the light. But this time you know the shape.


You rise.

They run.

They step out from the blinding sun.

You stare.

They stare.

You cry.

They cry.

@resetText 

    +[I'm sill fucking freezing.]
    +[You're late.] -> YouWereWonderful
    +[Fuck you.]



- You embrace

You hold each other till the end times.

The man smiles in the background.

The sound of an engine rumbles round the corner.

You wait together.

You smile together.

You step onto steel platform and behind glass doors.

Together.

Of course.

You hold hands.

++[No one dares to stop you.]

->YouWereWonderful

=YouWereWonderful

+You were wonderful.
++And you were here.
->Finale

=Finale
You travel long into the night and then again into the morning. You rest against eachother as the bus rumbles across the border into Turkey. You meet with family in Rize and they take you both in with open arms. You rest. You recover. And when you return to the world together, it welcomes you with infinite possibility. 

{VeraLie and not VeraFinalTruth or VeraFinalLie or NoOrNeigh and not VeraFinalTruth:With the collapse of the Soviet Union, Millia found herself torn between the country of her birth and the country that had taken her in. Tired from a life of service, she resolved to remove herself from direct connection to either, living the rest of her days amidst the wilds between both nations.}

{VeraFinalTruth or VeraTruth and not VeraFinalLie:Following your guidance, Vera connected with Millia just as she was due to return to Moscow. Intrigued by the girl's tenacity, Millia took her on as an apprentice, and Vera readily accepted. The two travelled to Moscow, but with the collapse of the Union, they soon found themselves safer in the wilds between Ukraine and Russia.}

{VeraFinalTruth or VeraTruth and not VeraFinalLie:Here, Millia passed on a lifetime of political and military knowledge. Eventually, Vera returned to her home town, where she joined the armed forces. Tired from a life of service, Millia lived out the rest of her days in the wilds.}

{VeraLie and not VeraFinalTruth or VeraFinalLie or NoOrNeigh and not VeraFinalTruth:Following your choice not to divulge the truth about Millia, Vera continued her search alone. But as the Soviet Union continued to deteriorate, she was eventually forced to return home by a combination of fear and exhaustion.}

{VeraLie and not VeraFinalTruth or VeraFinalLie or NoOrNeigh and not VeraFinalTruth:Frustrated, Vera found solace in music. Her obsessive nature lent itself well to the artform, and in time she would find herself performing with and for Orchestras all around Europe.}

{not Chapter4StephanEnd:Stephan returned to the bus stop, every day without fail. He started playing music while he waited, finding joy in the tiny stories of those that passed his tiny town. Many a weary traveller found momentary refuge with him and his wife.}

{not Chapter4StephanEnd:Stephan's child never returned. He bore his regret for the rest of his life.}

{Chapter4StephanEnd:Stephan returned to the bus stop every day without fail. He started playing music while he waited, finding joy in the tiny stories of those that passed his tiny town. Many a weary traveller found momentary refuge with him and his wife.}

{Chapter4StephanEnd:Pascha, spurred by your last refrain, took a pitstop on their way to Moscow. They returned home to a weeping father and mother, who held them close until the dawn. Stephan apologised a thousand times. Forgiveness came slowly, but it came. They healed together.}

{Chapter4StephanEnd:When Pascha's journey turned to Moscow, Stephan accompanied them. Together, they freed Pascha's lover and returned home, where they lived long and happy lives, filled with love.}

{not Chapter4StephanEnd and GivePaschaRifle:Spurred by your encounter, rifle in hand, Pascha stormed Moscow with a deathwish. They'd never planned on surviving, they'd never been told that was possible.}

{not Chapter4StephanEnd and GivePaschaRifle:But they did.}

{not Chapter4StephanEnd and GivePaschaRifle:They freed their lover and together they fled north into Finland and on into Sweden. They lived a long and happy life, a concious choice in spite of expectation}

{not Chapter4StephanEnd and not GivePaschaRifle:Spurred by your encounter, Pascha stormed Moscow with a deathwish. They'd never planned on surviving, they'd never been told that was possible. They found their lover, they freed them. But the two were never heard from again.}

Zurab lived out the rest of his days in Abkhazia. He cleaned the bus stop daily. He taught art, and made music, and spurred into creation a thousand tiny relics of a world that was. Zurab didn't hate the new world, only saught to remember where it came from. His ambitions were carried forward by his students.

->BonfireFinale

=BonfireFinale

The finale cinematic.
@spawn

@startTrans

->DONE














