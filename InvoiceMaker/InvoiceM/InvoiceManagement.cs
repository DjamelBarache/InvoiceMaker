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

namespace InvoiceMaker.InvoiceM
{
    public partial class InvoiceManagement : Form
    {
        private ClientsModel clientsModel;

        class MonthTicker
        {
            public DateTime CurrentDate { get; set; }
            public string DisplayName { get; set; }
        }

        public InvoiceManagement()
        {
            InitializeComponent();
        }

        private void InvoiceManager_Load(object sender, EventArgs e)
        {
            InitDateMonth();
            InitClients();
        }

        private void InitClients()
        {
            var json = System.IO.File.ReadAllText(Environment.CurrentDirectory + @"\Clients\ClientList.json");
            clientsModel = JsonConvert.DeserializeObject<ClientsModel>(json);
            cbClient.DataSource = clientsModel.Clients;
            cbClient.DisplayMember = "Name";
            cbClient.ValueMember = "Id";
        }

        private void InitDateMonth()
        {
            var startDate= DateTime.Now.AddMonths(-2);
            var startDateRelative = new DateTime(startDate.Year, startDate.Month, 1); ;
            List<MonthTicker> monthTickers = new List<MonthTicker>();
            for (var i = 0; i < 10; i++)
            {
                monthTickers.Add(new MonthTicker { CurrentDate = startDateRelative, DisplayName = startDateRelative.ToString("MMM yyyy").ToUpper() });
                startDateRelative = startDateRelative.AddMonths(1);
            }

            cbMonth.DataSource = monthTickers;
            cbMonth.DisplayMember = "DisplayName";
            cbMonth.ValueMember = "CurrentDate";

            cbMonth.SelectedIndex = cbMonth.SelectedIndex + 2;
        }

        private void cbMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedMonth = cbMonth.SelectedItem as MonthTicker;
            if (selectedMonth != null)
            {
                tbInvoiceNumber.Text = selectedMonth.CurrentDate.ToString("yyyyMM-MM");
                int nbDays = 0;
                var tempDate = selectedMonth.CurrentDate;
                DateTime endDate = selectedMonth.CurrentDate.AddMonths(1);

                for (var i= tempDate; i< endDate; i= i.AddDays(1))
                {
                    if(i.DayOfWeek != DayOfWeek.Saturday && i.DayOfWeek != DayOfWeek.Sunday)
                    {
                        nbDays++;
                    }
                }

                tbNbDays.Text = nbDays.ToString();
            }
           
        }
    }
}
