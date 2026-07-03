using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using caro.client.network;
using caro.share.DTOs;
using caro.share.DTOs.Constants;

namespace caro.client.form
{
    public partial class MatchHistory : Form
    {
        private List<MatchHistoryItemDTO> _histories = new();
        public MatchHistory()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;

            // Cấu hình cột hiển thị cho Grid
            SetupDataGridView();

            // Đăng ký sự kiện nhận lịch sử đấu từ Server
            TCPClientManager.Instance.OnMatchHistoryReceived += HandleMatchHistoryReceived;
            FormClosed += MatchHistory_FormClosed;

            ApplyThemeColors();
        }

        private void ApplyThemeColors()
        {
            this.BackColor = UITheme.FormBackColor;
            this.ForeColor = UITheme.TextForeColor;

            if (lblTitle != null)
            {
                lblTitle.BackColor = Color.Transparent;
                lblTitle.ForeColor = UITheme.TitleColor;
                lblTitle.Font = new Font("Segoe UI Semibold", 24F, FontStyle.Bold);
            }

            if (btnBack != null)
            {
                btnBack.BackColor = UITheme.DangerButtonBackColor;
                btnBack.ForeColor = UITheme.DangerButtonForeColor;
                btnBack.FlatStyle = FlatStyle.Flat;
                btnBack.FlatAppearance.BorderSize = 0;
                btnBack.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
            }
        }

        private void SetupDataGridView()
        {
            dgvHistory.Columns.Clear();
            dgvHistory.Columns.Add("Opponent", "Đối thủ");
            dgvHistory.Columns.Add("Result", "Kết quả");
            dgvHistory.Columns.Add("Type", "Chế độ");
            dgvHistory.Columns.Add("Date", "Ngày chơi");
            
            DataGridViewButtonColumn replayColumn = new DataGridViewButtonColumn();
            replayColumn.Name = "Replay";
            replayColumn.HeaderText = "Xem lại";
            replayColumn.Text = "Xem";
            replayColumn.UseColumnTextForButtonValue = true;
            replayColumn.DefaultCellStyle.BackColor = UITheme.ButtonBackColor;
            replayColumn.DefaultCellStyle.ForeColor = UITheme.ButtonForeColor;
            replayColumn.DefaultCellStyle.SelectionBackColor = UITheme.ButtonHoverBackColor;
            replayColumn.DefaultCellStyle.SelectionForeColor = UITheme.ButtonHoverForeColor;
            replayColumn.FlatStyle = FlatStyle.Flat;
            dgvHistory.Columns.Add(replayColumn);
            
            // Thiết lập phong cách tối (Dark theme) đồng bộ giao diện Caro
            dgvHistory.EnableHeadersVisualStyles = false;
            dgvHistory.BackgroundColor = UITheme.CardBackColor;
            dgvHistory.GridColor = UITheme.GridColor;
            
            dgvHistory.ColumnHeadersDefaultCellStyle.BackColor = UITheme.FormBackColor;
            dgvHistory.ColumnHeadersDefaultCellStyle.ForeColor = UITheme.TextForeColor;
            dgvHistory.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 11, FontStyle.Bold);
            dgvHistory.ColumnHeadersDefaultCellStyle.SelectionBackColor = UITheme.FormBackColor;
            dgvHistory.ColumnHeadersDefaultCellStyle.SelectionForeColor = UITheme.TextForeColor;

            dgvHistory.DefaultCellStyle.BackColor = UITheme.CardBackColor;
            dgvHistory.DefaultCellStyle.ForeColor = UITheme.TextForeColor;
            dgvHistory.DefaultCellStyle.SelectionBackColor = UITheme.CellHoverColor;
            dgvHistory.DefaultCellStyle.SelectionForeColor = UITheme.TextForeColor;
        }

        private async void MatchHistory_Load(object sender, EventArgs e)
        {
            // Gửi gói tin yêu cầu lịch sử đấu lên Server
            await TCPClientManager.Instance.SendPacketAsync(PacketType.MatchHistoryRequest, new { });
        }

        private void HandleMatchHistoryReceived(MatchHistoryResponseDTO response)
        {
            if (IsDisposed) return;
            if (response?.histories == null) return;

            if (InvokeRequired)
            {
                Invoke(() => HandleMatchHistoryReceived(response));
                return;
            }
            _histories = response.histories;
            dgvHistory.Rows.Clear();

            string currentUsername = TCPClientManager.Instance.CurrentUsername;

            foreach (var item in response.histories)
            {
                // Tìm tên của đối thủ
                string opponent = item.Player1 == currentUsername ? item.Player2 : item.Player1;

                // Xác định kết quả
                string resultText = "Hòa";
                if (!string.IsNullOrEmpty(item.Winner))
                {
                    resultText = item.Winner == currentUsername ? "Thắng" : "Thua";
                }

                int rowIndex = dgvHistory.Rows.Add(
                    opponent,
                    resultText,
                    item.MatchType,
                    item.PlayedAt.ToLocalTime().ToString("dd/MM/yyyy HH:mm")
                );

                // Tô màu theo kết quả đấu
                if (resultText == "Thắng")
                {
                    dgvHistory.Rows[rowIndex].Cells[1].Style.ForeColor = UITheme.SubtitleColor;
                }
                else if (resultText == "Thua")
                {
                    dgvHistory.Rows[rowIndex].Cells[1].Style.ForeColor = UITheme.XColor;
                }
                else
                {
                    dgvHistory.Rows[rowIndex].Cells[1].Style.ForeColor = UITheme.WinningColor;
                }
            }
        }

        private void MatchHistory_FormClosed(object? sender, FormClosedEventArgs e)
        {
            TCPClientManager.Instance.OnMatchHistoryReceived -= HandleMatchHistoryReceived;
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            try
            {
                var home = Application.OpenForms.OfType<Home>().FirstOrDefault() ?? new Home();
                home.Show();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Lỗi hiển thị Home: " + ex.Message);
            }
            this.Close();
        }

        private void dgvHistory_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (dgvHistory.Columns[e.ColumnIndex].Name == "Replay")
            {
                if (e.RowIndex >= _histories.Count) return;

                var selectedMatch = _histories[e.RowIndex];

                if (string.IsNullOrWhiteSpace(selectedMatch.MovesData))
                {
                    MessageBox.Show("Trận đấu này không có dữ liệu nước đi để xem lại!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                MatchReplay replayForm = new MatchReplay(selectedMatch);
                replayForm.ShowDialog();
            }
        }
    }
}