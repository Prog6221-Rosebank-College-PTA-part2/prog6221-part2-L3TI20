
namespace CyberSecurityAwarenessBot.Themes
{
    public class ChatTheme
    {
        public string Name { get; init; } = string.Empty;
        public Color PageBg { get; init; }
        public Color SidebarBg { get; init; }
        public Color TopBarBg { get; init; }
        public Color InputBg { get; init; }
        public Color BotBubble { get; init; }
        public Color UserBubble { get; init; }
        public Color Accent { get; init; }
        public Color AccentLight { get; init; }
        public Color Highlight { get; init; }
        public Color TextHi { get; init; }
        public Color TextMid { get; init; }
        public Color TextLo { get; init; }
        public Color Border { get; init; }
        public Color BtnHover { get; init; }
        public Color InputText { get; init; }
    }

    public static class ThemeManager
    {
        public static readonly IReadOnlyList<ChatTheme> Themes = new List<ChatTheme>
        {
            new ChatTheme
            {
                Name        = "Purple Night",
                PageBg      = Color.FromArgb(14,  11,  28),
                SidebarBg   = Color.FromArgb(20,  16,  40),
                TopBarBg    = Color.FromArgb(26,  21,  50),
                InputBg     = Color.FromArgb(20,  16,  40),
                BotBubble   = Color.FromArgb(32,  26,  60),
                UserBubble  = Color.FromArgb(90,  46, 186),
                Accent      = Color.FromArgb(140,  80, 255),
                AccentLight = Color.FromArgb(185, 130, 255),
                Highlight   = Color.FromArgb(48,  220, 175),
                TextHi      = Color.FromArgb(242, 238, 255),
                TextMid     = Color.FromArgb(160, 150, 200),
                TextLo      = Color.FromArgb(95,   85, 135),
                Border      = Color.FromArgb(50,   42,  85),
                BtnHover    = Color.FromArgb(52,   44,  90),
                InputText   = Color.FromArgb(30,   24,  56),
            },
            new ChatTheme
            {
                Name        = "Cyber Green",
                PageBg      = Color.FromArgb(4,   14,   4),
                SidebarBg   = Color.FromArgb(6,   20,   6),
                TopBarBg    = Color.FromArgb(8,   26,   8),
                InputBg     = Color.FromArgb(6,   20,   6),
                BotBubble   = Color.FromArgb(10,  32,  10),
                UserBubble  = Color.FromArgb(20,  120,  20),
                Accent      = Color.FromArgb(0,   220,  80),
                AccentLight = Color.FromArgb(80,  255, 140),
                Highlight   = Color.FromArgb(0,   200, 200),
                TextHi      = Color.FromArgb(220, 255, 220),
                TextMid     = Color.FromArgb(120, 200, 120),
                TextLo      = Color.FromArgb(60,  110,  60),
                Border      = Color.FromArgb(20,   60,  20),
                BtnHover    = Color.FromArgb(14,   46,  14),
                InputText   = Color.FromArgb(8,    26,   8),
            },
            new ChatTheme
            {
                Name        = "Ocean Blue",
                PageBg      = Color.FromArgb(6,   16,  32),
                SidebarBg   = Color.FromArgb(8,   22,  44),
                TopBarBg    = Color.FromArgb(10,  28,  56),
                InputBg     = Color.FromArgb(8,   22,  44),
                BotBubble   = Color.FromArgb(12,  34,  68),
                UserBubble  = Color.FromArgb(0,   90, 200),
                Accent      = Color.FromArgb(0,  160, 255),
                AccentLight = Color.FromArgb(80, 200, 255),
                Highlight   = Color.FromArgb(255, 200,   0),
                TextHi      = Color.FromArgb(220, 240, 255),
                TextMid     = Color.FromArgb(120, 170, 220),
                TextLo      = Color.FromArgb(60,   90, 140),
                Border      = Color.FromArgb(20,   50, 100),
                BtnHover    = Color.FromArgb(14,   38,  80),
                InputText   = Color.FromArgb(10,   28,  56),
            },
            new ChatTheme
            {
                Name        = "Crimson Dark",
                PageBg      = Color.FromArgb(20,   6,   6),
                SidebarBg   = Color.FromArgb(30,   8,   8),
                TopBarBg    = Color.FromArgb(40,  10,  10),
                InputBg     = Color.FromArgb(30,   8,   8),
                BotBubble   = Color.FromArgb(50,  14,  14),
                UserBubble  = Color.FromArgb(180,  30,  30),
                Accent      = Color.FromArgb(255,  60,  60),
                AccentLight = Color.FromArgb(255, 130, 130),
                Highlight   = Color.FromArgb(255, 200,  60),
                TextHi      = Color.FromArgb(255, 230, 230),
                TextMid     = Color.FromArgb(200, 140, 140),
                TextLo      = Color.FromArgb(120,  70,  70),
                Border      = Color.FromArgb(80,   24,  24),
                BtnHover    = Color.FromArgb(60,   18,  18),
                InputText   = Color.FromArgb(40,   10,  10),
            },
            new ChatTheme
            {
                Name        = "Arctic White",
                PageBg      = Color.FromArgb(245, 248, 252),
                SidebarBg   = Color.FromArgb(230, 236, 246),
                TopBarBg    = Color.FromArgb(255, 255, 255),
                InputBg     = Color.FromArgb(255, 255, 255),
                BotBubble   = Color.FromArgb(220, 230, 245),
                UserBubble  = Color.FromArgb(60,  100, 220),
                Accent      = Color.FromArgb(50,   90, 210),
                AccentLight = Color.FromArgb(90,  130, 240),
                Highlight   = Color.FromArgb(0,   160, 120),
                TextHi      = Color.FromArgb(20,   30,  50),
                TextMid     = Color.FromArgb(80,  100, 140),
                TextLo      = Color.FromArgb(150, 170, 200),
                Border      = Color.FromArgb(200, 215, 235),
                BtnHover    = Color.FromArgb(210, 220, 240),
                InputText   = Color.FromArgb(240, 244, 250),
            },
        };
    }
}