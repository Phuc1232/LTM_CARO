using System;
using System.Drawing;
using System.Windows.Forms;
using caro.client.network;
using caro.share.DTOs;
using caro.share.DTOs.Constants;

namespace caro.client.form
{
    public partial class Ranking : Form
    {
        private DataGridView dgvRanking = new DataGridView();

        public Ranking()
        {
            InitializeComponent();

            Text = "Bảng xếp hạng";
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(128, 255, 255);
            ClientSize = new Size(800, 520);

            BuildUI();

            TCPClientManager.Instance.OnBestRecordReceived += HandleBestRecordReceivedSafe;

            Load += async (s, e) =>
            {
                await TCPClientManager.Instance.SendPacketAsync(
                    PacketType.BestRecordRequest,
                    new { });
            };

            FormClosed += (s, e) =>
            {
                TCPClientManager.Instance.OnBestRecordReceived -= HandleBestRecordReceivedSafe;

                var home = Application.OpenForms["Home"];
                if (home != null) home.Show();
            };
        }

        private void BuildUI()
        {
            Label lblTitle = new Label
            {
                Text = "BẢNG XẾP HẠNG",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(280, 20)
            };
            Controls.Add(lblTitle);

            dgvRanking.Location = new Point(30, 80);
            dgvRanking.Size = new Size(740, 350);
            dgvRanking.BackgroundColor = Color.FromArgb(128, 255, 255);
            dgvRanking.ForeColor = Color.Black;
            dgvRanking.GridColor = Color.FromArgb(90, 90, 120);
            dgvRanking.AllowUserToAddRows = false;
            dgvRanking.ReadOnly = true;
            dgvRanking.RowHeadersVisible = false;
            dgvRanking.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvRanking.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            dgvRanking.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(128, 255, 255);
            dgvRanking.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgvRanking.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvRanking.EnableHeadersVisualStyles = false;

            dgvRanking.DefaultCellStyle.BackColor = Color.FromArgb(128, 255, 255);
            dgvRanking.DefaultCellStyle.ForeColor = Color.Black;
            dgvRanking.DefaultCellStyle.SelectionBackColor = Color.FromArgb(70, 70, 100);
            dgvRanking.DefaultCellStyle.SelectionForeColor = Color.White;

            dgvRanking.Columns.Add("Rank", "Hạng");
            dgvRanking.Columns.Add("Username", "Người chơi");
            dgvRanking.Columns.Add("Scores", "Điểm");
            dgvRanking.Columns.Add("Wins", "Thắng");
            dgvRanking.Columns.Add("Draws", "Hòa");
            dgvRanking.Columns.Add("Losses", "Thua");
            dgvRanking.Columns.Add("MaxWinStreak", "Chuỗi thắng");
            

            Controls.Add(dgvRanking);

            Button btnBack = new Button
            {
                Text = "Quay lại",
                Size = new Size(120, 45),
                Location = new Point(650, 450),
                ForeColor = Color.Black,
                BackColor = Color.FromArgb(128, 255, 255),
                FlatStyle = FlatStyle.Flat,
                UseVisualStyleBackColor = false
            };
            btnBack.FlatAppearance.BorderColor = Color.White;
            btnBack.FlatAppearance.BorderSize = 2;
            btnBack.Click += (s, e) => Close();

            Controls.Add(btnBack);
        }

        private void HandleBestRecordReceivedSafe(BestRecordResponseDTO dto)
        {
            if (IsDisposed) return;

            if (InvokeRequired)
            {
                BeginInvoke(() => HandleBestRecordReceivedSafe(dto));
                return;
            }

            dgvRanking.Rows.Clear();

            int rank = 1;

            foreach (var r in dto.Records)
            {
                if (r.Username.Trim().Equals("AI_Bot", StringComparison.OrdinalIgnoreCase))
                    continue;
                string shortestWin = r.ShortestWinMoves == int.MaxValue || r.ShortestWinMoves <= 0
                    ? "-"
                    : r.ShortestWinMoves.ToString();

                dgvRanking.Rows.Add(
                    rank,
                    r.Username,
                    r.Scores,
                    r.Wins,
                    r.Draws,
                    r.Losses,
                    r.MaxWinStreak
                );

                rank++;
            }
        }

        private void Ranking_Load(object sender, EventArgs e)
        {

        }
    }
}