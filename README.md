# FakeAchievements

FakeAchievements is a Rain World helper which allows the creation of custom, Steam-like achievements that can be displayed anytime on screen.

## Creating achievements

To create an achievement follow these steps:

1. Install the FakeAchievements mod
2. Download and add the [stripped dll](https://github.com/ddemile/FakeAchievements/releases/download/0.1.2-beta/Stripped-FakeAchievements.dll) to your project's referenced assemblies and add the mod as a dependency of your mod
3. Create an `achievements` folder in your mod root
4. Create a new folder in that folder named after you achievement id
5. Add a file named `image.png` in the folder (this will be your achievement icon)
6. And another file named `langs.json`, this file will contain your achievement translations

## Displaying achievemnents

- Use `achievements grant [cosmeticOnly: bool = false]` command in the DevConsole mod console (for debbuging)
- Use `AchievementsManager.GrantAchievement("your_achievement_id", [bool cosmeticOnly = false])` in your mod code to display the achievement

Note achievements are by default only ever displayed once; To allow an achievement to be displayed again:

- Use `achievements revoke` command in the DevConsole's mod console (for debugging)
- Use `AchievementsManager.RevokeAchievement("your_achievement_id")` in your mod's code to allow the achievement to be "unlocked" and displayed again

## Examples

The full code of the examples below can be found on the [example mod repository](https://github.com/ddemile/FakeAchievementsExample?tab=readme-ov-file).

### Example plugin that triggers an achievement once when the player dies

```cs
using FakeAchievements;
using BepInEx;

namespace YourModNamespace
{
    [BepInPlugin("your_mod_id", "Your mod name", "0.1.0")]
    class Plugin : BaseUnityPlugin
    {
        public void OnEnable()
        {
            On.Player.Die += Player_Die;
        }

        private void Player_Die(On.Player.orig_Die orig, Player self)
        {
            orig(self);

            AchievementsManager.GrantAchievement("your_achievement_id");
            // Or
            AchievementsManager.GrantAchievement("your_mod_id/your_achievement_id");
        }
    }
}
```

### Example plugin that always triggers an achievement when the player dies

```cs
using FakeAchievements;
using BepInEx;

namespace YourModNamespace
{
    [BepInPlugin("your_mod_id", "Your mod name", "0.1.0")]
    class Plugin : BaseUnityPlugin
    {
        public void OnEnable()
        {
            On.Player.Die += Player_Die;
        }

        private void Player_Die(On.Player.orig_Die orig, Player self)
        {
            orig(self);

            // This unlocks the achievement itself
            AchievementsManager.GrantAchievement("your_achievement_id");
            // Or
            AchievementsManager.GrantAchievement("your_mod_id/your_achievement_id");
            
            // This only displays the achievement without actually marking it as granted
            AchievementsManager.GrantAchievement("your_mod_id/your_achievement_id", true);

            // This "locks" / "revoke" the achievement, allowing it to be unlocked again
            AchievementsManager.RevokeAchievement("your_achievement_id");
            // Or
            AchievementsManager.RevokeAchievement("your_mod_id/your_achievement_id");
        }
    }
}
```

### Example `langs.json` file

```json
{
    "english": {
        "title": "You died :(",
        "description": "My achievement description"
    },
    "french": {
        "title": "Vous êtes mort :(",
        "description": "La description de mon succés"
    }
}
```

## Reloading achievements

If you want to reload the achievements at any time you can use:

- The `achievements reload` command in the DevConsole mod console
- `AchievementsManager.LoadAchievements()`
