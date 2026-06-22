using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;

namespace caro.client.ui_components
{
    public partial class MenuButton : UserControl
    {
        [Browsable(true)]
        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color HoverBackColor { get; set; } = Color.DeepSkyBlue;

        [Browsable(true)]
        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color HoverForeColor { get; set; } = Color.White;

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
            }
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
                btnMenu.BackColor = Color.DodgerBlue;
                btnMenu.ForeColor = Color.White;
            }
        }

        private void btnMenu_Click(object sender, EventArgs e)
        {

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


    }
}