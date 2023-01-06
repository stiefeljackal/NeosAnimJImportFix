# NeosVR AnimJ Import Fix Mod (NeosAnimJImportFix)

A mod that fixes the current AnimJ importing issues since version `2022.1.28.XXXX`. Currently, the mod addresses the following issues with AnimJ importing:

* Addresses parsing issues with `color` and `floatQ` as mentioned in (NeosPublic #3822)[https://github.com/Neos-Metaverse/NeosPublic/issues/3822].
* Fixes the `Setup fields by name` button to properly map tracks to fields on a component as mentioned in (NeosPublic #3726)[https://github.com/Neos-Metaverse/NeosPublic/issues/3726].

## Installation

1. Install [NeosModLoader](https://github.com/zkxs/NeosModLoader).
2. Place [JworkzNeosAnimJImportFix.dll](https://github.com/stiefeljackal/NeosAnimJImportFix/releases/latest/download/JworkzNeosAnimJImportFix.dll) into your `nml_mods` folder. This folder should be at `C:\Program Files (x86)\Steam\steamapps\common\NeosVR\nml_mods` for a default install. You can create it if it's missing, or if you launch the game once with NeosModLoader installed it will create the folder for you.
3. Start the game. If you want to verify that the mod is working, you can check your Neos logs.

# Why would you want to use this mod?

This mod helps address the follow workarounds that one must utilize if AnimJ was imported with this mod:

* Neos has the capability of driving color fields with AnimX. With the current parsing bug, one will need to use float4 and a Swizzle component to drive the color.
* Although you can manually associate fields that should be driven per track, this is a burden you have to drive 30 or more of these fields. This especially true if your project requires a total of 1400 fields to be driven!

With the help of this mod, creators were able to unlock the full potential of AnimJ in the following worlds:

* CJ New Year's Eve 2023

# Other Information

## Types with Missing Parsing Instructions in Vanilla

* color
* floatQ
* float2x2
* float3x3
* float4x4
* doubleQ
* double2x2
* double3x3
* double4x4

## Types That This Mod Fixes Currently

* color
* floatQ

# Thank You

This mod is dedicated to the people of Creator Jam (CJ) 🍞. Without them, this mod may not have came into existence.