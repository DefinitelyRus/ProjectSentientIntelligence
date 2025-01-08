# Zombies
Each day, the spawn director will be given a certain number of points. These points will be randomly distributed between number of zombies and total stats the zombies will have. Each zombie will then receive a random number of stat points until no more points are available. Each zombie will then semi-randomly assign points to stats.

There are 3 kinds of zombies:
## Regular Zombie
- Very easy to kill with any weapon.
- Spawns 9x as often as the other kinds.
- May be a problem in large numbers.
- Attacks twice before biting.

### Base stats
- 100hp
- Unarmored
- 30hp attack
- 60% chance of infection on bite
- 1.5m/s movement

## Mutated Zombie
- Weak against shotguns.
- Resistant to low-caliber weapons.
- Slow attack, but only bites.
- More likely to spawn from the biohazard lab.

### Base stats
- 500hp
- Unarmored
- 65hp attack
- 100% chance of infection on bite
- 0.75m/s movement

## Armored Zombie
- Weak against high-caliber weapons.
- Resistant to shotguns.
- Fast attack, but never bites. (They can't. They're wearing a helmet.)
- More likely to spawn from the main entrance.

### Base stats
- 100hp
- Armored (-8hp damage taken per hit)
- 45hp attack
- 1.0m/s movement

# Stat Upgrades (Optional feature)
These are stat boosts that will be applied to zombies as the game progresses.

## Speed
## HP
## Attack

# Spawning

## Spawning Rules
- Zombies cannot spawn in the following rooms regardless of occupation:
    - Any lost-survivor-occupied rooms.
    - The stairwell to the rooftop.
- Zombies cannot spawn in rooms recently (past 48 hours) visited by a survivor unless specified otherwise.
- Technical: Zombies are spawned in a suspended state. They will not be instantiated into a node.
- Technical: Suspended zombies will be instantiated when the room they're in or the room immediately adjacent to it has been loaded, or if the room was selected to attack Sector J1.
- Technical: Suspended zombies still count towards the total zombie count in macro view.


## Navigation Rules
### Macro
- Zombies will not always pick the shortest path to Sector J1. They will be evenly distributed across different possible paths to spread out the volume of zombies attacking at once.
- Zombies will stay in the room they spawned in until the rest period, at which point, a random selection of rooms will have some of the zombies inside be transferred to an adjacent room.
- Whenever a gate outside of Sector J1 is opened, Ruleset B will immediately be active on that sector.

### Micro
- When a survivor enters a room, zombies will stay still for some time before attacking the survivor. This gives the survivor some time to clear some space.
- When attacking a survivor, zombies will not always take the shortest path. They may opt to try to flank the survivor, exit the room and wait by the doorway, or (if a path is available) go around the survivor by exiting the room and entering back in from behind the survivor. This presents interesting behavior for the player to deal with.

## Rulesets
These are premade rule sets that dictate how zombies will behave on the macro scale to present interesting gameplay for the player.

Rulesets are made active when certain conditions are met, after some time has passed, or when a survivor (likely the player) performs a certain action.

### Ruleset A
Zombies will spawn but will not navigate to Sector J1. This lets the player have some breathing room to strategize and manage their resources for a while.

- Regular zombies will spawn somewhat evenly across the entire facility.
- Non-regular zombies will only spawn within pre-selected rooms unless the room has been recently visited by a survivor.
- Zombies will only navigate towards the survivors if they're in the same room or in a room immediately adjacent to it.

### Ruleset B
Zombies will not spawn and a random selection will navigate to Sector J1. It's a standard onslaught of zombies that's meant to serve as the default state.

- No zombies will spawn.
- Zombies within a random selection of rooms will rush towards Sector J1.

### Ruleset C
Only non-regular zombies will spawn and a random selection of any kind of zombie will navigate to Sector J1. This is meant to present an interesting scenario where the player has to suddenly deal with 8 armored zombies in a room, or perhaps have to defend against 20 mutated zombies simply out of bad luck.

- Total zombie spawn count is reduced.
- Regular zombies will not spawn.
- Non-regular zombies will spawn in a random selection of rooms.
- Zombies within a random selection of rooms will rush towards Sector J1.

### Ruleset D
Only regular zombies will spawn and a random selection of any kind of zombie will navigate to Sector J1. This compliments Ruleset C as it forces the player to deal with sheer numbers instead of tougher armored or mutated zombies.

- Total zombie spawn count is increased.
- Regular zombies will spawn somewhat evenly across the entire facility.
- Non-regular zombies will not spawn.
- Zombies within a random selection of rooms will rush towards Sector J1.

### Ruleset E
No zombies will spawn and no zombies will navigate towards Sector J1. This is to give the player some breathing room and allow the player to explore freely without much worry about the other survivors.

- No zombies will spawn.
- All zombies will stay.

### Ruleset F
No zombies will spawn and all remaining zombies will navigate towards Sector J1. This is meant to pressure the player to make a move and escape. While no additional zombies were spawned, this increased volume of zombies attacking at once is nearly impossible to handle and thus cannot be ignored.

- No zombies will spawn.
- Some zombies will navigate via vents, entering from behind the gate.
- All zombies will navigate towards Sector J1.

## Day 1
Rulesets: A, B

## Day 2
Rulesets: A, B, D

## Day 3
Rulesets: A, C, D, E

## Day 4
Rulesets: A, B, E

## Day 5
Rulesets: A, B, C, D, E

## Day 6
Rulesets: B, C, D, E

## Day 7
Rulesets: E, F

## Day 8
Rulesets: E

Starting Day 8, all reserve power is out. Lights are completely off and the control panel is inaccessible. No zombies will spawn nor navigate towards Sector J1, so the player should spend this time to clear the area and find supplies to last another day.

## Day 9
Rulesets: A, B

## Day 10
Rulesets: C, D, E

## Day 11
Rulesets: B, E

## Day 12
Rulesets: B, E

## Day 13
Rulesets: B, E

## Day 14
Rulesets: F

A non-stop onslaught of zombies. This is meant to be extremely difficult

## Day 15
Rulesets: E

If the player is somehow still alive by this day, the fire exit will be breached open where the player can escape.