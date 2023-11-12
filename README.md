# Salt and Sacrifice Adjustments

This is a mod for Salt and Sacrifice that adds a variaty of options that can help you adjust the gameplay difficulty to your liking. 
This mod requres BepInEx to work.

## Mod features:
- Cursor inversion lock
- Do not drop salt on death
- Stat regeneration (health, stamina and other)
- Item regeneration (ammo, potions)
- Player stat adjustment (damage, defense)

Every feature in this mod is optional and can by turned on or off in the mod configuration file.
Configuration file location: BepInEx\config\com.aurocosh.salt_and_sacrifice_adjustments.cfg.

### Cursor inversion
For some reason mouse cursor in this game is mirrored horizontally when it is located on the right side of the screen. I do not know why game developers added that, but you cannot turn it off in the game settings. It really bothered me, so i added to this mod option that disables cursor inversion. When this opiton is enabled the cursor will be normal and it will not be mirrored on any side of the screen.

### Do not drop salt on death
This feature does exactly what is says on the tin, it disables salt loss on death. If you enable this option you will not drop any salt on death.

### Stat regeneration
Stat regeneration is constant and is applied every frame at fixed rate. Regeneration rate can be disabled for each stat separately. You can customize regeneration rate for each stat in a mod config file.
Available stat regeneration types:
- Health regeneration
- Stamina regeneration
- Mana regeneration
- Rage regeneration
By default all stat regeneration types are enabled with somewhat slow regeneration rate. Stamina regeneration does not affect natural stamina regeneration, but instead adds additional stamina regeneration on top of natural stamina regeneration.

### Item regeneration
You can optionally enable regeneration of certain types of items during your run. Items will not be regenerated constantly, but instead they will regenerate 1 item at a time at fixed time intervals. You can enable or disable item regeneration for each item type separately. Regeneration timer for each item type is also separate and you can change regeneration period individually for each item type. For example by default ammo regeneration is enabled and you will regenerate 1 free ranged ammo every 2 minutes.
Regen is available for following items:
- Ranged ammo
- Health potions
- Focus potions
- Gray pearls
With default settings you will regenerate 1 health and 1 focus potion every 10 minutes. Every 2 minutes you will regenerate 1 ranged ammo, and 1 gray pearl every 5 minutes. Item regeneration countdown resets when you rest at the bonfire. 

### Player stat adjustment
This mod allows you to freely adjust player damage and defense by a fixed multiplier. There are 2 seperate multipliers for player damage and for player defense. Values higher then 1 will increase player damage or defense. Values between 0 and 1 will decrease player damage or defense. Value equal to 1 disables damage and defense adjustments.

For example:
Damage multiplier 2 will double player damage.
Damage multiplier 0.5 will reduce player damage by half.
Defense multiplier 1.2 will give player 20% extra defense.
Defense multiplier 0.8 will reduce player defense by 20%.
Damage multiplier 1 will do nothing.
