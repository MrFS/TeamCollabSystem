﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;

using System.Net;
using System.Net.NetworkInformation;

namespace Core.DB
{
    public class DBConnect
    {

        private MySqlConnection con;
        private string server;
        private string database;
        private string uid;
        private string password;

        public DBConnect()
        {
            Initialize();
        }

        //Initialize values
        private void Initialize()
        {
            server = "localhost";
            database = "socialexperiment";
            uid = "root";
            password = "root";
            string connectionString;
            connectionString = "SERVER=" + server + ";" + "DATABASE=" +
            database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";

            con = new MySqlConnection(connectionString);
        }

        //open connection to database
        private bool OpenConnection()
        {
            try
            {
                con.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                //0: Cannot connect to server.
                //1045: Invalid user name and/or password.
                switch (ex.Number)
                {
                    case 0:
                        MessageBox.Show("Cannot connect to server.  Contact administrator");
                        break;

                    case 1045:
                        MessageBox.Show("Invalid username/password, please try again");
                        break;
                }
                return false;
            }
        }

        //Close connection
        private bool CloseConnection()
        {
            try
            {
                con.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        //State statement

        public ConnectionState States()
        {
            try
            {
                con.Open();
                return con.State;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
                
                return ConnectionState.Broken + ex.HResult;
                throw;
            }
            finally
            {
                con.Close();
            }
        }

        //PING
        
        public int DBPing()
        {
            Ping p = new Ping();

            return (int)p.Send(server).RoundtripTime;
        }

        //DB Version

        public string DBv()
        {
            try
            {
                con.Open();
                
                return con.ServerVersion;

            }
            catch (Exception ex)
            {
                return "Unable to retrieve DBv" + Environment.NewLine + ex.Message;
                throw;
            }
            finally
            {
                con.Close();
            }
            
        }

        //Insert statement
        public void Insert(string query)
        {
            //string query = "INSERT INTO tableinfo (name, age) VALUES('John Smith', '33')";

            //open connection
            if (this.OpenConnection() == true)
            {
                //create command and assign the query and connection from the constructor
                MySqlCommand cmd = new MySqlCommand(query, con);

                //Execute command
                cmd.ExecuteNonQuery();

                //close connection
                this.CloseConnection();
            }
        }

        //Update statement
        public void Update(string query)
        {
            //string query = "UPDATE tableinfo SET name='Joe', age='22' WHERE name='John Smith'";

            //Open connection
            if (this.OpenConnection() == true)
            {
                //create mysql command
                MySqlCommand cmd = new MySqlCommand();
                //Assign the query using CommandText
                cmd.CommandText = query;
                //Assign the connection using Connection
                cmd.Connection = con;

                //Execute query
                cmd.ExecuteNonQuery();

                //close connection
                this.CloseConnection();
            }
        }

        //Delete statement
        public void Delete(string query)
        {
            //string query = "DELETE FROM tableinfo WHERE name='John Smith'";

            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, con);
                cmd.ExecuteNonQuery();
                this.CloseConnection();
            }
        }

        //Select statement
        public DataTable Select(string query)
        {
            //string query = "SELECT * FROM tableinfo";

            //Create a datatable to store the result
            DataTable dt = new DataTable();

            //Open connection
            if (this.OpenConnection() == true)
            {
                //Create Command
                MySqlCommand cmd = new MySqlCommand(query, con);
                //Create a data reader and Execute the command
                //MySqlDataReader dataReader = cmd.ExecuteReader();
                MySqlDataAdapter da = new MySqlDataAdapter();

                //Read the data and store them in the list
                //while (dataReader.Read())
                //{
                //    list[0].Add(dataReader["id"] + "");
                //    list[1].Add(dataReader["name"] + "");
                //    list[2].Add(dataReader["age"] + "");
                //}

                da.SelectCommand = cmd;
                da.Fill(dt);


                //close Data Reader
                //dataReader.Close();

                //close Connection
                this.CloseConnection();

                //return list to be displayed
                return dt;
            }
            else
            {
                dt.Columns.Add("Error", typeof(string));
                dt.Rows.Add("Unable to retrieve database info");
                return dt;
            }
        }

        //Count statement
        public int Count(string query)
        {
            //string query = "SELECT Count(*) FROM tableinfo";
            int Count = -1;

            //Open Connection
            if (this.OpenConnection() == true)
            {
                //Create Mysql Command
                MySqlCommand cmd = new MySqlCommand(query, con);

                //ExecuteScalar will return one value
                Count = int.Parse(cmd.ExecuteScalar() + "");

                //close Connection
                this.CloseConnection();

                return Count;
            }
            else
            {
                return Count;
            }
        }

        //Backup
        public void Backup()
        {
            try
            {
                DateTime Time = DateTime.Now;
                int year = Time.Year;
                int month = Time.Month;
                int day = Time.Day;
                int hour = Time.Hour;
                int minute = Time.Minute;
                int second = Time.Second;
                int millisecond = Time.Millisecond;

                //Save file to C:\ with the current date as a filename
                string path;
                path = "C:\\MySqlBackup" + year + "-" + month + "-" + day +
            "-" + hour + "-" + minute + "-" + second + "-" + millisecond + ".sql";
                StreamWriter file = new StreamWriter(path);


                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "mysqldump";
                psi.RedirectStandardInput = false;
                psi.RedirectStandardOutput = true;
                psi.Arguments = string.Format(@"-u{0} -p{1} -h{2} {3}",
                    uid, password, server, database);
                psi.UseShellExecute = false;

                Process process = Process.Start(psi);

                string output;
                output = process.StandardOutput.ReadToEnd();
                file.WriteLine(output);
                process.WaitForExit();
                file.Close();
                process.Close();
            }
            catch (IOException ex)
            {
                MessageBox.Show("Error , unable to backup! " + ex.Message);
            }
        }

        //Restore
        public void Restore()
        {
            try
            {
                //Read file from C:\
                string path;
                path = "C:\\MySqlBackup.sql";
                StreamReader file = new StreamReader(path);
                string input = file.ReadToEnd();
                file.Close();

                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "mysql";
                psi.RedirectStandardInput = true;
                psi.RedirectStandardOutput = false;
                psi.Arguments = string.Format(@"-u{0} -p{1} -h{2} {3}",
                    uid, password, server, database);
                psi.UseShellExecute = false;


                Process process = Process.Start(psi);
                process.StandardInput.WriteLine(input);
                process.StandardInput.Close();
                process.WaitForExit();
                process.Close();
            }
            catch (IOException ex)
            {
                MessageBox.Show("Error , unable to Restore! " + ex.Message);
            }
        }
    }
}
