using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace caro.server.form
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }
        public void UpdateStatus(string status)
        {
            if (this.lblStatus.InvokeRequired)
            {
                this.lblStatus.Invoke(new Action(() => UpdateStatus(status)));
                return;
            }
            lblStatus.Text = status;
        }
        public void AppendLog(string message)
        {
            if (this.rtbLogs.InvokeRequired)
            {
                this.rtbLogs.Invoke(new Action(() => AppendLog(message)));
                return;
            }

            rtbLogs.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\n");
            rtbLogs.ScrollToCaret(); // Tự động cuộn xuống dòng mới nhất
        }

        // Cập nhật danh sách kết nối/ngắt kết nối công khai (Online players list)
        public void UpdatePlayerList(string playerInfo, bool isConnecting)
        {
            if (this.lstPlayers.InvokeRequired)
            {
                this.lstPlayers.Invoke(new Action(() => UpdatePlayerList(playerInfo, isConnecting)));
                return;
            }

            if (isConnecting)
            {
                if (!lstPlayers.Items.Contains(playerInfo))
                    lstPlayers.Items.Add(playerInfo);
            }
            else
            {
                lstPlayers.Items.Remove(playerInfo);
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void OnlinePlayer_Enter(object sender, EventArgs e)
        {

        }

        private void txtlogs_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void lstPlayers_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
