using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;

namespace caro.client.ui_components
{
    public partial class MenuButton : UserControl
    {
        private bool _isDanger = false;

        [Browsable(true)]
        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool IsDanger
        {
            get => _isDanger;
            set
            {
                _isDanger = value;
                ApplyThemeColors();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color HoverBackColor { get; set; } = UITheme.ButtonHoverBackColor;

        [Browsable(true)]
        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color HoverForeColor { get; set; } = UITheme.ButtonHoverForeColor;

        public MenuButton()
        {
            InitializeComponent();

            // subscribe safely only if designer created the inner button
            if (btnMenu != null)
            {
                btnMenu.MouseEnter += BtnMenu_MouseEnter;
                btnMenu.MouseLeave += BtnMenu_MouseLeave;
                btnMenu.Click += (s, e) => this.OnClick(e);

                // Ensure initial sync between base.Text and the inner button
                if (!string.IsNullOrEmpty(base.Text))
                {
                    btnMenu.Text = base.Text;
                }

                ApplyThemeColors();
            }
        }

        public void ApplyThemeColors()
        {
            if (btnMenu == null) return;

            Color defaultBg = _isDanger ? UITheme.DangerButtonBackColor : UITheme.ButtonBackColor;
            Color defaultFg = _isDanger ? UITheme.DangerButtonForeColor : UITheme.ButtonForeColor;
            HoverBackColor = _isDanger ? UITheme.DangerButtonHoverBackColor : UITheme.ButtonHoverBackColor;
            HoverForeColor = _isDanger ? UITheme.DangerButtonForeColor : UITheme.ButtonHoverForeColor;

            btnMenu.BackColor = defaultBg;
            btnMenu.ForeColor = defaultFg;
            btnMenu.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
        }

        private void BtnMenu_MouseEnter(object? sender, EventArgs e)
        {
            if (btnMenu != null)
            {
                btnMenu.BackColor = HoverBackColor;
                btnMenu.ForeColor = HoverForeColor;
            }
        }

        private void BtnMenu_MouseLeave(object? sender, EventArgs e)
        {
            if (btnMenu != null)
            {
                btnMenu.BackColor = _isDanger ? UITheme.DangerButtonBackColor : UITheme.ButtonBackColor;
                btnMenu.ForeColor = _isDanger ? UITheme.DangerButtonForeColor : UITheme.ButtonForeColor;
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The text associated with the control.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Bindable(true)]
        [Localizable(true)]
        public override string Text
        {
            get => btnMenu?.Text ?? base.Text;
            set
            {
                base.Text = value;

                if (btnMenu != null)
                {
                    btnMenu.Text = value;
                    Invalidate();
                }

                OnTextChanged(EventArgs.Empty);
            }
        }

        private void btnMenu_Click(object sender, EventArgs e)
        {
        }
    }
}