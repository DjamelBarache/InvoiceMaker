using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InvoiceMaker
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void clientListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clients clients = new Clients();
            clients.MdiParent = this;
            clients.Show();
        }
    }
}
