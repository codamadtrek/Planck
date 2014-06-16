using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Timers;
using System.Windows.Forms;
using PTM.Addin;
using PTM.Framework;
using PTM.Framework.Infos;
using PTM.View.Controls.TreeListViewComponents;
using PTM.View.Forms;

namespace PTM.View.Controls
{
	/// <summary>
	/// Summary description for Summary.
	/// </summary>
	internal class SummaryControl : AddinTabPage
	{
		private TreeListView taskList;
		private GroupBox groupBox1;
		private GroupBox groupBox3;
		private ColumnHeader PercentHeader;
		private IndicatorControl indicator1;
		private ColumnHeader TaskHeader;
		private Label label2;
		private ComboBox parentTaskComboBox;
		private IContainer components;
		private Button browseButton;
		private GroupBox groupBox2;
		private IndicatorControl indicator2;
		private Panel panel1;
		private ToolBar toolBar;
		private ToolBarButton toolBarButton1;
		private ToolBarButton toolBarButton2;
		private ImageList toolBarImages;

		private ArrayList recentParentTasksList = new ArrayList();
		private ColumnHeader PercentGoalHeader;
		private ColumnHeader TimeHeader;
		private DateTimePicker fromDateTimePicker;
		private DateTimePicker toDateTimePicker;
		private RadioButton fromRadioButton;
		private RadioButton toRadioButton;
		private ToolTip toolTip;
		private Task parentTask;
		private GroupBox groupBox4;
		private IndicatorControl indicator3;
        private IndicatorControl indicator4;
        private ColumnHeader GoalHeader;
        private GroupBox groupBox5;
        private ColumnHeader ActiveTimeHeader;
        private ColumnHeader InactiveTimeHeader;
        private RadioButton allRadioButton;
        private BackgroundWorker worker;

		internal SummaryControl()
		{
            // This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
            InitializaInnerUserControls();

            worker = new BackgroundWorker();
		    worker.WorkerSupportsCancellation = true;
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            //worker.OnBeforeDoWork += new AsyncWorker.OnBeforeDoWorkDelegate(worker_OnBeforeDoWork);
            //worker.OnWorkDone += new AsyncWorker.OnWorkDoneDelegate(worker_OnWorkDone);

			this.taskList.SmallImageList = IconsManager.IconsList;
			recentParentTasksList.Add(Tasks.RootTask);
			this.parentTaskComboBox.DisplayMember = "Description";
			this.parentTaskComboBox.ValueMember = "Id";
			this.parentTaskComboBox.DataSource = recentParentTasksList;

			this.fromDateTimePicker.Value = DateTime.Today;
			this.toDateTimePicker.Value = DateTime.Today;

			if (recentParentTasksList.Count > 0)
			{
				parentTask = (Task) recentParentTasksList[0];
				this.parentTaskComboBox.SelectedValue = parentTask.Id;
			}
			this.fromDateTimePicker.ValueChanged += new EventHandler(this.dateTimePicker_ValueChanged);
            fromDateTimePicker.DropDown += new EventHandler(fromDateTimePicker_DropDown);
            fromDateTimePicker.CloseUp += new EventHandler(fromDateTimePicker_CloseUp);
			this.toDateTimePicker.ValueChanged += new EventHandler(this.dateTimePicker_ValueChanged);
            toDateTimePicker.DropDown += new EventHandler(toDateTimePicker_DropDown);
            toDateTimePicker.CloseUp += new EventHandler(toDateTimePicker_CloseUp);			
			this.parentTaskComboBox.SelectedIndexChanged += new EventHandler(parentTaskComboBox_SelectedIndexChanged);

			Logs.CurrentLogDurationChanged += new ElapsedEventHandler(TaskLogTimer_Elapsed);
			this.taskList.DoubleClick += new EventHandler(taskList_DoubleClick);

			this.Status = "Ready";
		}

        protected override void OnHandleDestroyed(EventArgs e)
		{
			Logs.CurrentLogDurationChanged -= new ElapsedEventHandler(TaskLogTimer_Elapsed);
			base.OnHandleDestroyed(e);
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            PTM.View.Controls.TreeListViewComponents.TreeListViewItemCollection.TreeListViewItemCollectionComparer treeListViewItemCollectionComparer1 = new PTM.View.Controls.TreeListViewComponents.TreeListViewItemCollection.TreeListViewItemCollectionComparer();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SummaryControl));
            this.taskList = new PTM.View.Controls.TreeListViewComponents.TreeListView();
            this.TaskHeader = new System.Windows.Forms.ColumnHeader();
            this.TimeHeader = new System.Windows.Forms.ColumnHeader();
            this.PercentHeader = new System.Windows.Forms.ColumnHeader();
            this.GoalHeader = new System.Windows.Forms.ColumnHeader();
            this.PercentGoalHeader = new System.Windows.Forms.ColumnHeader();
            this.ActiveTimeHeader = new System.Windows.Forms.ColumnHeader();
            this.InactiveTimeHeader = new System.Windows.Forms.ColumnHeader();
            this.fromDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.parentTaskComboBox = new System.Windows.Forms.ComboBox();
            this.browseButton = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.toolBar = new System.Windows.Forms.ToolBar();
            this.toolBarButton1 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton2 = new System.Windows.Forms.ToolBarButton();
            this.toolBarImages = new System.Windows.Forms.ImageList(this.components);
            this.fromRadioButton = new System.Windows.Forms.RadioButton();
            this.toRadioButton = new System.Windows.Forms.RadioButton();
            this.toDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.allRadioButton = new System.Windows.Forms.RadioButton();
            this.groupBox3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // taskList
            // 
            this.taskList.Alignment = System.Windows.Forms.ListViewAlignment.Default;
            this.taskList.AllowColumnReorder = true;
            this.taskList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.taskList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.TaskHeader,
            this.TimeHeader,
            this.PercentHeader,
            this.GoalHeader,
            this.PercentGoalHeader,
            this.ActiveTimeHeader,
            this.InactiveTimeHeader});
            treeListViewItemCollectionComparer1.Column = 0;
            treeListViewItemCollectionComparer1.SortOrder = System.Windows.Forms.SortOrder.None;
            this.taskList.Comparer = treeListViewItemCollectionComparer1;
            this.taskList.HideSelection = false;
            this.taskList.Location = new System.Drawing.Point(8, 16);
            this.taskList.MultiSelect = false;
            this.taskList.Name = "taskList";
            this.taskList.Size = new System.Drawing.Size(376, 184);
            this.taskList.Sorting = System.Windows.Forms.SortOrder.None;
            this.taskList.TabIndex = 0;
            this.taskList.UseCompatibleStateImageBehavior = false;
            // 
            // TaskHeader
            // 
            this.TaskHeader.Text = "Description";
            this.TaskHeader.Width = 150;
            // 
            // TimeHeader
            // 
            this.TimeHeader.Text = "Time Elapsed";
            this.TimeHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // PercentHeader
            // 
            this.PercentHeader.Text = "%";
            this.PercentHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.PercentHeader.Width = 50;
            // 
            // GoalHeader
            // 
            this.GoalHeader.Text = "Estimated";
            this.GoalHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // PercentGoalHeader
            // 
            this.PercentGoalHeader.Text = "% Elapsed";
            this.PercentGoalHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.PercentGoalHeader.Width = 50;
            // 
            // ActiveTimeHeader
            // 
            this.ActiveTimeHeader.Text = "Active Time";
            this.ActiveTimeHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // InactiveTimeHeader
            // 
            this.InactiveTimeHeader.Text = "Inactive Time";
            this.InactiveTimeHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // fromDateTimePicker
            // 
            this.fromDateTimePicker.CustomFormat = "";
            this.fromDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.fromDateTimePicker.Location = new System.Drawing.Point(80, 32);
            this.fromDateTimePicker.Name = "fromDateTimePicker";
            this.fromDateTimePicker.Size = new System.Drawing.Size(88, 20);
            this.fromDateTimePicker.TabIndex = 0;
            this.fromDateTimePicker.Value = new System.DateTime(2006, 10, 2, 0, 0, 0, 0);
            // 
            // groupBox1
            // 
            this.groupBox1.ForeColor = System.Drawing.Color.Blue;
            this.groupBox1.Location = new System.Drawing.Point(8, 64);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(72, 80);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Total Time";
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.taskList);
            this.groupBox3.ForeColor = System.Drawing.Color.Blue;
            this.groupBox3.Location = new System.Drawing.Point(8, 144);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(392, 208);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Tasks";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(8, 4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 23);
            this.label2.TabIndex = 6;
            this.label2.Text = "Detail Level:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // parentTaskComboBox
            // 
            this.parentTaskComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.parentTaskComboBox.Location = new System.Drawing.Point(80, 5);
            this.parentTaskComboBox.MaxLength = 50;
            this.parentTaskComboBox.Name = "parentTaskComboBox";
            this.parentTaskComboBox.Size = new System.Drawing.Size(232, 21);
            this.parentTaskComboBox.TabIndex = 1;
            // 
            // browseButton
            // 
            this.browseButton.Location = new System.Drawing.Point(320, 4);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(75, 23);
            this.browseButton.TabIndex = 2;
            this.browseButton.Text = "Browse...";
            this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.ForeColor = System.Drawing.Color.Blue;
            this.groupBox2.Location = new System.Drawing.Point(88, 64);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(72, 80);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "% Active";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.toolBar);
            this.panel1.Location = new System.Drawing.Point(352, 120);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(48, 24);
            this.panel1.TabIndex = 14;
            // 
            // toolBar
            // 
            this.toolBar.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
            this.toolBarButton1,
            this.toolBarButton2});
            this.toolBar.Divider = false;
            this.toolBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolBar.DropDownArrows = true;
            this.toolBar.ImageList = this.toolBarImages;
            this.toolBar.Location = new System.Drawing.Point(0, 0);
            this.toolBar.Name = "toolBar";
            this.toolBar.ShowToolTips = true;
            this.toolBar.Size = new System.Drawing.Size(48, 26);
            this.toolBar.TabIndex = 0;
            this.toolBar.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBar_ButtonClick);
            // 
            // toolBarButton1
            // 
            this.toolBarButton1.ImageIndex = 0;
            this.toolBarButton1.Name = "toolBarButton1";
            this.toolBarButton1.ToolTipText = "Down level";
            // 
            // toolBarButton2
            // 
            this.toolBarButton2.ImageIndex = 1;
            this.toolBarButton2.Name = "toolBarButton2";
            this.toolBarButton2.ToolTipText = "Up level";
            // 
            // toolBarImages
            // 
            this.toolBarImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("toolBarImages.ImageStream")));
            this.toolBarImages.TransparentColor = System.Drawing.Color.Transparent;
            this.toolBarImages.Images.SetKeyName(0, "");
            this.toolBarImages.Images.SetKeyName(1, "");
            // 
            // fromRadioButton
            // 
            this.fromRadioButton.Checked = true;
            this.fromRadioButton.Location = new System.Drawing.Point(28, 32);
            this.fromRadioButton.Name = "fromRadioButton";
            this.fromRadioButton.Size = new System.Drawing.Size(54, 24);
            this.fromRadioButton.TabIndex = 15;
            this.fromRadioButton.TabStop = true;
            this.fromRadioButton.Text = "Date:";
            this.fromRadioButton.CheckedChanged += new System.EventHandler(this.fromRadioButton_CheckedChanged);
            // 
            // toRadioButton
            // 
            this.toRadioButton.Location = new System.Drawing.Point(186, 32);
            this.toRadioButton.Name = "toRadioButton";
            this.toRadioButton.Size = new System.Drawing.Size(43, 24);
            this.toRadioButton.TabIndex = 16;
            this.toRadioButton.Text = "To:";
            this.toRadioButton.CheckedChanged += new System.EventHandler(this.toRadioButton_CheckedChanged);
            // 
            // toDateTimePicker
            // 
            this.toDateTimePicker.CustomFormat = "";
            this.toDateTimePicker.Enabled = false;
            this.toDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.toDateTimePicker.Location = new System.Drawing.Point(224, 32);
            this.toDateTimePicker.Name = "toDateTimePicker";
            this.toDateTimePicker.Size = new System.Drawing.Size(88, 20);
            this.toDateTimePicker.TabIndex = 17;
            this.toDateTimePicker.Value = new System.DateTime(2006, 10, 2, 0, 0, 0, 0);
            // 
            // toolTip
            // 
            this.toolTip.AutomaticDelay = 0;
            this.toolTip.ShowAlways = true;
            // 
            // groupBox4
            // 
            this.groupBox4.ForeColor = System.Drawing.Color.Blue;
            this.groupBox4.Location = new System.Drawing.Point(248, 64);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(77, 80);
            this.groupBox4.TabIndex = 18;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Time/Days";
            // 
            // groupBox5
            // 
            this.groupBox5.ForeColor = System.Drawing.Color.Blue;
            this.groupBox5.Location = new System.Drawing.Point(168, 64);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(72, 80);
            this.groupBox5.TabIndex = 19;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "%Elapsed";
            // 
            // allRadioButton
            // 
            this.allRadioButton.AutoSize = true;
            this.allRadioButton.Location = new System.Drawing.Point(320, 34);
            this.allRadioButton.Name = "allRadioButton";
            this.allRadioButton.Size = new System.Drawing.Size(58, 17);
            this.allRadioButton.TabIndex = 20;
            this.allRadioButton.TabStop = true;
            this.allRadioButton.Text = "All time";
            this.allRadioButton.UseVisualStyleBackColor = true;
            this.allRadioButton.CheckedChanged += new System.EventHandler(this.allRadioButton_CheckedChanged);
            // 
            // SummaryControl
            // 
            this.Controls.Add(this.allRadioButton);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.fromDateTimePicker);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.browseButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.toDateTimePicker);
            this.Controls.Add(this.toRadioButton);
            this.Controls.Add(this.fromRadioButton);
            this.Controls.Add(this.parentTaskComboBox);
            this.Name = "SummaryControl";
            this.Size = new System.Drawing.Size(408, 360);
            this.groupBox3.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

        private void InitializaInnerUserControls()
        {
            this.indicator1 = new PTM.View.Controls.IndicatorControl();
            this.indicator2 = new PTM.View.Controls.IndicatorControl();
            this.indicator3 = new PTM.View.Controls.IndicatorControl();
            this.indicator4 = new PTM.View.Controls.IndicatorControl();
            // 
            // indicator1
            // 
            this.indicator1.BackColor = System.Drawing.Color.Black;
            this.indicator1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.indicator1.ForeColor = System.Drawing.Color.Lime;
            this.indicator1.Location = new System.Drawing.Point(3, 16);
            this.indicator1.Name = "indicator1";
            this.indicator1.Size = new System.Drawing.Size(66, 61);
            this.indicator1.TabIndex = 0;
            // 
            // indicator2
            // 
            this.indicator2.BackColor = System.Drawing.Color.Black;
            this.indicator2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.indicator2.ForeColor = System.Drawing.Color.Lime;
            this.indicator2.Location = new System.Drawing.Point(3, 16);
            this.indicator2.Name = "indicator2";
            this.indicator2.Size = new System.Drawing.Size(66, 61);
            this.indicator2.TabIndex = 0;
            // 
            // indicator3
            // 
            this.indicator3.BackColor = System.Drawing.Color.Black;
            this.indicator3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.indicator3.ForeColor = System.Drawing.Color.Lime;
            this.indicator3.Location = new System.Drawing.Point(3, 16);
            this.indicator3.Name = "indicator3";
            this.indicator3.Size = new System.Drawing.Size(66, 61);
            this.indicator3.TabIndex = 0;

            // 
            // indicator4
            // 
            this.indicator4.BackColor = System.Drawing.Color.Black;
            this.indicator4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.indicator4.ForeColor = System.Drawing.Color.Lime;
            this.indicator4.Location = new System.Drawing.Point(3, 16);
            this.indicator4.Name = "indicator4";
            this.indicator4.Size = new System.Drawing.Size(66, 61);
            this.indicator4.TabIndex = 0;

            this.groupBox1.Controls.Add(this.indicator1);
            this.groupBox2.Controls.Add(this.indicator2);
            this.groupBox4.Controls.Add(this.indicator3);
            this.groupBox5.Controls.Add(this.indicator4);
        }
		#endregion

		private double totalTime = 0;
		private double totalActiveTime = 0;
		private int workedDays = 0;
        private double totalEstimation = 0;
	    private double totalTimeOverEstimation = 0;

        #region Events

        public override void OnTabPageSelected()
		{
			base.OnTabPageSelected();
            if(fromDateTimePicker.Value.Date == DateTime.Today) //autorefresh if today summary is selected.
                LaunchSummarySearch();
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            TaskSelectForm tgForm = new TaskSelectForm();
            if (tgForm.ShowDialog(this) == DialogResult.OK)
                SetParent(tgForm.SelectedTaskId);

        }

        private void dateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            LaunchSummarySearch();
        }

        void fromDateTimePicker_CloseUp(object sender, EventArgs e)
        {
            if (this.fromRadioButton.Checked)
            {
                this.toDateTimePicker.ValueChanged -= new EventHandler(dateTimePicker_ValueChanged);
                this.toDateTimePicker.Value = this.fromDateTimePicker.Value;
                this.toDateTimePicker.ValueChanged += new EventHandler(dateTimePicker_ValueChanged);
            }
            this.fromDateTimePicker.ValueChanged += new EventHandler(dateTimePicker_ValueChanged);
            if (!fromDateTimePicker.Value.Equals(timeBeforeDropDown_fromDateTimePicker))
                LaunchSummarySearch();
        }

        private DateTime timeBeforeDropDown_fromDateTimePicker;
        void fromDateTimePicker_DropDown(object sender, EventArgs e)
        {
            timeBeforeDropDown_fromDateTimePicker = this.fromDateTimePicker.Value;
            this.fromDateTimePicker.ValueChanged -= new EventHandler(dateTimePicker_ValueChanged);
        }

        void toDateTimePicker_CloseUp(object sender, EventArgs e)
        {
            this.toDateTimePicker.ValueChanged += new EventHandler(dateTimePicker_ValueChanged);
            if (!toDateTimePicker.Value.Equals(timeBeforeDropDown_toDateTimePicker))
                LaunchSummarySearch();
        }

        private DateTime timeBeforeDropDown_toDateTimePicker;
        void toDateTimePicker_DropDown(object sender, EventArgs e)
        {
            timeBeforeDropDown_toDateTimePicker = this.toDateTimePicker.Value;
            this.toDateTimePicker.ValueChanged -= new EventHandler(dateTimePicker_ValueChanged);
        }

        private void parentTaskComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (parentTaskComboBox.SelectedIndex == -1)
                return;

            parentTask = FindOnRecentParentTaskById(Convert.ToInt32(parentTaskComboBox.SelectedValue));
            if(this.allRadioButton.Checked)
                DoRangeSummarySearch();
            else
                LaunchSummarySearch();
        }

        private void toolBar_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            if (e.Button.ImageIndex == 0)
                GoToChildDetail();
            else if (e.Button.ImageIndex == 1)
                GoToParentDetail();
        }


        private void taskList_DoubleClick(object sender, EventArgs e)
        {
            if (this.taskList.SelectedItems.Count == 0)
                return;

            TaskSummary sum = (TaskSummary)this.taskList.SelectedItems[0].Tag;
            SetParent(sum.TaskId);
        }

        private void toRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if(this.toRadioButton.Checked)
            {
                this.fromDateTimePicker.Enabled = true;
                this.toDateTimePicker.Enabled = true;
                this.fromRadioButton.Text = "From:";               
            }
        }

        private void fromRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if(this.fromRadioButton.Checked)
            {
                this.fromDateTimePicker.Enabled = true;
                this.toDateTimePicker.Enabled = false;
                this.toDateTimePicker.Value = this.fromDateTimePicker.Value;
                this.fromRadioButton.Text = "Date:";               
            }
        }

        private void allRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if(this.allRadioButton.Checked)
            {
                DoRangeSummarySearch();
            }
        }

	    private void DoRangeSummarySearch()
	    {
	        this.fromRadioButton.Text = "From:";
	        this.fromDateTimePicker.Enabled = false;
	        this.toDateTimePicker.Enabled = false;
	        DateRange range;
	        SetWaitCursor();
	        range = Logs.GetTaskLogDateRange(this.parentTask.Id);
	        if (range.StartDate > range.EndDate)
	        {
	            this.toDateTimePicker.ValueChanged -= new EventHandler(dateTimePicker_ValueChanged);
	            this.toDateTimePicker.Value = DateTime.Today;
	            this.toDateTimePicker.ValueChanged += new EventHandler(dateTimePicker_ValueChanged);

	            this.fromDateTimePicker.ValueChanged -= new EventHandler(dateTimePicker_ValueChanged);
	            this.fromDateTimePicker.Value = DateTime.Today;
	            this.fromDateTimePicker.ValueChanged += new EventHandler(dateTimePicker_ValueChanged);
	        }
	        else
	        {
	            this.toDateTimePicker.ValueChanged -= new EventHandler(dateTimePicker_ValueChanged);
	            this.toDateTimePicker.Value = range.EndDate.Date;
	            this.toDateTimePicker.ValueChanged += new EventHandler(dateTimePicker_ValueChanged);

	            this.fromDateTimePicker.ValueChanged -= new EventHandler(dateTimePicker_ValueChanged);
	            this.fromDateTimePicker.Value = range.StartDate.Date;
	            this.fromDateTimePicker.ValueChanged += new EventHandler(dateTimePicker_ValueChanged);
	        }
	        LaunchSummarySearch();
	    }

	    #endregion

        private void UpdateTasksSummary(TaskSummaryResult taskSummaryResult)
		{
			try
			{
				totalTime = 0;
				workedDays = taskSummaryResult.WorkedDays;
				this.taskList.BeginUpdate();
				this.taskList.Items.Clear();
				foreach (TaskSummary summary in taskSummaryResult.SummaryList)
				{
					totalTime += summary.TotalActiveTime;
					totalTime += summary.TotalInactiveTime;
				    totalEstimation += summary.TotalEstimation;
                    totalTimeOverEstimation += summary.TotalTimeOverEstimation;
					if (summary.IsActive)
						totalActiveTime += summary.TotalActiveTime;

                    TreeListViewItem lvi =
                        new TreeListViewItem(summary.Description,
                                             new string[]
                                                {
                                                    String.Empty,
                                                    0.ToString("0.0%", CultureInfo.InvariantCulture),
                                                    String.Empty,
                                                    0.ToString("0.0%", CultureInfo.InvariantCulture),
                                                    String.Empty,
                                                    String.Empty
                                                });

                    lvi.ImageIndex = summary.IconId;

					lvi.Tag = summary;
					this.taskList.Items.Add(lvi);
				}
				CalculateTasksPercents();
				SetIndicatorsValues();
			}
			finally
			{
				this.taskList.EndUpdate();
				SetReadyState();
			}
		}

		private void CalculateTasksPercents()
		{
			foreach (ListViewItem item in this.taskList.Items)
			{
				double percent = 0;
			    double goalPercent = 0;

				TaskSummary summary = (TaskSummary) item.Tag;

				if (totalTime > 0)
				    percent = (summary.TotalActiveTime + summary.TotalInactiveTime)/totalTime;

                if (summary.TotalEstimation > 0)
                    goalPercent = summary.TotalTimeOverEstimation / (summary.TotalEstimation*60);

                TimeSpan activeTimeSpan = new TimeSpan(0, 0, Convert.ToInt32(summary.TotalActiveTime));
                TimeSpan inactiveTimeSpan = new TimeSpan(0, 0, Convert.ToInt32(summary.TotalInactiveTime));
                TimeSpan elapsedTimeSpan = new TimeSpan(0, 0, Convert.ToInt32(summary.TotalActiveTime + summary.TotalInactiveTime));
                TimeSpan estimationTimeSpan = new TimeSpan(0, Convert.ToInt32(summary.TotalEstimation), 0);
                string estimation;
                if (summary.TotalEstimation == 0)
                    estimation = "Not estimated";
                else
                    estimation = ViewHelper.TimeSpanToTimeString(estimationTimeSpan);

                item.SubItems[TimeHeader.Index].Text = ViewHelper.TimeSpanToTimeString(elapsedTimeSpan);
                item.SubItems[ActiveTimeHeader.Index].Text = ViewHelper.TimeSpanToTimeString(activeTimeSpan);
                item.SubItems[InactiveTimeHeader.Index].Text = ViewHelper.TimeSpanToTimeString(inactiveTimeSpan);
                item.SubItems[GoalHeader.Index].Text = estimation;
				item.SubItems[PercentHeader.Index].Text = percent.ToString("0.0%", CultureInfo.InvariantCulture);
                item.SubItems[PercentGoalHeader.Index].Text = goalPercent.ToString("0.0%", CultureInfo.InvariantCulture);

                if (goalPercent>1)
			        item.SubItems[PercentGoalHeader.Index].ForeColor = Color.Red;
                			    
			}
		}

		private void SetIndicatorsValues()
		{
			indicator1.Value = Convert.ToInt32(Math.Min(30600, totalTime));
			indicator1.TextValue =
				new TimeSpan(0, 0, Convert.ToInt32(totalTime, CultureInfo.InvariantCulture)).TotalHours.ToString("0.00",
				                                                                                                 CultureInfo.
				                                                                                                 	InvariantCulture) +
				" hrs.";

			if (totalTime > 0)
			{
				int percentActiveTime = Convert.ToInt32(this.totalActiveTime*100/totalTime);
				indicator2.Value = percentActiveTime;
				indicator2.TextValue = percentActiveTime + "%";
				string activeTime = new TimeSpan(0, 0, Convert.ToInt32(totalActiveTime, CultureInfo.InvariantCulture)).TotalHours.
				                    	ToString("0.00",
				                    	         CultureInfo.
				                    	         	InvariantCulture) +
				                    " hrs.";
				toolTip.SetToolTip(this.indicator2, activeTime);
				toolTip.SetToolTip(this.groupBox2, activeTime);
			}

			if(workedDays>0)
			{
				indicator3.Value = Convert.ToInt32(Math.Min(30600, totalTime/workedDays));
				indicator3.TextValue =
					new TimeSpan(0, 0, Convert.ToInt32(totalTime/workedDays, CultureInfo.InvariantCulture)).TotalHours.ToString("0.00",
					CultureInfo.
					InvariantCulture) +
					" hrs.";
			}

            toolTip.SetToolTip(this.indicator3, workedDays + " worked days");
            toolTip.SetToolTip(this.groupBox4, workedDays + " worked days");

            if (totalEstimation > 0)
            {
                int percentGoals = Convert.ToInt32(totalTimeOverEstimation * 100 / (totalEstimation * 60.0));
                indicator4.Value = Convert.ToInt32(Math.Min(100, percentGoals));
                indicator4.TextValue = percentGoals + "%";
                if (percentGoals>100)
                    indicator4.ForeColor = Color.Red;    
            }

		    string totalEstimatedString = ViewHelper.TimeSpanToTimeString(new TimeSpan(0, 0, Convert.ToInt32(totalEstimation*60)));

            toolTip.SetToolTip(this.indicator4, totalEstimatedString + " estimated time");
            toolTip.SetToolTip(this.groupBox5, totalEstimatedString + " estimated time");


		}    
		       			
		private Task FindOnRecentParentTaskById(int taskId)
		{
			for(int i = 0;i<recentParentTasksList.Count;i++)
			{
				Task task = (Task)recentParentTasksList[i];
				if(task.Id == taskId)
					return task.Clone();
			}
			return null;
		}

        private void SetParent(int parentId)
		{
			if (FindOnRecentParentTaskById(parentId) == null)
			{
				parentTask = Tasks.FindById(parentId);
				parentTask.Description = ViewHelper.FixTaskPath(Tasks.GetFullPath(parentTask.Id), this.parentTaskComboBox.MaxLength);
				this.recentParentTasksList.Insert(0, parentTask);
				this.parentTaskComboBox.BeginUpdate();
				this.parentTaskComboBox.DataSource = null;
				this.parentTaskComboBox.DisplayMember = "Description";
				this.parentTaskComboBox.ValueMember = "Id";
				this.parentTaskComboBox.DataSource = recentParentTasksList;
				this.parentTaskComboBox.EndUpdate();
			}
			this.parentTaskComboBox.SelectedValue = parentId;
		}

		private void GoToParentDetail()
		{
			if (Tasks.RootTask.Id == this.parentTask.Id)
			{
				return;
			}
			SetParent(parentTask.ParentId);
		}

		private void GoToChildDetail()
		{
			if (this.taskList.SelectedItems.Count == 0)
			{
				return;
			}
			TaskSummary sum = (TaskSummary) this.taskList.SelectedItems[0].Tag;
			SetParent(sum.TaskId);
        }

        #region Framework events

        private delegate void TaskLogTimer_ElapsedDelegate(object sender, ElapsedEventArgs e);
        private void TaskLogTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (this.InvokeRequired)
            {
                TaskLogTimer_ElapsedDelegate del = new TaskLogTimer_ElapsedDelegate(TaskLogTimer_Elapsed);
                this.Invoke(del, new object[] { sender, e });
            }
            else
            {
                UpdateSummaryTime();
            }
        }

        private void UpdateSummaryTime()
        {
            if (!this.Visible)
                return;

            if (this.fromDateTimePicker.Value > DateTime.Today || this.toDateTimePicker.Value < DateTime.Today)
                return;

            ListViewItem currentTaskSummary = null;

            int minGeneration = Int32.MaxValue;
            foreach (ListViewItem item in this.taskList.Items)
            {
                TaskSummary sum = (TaskSummary)item.Tag;
                int generations = Tasks.IsParent(sum.TaskId, Logs.CurrentLog.TaskId);
                if (generations >= 0 && generations < minGeneration)
                {
                    minGeneration = generations;
                    currentTaskSummary = item;
                }
            }

            if (currentTaskSummary != null)
            {
                TaskSummary sum = (TaskSummary)currentTaskSummary.Tag;
                if (!sum.IsActive)
                {
                    sum.TotalInactiveTime++;
                }
                else
                {
                    totalActiveTime++;
                    sum.TotalActiveTime++;
                }
                totalTime++;

                this.CalculateTasksPercents();
                SetIndicatorsValues();
            }
        }

        #endregion

        #region AsyncWork

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = GetTasksSummary((TaskSummaryArguments)e.Argument);
            if (worker.CancellationPending)
                e.Cancel = true;
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled) return;
            UpdateTasksSummary((TaskSummaryResult)e.Result);
        }

        private void LaunchSummarySearch()
        {
            SetWaitState();
            TaskSummaryArguments args = new TaskSummaryArguments();
            args.FromDate = fromDateTimePicker.Value.Date;
            args.ToDate = toDateTimePicker.Value.Date.AddDays(1).AddSeconds(-1);
            args.ParentTask = Tasks.FindById((int)this.parentTaskComboBox.SelectedValue);
            if (worker.IsBusy) worker.CancelAsync();
            while (worker.IsBusy) Application.DoEvents();
            worker.RunWorkerAsync(args);
        }

        private static TaskSummaryResult GetTasksSummary(TaskSummaryArguments args)
		{			
			TaskSummaryResult result = new TaskSummaryResult();
			result.SummaryList = TasksSummaries.GetTaskSummary(args.ParentTask, args.FromDate, args.ToDate);
            result.WorkedDays = TasksSummaries.GetWorkedDays(args.FromDate.Date, args.ToDate.Date);
			return result;
		}

		private class TaskSummaryResult
		{
			public ArrayList SummaryList;
			public int WorkedDays;
		}

        private class TaskSummaryArguments
        {
            public Task ParentTask;
            public DateTime FromDate;
            public DateTime ToDate;
        }

		private void SetWaitState()
		{
			this.Status = "Retrieving data...";
            this.parentTaskComboBox.Enabled = false;
            toDateTimePicker.Enabled = false;
            fromDateTimePicker.Enabled = false;
            this.fromRadioButton.Enabled = false;
            this.toRadioButton.Enabled = false;
            this.allRadioButton.Enabled = false;
            this.browseButton.Enabled = false;
            this.taskList.Items.Clear();

			indicator1.Maximum = 30600; //8.5 hrs.
			indicator1.Value = 0;
			indicator1.TextValue = "0.00 hrs.";
			indicator1.ForeColor = Color.Lime;

			indicator2.Maximum = 100;
			indicator2.Value = 0;
			indicator2.TextValue = "0%";

			indicator3.Maximum = 30600; //8.5 hrs.
			indicator3.Value = 0;
			indicator3.TextValue = "0.00 hrs.";
			indicator3.ForeColor = Color.Lime;

            indicator4.Maximum = 100;
            indicator4.Value = 0;
            indicator4.TextValue = "0%";
            indicator4.ForeColor = Color.Lime;

			totalTime = 0;
			totalActiveTime = 0;
            totalEstimation = 0;
            totalTimeOverEstimation = 0;

			this.Refresh();
		    SetWaitCursor();
		}

	    private void SetWaitCursor()
	    {
	        this.Cursor = Cursors.WaitCursor;
	        foreach (Control control in this.Controls)
	        {
	            control.Cursor = Cursors.WaitCursor;
	        }
	    }

	    private void SetReadyState()
		{
			this.Status = "Ready";
	        SetDefaultCursor();
	        this.parentTaskComboBox.Enabled = true;
            
            if(this.fromRadioButton.Checked)
            {
                fromDateTimePicker.Enabled = true;
            }
			else if (this.toRadioButton.Checked)
			{
			    toDateTimePicker.Enabled = true;
                fromDateTimePicker.Enabled = true;
			}
			this.browseButton.Enabled = true;
            this.fromRadioButton.Enabled = true;
            this.toRadioButton.Enabled = true;
            this.allRadioButton.Enabled = true;
		}

	    private void SetDefaultCursor()
	    {
	        this.Cursor = Cursors.Default;
	        foreach (Control control in this.Controls)
	        {
	            control.Cursor = Cursors.Default;
	        }
	    }

	    #endregion
        
	}
}
