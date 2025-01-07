# Project Cobalt Island
Draft name: "Attrition"

Attrition combinates elements from RTS, rogue-like, resource management, and survival genres to create a new thrilling experience of being unsure if making it out alive is possible. It borrows the pre-positioned randomized room contents from the game "SCP: Containment Breach" where there is a set of fixed hallway layouts but the individual rooms are always randomized on each run. It also borrows the item-hunting theme from "The Escapists" series where the player has to collect a certain set of items or complete a series of tasks in order to escape the prison in which they are locked up in.

## Intro story

"2 months pass since the outbreak began. Luckily, I found myself some 50 other survivors in the city port. Not so luckily, we're running critically low on supplies. The colony scouts found one radio tower beaming red lights at night and they suggested that it's our only way out given the supply constraints. A last ditch effort was made to reach the facility, and we've already lost nearly half our number just exiting the port, and another 9 by the time we entered the facility. Strangely, the power in the facility looked intentionally powered off, save for some critical components like fire exit lights and our target, the radio tower. We had no time to find the generators and power them on, so we just booked it to the rooftop. We lost another 5 people in the process.

Now we're at the rooftop, with no food and low on ammo. Good thing no one's injured. Well, at least the ones still alive. The squad of soldiers that came with us lost 4 of theirs, including their leader, and they seem lost now that no one's in command. 7 of them are holding off the zombies from just below the staircase, and the one with me here is going through all the frequencies on the radio to see if a single one would pick up. Nothing but static. He told me to keep trying while he goes back and help defend. I tried going off his list of known frequencies and after some stroke of good luck, I got someone."

Player: "Is there anyone on this frequency?"

Goldfish: "... Affirmative. This is callsign Goldfish, 1st Intelligence Company, how copy?"

Player: "I- I hear you! Look, we're at the rooftop of some medical research center. The building with this tall radio tower and a helipad next to it? Can you come pick us up? Please. We've lost over two-thirds of our group trying to get here, we're low on ammo and have no food left; you're our only way out."

Goldfish: "How many souls?"

Player: "16, I think."

Goldfish: "Copy. I'll get back to you in a moment."

Player: "..."

Goldfish: "..."

Player: "How's the situation downstairs?"

Civilian: "Looks like it's clearing up a bit! We can try getting to the fire exit if we really have to!"

Goldfish: "Miss radio tower, are you there?"

Player: "I'm here."

Goldfish: "Here's the situation: we can't pick you up. You'll have to find another way out. I can give you guidance on how to do that, but we can't spend any more resources than we absolutely have to. That's of course unless you have something valuable enough for us to consider. Boss' words, not mine."

Player: "What?! Ugh... I should've dropped out of pharmacy long ago... Alright. Can you at least tell me how we can get out of here?"

Goldfish: "You're a pharmacist, you say? Miss radio, do you know exactly where you are right now?"

Player: "I just told you though? Some medical research center wit-"

Goldfish: "No, no. That's rhetorical. You're on a biochemical weapon research lab hidden in plain sight. It's likely, no, probable that you'll find some form of cure there, or at least the blueprint to make one yourself. If you can make one and prove that it works, boss is likely willing to let you in. I know it's a big ask but this might just be your only chance."

Player: "... Got it."

Goldfish: "I'll be waiting here on the radio. Don't stay in there for too long; the power grid's in shambles, so whatever's powering that radio is gonna die out soon. And if I don't hear from you in 15 days, I'll just assume you've joined 'em, you hear?"

Player: "Roger. This is 'Miss Radio Tower', getting off this net, time now. Out."

Goldfish: "Impressive! Out."

<!-- This radio exchange is supposedly between a civilian and professional military. As you may notice, it's not very formal. That's because Goldfish picks up on the fact that he's talking with a civilian and adjusts his language appropriately. Player later surprises Goldfish by properly ending the exchange, as a callback to her pre-military training. -->

## Basic Info
Just stuff to mention so you can imagine what the game is supposed to look and feel like.
- View: Isometric Top-down, 30-45 deg off on Y-axis
- Art style: Simplified Signalis but in HD 2D. No custom pixel art assets.
- Control scheme: Tap to select character, then tap somewhere to move. That's it.
- Intended difficulty: Similar to FTL (Hard but not frustrating)
- What the player should feel: Tense, Focused
- Vibe: "We've lost over two-thirds of our group trying to get here, we're low on ammo and have no food left. However, I'm a pharmacist on a biochemical research facility. I can make a cure."

# Objectives
1. Enter the medical research center laboratory.
    - Optional: Obtain as much supplies as you can on your way to the lab.
    - Optional: Secure an adequately sized perimeter that extends to the rooftop.
2. Find the experimental cure.
    - Find the formula drafts.
    - Find the experimental cure vials.
    - Find the chemistry lab.
3. Follow and iterate on the formula drafts.
    - Find the necessary ingredients (see formula drafts for a short list of possible formulas).
    - Find suitable test subjects.
    - Inject the experimental cure into the test subject.
    - If incorrect formula: Kill the monster you created.
4. Report that the cure has been found and verified.
5. Hold out at the rooftop until the 8th day.
6. If 5 or more survivors: Pick 4 to come with you.
    - If you pick someone in your place: Survive.



# Gameplay
The game mixes roguelike, real-time strategy, and resource management into one game, and each of which is highlighted depending on where the your character is and when it's happening.

## Command phase
When your character enters the control room and operates the control panel, the game implicitly enters the command phase, highlighting the game's RTS elements. You can control all units from anywhere on the map, either on the macro (full map) or micro (per-room) scale.

On the micro scale, your job is to ensure your units are dynamically assigned to the appropriate panels, defensive positions, and distributing resources to the right units.

On the macro scale, you're given an overall view of the situation across the entire facility, giving you a chance to evaluate your next move.

The purpose of this phase is to give you a way to have overall control of how resources flow in your group. It forces you to strategize and ensure that all survivors are supplied well, are placed in advantageous positions, and given appropriate tasks. If done well, the survivors can self-sustain for some time without your inputs until a new situation is presented. If done poorly, survivors will run out of food and ammo, or worse, have your defensive positions overrun.

## Explore phase
When your character exits the control room, and more crucially, leaves sector J1, the game implicitly enters the explore phase, highlighting the game's roguelike elements. Since you're no longer in the control room, you'll naturally enter the micro view.

The purpose of this phase is to give you a reason to get out of your designated safe area and take risks. In order for the survivors to escape, they'll need to explore the facility to find everything they need to create the cure. And more crucially, they need to explore to find food and ammo. In theory, you could complete this simply by controling your units around and look for all the necessary things while you're up in your ivory tower safe and sound. You can't do that here. You must go out there and find the power generator for each sector you wish to gain control over, which means you too must explore. Additionally, there are things only your character can do, especially in regards with your objectives.

## Management phase
This phase highlights the game's resource management elements. It can happen anywhere and anytime, but mostly in sector J1.

The purpose of this phase is to force the player to make hard decisions. In this facility, resources are limited and you'll have to fight tooth and nail just to find enough resources to last another day. Sometimes, a stroke of bad luck or a chain of poor decisions happen, and not enough food was found to feed everyone for the day. Or perhaps there simply isn't enough ammo for everyone, or not enough effective weapons to fight the horde. You're forced to make decisions and these decisions dictate if you and your fellow survivors survive.



- Only Sector J1 is powered on by default. All other sectors are not powered and thus cannot be viewed via the control panel. Their generators found in each sector must first be enabled and powered on before it can be controlled remotely via the control panel. This ensures that you don't just stay in the control room all day and not interfacing with the explore phase of the game.
- Combatants will start with Blau EDCs. Replacement weapons can be found outside of Sector J1, spread across the facility.
- Only the player has a tier 4 access card, which is required to access certain rooms and the panels in it, such as the control room and the laboratory.







# ChatGPT Suggestions lol
Your idea, *Attrition*, has excellent potential to offer a unique gaming experience. Here's a breakdown of its strengths and areas for enhancement:

### **Strengths:**
1. **Genre Fusion:** Combining rogue-like randomness, RTS tactics, and survival mechanics provides a versatile gameplay loop. This encourages replayability and creative problem-solving.
2. **Dynamic Randomization:** Inspired by *SCP: Containment Breach*, randomized layouts for rooms and resources will ensure each playthrough feels fresh.
3. **Layered Objectives:** The blend of scavenging, crafting, and strategic perimeter defense adds depth without overwhelming the player.
4. **Narrative and Atmosphere:** The premise is compelling, with an ethical dilemma tied to survival. The time limit increases tension and urgency.

### **Suggestions for Improvement:**
1. **Character Depth:** Flesh out the squad members with unique abilities or quirks to add emotional stakes when choosing who survives.
2. **Resource Management Complexity:** Introduce trade-offs for looting versus advancing. For example, spend time fortifying the rooftop or use it to scavenge rare ingredients.
3. **Difficulty Scaling:** Ensure early levels gradually introduce mechanics (like curing zombies or managing supplies) to avoid overwhelming new players.
4. **Day/Night Cycle:** Introduce night phases where visibility drops and zombies become more aggressive, making the rooftop holdout sections even more thrilling.
5. **Replayability Enhancements:** Unlockable squad upgrades, alternate endings, or randomized challenges for prolonged engagement.
6. **Simple Controls:** As this is for mobile, ensure controls are intuitive and responsive. Perhaps use a drag-to-move system for squad management and tap/hold for actions.

### **Final Thoughts:**
Your game has the foundation of an engaging title, especially for players who enjoy strategy and survival in tense, resource-limited settings. Focus on balancing the complexity and accessibility for mobile, and this could stand out as a hit!