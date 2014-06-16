using System;
using System.ComponentModel;
using System.Windows.Forms;
using PTM.Framework;
using PTM.View.Controls;

namespace PTM.View.Forms
{
	/// <summary>
	/// Summary description for TasksGroupsForm.
	/// </summary>
	internal class TasksExplorerForm : Form
	{
		private TasksTreeViewControl tasksTreeViewControl;
		private Button newButton;
		private Button okButton;
		private Button deleteButton;
		private Button editButton;
		private IContainer components;
        private CheckBox showHiddenCheckBox;
		private Button propertiesButton;

		internal TasksExplorerForm()
		{
			InitializeComponent();
			tasksTreeViewControl.SelectedTaskChanged += new EventHandler(TreeView_AfterSelect);
            tasksTreeViewControl.DoubleClick += new EventHandler(tasksTreeViewControl_DoubleClick);
			this.tasksTreeViewControl.Initialize();
            this.showHiddenCheckBox.Checked = tasksTreeViewControl.ShowHidden;
		}

        void tasksTreeViewControl_DoubleClick(object sender, EventArgs e)
        {
            this.tasksTreeViewControl.ShowPropertiesSelectedTask();
        }

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TasksExplorerForm));
            this.newButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.deleteButton = new System.Windows.Forms.Button();
            this.editButton = new System.Windows.Forms.Button();
            this.tasksTreeViewControl = new PTM.View.Controls.TasksTreeViewControl();
            this.propertiesButton = new System.Windows.Forms.Button();
            this.showHiddenCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // newButton
            // 
            this.newButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.newButton.Location = new System.Drawing.Point(375, 16);
            this.newButton.Name = "newButton";
            this.newButton.Size = new System.Drawing.Size(80, 23);
            this.newButton.TabIndex = 1;
            this.newButton.Text = "New Task";
            this.newButton.Click += new System.EventHandler(this.newButton_Click);
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(375, 321);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(80, 23);
            this.okButton.TabIndex = 4;
            this.okButton.Text = "Close";
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // deleteButton
            // 
            this.deleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.deleteButton.Location = new System.Drawing.Point(375, 96);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(80, 23);
            this.deleteButton.TabIndex = 3;
            this.deleteButton.Text = "Delete";
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // editButton
            // 
            this.editButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.editButton.Location = new System.Drawing.Point(375, 56);
            this.editButton.Name = "editButton";
            this.editButton.Size = new System.Drawing.Size(80, 23);
            this.editButton.TabIndex = 2;
            this.editButton.Text = "Edit";
            this.editButton.Click += new System.EventHandler(this.editButton_Click);
            // 
            // tasksTreeViewControl
            // 
            this.tasksTreeViewControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tasksTreeViewControl.Location = new System.Drawing.Point(8, 8);
            this.tasksTreeViewControl.Name = "tasksTreeViewControl";
            this.tasksTreeViewControl.ShowHidden = false;
            this.tasksTreeViewControl.Size = new System.Drawing.Size(359, 337);
            this.tasksTreeViewControl.TabIndex = 0;
            // 
            // propertiesButton
            // 
            this.propertiesButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.propertiesButton.Location = new System.Drawing.Point(375, 136);
            this.propertiesButton.Name = "propertiesButton";
            this.propertiesButton.Size = new System.Drawing.Size(80, 23);
            this.propertiesButton.TabIndex = 5;
            this.propertiesButton.Text = "Properties";
            this.propertiesButton.Click += new System.EventHandler(this.propertiesButton_Click);
            // 
            // showHiddenCheckBox
            // 
            this.showHiddenCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.showHiddenCheckBox.AutoSize = true;
            this.showHiddenCheckBox.Location = new System.Drawing.Point(375, 176);
            this.showHiddenCheckBox.Name = "showHiddenCheckBox";
            this.showHiddenCheckBox.Size = new System.Drawing.Size(88, 17);
            this.showHiddenCheckBox.TabIndex = 11;
            this.showHiddenCheckBox.Text = "Show Hidden";
            this.showHiddenCheckBox.UseVisualStyleBackColor = true;
            this.showHiddenCheckBox.CheckedChanged += new System.EventHandler(this.showHiddenCheckBox_CheckedChanged);
            // 
            // TasksExplorerForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(463, 351);
            this.Controls.Add(this.showHiddenCheckBox);
            this.Controls.Add(this.propertiesButton);
            this.Controls.Add(this.tasksTreeViewControl);
            this.Controls.Add(this.editButton);
            this.Controls.Add(this.deleteButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.newButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(471, 381);
            this.Name = "TasksExplorerForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Tasks Explorer";
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private void newButton_Click(object sender, EventArgs e)
		{
			tasksTreeViewControl.AddNewTask();
		}

		private void editButton_Click(object sender, EventArgs e)
		{
			tasksTreeViewControl.EditSelectedTaskDescription();
		}

		private void deleteButton_Click(object sender, EventArgs e)
		{
			tasksTreeViewControl.DeleteSelectedTask();
		}

		private void TreeView_AfterSelect(object sender, EventArgs e)
		{
			if (tasksTreeViewControl.SelectedTaskId == Tasks.RootTask.Id)
			{
				this.editButton.Enabled = false;
				this.deleteButton.Enabled = false;
				this.propertiesButton.Enabled = false;
			}
			else
			{
				this.editButton.Enabled = true;
				this.deleteButton.Enabled = true;
				this.propertiesButton.Enabled = true;
			}
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void propertiesButton_Click(object sender, EventArgs e)
		{
			this.tasksTreeViewControl.ShowPropertiesSelectedTask();
		}

        private void showHiddenCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            tasksTreeViewControl.ShowHidden = this.showHiddenCheckBox.Checked;
            tasksTreeViewControl.LoadTree();
        }

	}
}