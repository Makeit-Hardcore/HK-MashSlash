using Modding;
using GlobalEnums;
using Satchel.BetterMenus;
using System;

namespace MashSlash
{
    public class MashSlash : Mod, IGlobalSettings<GlobalSettings>, ICustomMenuMod, ITogglableMod
    {
        private Menu MenuRef;

        private static MashSlash? _instance;
        public bool ToggleButtonInsideMenu => true;

        public static GlobalSettings GS { get; set; } = new GlobalSettings();

        new public string GetName() => "Mash Slash";

        public override string GetVersion() => "1.0.0";

        public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? modToggleDelegates)
        {
            if (MenuRef == null)
            {
                MenuRef = new Menu("Mash Slash", new Element[]
                {
                    Blueprints.CreateToggle(
                        modToggleDelegates.Value,
                        "Mod Enabled",
                        ""
                        ),
                    new HorizontalOption(
                        "Require Quick Slash?",
                        "",
                        new string[] {"NO","YES"},
                        (setting) =>
                        {
                            GS.quickslash = setting;
                        },
                        () => GS.quickslash
                        )
                }
                );
            }
            return MenuRef.GetMenuScreen(modListMenu);
        }

        internal static MashSlash Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new InvalidOperationException($"{nameof(MashSlash)} was never initialized");
                }
                return _instance;
            }
        }

        public MashSlash() : base()
        {
            _instance = this;
        }

        // if you need preloads, you will need to implement GetPreloadNames and use the other signature of Initialize.
        public override void Initialize()
        {
            Log("Initializing");

            On.HeroController.CanAttack += CanAttack;

            Log("Initialized");
        }

        private bool CanAttack(On.HeroController.orig_CanAttack orig, HeroController self)
        {
            if (GS.quickslash == 1 && !HeroController.instance.playerData.GetBool("equippedCharm_32"))
            {
                return orig(self);
            }
            if (!HeroController.instance.cState.dashing
                && !HeroController.instance.cState.dead
                && !HeroController.instance.cState.hazardDeath
                && !HeroController.instance.cState.hazardRespawning
                && !HeroController.instance.controlReqlinquished
                && HeroController.instance.hero_state != ActorStates.no_input
                && HeroController.instance.hero_state != ActorStates.hard_landing
                && HeroController.instance.hero_state != ActorStates.dash_landing)
            {
                return true;
            }

            return false;
        }

        public void Unload()
        {
            On.HeroController.CanAttack -= CanAttack;
            Log("Unloaded");
        }

        public void OnLoadGlobal(GlobalSettings s) => GS = s;

        public GlobalSettings OnSaveGlobal() => GS;
    }
}

public class GlobalSettings
{
    public int quickslash = 0;
}
