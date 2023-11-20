using System;
using System.Windows.Forms;

namespace Berzerk.UI
{
    public partial class MainMenu : Form
    {
        private readonly GameWindow _gameWindow;

        public MainMenu(GameWindow gameWindow)
        {
            InitializeComponent();
            _gameWindow = gameWindow;
        }

        private void MainMenu_Load(object sender, EventArgs e)
        {
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            _gameWindow.Show();
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}