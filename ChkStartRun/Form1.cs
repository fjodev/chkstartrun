 
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChkStartRun
{
    public partial class frmDiffShow : Form
    {
        public TextBox tbTxtFile;
        public TextBox tbBakFile;
        public Label lblNewStat;
        public Label lblOldStat;
        public Button btnSair;
        private Label lblDiffLines;
        public TextBox tbDiffLines;
        public LinkLabel lnlblVerLog;
        private Label lblTitulo;
        public CheckBox ckbConfDiff;
        public DialogResult drDiffLines;
    
        public frmDiffShow()
        {
            InitializeComponent();
            
        }

        private void InitializeComponent()
        {
            this.tbDiffLines = new System.Windows.Forms.TextBox();
            this.tbTxtFile = new System.Windows.Forms.TextBox();
            this.tbBakFile = new System.Windows.Forms.TextBox();
            this.lblNewStat = new System.Windows.Forms.Label();
            this.lblOldStat = new System.Windows.Forms.Label();
            this.btnSair = new System.Windows.Forms.Button();
            this.lblDiffLines = new System.Windows.Forms.Label();
            this.lnlblVerLog = new System.Windows.Forms.LinkLabel();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.ckbConfDiff = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // tbDiffLines
            // 
            this.tbDiffLines.BackColor = System.Drawing.Color.White;
            this.tbDiffLines.Location = new System.Drawing.Point(12, 517);
            this.tbDiffLines.Multiline = true;
            this.tbDiffLines.Name = "tbDiffLines";
            this.tbDiffLines.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbDiffLines.Size = new System.Drawing.Size(663, 69);
            this.tbDiffLines.TabIndex = 0;
            this.tbDiffLines.WordWrap = false;
            // 
            // tbTxtFile
            // 
            this.tbTxtFile.BackColor = System.Drawing.Color.White;
            this.tbTxtFile.Location = new System.Drawing.Point(12, 75);
            this.tbTxtFile.Multiline = true;
            this.tbTxtFile.Name = "tbTxtFile";
            this.tbTxtFile.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbTxtFile.Size = new System.Drawing.Size(550, 400);
            this.tbTxtFile.TabIndex = 1;
            this.tbTxtFile.WordWrap = false;
            // 
            // tbBakFile
            // 
            this.tbBakFile.BackColor = System.Drawing.Color.White;
            this.tbBakFile.Location = new System.Drawing.Point(574, 75);
            this.tbBakFile.Multiline = true;
            this.tbBakFile.Name = "tbBakFile";
            this.tbBakFile.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbBakFile.Size = new System.Drawing.Size(550, 400);
            this.tbBakFile.TabIndex = 2;
            this.tbBakFile.WordWrap = false;
            // 
            // lblNewStat
            // 
            this.lblNewStat.AutoSize = true;
            this.lblNewStat.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewStat.Location = new System.Drawing.Point(9, 56);
            this.lblNewStat.Name = "lblNewStat";
            this.lblNewStat.Size = new System.Drawing.Size(107, 16);
            this.lblNewStat.TabIndex = 3;
            this.lblNewStat.Text = "Situação atual";
            // 
            // lblOldStat
            // 
            this.lblOldStat.AutoSize = true;
            this.lblOldStat.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOldStat.Location = new System.Drawing.Point(571, 56);
            this.lblOldStat.Name = "lblOldStat";
            this.lblOldStat.Size = new System.Drawing.Size(126, 16);
            this.lblOldStat.TabIndex = 4;
            this.lblOldStat.Text = "Situação anterior";
            // 
            // btnSair
            // 
            this.btnSair.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnSair.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSair.ForeColor = System.Drawing.Color.DarkGreen;
            this.btnSair.Location = new System.Drawing.Point(831, 546);
            this.btnSair.Name = "btnSair";
            this.btnSair.Size = new System.Drawing.Size(198, 40);
            this.btnSair.TabIndex = 6;
            this.btnSair.Text = "Sair";
            this.btnSair.UseVisualStyleBackColor = true;
            this.btnSair.Click += new System.EventHandler(this.btnSair_Click);
            // 
            // lblDiffLines
            // 
            this.lblDiffLines.AutoSize = true;
            this.lblDiffLines.Location = new System.Drawing.Point(9, 501);
            this.lblDiffLines.Name = "lblDiffLines";
            this.lblDiffLines.Size = new System.Drawing.Size(120, 13);
            this.lblDiffLines.TabIndex = 8;
            this.lblDiffLines.Text = "Diferenças encontradas";
            // 
            // lnlblVerLog
            // 
            this.lnlblVerLog.AutoSize = true;
            this.lnlblVerLog.Location = new System.Drawing.Point(12, 598);
            this.lnlblVerLog.Name = "lnlblVerLog";
            this.lnlblVerLog.Size = new System.Drawing.Size(92, 13);
            this.lnlblVerLog.TabIndex = 10;
            this.lnlblVerLog.TabStop = true;
            this.lnlblVerLog.Text = "Ver ficheiro de log";
            this.lnlblVerLog.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnlblVerLog_LinkClicked);
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitulo.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.lblTitulo.Location = new System.Drawing.Point(477, 9);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(207, 24);
            this.lblTitulo.TabIndex = 11;
            this.lblTitulo.Text = "Diferenças Detetadas";
            // 
            // ckbConfDiff
            // 
            this.ckbConfDiff.AutoSize = true;
            this.ckbConfDiff.Location = new System.Drawing.Point(831, 517);
            this.ckbConfDiff.Name = "ckbConfDiff";
            this.ckbConfDiff.Size = new System.Drawing.Size(122, 17);
            this.ckbConfDiff.TabIndex = 12;
            this.ckbConfDiff.Text = "Confirmar alterações";
            this.ckbConfDiff.UseVisualStyleBackColor = true;
            // 
            // frmDiffShow
            // 
            this.AcceptButton = this.btnSair;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton = this.btnSair;
            this.ClientSize = new System.Drawing.Size(1136, 623);
            this.ControlBox = false;
            this.Controls.Add(this.ckbConfDiff);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.lnlblVerLog);
            this.Controls.Add(this.lblDiffLines);
            this.Controls.Add(this.btnSair);
            this.Controls.Add(this.lblOldStat);
            this.Controls.Add(this.lblNewStat);
            this.Controls.Add(this.tbBakFile);
            this.Controls.Add(this.tbTxtFile);
            this.Controls.Add(this.tbDiffLines);
            this.Enabled = false;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmDiffShow";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = Application.ProductName + " " + Application.ProductVersion +" (c) Fernando Oliveira";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmDiffShow_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void frmDiffShow_FormClosing(object sender, EventArgs e)
        {
            this.DialogResult = drDiffLines;
        }

        private void btnSair_Click(Object sender, EventArgs e)
        {
            if (this.ckbConfDiff.Checked)
            {
                drDiffLines = System.Windows.Forms.DialogResult.Yes;
            }
            else
            {
                drDiffLines = System.Windows.Forms.DialogResult.No;
            }
            this.Close();
        }
                             
        private void lnlblVerLog_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(lnlblVerLog.Text);
        }
    }
}
