# FakeAchievements
FakeAchievements is a Rain World helper that allows you to create fake steam-like achievements that you can display whenever you want on screen

## Creating achievements 
To create an achievement follow the following steps :
1. Install the FakeAchievements mod
2. Download and add the [stripped dll](TODO: put release link) to your project libs and your mod dependencies
3. Create an `achievements` folder in your mod root
4. Create a new folder in that folder named after you achievement id
5. Add a file `image.png` named in the folder (this will be your achievement icon)
6. And another file named `info.json`, this file will contain your achievement translations

## Displaying achievemnents
- Use achievements grant command in the DevConsole mod console (for debbuging)
- Use `FakeAchievementManager.ShowAchievement("your_achievement_id")` in your mod code to display the achievement

## Examples
### Example plugin that triggers an achievemnt when the player dies
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

            FakeAchievementManager.ShowAchievement("your_achievement_id");
            // Or
            FakeAchievementManager.ShowAchievement("your_mod_id/your_achievement_id");
        }
    }
}
```

### Example `info.json` file
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
- The `achievement reload` command in the DevConsole mod console
- `FakeAchievementManager.LoadAchievements()`
