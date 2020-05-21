using InvoiceMaker.InvoiceM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InvoiceMaker
{
    static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Main());
            InvoiceGenerator invoiceGenerator = new InvoiceGenerator(new ClientModel { Name = "a" }, 22, 18, 2222, "Mai 2020");

            Application.Run(new InvoiceMaker.Company.Company());
        }
    }
}
