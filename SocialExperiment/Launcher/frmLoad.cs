#region Copyright Syncfusion Inc. 2001-2016.
// Copyright Syncfusion Inc. 2001-2016. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Core;
namespace Launcher
{
    public partial class frmLoad : Syncfusion.Windows.Forms.MetroForm
    {
        public frmLoad()
        {
            InitializeComponent();
        }
        
        private void frmLoad_Load(object sender, EventArgs e)
        {
            //Core.DB.DBConnect con = new Core.DB.DBConnect(); <- Legg i en static class
        }
    }
}
