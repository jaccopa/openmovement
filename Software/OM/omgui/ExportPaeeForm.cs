﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OmGui
{
    public partial class ExportPaeeForm : Form
    {
        public ExportPaeeForm()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void buttonResample_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        public int Epoch { get { int epoch = 1; int.TryParse(comboBoxRate.Text, out epoch); return epoch; } }
        public int Model { get { return comboBox1.SelectedIndex; } }
    }
}
