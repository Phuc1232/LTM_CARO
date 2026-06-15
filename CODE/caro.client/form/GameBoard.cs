using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using caro.client.network;
using caro.share.DTOs;
using caro.share.DTOs.Constants;

namespace caro.client.form
{
    public partial class GameBoard : Form
    {
        public GameBoard()
        {
            InitializeComponent();
            boardControl1.OnCellClicked += async (row, col) =>
            {
                await TCPClientManager.Instance.SendPacketAsync(
                    PacketType.MoveRequest,
                    new MoveRequestDTO
                    {
                        row = row,
                        col = col
                    });
            };

            TCPClientManager.Instance.OnMoveNotify += move =>
            {
                if (InvokeRequired)
                {
                    Invoke(() => HandleMoveNotify(move));
                    return;
                }

                HandleMoveNotify(move);
            };

            TCPClientManager.Instance.OnGameEnded += gameEnd =>
            {
                if (InvokeRequired)
                {
                    Invoke(() => MessageBox.Show($"{gameEnd.WinnerName} thắng!\n{gameEnd.reason}"));
                    return;
                }

                MessageBox.Show($"{gameEnd.WinnerName} thắng!\n{gameEnd.reason}");
            };
        }
        private void HandleMoveNotify(MoveNotifyDTO move)
        {
            string symbol;

            if (move.player == TCPClientManager.Instance.CurrentUsername)
                symbol = "X";
            else
                symbol = "O";

            boardControl1.SetCell(move.row, move.col, symbol);
        }

        private void playerCard2_Load(object sender, EventArgs e)
        {

        }

        private void menuButton6_Load(object sender, EventArgs e)
        {

        }

        private void menuButton4_Load(object sender, EventArgs e)
        {

        }

        private void btnSurrender_Click(object sender, EventArgs e)
        {

        }

        private void menuButton4_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
      "Bạn có chắc muốn đầu hàng không?",
      "Surrender",
      MessageBoxButtons.YesNo,
      MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                MessageBox.Show(
                    "Đối thủ đầu hàng!",
                    "Game Over",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                boardControl1.NewGame();
            }
        }

        private void menuButton6_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
       "Bạn có muốn quay về trang chủ không?",
       "Quit",
       MessageBoxButtons.YesNo,
       MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Home home = new Home();
                home.Show();

                this.Hide();
            }
        }

        private void GameBoard_Load(object sender, EventArgs e)
        {

        }
    }
}
