The Assembly name for our linked MoonSharp dll is not `MoonSharp` but actually `MunSharp`.

By default, **MoonSharp** disables automatic registration of classes. So when you try to use any CLR object with MoonSharp, it will complain with an error if you have not manually registered it with the MoonSharp system.

To get around this, MoonSharp allows the following setting:
```
UserData.RegistrationPolicy = InteropRegistrationPolicy.Automatic;
```

We use this setting, and we also use multiple custom conversions. 

However, these settings are global, and other KSP mods use MoonSharp as well. For example, [MAS Interactive IVA!](https://forum.kerbalspaceprogram.com/index.php?/topic/160856-wip-18x-moardvs-avionics-systems-mas-interactive-iva-v110-4-november-2019/&tab=comments#comment-3062225).

So to try to prevent problems with this, we package a MoonSharp dll with the Assembly name of `MunSharp`. This should prevent our global settings from affecting other users of MoonSharp.

Additionally, we have made a modification to the way MoonSharp automatically handles registration of classes.