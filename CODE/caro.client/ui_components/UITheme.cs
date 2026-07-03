using System.Drawing;

namespace caro.client
{
    public static class UITheme
    {
        // Background color of forms (Sand Beige #F4EAD4)
        public static readonly Color FormBackColor = Color.FromArgb(244, 234, 212);
        
        // Background color of secondary panel/cards (Soft Paper Cream #FFFDF0)
        public static readonly Color CardBackColor = Color.FromArgb(255, 253, 240);
        public static readonly Color CardForeColor = Color.FromArgb(74, 55, 40); // Charcoal Brown (#4A3728)
        
        // Text colors
        public static readonly Color TextForeColor = Color.FromArgb(74, 55, 40); // Charcoal Brown
        public static readonly Color TitleColor = Color.FromArgb(92, 64, 51); // Deep Wood Brown (#5C4033)
        public static readonly Color SubtitleColor = Color.FromArgb(90, 110, 80); // Sage Green
        
        // Grid properties
        public static readonly Color CellBackColor = Color.FromArgb(238, 220, 130); // Warm Bamboo Wood (#EEDC82)
        public static readonly Color CellForeColor = Color.FromArgb(26, 26, 26); // Ink Black
        public static readonly Color CellHoverColor = Color.FromArgb(227, 199, 95); // Slightly darker/warmer bamboo
        public static readonly Color GridColor = Color.FromArgb(74, 55, 40); // Charcoal Brown gridlines
        
        // Player turns colors
        public static readonly Color XColor = Color.FromArgb(26, 26, 26); // Ink Black (#1A1A1A)
        public static readonly Color OColor = Color.FromArgb(178, 34, 34); // Crimson Red (#B22222)
        
        // Move Highlights
        public static readonly Color LastMoveBackColor = Color.FromArgb(250, 219, 216); // Soft peach/rose highlight (#FADBD8)
        public static readonly Color LastMoveForeColor = Color.FromArgb(26, 26, 26);
        public static readonly Color WinningColor = Color.FromArgb(46, 204, 113); // Emerald Green (#2ECC71)
        
        // Button colors (Wooden/Leather Brown #CD853F)
        public static readonly Color ButtonBackColor = Color.FromArgb(205, 133, 63);
        public static readonly Color ButtonForeColor = Color.White;
        public static readonly Color ButtonHoverBackColor = Color.FromArgb(222, 184, 135); // Burly Wood (#DEB887)
        public static readonly Color ButtonHoverForeColor = Color.FromArgb(74, 55, 40);
        
        // Secondary/Danger Button (Deep Crimson #B22222)
        public static readonly Color DangerButtonBackColor = Color.FromArgb(178, 34, 34);
        public static readonly Color DangerButtonForeColor = Color.White;
        public static readonly Color DangerButtonHoverBackColor = Color.FromArgb(210, 75, 75);
        
        // Input fields (TextBox, RichTextBox)
        public static readonly Color InputBackColor = Color.FromArgb(255, 253, 240); // Paper Cream
        public static readonly Color InputForeColor = Color.FromArgb(26, 26, 26); // Ink Black
        public static readonly Color InputBorderColor = Color.FromArgb(74, 55, 40); // Charcoal Brown border
    }
}
