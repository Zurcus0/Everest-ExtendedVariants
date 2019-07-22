# Extended Variant Mode

A simple code mod for Celeste I did to play around with the IL manipulation capabilities of Everest. 
IL manipulation basically allows code modders to inject their code (almost) anywhere in Celeste, and that can be used to modify some specific things in the game's engine... that's exactly what this code mod does.

This mod allows to ~~break~~ change some stuff in the game mechanics... much like Variant Mode in the original.

Adds the following options to the Mod Options menu:
* **Gravity** and **Max fall speed**: can be used together to create a "space" effect, where falling is slower
* **Jump height**: control the max height of Madeline's jumps
* **Walk speed**: affects all horizontal movement when going around
* **Stamina**: affects the ability to climb and grab
* **Dash speed**: modifies the speed of dashing (... duh)
* **Dash count**: disable dashing or give Madeline up to 5 dashes (also affects refills)
* **Ground friction**: make the ground more or less slippery everywhere
* **Disable wall jumping**: remove your ability to wall jump (... let's make Celeste not Celeste anymore)
* **Double jumping**: gives you one more jump in midair

Enabling Extended Variant Mode will add the Variant Mode logo to the Chapter Complete screen.

Variants are also usable by maps, with the Extended Variants Trigger. Check the [README-Mappers.txt](ExtendedVariantMode/README-Mappers.txt) for more details.

**Compatible with Everest 862 and later.**

## How to install

You can download this code mod [on GameBanana](https://gamebanana.com/gamefiles/9486).

To build the project yourself:
* Clone or download the repo
* Compile the project with Visual Studio
* Copy the directory `ExtendedVariantMode\bin\Debug` in the `Mods` folder located in the Celeste directory
