using Eugene.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Eugene.Desktop.Dialogs
{
    /// <summary>
    /// Interaktionslogik für ExcelImportDialog.xaml
    /// </summary>
    public partial class ExcelImportDialog : Window
    {
        public ExcelImportOptions ImportOptions
        {
            get
            {
                try
                {
                    var options = new ExcelImportOptions
                    {
                        WorksheetName = txtWorksheetName.Text,
                        FirstDataRow = Convert.ToInt32(txtFirstDataRow.Text),
                        ColumnNumberTestcaseId = Convert.ToInt32(txtColumnNumberTestcaseId.Text),
                        ColumnNumberTestcaseName = Convert.ToInt32(txtColumnNumberTestcaseName.Text),
                        ColumnNumberTestType = Convert.ToInt32(txtColumnNumberTestType.Text),
                        ColumnNumberApplicationModule = Convert.ToInt32(txtColumnNumberApplicationModule.Text),
                        ColumnNumberBlockerNames = Convert.ToInt32(txtColumnNumberBlockerNames.Text),
                        BlockerSeparator = txtBlockerSeparator.Text
                    };

                    return options;
                }
                catch
                {
                    return null;
                }
            }
        }


        public ExcelImportDialog()
        {
            InitializeComponent();

            txtWorksheetName.Text = "";
            txtFirstDataRow.Text = "2";
            txtColumnNumberTestcaseId.Text = "2";
            txtColumnNumberTestcaseName.Text = "4";
            txtColumnNumberTestType.Text = "12";
            txtColumnNumberApplicationModule.Text = "15";
            txtColumnNumberBlockerNames.Text = "1";
            txtBlockerSeparator.Text = ",";
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
