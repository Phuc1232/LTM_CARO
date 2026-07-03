using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace caro.client.ui_components
{
    public partial class PlayerCard : UserControl
    {
        public PlayerCard()
        {
            InitializeComponent();
            picAvatar.Visible = false;

            lblPlayerName.Location = new Point(10, 15);
            lblTime.Location = new Point(10, 42);

            ApplyThemeColors();
        }

        public void ApplyThemeColors()
        {
            this.BackColor = UITheme.CardBackColor;
            this.ForeColor = UITheme.CardForeColor;

            lblPlayerName.ForeColor = UITheme.CardForeColor;
            lblPlayerName.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);

            lblTime.ForeColor = UITheme.SubtitleColor;
            lblTime.Font = new Font("Segoe UI", 10F, FontStyle.Italic);

            picAvatar.BackColor = UITheme.FormBackColor;
        }
        [Browsable(true)]
        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]

        public string PlayerName
        {
            get => lblPlayerName.Text;
            set => lblPlayerName.Text = value;
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]

        public string TimeText
        {
            get => lblTime.Text;
            set => lblTime.Text = value;
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]

        public Image Avatar
        {
            get => picAvatar.Image;
            set => picAvatar.Image = value;
        }

        private void lblTime_Click(object sender, EventArgs e)
        {

        }

        private void picAvatar_Click(object sender, EventArgs e)
        {

        }

        private void PlayerCard_Load(object sender, EventArgs e)
        {

        }
    }
}
