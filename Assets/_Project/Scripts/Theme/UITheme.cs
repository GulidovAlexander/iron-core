using UnityEngine;

namespace Game.Scripts.UI.Theme
{
    public static class UITheme
    {
        // ── Accent ────────────────────────────────
        public static readonly Color32 Accent         = new(255, 33, 53, 255);  // #FF2135 100%
        public static readonly Color32 AccentHover    = new(255, 33, 53, 217);  // #FF2135 85%
        public static readonly Color32 AccentPressed  = new(255, 33, 53, 64);   // #FF2135 25%
        public static readonly Color32 AccentDim      = new(255, 33, 53, 31);   // #FF2135 12%
        public static readonly Color32 AccentFocus    = new(255, 33, 53, 51);   // #FF2135 20%
        public static readonly Color32 AccentDisabled = new(255, 33, 53, 77);   // #FF2135 30%

        // ── Text ──────────────────────────────────
        public static readonly Color32 TextNormal     = new(85, 92, 106, 255);  // #555C6A
        public static readonly Color32 TextHover      = new(255, 255, 255, 255);// #FFFFFF
        public static readonly Color32 TextPressed    = new(255, 33, 53, 255);  // #FF2135
        public static readonly Color32 TextFocus      = new(255, 255, 255, 255);// #FFFFFF
        public static readonly Color32 TextDisabled   = new(85, 92, 106, 102);  // #555C6A 40%

        // ── Button ────────────────────────────────
        public const float ButtonPaddingX = 20f;
        public const float ButtonPaddingY = 10f;
        public const float ButtonMinWidth = 120f;

        // ── Animation ─────────────────────────────
        public const float ButtonDuration = 0.2f;
        public const float FadeDuration   = 0.3f;

        // ── Spacing ───────────────────────────────
        public const float S2   = 2f;
        public const float S4   = 4f;
        public const float S6   = 6f;
        public const float S8   = 8f;
        public const float S10  = 10f;
        public const float S12  = 12f;
        public const float S16  = 16f;
        public const float S20  = 20f;
        public const float S24  = 24f;
        public const float S32  = 32f;
        public const float S40  = 40f;
        public const float S48  = 48f;
        public const float S56  = 56f;
        public const float S64  = 64f;
        public const float S80  = 80f;
        public const float S120 = 120f;
    }
}
