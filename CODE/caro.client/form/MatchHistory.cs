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
            this.BackColor = Color.FromArgb(30, 30, 46);

            // Cấu hình cột hiển thị cho Grid
            SetupDataGridView();

            // Đăng ký sự kiện nhận lịch sử đấu từ Server
            TCPClientManager.Instance.OnMatchHistoryReceived += HandleMatchHistoryReceived;
            FormClosed += MatchHistory_FormClosed;
        }

        private void SetupDataGridView()
        {
            dgvHistory.Columns.Add("Opponent", "Đối thủ");
            dgvHistory.Columns.Add("Result", "Kết quả");
            dgvHistory.Columns.Add("Type", "Chế độ");
            dgvHistory.Columns.Add("Date", "Ngày chơi");
            DataGridViewButtonColumn replayColumn = new DataGridViewButtonColumn();
            replayColumn.Name = "Replay";
            replayColumn.HeaderText = "Xem lại";
            replayColumn.Text = "Xem";
            replayColumn.UseColumnTextForButtonValue = true;
            dgvHistory.Columns.Add(replayColumn);
            // Thiết lập phong cách tối (Dark theme) đồng bộ giao diện Caro
            dgvHistory.EnableHeadersVisualStyles = false;
            dgvHistory.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(60, 60, 80);
            dgvHistory.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvHistory.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgvHistory.DefaultCellStyle.BackColor = Color.FromArgb(42, 42, 60);
            dgvHistory.DefaultCellStyle.ForeColor = Color.White;
            dgvHistory.DefaultCellStyle.SelectionBackColor = Color.FromArgb(70, 70, 95);
            dgvHistory.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvHistory.GridColor = Color.FromArgb(90, 90, 120);
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
                    dgvHistory.Rows[rowIndex].Cells[1].Style.ForeColor = Color.LightGreen;
                }
                else if (resultText == "Thua")
                {
                    dgvHistory.Rows[rowIndex].Cells[1].Style.ForeColor = Color.Salmon;
                }
                else
                {
                    dgvHistory.Rows[rowIndex].Cells[1].Style.ForeColor = Color.Yellow;
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

                MatchReplay replayForm = new MatchReplay(selectedMatch);
                replayForm.ShowDialog();
            }
        }
    }
}