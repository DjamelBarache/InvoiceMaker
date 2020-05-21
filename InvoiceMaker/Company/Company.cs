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

namespace InvoiceMaker.Company
{
    public partial class Company : Form
    {
        CompanyModel companyModel;
        public Company()
        {
            InitializeComponent();
        }

        private void Company_Load(object sender, EventArgs e)
        {
            var json = System.IO.File.ReadAllText(Environment.CurrentDirectory + @"\Company\Company.json");
            companyModel = JsonConvert.DeserializeObject<CompanyModel>(json);
            tbLegal.Text = companyModel.Legal;
            tbName.Text = companyModel.Name;
            tbSiret.Text = companyModel.Siret;
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            companyModel.Legal = tbLegal.Text;
            companyModel.Name = tbName.Text;
            companyModel.Siret = tbSiret.Text;

            var json = JsonConvert.SerializeObject(companyModel);
            System.IO.File.WriteAllText(Environment.CurrentDirectory + @"\Company\Company.json", json);
        }
    }
}
