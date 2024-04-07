This is a mod for Death's Door that displays a list of all
installed mods (specifically, all loaded BepInEx plugins) at
the top left corner of the title screen. Its primary purpose is
to help diagnose basic mod installation problems, like having
the wrong versions of BepInEx or some mods installed.

# Initialization Failure Detection

To make it easier to diagnose issues caused by a failure to
initialize a mod, that mod can optionally expose a property
named IntStatus of type int in its main plugin class (the one
that inherits from [BaseUnityPlugin][bepbup]). That property
should be set to one of the following values:

- 0 if the mod has not initialized yet.
- 1 if initialization has completed successfully.
- 2 if initialization has failed.

For an example of how to implement that property, see
[ItemChanger's implementation][IC].

[bepbup]: https://docs.bepinex.dev/api/BepInEx.BaseUnityPlugin.html
[IC]: https://github.com/dpinela/DeathsDoor.ItemChanger/blob/v1.2/ItemChanger/ItemChangerPlugin.cs
