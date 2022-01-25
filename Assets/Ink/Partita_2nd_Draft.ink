/ ART A silhouette of two figures huddled over a fire in the snow.
///@back Campfire
///@sfx Burning Bonfire + Snow Fall + Night Ambient Music
Winter curls around your ankles, biting at the flame that binds you.

"It's time to go."

"I'm not ready."

"Who is?"

"I'm cold."

"Everyone's cold."

"Well I'm fucking freezing."

"It's winter."

"Hold me."

"-"

"Please."

"We don't hav-"
///@bgm Romance BGM
You embrace.

"We have to go."

"I know."

"I'll leave letters.”

"I know."

“Find them.”

“I know.

"I'll see you soon."

"I know."

"I love you."

"I love you."
->Chapter1
///@stopBgm Romance
//Fade into 1st Bus Station.
@startTrans
@back busstop1
//; insert bgm here
@finishTrans CircleReveal
=Chapter1
/ART 1st Bus Station. 

/Protag 1 exits the bus. The bus leaves. They meet Millia with a Sniper Rifle.
///@sfx Snow Fall
Weary eyes pierce through glaring snow, as your feet touch soft powder. An ancient woman sits, alert, back pressed against concrete wall. In her arms she cradles a rifle, trained delicately on your approach.

    Her voice is backed with a surprising vigor.

@printer Wide
@char Millia scale:1.2 avatar:Protag
MILLIA: "No."
// This flow seems kind of abrupt -sid
+[Stay Silent]
->SilentType
+Yes.
She scoffs and flips her scope in your direction.
MILLIA: "Say that again?"
///@bgm Tense
    ++[Stay Silent]
    ->SilentType
    ++No.
    Her eyes roll. She bemoans on an exasperated sigh.//Can we have animation of this? 
    ///@stopBgm Tense
    MILLIA: "Boring."
    She flips the rifle back towards herself, where it settles harmlessly in her lap.
    ->WhereFrom
    ++Yes.
    BANG!
    A bullet flies towards you, grazing the outside of your neck. 
    She laughs.
    You feel a trickle of blood.
        +++Take a step towards her.
        ///@sfx gunshot 
        BANG! 
        Another bullet. Cutting delicately into the skin of your cheek
        Another laugh. 
        Another trickle of blood.
        Then silence.
        ///@stopBgm Tense
        She stares. 
        Curled lips smirk in amusement.
            +++If you wanted me dead, I'd be dead.
            ->IfYouWantedMeDead
            +++What the fuck!?
            ++++Run at her.
            Are you sure?
                +++++Yes.
                ->MilliaDeath
                +++++No.
                ->WhatTheFuck
        ++What the fuck?
    ++Take a step towards her.
    BANG! A bullet flies towards you, grazing the outside of your neck. 
    She laughs.
    You feel a trickle of blood.
        +++Take another step towards her.
        BANG!
        Another bullet. Cutting delicately into the skin of your cheek.
        Another laugh. 
        Another trickle of blood.
        Then silence.
        She stares. 
        Curled lips smirk in amusement.
            +++If you wanted me dead, I'd be dead.
            ->IfYouWantedMeDead
            +++What the fuck!?
            ++++Run towards her.
               Are you sure?
                +++++Yes.
                ->MilliaDeath
                +++++No.
                ->WhatTheFuck
        ++What the fuck!?
        
- ->WhatTheFuck

=MilliaDeath
You press one foot down at an angle into the snow, propelling yourself forward as the woman's eyes shift from amused to a terrifying focus. She tilt her scope toward you, and before you can bring your next leg forward, a bullet flies clean through your sk-

/ART Cut to black.
Everything.
Ends.
+[Try again.]
->Chapter1

=WhatTheFuck
She howls with laughter. Her head reels back to the sky and she splutters for a moment before it returns.
MILLIA: "You play with fire."
The rifle remains poised, she steadies her aim in between the first and second shot.
Her finger lies pressed calmly on the trigger.
She smirks.
And flips it back over her shoulder as her lips give way to a wry smile.
"I'm entertained."
->WhereFrom
        
=IfYouWantedMeDead
Her smirk gives way to laughter. 
She releases the rifle and raises her hands as it falls into her lap.
MILLIA: "You got me."
->WhereFrom

=SilentType
MILLIA: "Silent type?"
        +[Stay Silent]
        MILLIA: "-"
            ++[Stay Silent]
            MILLIA:"-"
                +++[Stay Silent]
                The woman stares into your soul, her eyes refuse to blink as they start to water in the snow. Tiny teardrops freezing over wrinkled skin. 
                    ++++Are we done?
                    MILLIA: "HAH! I WIN!" 
                    The woman grins with childlike ambition as one hand wipes the frozen salt from her eyes.
                    ->WhereFrom
                    ++++[Stay Silent]
                    Your eyes both remain unblinking, tears forming on the boundaries of your vision in some ridiculous pissing contest. You lock gazes against the snow, frozen salt blinding you both until your bodies give way and force your hands to wipe your eyes.
                    
                        As your vision returns, the sound of spluttered laughter carries across the distance between you. A smoker's lung.
                        ->WhereFrom
                +++Are we done?
                MILLIA: "I don't know. Are we?"
                    ++++What are you, a child?
                    MILLIA: "Just a very bored old woman."
                    ->WhereFrom
                    ++++True.
                     MILLIA: "Well aren't you agreeable?"
                     ->WhereFrom
                    ++++You're holding a rifle.
                    MILLIA: "Correct. Mosin's, to be exact."
                    ->WhereFrom
            ++This is ridiculous.
            MILLIA: "You started it."
                +++What are you, a child?
                MILLIA: "Just a very bored old woman."
                ->WhereFrom
                +++True.
                MILLIA: "Well aren't you agreeable?"
                ->WhereFrom
                +++You're holding a rifle.
                MILLIA: "Correct. Mosin's, to be exact."
                ->WhereFrom
        +Just scared.
        "Smart."
    ->WhereFrom
    
=WhereFrom 
Old eyes slowly scan you.
Your face
Your body. 
Your posture. 
Your response.
MILLIA: "Where are you from?"
    Moscow."
    Her gaze converges on yours, she's intrigued.
    MILLIA: "And before that?
        +Not Moscow.
        She scoffs.
        MILLIA: "Well I suppose you're not lying at least."
        +None of your business.
        She squints, digging deeper for a moment. 
        MILLIA: "Well. Okay then."
        +Where are you from?
        "Moscow nowadays. Born in Bila Tserkva. But who knows what we'll be calling either of those a year from now."
    None of your business."
    She squints, digging deeper for a moment. 
    MILLIA: "Well. Okay then."
    Where are you from?"
    MILLIA: "Moscow nowadays. Born in Bila Tserkva. But who knows what we'll be calling either of those a year from now."
    
    - ->MilliaFire
    
=MilliaFire
Her eyes soften, if only slightly. The rifle lies harmlessly in her lap.

MILLIA: "You must be cold. I'll start a fire.

After a moment of wariness, the woman's shifting demeanor seems genuine. You help her gather firewood, and as you reach for a pile of kindling, a thin red envelope catches your eye amidst the shrubbery.

You know that this was meant for you.

You open it, and familiar handwriting sends warmth wrapping round your body.

LETTER: I met a girl with a brimstone 4eart. Fire in her veins that rushed to kindle dead eyes. She was looking for someone. Filled with hate, or passion, I couldn't tell which. I haven't seen that kind of drive in years. Everyone el5e I've met seems dead or dying. She reminded me of you, in a way. I asked her if she'd seen you. She said no. It stung. She wanted to be kind, I think, I think sh3'd forgotten how.

MILLIA: "Letter from a lover?"
    +Yes.
    She smiles, gently. 
    MILLIA: "You should burn it."
    +No.
    She smiles, knowingly. 
    MILLIA: "You should burn it."
    +[Stay Silent]
    She smiles, solemnly. 
    MILLIA: "You should burn it."
    - ->YouShouldBurnIt
    
=YouShouldBurnIt
    Can't."
    MILLIA: "If you're finding letters stashed in bus stops, they're worth burning. Memorise, then burn"  
    Why?"
    MILLIA: "If you're reading letters stashed in bus stops, they're worth burning. Memorise, then burn."
    I know."
    MILLIA: "I'm sure you do. Take the time to memorise."
- ->YouCouldHaveKilledMe

=YouCouldHaveKilledMe
    As you sit in silence against the coming night, the woman cradles the fire, eyeing off the letter in your hands with unbridled curiosity.
    
    After some time, her eyes turn back to the flame. She seems lost in the sight, some strange nostalgia gripping at her mind. 
    
    For a second you think you see a tear, but she catches your stare with a smirk and a wiped eye before you can look closer.
    
    +Why didn't you kill me?
    MILLIA: "I've killed thousands for a country that's about to implode anyway. It's no fun anymore. There's nothing to be won. No grand prize. No medal. No accolades. Just a world wasting away on shriveled ambition."
    
        Her eyes rise to laze on the horizon.
        
        "Besides. You seemed harmless enough."
        
        ++Play her a song.
        You gently curl your backpack towards the ground, pulling delicate red timber to your chin. As you raise a bow to match with string, you can't help but feel a little more alive.
        ->MilliaSong
        
        ++We should sleep.
        MILLIA: "True. I'll keep first watch."
        ->MilliaNoSong
        
    +Do you like music?
    MILLIA: "I adore it"
    You gently curl your backpack towards the ground, pulling delicate red timber to your chin. As you raise bow to match with string, you can't help but feel a little more alive.
    ->MilliaSong

    +We should sleep.
    MILLIA: I'll keep first watch.
    ->MilliaNoSong


=MilliaSong
/MUSIC PROTAG 1 plays Viola solo, eventually MILLIA joins in with her own instrument.

The fire dances as your. Tiny ripples of sound from the woman to you and you to her, each carving a path through the fire. Smoke ebbs and flow as it rises into the clouded sky.

As the music comes to a close and your eyes give way to sleep, you turn your attention to the fire, letter dangling in your lap.
->1stLetter

=MilliaNoSong
As your eyes give way to sleep, you turn your attention to the fire, letter dangling in your lap.
->1stLetter

=1stLetter
+Burn it.
You take one last glance. Breathe. Then press the paper into the fire. It burns a little brighter and you feel a little warmer as your eyes give way to sleep.
+Keep it.
You press the letter to your chest. Holding it tightly in place against the winter winds. There's a cold chill that sways you to sleep against the warmth of the fire.

/ART Fade to black/white OR Millia sleeping

- You wake before the birds, your eyes pressed open against the weight of the world. Beside you, the sleeping woman's rifle lies loose. There should be a bus arriving soon.
    +Take it.
    ->TakeTheRifle
    +Leave it.
    You consider the thought.
    And let it pass.
    In the distance, an engine rumbles on approach.
    You turn back to the roadside and press on into morning.
->Chapter2

=TakeTheRifle

    You tread carefully on fresh snow, sidling your way longside the first birdcall. 
    She mumbles.
    You freeze.
    You wait.
    You hear the rumbling of an engine in the distance.
    Your hands stretch out against the dying night, pulling the rifle towards you.
/ART Change main character sprite, add Rifle.
    She sleeps.
    You turn towards the coming bus.
    ->Chapter2

=Chapter2
//@back bs_2_g
The road is long and monotonous. Grey snow falls against a dull backdrop. Faint hints of color splash amidst the skyline and before you know it, the ride is over.

@char Stephan
As you press on to sleeted ground, a man waits nearby. Older, but not elder, he's frail if not determined in demeanor. Grey eyes seem to glance towards you, then behind you to the closing door. 

As the bus departs, his gaze follows. 

In one hand he holds a slowly burning cigar. In the other, a letter.

Your letter.

I think that might be for me."
Weary eyes smile at you.
STEPHAN: "I think you might be correct."
He extends the letter out towards you. You take it, nestling it inside your coat.
The man watches with warm intrigue.
->MindIfIAskWho
+Stare at him.
As the bus dips behind a cradle of trees, the man's eyes return to lock with yours. He smiles. It's surprisingly warm against the cold air.
"I'd thought this was for me. Seems I was wrong."
He raises the letter towards you.
You take it. Nestling it inside your coat.
->MindIfIAskWho
+ {TakeTheRifle} Train your rifle on him.
His arms fly upwards, eyes full of fear. The letter dangles gently from one hand, as smoke rises from the cigar in the other.
    +Drop it."
    STEPHAN: "I haven't opened it."
    His voice is tired.
    STEPHAN: "I thought it was for me."
        ++It's not."
        A wry, forced smile. 
        STEPHAN: "I know."
            +++Drop. It."
        He drops the letter. His hands remain pressed toward the sky.
            ++++Grab the letter.
        You rush forward and swipe the letter from his hands.
        ++Drop. It."
        He drops the letter. 
        His hands remain pressed toward the sky.
        You rush forward and swipe it from the snow.
    ++Move closer.
    He winces at the approach.
        +++Grab the letter.
        You rush forward and swipe the letter from his hands.
        ++Give me the letter."
        He holds a palm out towards you. It trembles in the cold.
        You walk forward, rifle still trained as you swipe the letter from his palms.
        - ->IDontWantTrouble


=IDontWantTrouble
STEPHAN: "I don't want trouble."
Sorry. Tense."
STEPHAN: "Tense times. Can hardly blame you."
Neither do I."
STEPHAN: "And so we've reached an accord. No bullets then?"
He chuckles to himself, vaguely masking a residual fear.
+Look away.
STEPHAN: "I really didn't read it, if that helps? Seemed pretty well hidden too so I doubt anyone else has."
- ->MindIfIAskWho

=MindIfIAskWho
His eyes laze towards the letter.
STEPHAN: "Mind if I ask who?"
    +..."
    STEPHAN: "I understand."
    ->AndYou
    +I shouldn't say."
    STEPHAN: "I understand."
    ->AndYou
    +A lover."
    STEPHAN: "A secret lover!"
        ++That or dead."
        His tone softens.
        STEPHAN: "Ah."
        His face hardens.
        STEPHAN: "Heading south then?"
        ->South
        ++I suppose you could say that.
        STEPHAN: "Heading south amidst the chaos?"
        ->South
        
=South  
..."
->YouSmoke
+I shouldn't..."
->YouSmoke 
Through Abkhazia. Then on to Turkey. I have family there."
STEPHAN: "Rough road. Suppose they all are right now."
->AndYou
        
=YouSmoke
The man glances towards your face for a moment. He nods and smiles. 
STEPHAN: "Sorry. I'll drop it."
He leans back, head to the sky, dragging in a mound of smoke off the end of his cigar before exhaling. You watch it dance against the fog.
STEPHAN: "You smoke?"
    +No."
    STEPHAN: "Ah."
    He presses the cigar down against his coat.
    STEPHAN: "Sorry about that."
    ->AndYou
    +No. Thank You."
    STEPHAN: "Ah."
    He presses the cigar down against his coat.
    STEPHAN: "Sorry about that."
    ->AndYou
    +I do."
    Cigar in mouth, he flicks open a small box from his trouser pocket. From inside his coat he removes a lighter and strikes a light. 
    "Here. Take the edge off."
    You take the cigar and inhale. There's comfort in the heat.
    ->AndYou
        
=AndYou
+Who are you waiting for?
STEPHAN: "My son."
+Why wait in the cold? That was the only bus for hours.
STEPHAN: "Just tradition."
He laughs at himself.
STEPHAN: "My son."

- STEPHAN: "They're not coming through."

Sorry for your loss."
STEPHAN: "Oh they're not dead. Least I hope not."
Sorry."
STEPHAN: "Don't be. My own damn fault."

- STEPHAN: "You have a child that outruns the world and sometimes you just can't keep up. Sometimes you just say terrible things. I don't blame them for running off.  Gifted kid. Musical too. Brilliant bassist, I hate that instrument but God they were good. I just hope they're ok. Last I heard was Moscow. That was years ago."
City for the strays."
STEPHAN: "Sounds like them."
I lived there. Before all this."
STEPHAN: "Ah! Wouldn't have guessed."
 - A cold breeze rushes past you and wind whips powdered snow around your feet.
 STEPHAN: "Ahhh. I guess we're done for the day."
 I've got a bus to catch."
 STEPHAN: "You'll freeze before it comes. Stay with my wife and I. Rest. Recover."
 It was nice to meet you."
 STEPHAN: "You'll freeze before the bus comes. Stay with my wife and I. Rest. Recover."
- The breeze returns with harsher ambition.
 I appreciate the concern. I'll be ok."
 He frowns with furrowed brow.
 STEPHAN: "Yellow house. Just behind the bend due south. Don't die stupid."
 As he goes to leave, he waves with his opposite hand, and you spot fractured stubs on the tips of his fingers. Frostbite.
 ->SoloSnow
 As long as I'm not imposing."
 The man grins. 
 STEPHAN: "Impossible. Come on then."
 ->ThroughTheSnow
 
 =SoloSnow
 You wait in silence shattered by whistling wind. The storm rises as you pull your scarf to cover your face.
 +Stay.
 Your vision starts to cloud in blanket white, snow no longer merely bites at your ankles but rises to surround you on all sides. The cold cuts at your skin.
    ++Stay.
        You can feel your blood start to freeze, your skin frosting over. There's a dangerous illusion of warmth as you begin to lose perception of your ligaments.
        +++Stay
            Are you sure?
            ++++Stay
            ->Death2
            ++++Leave
        +++Leave
    ++Leave.
 +Leave.
 - ->ThroughTheSnow
 
  
=Death2
/ART Fade to white 

Your eyes give way to pure white. 
Cold gives way to warmth. 
Light gives way to dark. 

/ART Fade to white

The pain.
Ends.
+Try again.
->Chapter2
 
 =ThroughTheSnow
 /ART Fade to white
 You trudge against the storm. Yellow paint cuts through the white like a knife, glistening in the distance. The walk is hard, but short. 
 /ART prelude Bonfire
 The man and his wife brighten in your presence. The house is warm. The food is warm. You feel both full and light. 
 STEPHAN: "Play with me."
 Why not?"
 ->StephanDuet
 
 
 I should sleep."
 The man face betrays his dissapointment, but he smiles regardless.
 STEPHAN: "I understand. Goodnight."
 ->Chapter2Epilogue
 
 
=Chapter2Epilogue
You welcome the propect of sleep, but not before you remove the letter from your coat, reading secret words bathed in firelight.
 
 LETTER: I miss you. I miss Moscow. I miss music and symphonies and sound that isn't wailing wind. I want to play music again. Prefferably, inside. I'd always loved wint3r before. Before I'd spent it flailing on the roa7. I hate it now. It's cruel and unkind. I want to see you. I will. 5tay safe. 
 
+Burn it.
You take one last glance. Breathe. Then press the paper into the fire. It burns a little brighter and you feel a little warmer as your eyes give way to sleep.
+Keep it.
You press the letter to your chest. Holding it tightly in place against the winter winds. There's a cold chill that sways you to sleep against the warmth of the fire.
-
 
 /ART Fade in on Chapter 2 Setup with STEPHAN and PROTAG 2. PORTAG 2 gets on the bus. The bus leaves.
  ->Chapter3
  
 =StephanDuet
Stephan Protag 2 DUET
 In this brief reprieve, music mutes the outside air. You play until the storm recedes. You laugh, and eat, and drink. It's remarkably simple, and beautifully melancholic.
 ->Chapter2Epilogue
 
 =Chapter3
 /ART Bus traveling. Vera and Portag 2.
 
 The bus rails on agains/
 
/"OI STRINGS!"
A voice cries out from across the other side of the bus. 
You look around for a moment, but find only a single figure besides yourself and the driver.
A teenage girl, treading the boundaries of adulthood. She squints as you stare.
VERA: "YEAH YOU, HORSEHAIR!"
What?"
->YouComeFromUpNorth
I don't appreciate the tone."
VERA: "YEAH WELL I DON'T APPRECIATE YOUR LONG ASS FACE, I GOT A QUESTION FOR YOU!"
Neigh."
VERA: "YEAH TROT ON BITCH I GOT A QUESTION FOR YOU!"
- What a great way to get an answer."
    Ask it then"
    - ->YouComeFromUpNorth
=YouComeFromUpNorth
Her face scrunches as she observes you for a moment, taking it all in.
VERA: "You come from up North?"
Yes and no."
VERA: "The fuck does that mean?"
    +Wasn't born there. Worked there a couple years. Moscow Orchestra."
    VERA: "Huh. Cool."
    She catches herself in a moment of sincerity.
    Her face twists.
    ->YouSeeAnOldBitch
    +Means yes and no."
    Her distaste is palpable. Her face scrunches again.
    ->YouSeeAnOldBitch
Yes."
->YouSeeAnOldBitch
No."
She groans, stretching out her fingers and throwing her head back in frustration.
VERA: "FUUUUUUUUCK!"
She sighs.
VERA: "Fuck it."
->YouSeeAnOldBitch
Neigh."
She groans, stretching out her fingers and throwing her head back in frustration.
VERA: "FUUUUUUUCK!"
She sighs.
VERA: "Fuck it."
->YouSeeAnOldBitch
=YouSeeAnOldBitch
VERA: "You see an old bitch on your trip? Seventies? Big ass nose? Good shot? {TakeTheRifle: Apparently she's got a Mosin, just like yours.}
{TakeTheRifle: She points to the rifle on your back}.
Yes."
Her face gives way to shock as simmering eyes converge on yours.
VERA: "Where?"
    ++[Tell the truth.] "Donetsk."
    ->VeraTruth
    ++[Lie] "Kursk."
    ->VeraLie
    
No."
->NoOrNeigh
Neigh."
->NoOrNeigh

=NoOrNeigh
She groans into the back of the headrest.
->WhatsYourBeef

=VeraTruth
Her eyes widen. She pulls out a notepad and presses it against the headrest, scribbling letters on the page.
->WhatsYourBeef

=VeraLie
Her eyes widen. She pulls out a notepad and presses it against the headrest, scribbling letters on the page.
->WhatsYourBeef

=WhatsYourBeef
What's your beef, Drumsticks?"
"Fuck off Horseface."
But you've caught her in a moment of weakness.
Why do you ask?"
- VERA: "Just."
She pauses. 
Frustrated.
VERA: "I just wanna learn.
How to kill?"
VERA: "How to survive."
Better lessons out there."
VERA: "I'm not a coward."
- VERA: "Everythings falling apart and everyone's pretending it's not. I just want to do something. That's all."
Embarrassed by her own honesty, she turns away to stare against the horizon. The rage is still there, resting behind the eyes, but resting none-the-less.
I get it."
VERA: Fuck off Shoestring.
    +That one's not even a pun."
    VERA: "You're not even a fucking pun."
    She smirks, stifling laughter.
    ->Silence
    +Aight, Whackstick."
    She smirks, stifling laughter.
    ->Silence
+Sit in silence.
->Silence

=Silence
Eventually, you settle into silence against the hum of the engine, your gazes drawn to the endless white horizon. Out of the corner of your eye, you see her soften at the sight. She's still a kid, tapping gently at the drum on her side.
+Play alongside her.
You draw bow to string, expecting some grand rebuke. She offers nothing more than a petty scowl and a louder beat. You slowly start to dance against each other's sound, watching as the world you both once knew floats by on a sea of snow. It's still beautiful. You think you see her smile, maybe. Probably not though. 
+Sit in silence.
She taps against the snare and the side of the bus, matching wardrum rhythm to the engine below. She continues for the rest of the trip, fiery eyes gazing out over a sea of snow.
- Eventually, the bus starts to slow against the final bend of the road. Her fire returns as she pulls out a map from her coat pocket, scouring dogears and scribbled notes. This is your stop. Not hers. You weigh the weight of truth.
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

=Chapter3Epilogue
You step back onto solid ground once more as the doors behind you hiss to a close. You turn back to face the window, where you're met by the sight of two middle fingers and two fiery eyes. She offers one last smirk as the bus curves around the trees and back out onto the empty road. Nothing to be done. You turn back to your own journey.->Chapter4

=Chapter3EpilogueHijack
You step back onto solid ground once more as the doors behind him hiss to a close. As you turn back to face the window, you're met by the sound of a cocking rifle from back inside the bus.
VERA: "ALRIGHT FUCKBUCKET! LET'S TURN THIS STEEL TURD AROUND. WE'RE HEADING NORTH BABY!"
Grimacing as the driver's desperate eyes catch your own, you watch as the bus slowly reverses back the way it came.
From a nearby window, a terrifying child offers a friendly wink before the bus disappears behind the bend.
Nothing to be done. You turn back to your own journey.
->Chapter4

=Chapter4
/ART Bustop 3 AND PROTAG2
As evening settles in, you cross the road to the next weary stop. It's empty for now. A strange, lonely reprieve from the norms of peculiar company. You scour the surroundings, until a familiar red hue pops out from the stump of a nearby tree. Warmth rushes through you as you pull the letter from it's hidey hole.

PASCHA: "Drugs?"

/ART PASCHA appears

    You rise with a start against the sudden new presence. Turning back towards the stop, you see a figure where there was none before. Sharp eyes. Hair too soft. Skin too shiny. A perfect smile: charming and yet full of grit. They are, in a word, "eccentric".
    Nothing that interesting."
    PASCHA: "I beg to differ. In fact I'd be more interested if it wasn't drugs. So now, I am thus, "more interested"."
    Just a letter."
    PASCHA: "Well that is much more interesting."
    - They siddle slowly towards you.
    PASCHA: "Albeit, I wouldn't say no to drugs. I am desperately bored."
    Sharp eyes turn down towards the envelope in your hands.
    PASCHA: "Read it to me?"
    No."
    Sharp eyes turn round as they plead with you.
    PASCHA: "Pleaaaase?"
    ->AreYouHigh
    Why?"
    PASCHA: "I told you, I'm bored.
    ->AreYouHigh
    Fine."
    ->PublicReading
    
=AreYouHigh
+Are you high?"
        PASCHA: "I fucking wish, my dear. Now come, you sweet summer thing. Why not spill secrets with a stranger? 
            ++Fine."
            ->PublicReading
            ++..."
            PASCHA: "Fine. I'll start." 
            They raise a dramatic hand against their forehead.
            "I'll be dead within a week." 
            ->IllBeDead

+I don't even know you."
        PASCHA: "Nor I you, you sweet summer thing. Why not spill secrets with a stranger?
            ++Fine."
            ->PublicReading
            ++..."
            PASCHA: "Fine. I'll start." 
            They raise a dramatic hand against their forehead.
            "I'll be dead within a week."  
            ->IllBeDead
            
=IllBeDead

Sure you will."
PASCHA: "Swear it on my grave. Dragged there by stupid stupid queer love for stupid stupid queers."
How?"
PASCHA: "Stupid, stupid queer love for stupid stupid queers."
How dramatic."
PASCHA: "Death is pure drama. But it's love that pulls me there. Stupid, stupid queer love for stupid stupid queers."
- They twirl towards you. Honest eyes stare into your soul.
PASCHA: "I get half a day's walk from the border and my fuckwit of a lover gets caught in Moscow. Now me and my stupid bleeding heart are stuck racing back into the dark."
They sigh.
PASCHA: "Anyway. Your turn!"
Before you can respond, they slip the letter from your grasp, laughing as they race back into the snow.
+Chase them.
You lumber through thick powder, but they dash away with a tedious ease, tearing open the envelope as they bound through the snow.
+Let them go.
They settle a few feet away in the snow. Grounding themselves for some kind of performance and they rip the envelope open.

-PASCHA: "I'm about to cross in Abkhaz1a and I've just realised I've never heard back from you. It makes sense of course, me traveling ahead and all. But it's scary, terrifying, to never really know where you are. I w1sh I'd never asked you to stay in Moscow. I wish we'd left when you said we should. I'm 3orry. Please let me say that in person. Please let me meet you at the next stop. Please let us cross the rest together. I love you. I love you. I love you."
->PostPerformance


=PublicReading
    PASCHA: "Huh. That was easy."
    
    You sigh as you rip open the envelope, eyes greeting familiar, comforting handwriting.
    
    "I'm about to cross in Abkhaz1a and I've just realised I've never heard back from you. It makes sense of course, me traveling ahead and all. But it's scary, terrifying, to never really know where you are. I w1sh I'd never asked you to stay in Moscow. I wish we'd left when you said we should. I'm 3orry. Please let me say that in person. Please let me meet you at the next stop. Please let us cross the rest together. I love you. I love you. I love you."
    ->PostPerformance
    
    =PostPerformance
    They sigh and stare at you with sad eyes.
    PASCHA: "Truly, we are kin. Bound by dumb love. You for freedom, and I for the fire. Guess I should've chosen a smarter lover, one not fuckwitted enough to get caught in Moscow of all places."
    I'm sorry."
    PASCHA: "Don't be. Jealousy among outcasts is a tedious thing. I am happy for you."
    They smile.
    PASCHA: "Truly."
    You don't owe them your life."
    PASCHA: "Oh but I do. Love's a fickle Mistress like that. Take comfort that we'll never truly die, I suppose. Long as they keep breeding, we'll be being bred. Kind of Godlike like that, aren't we?"
    They chuckle.
    - PASCHA: "Play a dirge with me. Some battle song to feed me fake courage?"
    I won't entertain a deathwish."
    PASCHA: "You are a cruel thing, my dear."
    Pascha plays alone. You see faint tears catching moonlight. They keep you awake until the grasp of exhaustion drags you both into slumber.
    ->Chapter4Epilogue
    Of course."
    PASCHA: "Then let's dance the night away."
    ->PaschaDuet
    
    =PaschaDuet
    And so you dance for hours upon hours. You twist angels in the fresh laid snow and music in the clear moon sky. It's no dirge, no battle song, but rather seems a ballad for all the queers that ever did queer. You could swear for a moment you felt the earth shift, before evening takes hold and you both collapse harmlessly into a long night's sleep.
    ->Chapter4Epilogue
    
=Chapter4Epilogue
Their bus arriving earlier than your own, your momentary companion rises early against the dawn. Their skin is still too shiny and their hair is still too soft. At least their sharp eyes seem somewhat weary. They smile as you wake to bid them farewell.

+ {TakeTheRifle and not GiveVeraRifle} "Here you go Rambo." Give them the rifle.
They take the rifle with both hands, toying with it for a moment before nodding graciously and strapping it to their back.
PASCHA: "S'pose I'll be getting more in touch with my Masculine side."
Don't die stupid."
PASCHA: "Oofy doofy. That is a tall order my dear."
They chuckle.
PASCHA: "I'll try."
Good luck."
PASCHA: "I'm sure your well wishings will make all the difference my dear.
They chuckle, warmly.

- As their bus presses round the corner, they step off the platform and into the snow leading to the roadside.

I think I met your father."
They don't stop walking, but a voice responds.
PASCHA: Truly a real cunt of a man.
    +I've never seen so much regret."
    They stop. 
    The bus waits. 
    They wait.
    PASCHA: "Thank-you."
    And press forth once more, rising on steel steps and disappearing behind glass doors.
    ->Chapter4TrueEnd
    +Yeah..."

+Let them go.

-And they press on through the snow.

Up the stairs.

Disappearing behind glass doors.
->Chapter4TrueEnd

=Chapter4TrueEnd
An hour later, your bus arrives: a quiet, lonely shuttle to the edge of the Union's remains. 

Abkhazia awaits.

And so do they.
->Chapter5

=Chapter5
/Riding the bus
Abkhazia rolls in on the horizon. The Dead Sea breeze catches your nostrils with a sting that rights you alert and upright. Your stare out down the road ahead. They should be here.

They should.

They will.

They aren't.

As the bus pulls up and you stumble out into the street, the stark absence of them cuts you deeper than any winter wind.

+Look for anyone. Anyone at all.
Your eyes dash out against the cold horizon, marked only by the bus stop before you. They dart to and from every corner of the world, before resting on a figure hidden behind a bus stop pillar.
+Call out their name.
You cry out into the wind. You could swear it picks up a chill in response. Silence.
Then.
"Hello?"

Your eyes flash towards the source of the voice, a figure resting behind a pillar of the bus stop.

- Your smile widens as you race towards the stop. 

The figure emerges.

You pause in your tracks.

It's not them.

ZURAB: "Ah. You're not looking for me, are you?"

An old man, not so much disappointed as embarrassed. A slightly curved back and open eyes. They carry with them a pointed stick and a rubbish bag half full to the brim. As they wait for your response, they prick a metal can and shake it into the bag.

No. Sorry."
ZURAB: "Haven't seen anyone else yet, I'm afraid."
Have you seen anyone else here today?"
ZURAB: "No. Sorry."
-He examines you. 
ZURAB: "Come take a seat. You look weary."
The invitation is as alluring as the seat itself.
You take him up on the offer, grateful even for flimsy shelter from the cold breeze outside.
The man gently careers around the perimeter of the station, pricking cans of steel and plastic wrapping from the ground.
ZURAB: "You've come a long way?"
And then some."
ZURAB: "And still some yet, I'm sure."
Not really."
ZURAB: "Eyes say otherwise."
-He taps the side of the station.
ZURAB: "Seen many of these?"
Too many."
Not enough yet."
- His eyes enquire of yours with a strange sincerity.
ZURAB: "Do you like them?"
Yes."
He smiles.
ZURAB: "So do I."
No."
ZURAB: "Ahh. I'm sorry to hear that."
    +Not their fault. Not yours. Just want to leave them all behind."
    ZURAB: "Understandable.
    +No one's at fault for that.
    ZURAB: "Perhaps the artist is?"
I hadn't thought about it."
ZURAB: "Who could blame you? They're just bus stops."
- His eyes drift to the top of the station. Cracked, weathered paint withers away on the concrete.
ZURAB: "I just hope they remain a little longer than the world that built them. The absurdity. The color. Everything else is falling all around us and everyone else is itching to paint us in black and white and grey and anything but color. But we are red and gold and green and blue and anything and everything."
He smiles.
ZURAB: "I just hope that we outlive the fall. Language, art, music, silly bus stops. That might be all we have against a world that sees us mute and all a wash. I want them to know that we were here, and we were wonderful."
The smile fades to embarrassment, he pricks another can on the ground.
ZURAB: "A fickle fancy. I know.
What isn't?"
ZURAB: "Who can say?"
I understand."
He smiles again.
A little."
ZURAB: "Only a little?"
He chuckles.
- His eyes wander back towards you.
ZURAB: "Play a song with me? Pass some time until the end times?"
Sure."
His smile widens into a childlike grin, and you lose yourselves in the nothingness of fancificul sound. The world becomes more vibrant, color dances on the rooftop. Concrete heals over and grass grows through the morning snow. You think you see a sun behind the sky. For a time, the world is brighter, if only in your minds.
->Them
Haven't the energy." 
ZURAB: Then I'll play for you.
He eyes you from his periphery as he plays. Matching tune to meet your face. But your mind is stuck in questions of where they are. If they're still coming, if waiting is worth the wait. They should be here.
They should.
++They will.->Them

=Them
Footsteps meet the beat of the song, faint crunching of snow in the distance. Your eyes rise instantly to face a silhouette in the light. But this time you know the shape.

You rise.

They run.

They step out from the blinding sun.

You stare.

They stare.

You cry.

They cry.

I'm so fucking cold.

You hold each other till the end times.

The man smiles in the background.

The sound of an engine rumbles round the corner.

You wait together.

You smile together.

You step onto steel platform behind glass doors.

Together.

Of course.

You hold hands.

++No one dares to stop you.

->YouWereWonderful

=YouWereWonderful
+You were wonderful.
++And you were here.
-> DONE























