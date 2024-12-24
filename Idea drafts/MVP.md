<!-- Copyright © 2024 Okay Squad Developer Group. All Rights Reserved. -->
<!-- Licensed under the OSDG Source-Available License. See LICENSE for details. -->
<!-- DO NOT REMOVE -->

# Prototype v0.1
Everything that should be in the MVP and nothing that don't.

# Start Point
- The player starts at the rooftop, along with 12 legionnaires and 7 civilians.
- All weapons do not run out of ammo.\*

# Level
The level consists of 9 sectors in a 3x3 grid, separated by blast gates. The contents of these sectors are based on a randomly selected pre-defined layout from a list of possible sector layouts.

Similarly, each sector will have a randomly selected layout which also dictates how many rooms are in a sector.

Every sector will have at least 1 room dedicated to that sector. (e.g. The entrance and command center are always located at Sector I1, the lab at Sector J2, and Kitchens at I2, J1, J3, and K2.)

Sector I1, J2, and K3 will always have at least 8 rooms.

# Game Objects
## Player-controlled
### Survivors
Survivors instantly become a zombie upon getting bit.\*
- Player character
- Legionnaires
- Civilians

### Panels
- Command panel
- Biochemical storage cabinets
- Scrap piles
- Ammo-maker 2000
- Ingredient boxes & freezers
- Stoves

## Non-player-controlled
### Regular Zombies
- Very easy to kill with any weapon.
- Attacks twice before biting.
- 100hp
- 30hp attack
- Runs slightly faster than survivors.

# Objective
These are the objectives that the player must complete in order to proceed to the end point.

## Required one-time objectives
- Locate the lab. (Use the command panel)
- Find the template for the prototype cure.
- Find all the biochemicals listed in the template.
- Follow the template and recreate the prototype cure in the lab.
- Test it on a survivor.
    - If failed: Kill the monstrosity you created.
- Return to the rooftop.
- Call for rescue.
- Defend until rescue arrives.

## Ongoing objectives
These are technically optional objectives, but are necessary for the survivors to stay alive.
- Collect scrap and ingredients throughout the facility.
- Prepare food for the survivors.
- Distribute food to the survivors.
- Prepare ammunition for the combatants.
- Distribute ammunition to the combatants.

# Failure Condition
The conditions in which the end point can no longer be reached.

## Game Ends
- The player dies.
- The prototype cure is not made before the time limit (7 days).\*
- The player chooses not to ride the helicopter.\*

# End Point (Win Condition)
The condition in which the player wins.
- The player rides the helicopter and chooses 4 other survivors to come with them.

# Caveats
\* Details marked with this an asterisk (\*) are only applicable for the MVP and will be changed on later versions.