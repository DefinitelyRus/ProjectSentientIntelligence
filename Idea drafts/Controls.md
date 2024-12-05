# Control Scheme
This is how units and interactible objects in the game are controlled.

## Selecting
When in micro view, tapping a unit will select it. Tapping several units will select all of them.

When in macro view, holding a room will select all the units inside it.

## Commanding
When a unit is selected in micro view, you can tap on any tile to command them to move. If there is a panel on the target tile, the unit will automatically use the panel. If there are multiple units selected and the destination tile contains a panel, only the first selected unit will use the panel, and all other units will find the closest available tile from the panel.

When units are selected in macro view, tapping on a room will move all selected units to the target room. Whoever was in the selected room first will be the one operating any panels in the target room.

You have two views: macro view and micro view.

### Macro View
In macro view, you can see how many creatures are any given area. If there are survivors in a room, a green number will appear, counting the number of survivors in it. This view updates automatically every 4 minutes without needing to use micro view or checking the units in person, alarming when the number of survivors is less than when it last checked. You can update data for each room manually by using micro view or viewing in person.

The room will be shaded green or red, or a mix of both, based on the ratio of survivor-to-zombie count in the room. Empty rooms will be colored light gray, undiscovered rooms will be colored dark gray, and inaccessible rooms will be colored black.

You can command units here by holding down on a room and tapping on another room. The selected units will move to the target room and operate any panels automatically. The unit operating the panel previously will also operate the new panel.

Reference: Among Us admin room

### Micro View + In-person View
In micro view, you can see the entire selected room as if you were there in person, the only difference being that you are not in that room. Here, you can select any number of units and command them to move to another time within the same room. If there is a panel on the selected tile, the first (or only) unit will automatically operate said panel, and the rest will move to the closest vacant tile.

Reference: The Binding of Isaac

### Weapons & Food Panels
Both of these panels require one worker to produce the supplies and another to distribute it. To distribute the supplies, a worker must be selected and moved to the panel room. Once assigned, any supplier unit will haul supplies sequentially to the relevant locations based on certain factors. For ammo, the supplier will evenly distribute across individual trooper unless requested by the player to prioritize a certain trooper for a day before continuing with the usual pattern, or instructed to supply an ammo cache instead.

Since some rooms contain more survivors than others, higher population rooms will be prioritized automatically proportional to how many units there are in that particular room. Any excess food will be sent to the medical room unless requested by the player. Manually prioritizing supply distribution to a particular area can be dangerous, since the supplier will continue to provide supplies to the target room until their shift is over.

### Resource Area
Resource areas contain large amounts of raw supplies. Any assinged units here will automatically go to the relevant location
(e.g. food panel, weapons panel) and back to the resource area until there are no more raw supplies to obtain, at which point
they will return to the clinic to rest.

### Requests
When you hold a tile, certain options will appear. You can request to place a barricade on a tile, or set a room as an ammo cache for nearby troopers to resupply at instead of being supplied ammo individually.

If a unit is selected while a request is made, only that unit will perform the request. Otherwise, whichever unit is least busy will perform the request.

## Grouping (Optional Feature)
When two or more units are selected, you can hold any of the selected units and they will be assigned as the group leader, and all other units will be assigned to this group. Selecting any of the group's members will automatically select all of them, and any commands will be directed to the leader even if the leader is not in the same room.

Once given a job, groups will typically move together or perform a task together without further input from the player. This gives the benefit of completing tasks while the player is busy with something else, but increases the risk of waste or harm.

### Group Leader
The group leader will automatically distribute tasks taken from the player onto their members. They'll micro-manage their members on behalf of the player in order to maximize efficiency. However, this automatic micro-managing also means that they'll continue doing the task no matter how wasteful or harmful their task becomes while the player is looking away.