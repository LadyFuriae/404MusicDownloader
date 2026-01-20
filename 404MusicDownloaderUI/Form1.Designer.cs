namespace _404MusicDownloaderUI
{
    partial class Form1
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.Link = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.DownloadQueue = new System.Windows.Forms.ListView();
            this.SongName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ProgresStatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.button2 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.LabelPath = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.Format = new System.Windows.Forms.Label();
            this.FormatComboBox = new System.Windows.Forms.ComboBox();
            this.PlaylistCheckBox = new System.Windows.Forms.CheckBox();
            this.ToolTipPlaylist = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // Link
            // 
            this.Link.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Link.Location = new System.Drawing.Point(113, 52);
            this.Link.Name = "Link";
            this.Link.Size = new System.Drawing.Size(398, 26);
            this.Link.TabIndex = 0;
            this.Link.Click += new System.EventHandler(this.Link_Click);
            this.Link.TextChanged += new System.EventHandler(this.Link_TextChanged);
            // 
            // button1
            // 
            this.button1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(113, 96);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(196, 26);
            this.button1.TabIndex = 1;
            this.button1.Text = "Descargar";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // DownloadQueue
            // 
            this.DownloadQueue.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.SongName,
            this.ProgresStatus});
            this.DownloadQueue.HideSelection = false;
            this.DownloadQueue.Location = new System.Drawing.Point(12, 241);
            this.DownloadQueue.Name = "DownloadQueue";
            this.DownloadQueue.Size = new System.Drawing.Size(622, 306);
            this.DownloadQueue.TabIndex = 4;
            this.DownloadQueue.UseCompatibleStateImageBehavior = false;
            this.DownloadQueue.View = System.Windows.Forms.View.Details;
            this.DownloadQueue.SelectedIndexChanged += new System.EventHandler(this.DownloadQueue_SelectedIndexChanged);
            // 
            // SongName
            // 
            this.SongName.Text = "Nombre de canción";
            this.SongName.Width = 406;
            // 
            // ProgresStatus
            // 
            this.ProgresStatus.Text = "Estado: ";
            this.ProgresStatus.Width = 270;
            // 
            // button2
            // 
            this.button2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.Location = new System.Drawing.Point(315, 96);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(196, 26);
            this.button2.TabIndex = 5;
            this.button2.Text = "Seleccionar Carpeta";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(216, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(204, 31);
            this.label1.TabIndex = 7;
            this.label1.Text = "Introduce el link";
            this.label1.Click += new System.EventHandler(this.label1_Click_1);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(243, 197);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(145, 31);
            this.label2.TabIndex = 8;
            this.label2.Text = "Descargas";
            // 
            // LabelPath
            // 
            this.LabelPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelPath.Location = new System.Drawing.Point(95, 168);
            this.LabelPath.Name = "LabelPath";
            this.LabelPath.ReadOnly = true;
            this.LabelPath.Size = new System.Drawing.Size(450, 26);
            this.LabelPath.TabIndex = 9;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(259, 134);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(107, 31);
            this.label3.TabIndex = 10;
            this.label3.Text = "Destino";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // Format
            // 
            this.Format.AutoSize = true;
            this.Format.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Format.Location = new System.Drawing.Point(549, 24);
            this.Format.Name = "Format";
            this.Format.Size = new System.Drawing.Size(57, 16);
            this.Format.TabIndex = 11;
            this.Format.Text = "Formato";
            // 
            // FormatComboBox
            // 
            this.FormatComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.FormatComboBox.FormattingEnabled = true;
            this.FormatComboBox.Location = new System.Drawing.Point(537, 57);
            this.FormatComboBox.Name = "FormatComboBox";
            this.FormatComboBox.Size = new System.Drawing.Size(83, 21);
            this.FormatComboBox.TabIndex = 12;
            this.FormatComboBox.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // PlaylistCheckBox
            // 
            this.PlaylistCheckBox.AutoSize = true;
            this.PlaylistCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PlaylistCheckBox.Location = new System.Drawing.Point(524, 102);
            this.PlaylistCheckBox.Name = "PlaylistCheckBox";
            this.PlaylistCheckBox.Size = new System.Drawing.Size(110, 17);
            this.PlaylistCheckBox.TabIndex = 14;
            this.PlaylistCheckBox.Text = "Descargar Playlist";
            this.PlaylistCheckBox.UseVisualStyleBackColor = true;
            // 
            // ToolTipPlaylist
            // 
            this.ToolTipPlaylist.ToolTipTitle = "Pene";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(646, 560);
            this.Controls.Add(this.PlaylistCheckBox);
            this.Controls.Add(this.FormatComboBox);
            this.Controls.Add(this.Format);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.LabelPath);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.DownloadQueue);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.Link);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "404MusicDownloader";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox Link;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ListView DownloadQueue;
        private System.Windows.Forms.ColumnHeader SongName;
        private System.Windows.Forms.ColumnHeader ProgresStatus;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox LabelPath;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label Format;
        private System.Windows.Forms.ComboBox FormatComboBox;
        private System.Windows.Forms.CheckBox PlaylistCheckBox;
        private System.Windows.Forms.ToolTip ToolTipPlaylist;
    }
}

