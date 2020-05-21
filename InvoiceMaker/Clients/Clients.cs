using Newtonsoft.Json;
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
    public partial class Clients : Form
    {
        ClientsModel clientsModel;
        public Clients()
        {
            InitializeComponent();
            InitClientList();
        }

        private void InitClientList()
        {
            var json = System.IO.File.ReadAllText(Environment.CurrentDirectory + @"\Clients\ClientList.json");
            clientsModel = JsonConvert.DeserializeObject<ClientsModel>(json);
            RefreshGrid();
        }

        private void RefreshGrid()
        {
            bindingSourceClients.DataSource = null;
            dataGridView1.DataSource = null;
            bindingSourceClients.DataSource = clientsModel.Clients;

            dataGridView1.DataSource = bindingSourceClients;
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.AutoResizeColumns();

            // Configure the details DataGridView so that its columns automatically
            // adjust their widths when the data changes.
            dataGridView1.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.AllCells;

            dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            bindingSourceClients.CurrentItemChanged += BindingSourceClients_CurrentItemChanged;

            if (clientsModel.Clients.Count > 0)
                SetCurrentItemToForm(clientsModel.Clients[0]);
        }

        private void BindingSourceClients_CurrentItemChanged(object sender, EventArgs e)
        {
            var currentClient = bindingSourceClients.Current as ClientModel;
            if (currentClient != null)
            {

                SetCurrentItemToForm(currentClient);
            }
        }

        private void SetCurrentItemToForm(ClientModel clientModel)
        {
            tbName.Text = clientModel.Name;
            tbAdress.Text = clientModel.Adress;
            tbPhone.Text = clientModel.Phone;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbName.Text))
            {
                MessageBox.Show("Name of client must be filled", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var newClient = new ClientModel
            {
                Id = Guid.NewGuid().ToString()
                ,
                Name = tbName.Text
                ,
                Adress = tbAdress.Text
                ,
                Phone = tbPhone.Text
            };

            clientsModel.Clients.Add(newClient);

            RefreshGrid();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            var currentClient = bindingSourceClients.Current as ClientModel;
            if (currentClient != null)
            {
                var currentClientInList = clientsModel.Clients.Single(x => x.Id == currentClient.Id);
                currentClientInList.Adress = tbAdress.Text;
                currentClientInList.Name = tbName.Text;
                currentClientInList.Phone = tbPhone.Text;
                RefreshGrid();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var currentClient = bindingSourceClients.Current as ClientModel;
            if (currentClient != null)
            {
                var currentClientInList = clientsModel.Clients.Single(x => x.Id == currentClient.Id);
                clientsModel.Clients.Remove(currentClientInList);
                RefreshGrid();
            }
        }

        private void tbSave_Click(object sender, EventArgs e)
        {
            var json = JsonConvert.SerializeObject(clientsModel);
            System.IO.File.WriteAllText(Environment.CurrentDirectory + @"\Clients\ClientList.json",json);
        }
    }
}
