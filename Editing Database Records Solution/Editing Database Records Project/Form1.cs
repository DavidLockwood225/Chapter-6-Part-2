﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;

namespace Editing_Database_Records_Project
{
    public partial class frmPhoneDB : Form
    {
        SqlConnection phoneConnection;
        SqlCommand phoneCommand;
        SqlDataAdapter phoneAdapter;
        DataTable phoneTable;
        CurrencyManager phoneManager;
        string myState;
        int myBookmark;
        public frmPhoneDB()
        {
            InitializeComponent();
        }
        private void SetState(string appState)
        {
            myState = appState;
            switch (appState)
            {
                case "View":
                    btnFirst.Enabled = true;
                    btnPrevious.Enabled = true;
                    btnNext.Enabled = true;
                    btnLast.Enabled = true;
                    btnEdit.Enabled = true;
                    btnSave.Enabled = false;
                    btnCancel.Enabled = false;
                    btnAdd.Enabled = true;
                    txtID.BackColor = Color.White;
                    txtID.ForeColor = Color.Black;
                    txtName.ReadOnly = true;
                    txtNumber.ReadOnly = true;
                    break;
                default:
                    btnFirst.Enabled = false;
                    btnPrevious.Enabled = false;
                    btnNext.Enabled = false;
                    btnLast.Enabled = false;
                    btnEdit.Enabled = false;
                    btnSave.Enabled = true;
                    btnCancel.Enabled = true;
                    btnAdd.Enabled = false;
                    txtID.BackColor = Color.Red;
                    txtID.ForeColor = Color.White;
                    txtName.ReadOnly = false;
                    txtNumber.ReadOnly = false;
                    break;
            }
            txtName.Focus();
        }
        private void frmPhoneDB_Load(object sender, EventArgs e)
        {
            phoneConnection = new SqlConnection("Server = (localdb)\\MSSQLLocalDB;"
                                               + "AttachDbFilename=" + Path.GetFullPath("SQLPhoneDB.mdf")
                                               + ";Integrated Security=True;"
                                               + "Connect Timeout=30;");
            try
            {
                phoneConnection.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error connecting to database:\r\n" + ex.Message, "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            phoneCommand = new SqlCommand("SELECT * from phoneTable ORDER BY ContactName", phoneConnection);
            phoneAdapter = new SqlDataAdapter();
            phoneAdapter.SelectCommand = phoneCommand;
            phoneTable = new DataTable();
            phoneAdapter.Fill(phoneTable);
            txtID.DataBindings.Add("Text", phoneTable, "ContactID");
            txtName.DataBindings.Add("Text", phoneTable, "ContactNumber");
            txtNumber.DataBindings.Add("Text", phoneTable, "ContactName");
            phoneManager = (CurrencyManager)this.BindingContext[phoneTable];
            SetState("View");
        }

        private void frmPhoneDB_FormClosing(object sender, FormClosingEventArgs e)
        {
            try 
            {
                SqlCommandBuilder phoneAdapterCommands = new SqlCommandBuilder(phoneAdapter);
                phoneAdapter.Update(phoneTable);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving database to file:\r\n" + ex.Message, "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            phoneConnection.Close();
            phoneConnection.Dispose();
            phoneCommand.Dispose();
            phoneAdapter.Dispose();
            phoneTable.Dispose();
        }

        private void btnFirst_Click(object sender, EventArgs e)
        {
            phoneManager.Position = 0;
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            phoneManager.Position--;
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            phoneManager.Position++;
        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            phoneManager.Position = phoneManager.Count - 1;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            SetState("Edit");
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            phoneManager.EndCurrentEdit();
            SetState("View");
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            phoneManager.CancelCurrentEdit();
            if (myState.Equals("Add"))
            {
                phoneManager.Position = myBookmark;
            }
            SetState("View");
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            myBookmark = phoneManager.Position;
            SetState("Add");
            phoneManager.AddNew();
        }
    }
}
