﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;


namespace CS292Final_Kemerly
{
    public partial class History : Form
    {
        public History()
        {
            InitializeComponent();
        }
        
        //SQLite Goodness
        private const string dbRestaurants = "Data Source = ../../restaurants.db; version = 3";
        SQLiteConnection conn = new SQLiteConnection(dbRestaurants);
        SQLiteDataAdapter da;
        SQLiteCommand cmd;
        DataSet ds = new DataSet();
        string sql;

        private void DisplayTable()
        {
            sql = "Select Name, LastVisit FROM Restaurants";
            ds = new DataSet();
            da = new SQLiteDataAdapter(sql, conn);            
            conn.Open();
            da.Fill(ds);
            conn.Close();
            dgvHistory.DataSource = ds.Tables[0].DefaultView;
            dgvHistory.ClearSelection();            
        }

        private void SortTable()
        {// this is probably stupid. i can probably do this more elgantly with sql commands.
            //but it works, so.
            DataGridViewColumn LastVisit = new DataGridViewColumn();
            LastVisit = dgvHistory.Columns["LastVisit"];
            dgvHistory.Sort(LastVisit, ListSortDirection.Descending);
        }

        private void DisableDGVSorting(DataGridView intTable)
        {//pieced together with help from google -- does not work.
            for (int i = 0; i < intTable.Columns.Count; i++)
            {
                intTable.Columns[i].SortMode = DataGridViewColumnSortMode.Automatic;
            }
        }

        private string[] GetAutoVetoList()
        {
            string preOutput = "";
            
            using (SQLiteConnection conn = new SQLiteConnection(dbRestaurants))
            {
                conn.Open();
                string sql = "SELECT * FROM Restaurants ORDER BY LastVisit DESC LIMIT " + 
                    numHistoryVeto.Value.ToString() ;
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                {
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            preOutput += reader["Name"].ToString() + ",";                            
                        }
                    }
                }
            }
            preOutput = preOutput.Remove(preOutput.Length-1,1);
            string[] output = preOutput.Split(',');
            return output;
        }

        private void DeleteEntry(string inpName)
        {
            sql = "UPDATE Restaurants " +
                "Set LastVisit = NULL " +
                "WHERE Name = @name";
            conn.Open();
            cmd = new SQLiteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@name", inpName);
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        private void DeleteHistory()
        {
            sql = "UPDATE Restaurants " +
                "Set LastVisit = NULL";
            conn.Open();
            cmd = new SQLiteCommand(sql, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        private void chkHistoryVeto_CheckedChanged(object sender, EventArgs e)
        {
            numHistoryVeto.Enabled = chkHistoryVeto.Checked;
            label1.Enabled = chkHistoryVeto.Checked;
            Glb.autoVetoEnabled = chkHistoryVeto.Checked;
        }
        
        private void History_Load(object sender, EventArgs e)
        {
            DisplayTable();
            SortTable();
            //DisableDGVSorting(dgvHistory);
        }

        private void btnOkay_Click(object sender, EventArgs e)
        {
            //string dudebuddy = GetAutoVetoList();
            //MessageBox.Show(dudebuddy);
        }

        private void btnDeleteEntry_Click(object sender, EventArgs e)
        {
            if (dgvHistory.SelectedRows.Count <= 0)
            {
                System.Media.SystemSounds.Beep.Play();
                //get some kind of error message in here.
                return;
            }
            string selectedName = dgvHistory.SelectedRows[0].Cells["Name"].Value.ToString();
            //is the date null? does it matter? probably not.
            DeleteEntry(selectedName);


            
            
        }

        private void btnClearHistory_Click(object sender, EventArgs e)
        {

        }

        private void History_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (chkHistoryVeto.Checked)
            {
                Glb.autoVetoString = GetAutoVetoList();
                Glb.autoVetoEnabled = true;
            }
            if (!chkHistoryVeto.Checked)
            {
                Glb.autoVetoEnabled = false;
            }
        }
    }
}
