using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using PTM.Framework;
using PTM.Framework.Infos;
using PTM.View.Controls.TreeListViewComponents;
using PTM.View.Forms;

namespace PTM.View.Controls
{
	public class TasksTreeViewControl : UserControl
	{
		private IContainer components;
        public event EventHandler SelectedTaskChanged;

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
					components.Dispose();
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
            components = new System.ComponentModel.Container();
            PTM.View.Controls.TreeListViewComponents.TreeListViewItemCollection.TreeListViewItemCollectionComparer treeListViewItemCollectionComparer1 = new PTM.View.Controls.TreeListViewComponents.TreeListViewItemCollection.TreeListViewItemCollectionComparer();
            mTreeMenu = new System.Windows.Forms.ContextMenu();
            mNuProperties = new System.Windows.Forms.MenuItem();
            mEnuItem5 = new System.Windows.Forms.MenuItem();
            mNuAdd = new System.Windows.Forms.MenuItem();
            mNuRename = new System.Windows.Forms.MenuItem();
            mNuDelete = new System.Windows.Forms.MenuItem();
            mTreeView = new PTM.View.Controls.TreeListViewComponents.TreeListView();
            mTasksColumnHeader = new System.Windows.Forms.ColumnHeader();
            mPriorityColumnHeader = new System.Windows.Forms.ColumnHeader();
            SuspendLayout();
            // 
            // treeMenu
            // 
            mTreeMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            mNuProperties,
            mEnuItem5,
            mNuAdd,
            mNuRename,
            mNuDelete});
            // 
            // mnuProperties
            // 
            mNuProperties.DefaultItem = true;
            mNuProperties.Index = 0;
            mNuProperties.Shortcut = System.Windows.Forms.Shortcut.CtrlP;
            mNuProperties.Text = "Properties...";
            mNuProperties.Click += new System.EventHandler(_MnuPropertiesClick);
            // 
            // menuItem5
            // 
            mEnuItem5.Index = 1;
            mEnuItem5.Text = "-";
            // 
            // mnuAdd
            // 
            mNuAdd.Index = 2;
            mNuAdd.Shortcut = System.Windows.Forms.Shortcut.Ins;
            mNuAdd.Text = "Add New";
            mNuAdd.Click += new System.EventHandler(_MnuAddClick);
            // 
            // mnuRename
            // 
            mNuRename.Index = 3;
            mNuRename.Shortcut = System.Windows.Forms.Shortcut.F2;
            mNuRename.Text = "Rename";
            mNuRename.Click += new System.EventHandler(_MnuRenameClick);
            // 
            // mnuDelete
            // 
            mNuDelete.Index = 4;
            mNuDelete.Shortcut = System.Windows.Forms.Shortcut.Del;
            mNuDelete.Text = "Delete";
            mNuDelete.Click += new System.EventHandler(_MnuDeleteClick);
            // 
            // treeView
            // 
            mTreeView.AllowColumnReorder = true;
            mTreeView.AllowDrop = true;
            mTreeView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            mTasksColumnHeader,
            mPriorityColumnHeader});
            treeListViewItemCollectionComparer1.Column = 0;
            treeListViewItemCollectionComparer1.SortOrder = System.Windows.Forms.SortOrder.Ascending;
            mTreeView.Comparer = treeListViewItemCollectionComparer1;
            mTreeView.ContextMenu = mTreeMenu;
            mTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            mTreeView.HideSelection = false;
            mTreeView.LabelEdit = true;
            mTreeView.Location = new System.Drawing.Point(0, 0);
            mTreeView.MultiSelect = false;
            mTreeView.Name = "mTreeView";
            mTreeView.ShowGroups = false;
            mTreeView.Size = new System.Drawing.Size(359, 215);
            mTreeView.TabIndex = 0;
            mTreeView.UseCompatibleStateImageBehavior = false;
            // 
            // tasksColumnHeader
            // 
            mTasksColumnHeader.Text = "Tasks";
            mTasksColumnHeader.Width = 294;
            // 
            // priorityColumnHeader
            // 
            mPriorityColumnHeader.Text = "Priority";
            mPriorityColumnHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // TasksTreeViewControl
            // 
            Controls.Add(mTreeView);
            Name = "TasksTreeViewControl";
            Size = new System.Drawing.Size(359, 215);
            ResumeLayout(false);

		}

		#endregion

        private TreeListView mTreeView;
		private int mCurrentSelectedTask = -1;
		private MenuItem mEnuItem5;
		private ContextMenu mTreeMenu;
		private MenuItem mNuDelete;
		private MenuItem mNuRename;
		private MenuItem mNuProperties;
        private ColumnHeader mTasksColumnHeader;
        private ColumnHeader mPriorityColumnHeader;
        private MenuItem mNuAdd;
	    private bool mShowHidden;
        public const string NewTask = "New Task";


	    public bool ShowHidden
	    {
	        get { return mShowHidden; }
	        set { mShowHidden = value; }
	    }

	    #region Initialization
        public TasksTreeViewControl()
        {
            InitializeComponent();
            InitCommonControls();
            mTreeView.ItemDrag += _TreeViewItemDrag;
            mTreeView.DragDrop += _TreeViewDragDrop;
            mTreeView.DragOver += _TreeViewDragOver;
            mTreeView.DragEnter += _TreeViewDragEnter;
            mTreeView.DragLeave += _TreeViewDragLeave;
            mTreeView.GiveFeedback += _TreeViewGiveFeedback;
            mTreeView.DoubleClick += _TreeViewDoubleClick;
            mTimer.Tick += _TimerTick;
            mTreeView.SmallImageList = IconsManager.IconsList;
            mTreeView.SelectedIndexChanged += _TreeViewSelectedIndexChanged;
            mTreeView.AfterLabelEdit += _TreeViewAfterLabelEdit;
            mTreeView.BeforeLabelEdit += _TreeViewBeforeLabelEdit;
            mTimer.Interval = 200;
        }

        protected override void OnHandleDestroyed(EventArgs e)
		{
			base.OnHandleDestroyed(e);
			Tasks.TaskChanged -= Tasks_TasksRowChanged;
			Tasks.TaskDeleting -= Tasks_TasksRowDeleting;
		}

		internal void Initialize()
		{
			LoadTree();
			Tasks.TaskChanged += Tasks_TasksRowChanged;
			Tasks.TaskDeleting += Tasks_TasksRowDeleting;
		}

        #endregion

        public void LoadTree()
        {
            mTreeView.Items.Clear();
            TreeListViewItem nodeParent = CreateNode(Tasks.RootTask);
            mTreeView.Items.Add(nodeParent);
            AddChildNodes(Tasks.RootTask, nodeParent);
            nodeParent.Expand();
        }

        internal void AddNewTask()
		{
			int newId;
			try
			{
                if(mTreeView.SelectedItems.Count<=0)
                {
                    MessageBox.Show("Please select a task.", ParentForm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
			    var parentId = (int) mTreeView.SelectedItems[0].Tag;
			    string newTaskName = GetNewTaskName(parentId);
                newId = Tasks.AddTask(newTaskName, parentId).Id;
			}
			catch (ApplicationException aex)
			{
				MessageBox.Show(aex.Message, ParentForm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
			}
			Application.DoEvents();//first insert the new node (event fired)
            TreeListViewItem node = FindTaskNode(newId);
            node.EnsureVisible();
		    node.Selected = true;
            mTreeView.Refresh();
			node.BeginEdit();
		}

        private static string GetNewTaskName(int parentId)
        {
            if(Tasks.FindByParentIdAndDescription(parentId, NewTask)==null)
               return NewTask;
            
            int counter = 1;
            string newTaskName;
            do
            {
                newTaskName = NewTask + counter;
                counter++;
            } while (Tasks.FindByParentIdAndDescription(parentId, newTaskName) != null);
            return newTaskName;
        }

		internal void EditSelectedTaskDescription()
		{
            if (mTreeView.SelectedItems.Count <= 0)
            {
                MessageBox.Show("Please select a task.", ParentForm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
			mTreeView.SelectedItems[0].BeginEdit();
		}

        void _TreeViewBeforeLabelEdit(object sender, TreeListViewBeforeLabelEditEventArgs e)
        {
            if(mPriorityColumnHeader == mTreeView.Columns[e.ColumnIndex])
            {
                var cbx = new ComboBox();
                cbx.Items.AddRange(new object[]{"(null)",1,2,3,4,5,6,7,8,9});
                e.Editor = cbx;
                cbx.Text = e.Label;
            }
        }

		private void _TreeViewAfterLabelEdit(object sender, TreeListViewLabelEditEventArgs e)
		{
            Task task = Tasks.FindById(Convert.ToInt32(e.Item.Tag));

			if (task != null)
			{
				if(mTreeView.Columns[e.ColumnIndex] == mTasksColumnHeader)
                {
                    if (string.IsNullOrEmpty( e.Label ))
                    {
                        e.Cancel = true;
                        return;
                    }
                    task.Description = e.Label;
                }
                if (mPriorityColumnHeader == mTreeView.Columns[e.ColumnIndex])
                {
                    int priority;
                    if (string.IsNullOrEmpty( e.Label ) || e.Label == "(null)")
                    {
                        task.Priority = 0;
                    }
                    else if (int.TryParse(e.Label, out priority) && priority >= 0 && priority<=9)
                    {
                        task.Priority = priority;
                    }
                    else
                    {
                        e.Cancel = true;
                        return;
                    }
                }
				try
				{
					Tasks.UpdateTask(task);
				}
				catch (ApplicationException aex)
				{
					MessageBox.Show(aex.Message, ParentForm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
                finally
				{
                    e.Cancel = true; //always cancel, the event Tasks.TaskChanged will change the value.
				}
			}
			else
			{
				MessageBox.Show("This task has been deleted.", ParentForm.Text, MessageBoxButtons.OK,
				                MessageBoxIcon.Information);
			}
		}
        
        internal void DeleteSelectedTask()
        {
            if (mTreeView.SelectedItems.Count <= 0)
            {
                MessageBox.Show("Please select a task.", ParentForm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (MessageBox.Show(
                    "All tasks and sub-tasks assigned to this task will be deleted too. \nAre you sure you want to delete '" +
                    mTreeView.SelectedItems[0].Text + "'?",
                    ParentForm.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2,
                    MessageBoxOptions.DefaultDesktopOnly)
                == DialogResult.OK)
            {
                try
                {
                    Cursor.Current = Cursors.WaitCursor;
                    Tasks.DeleteTask((int)mTreeView.SelectedItems[0].Tag);
                }
                catch (ApplicationException aex)
                {
                    Cursor.Current = Cursors.Default;
                    MessageBox.Show(aex.Message, ParentForm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                finally
                {
                    Cursor.Current = Cursors.Default;
                }
            }
        }

        private void AddChildNodes(Task parentRow, TreeListViewItem nodeParent)
        {
            Task[] childTasks = Tasks.GetChildTasks(parentRow.Id);
            foreach (Task task in childTasks)
            {
                if (task.Id == Tasks.IdleTask.Id)
                    continue;
                if(task.Hidden && !mShowHidden)
                    continue;
                TreeListViewItem nodeChild = CreateNode(task);
                nodeParent.Items.Add(nodeChild);
                AddChildNodes(task, nodeChild);
            }
        }

        private static TreeListViewItem CreateNode(Task task)
        {
        	string priority = task.Priority > 0 ? task.Priority.ToString() : String.Empty;
            var node = new TreeListViewItem(task.Description, new[] { priority })
            {
            	ImageIndex = task.IconId,
            	Tag = task.Id
            };
        	return node;
        }

        private TreeListViewItem FindTaskNode(int taskId)
        {
            return FindNode(taskId, mTreeView.Items);
        }

        private static TreeListViewItem FindNode(int taskId, TreeListViewItemCollection nodes)
        {
            foreach (TreeListViewItem node in nodes)
            {
            	if ((int)node.Tag == taskId)
                {
                    return node;
                }
            	if (node.Items.Count > 0)
            	{
            		TreeListViewItem childnode = FindNode(taskId, node.Items);
            		if (childnode != null)
            			return childnode;
            	}
            }
        	return null;
        }


		internal int SelectedTaskId
		{
			get { return mCurrentSelectedTask; }
			set
			{
				if (mCurrentSelectedTask == value)
					return;
				TreeListViewItem node = FindTaskNode(value);
				if (node == null)
					return;
				mCurrentSelectedTask = value;
			    node.Selected = true;
				
				if (SelectedTaskChanged != null)
				{
					SelectedTaskChanged(this, new EventArgs());
				}
			}
		}

        void _TreeViewSelectedIndexChanged(object sender, EventArgs e)
        {
            if(mTreeView.SelectedItems.Count<=0)
            {
                return;
            }
            if (mCurrentSelectedTask != (int)mTreeView.SelectedItems[0].Tag)
            {
                mCurrentSelectedTask = (int)mTreeView.SelectedItems[0].Tag;
                if (SelectedTaskChanged != null)
                {
                    SelectedTaskChanged(sender, e);
                }
            }
        }

        #region Drag And Drop

        private readonly Timer mTimer = new Timer();
        private readonly ImageList mImageListDrag = new ImageList();
        private TreeListViewItem mDragNode;
        private TreeListViewItem mTempDropNode;

        [DllImport("comctl32.dll")]
        internal static extern bool InitCommonControls();

        [DllImport("comctl32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ImageList_BeginDrag(IntPtr himlTrack, int iTrack, int dxHotspot, int dyHotspot);

        [DllImport("comctl32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ImageList_DragMove(int x, int y);

        [DllImport("comctl32.dll", CharSet = CharSet.Auto)]
        internal static extern void ImageList_EndDrag();

        [DllImport("comctl32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ImageList_DragEnter(IntPtr hwndLock, int x, int y);

        [DllImport("comctl32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ImageList_DragLeave(IntPtr hwndLock);

        [DllImport("comctl32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ImageList_DragShowNolock([MarshalAs(UnmanagedType.Bool)]bool fShow);

        private const int TREE_VIEW_INDENT = 19;

        private void _TreeViewItemDrag(object sender, ItemDragEventArgs e)
        {
            // Get drag node and select it
            mDragNode = (TreeListViewItem)e.Item;
            if((int)mDragNode.Tag == Tasks.RootTask.Id)
                return;
            // Reset image list used for drag image
            mImageListDrag.Images.Clear();
            mImageListDrag.ImageSize =
            new Size(Math.Min(mDragNode.Bounds.Size.Width + TREE_VIEW_INDENT, 256), mDragNode.Bounds.Height);


            // Create new bitmap
            // This bitmap will contain the tree node image to be dragged
            var bmp = new Bitmap(Math.Min(mDragNode.Bounds.Width + TREE_VIEW_INDENT, 256), mDragNode.Bounds.Height);

            // Get graphics from bitmap
            using (Graphics gfx = Graphics.FromImage(bmp))
            {
                // Draw node icon into the bitmap
                gfx.DrawImage(IconsManager.IconsList.Images[mDragNode.ImageIndex], 0, 0);

                // Draw node label into bitmap
                gfx.DrawString(mDragNode.Text,
                               mTreeView.Font,
                               new SolidBrush(mTreeView.ForeColor),
                               TREE_VIEW_INDENT, 1.0f);
            }

            // Add bitmap to imagelist
            mImageListDrag.Images.Add(bmp);

            // Get mouse position in client coordinates
            Point p = mTreeView.PointToClient(MousePosition);
            // Compute delta between mouse position and node bounds
            //			int dx = p.X + treeView.Indent - dragNode.Bounds.Left;
            //			int dy = p.Y - dragNode.Bounds.Top;

            int dx = p.X - mDragNode.GetBounds(ItemBoundsPortion.Label).Left - mTreeView.Location.X;
            int dy = p.Y - mDragNode.Bounds.Top - mTreeView.Location.Y;

            // Begin dragging image
            if (ImageList_BeginDrag(mImageListDrag.Handle, 0, dx, dy))
            {
                // Begin dragging
                mTreeView.DoDragDrop(bmp, DragDropEffects.Move);
                // End dragging image
                ImageList_EndDrag();
            }
        }

        private void _TreeViewDragOver(object sender, DragEventArgs e)
        {
            // Compute drag position and move image
            Point formP = PointToClient(new Point(e.X, e.Y));
            ImageList_DragMove(formP.X - mTreeView.Left, formP.Y - mTreeView.Top);
            
            // Get actual drop node
            TreeListViewItem dropNode = mTreeView.GetItemAt(mTreeView.PointToClient(new Point(e.X, e.Y)));
            if (dropNode == null)
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            e.Effect = DragDropEffects.Move;

            // if mouse is on a new node select it
            if (mTempDropNode != dropNode)
            {
                ImageList_DragShowNolock(false);

                if (mTempDropNode != null)
                {
                    mTreeView.Refresh();                  
                }
                dropNode.DrawInsertionLine();

                ImageList_DragShowNolock(true);
                mTempDropNode = dropNode;
            }

            // Avoid that drop node is child of drag node 
            TreeListViewItem tmpNode = dropNode;
            while (tmpNode.Parent != null)
            {
                if (tmpNode.Parent == mDragNode) e.Effect = DragDropEffects.None;
                tmpNode = tmpNode.Parent;
            }
        }

        private void _TreeViewDragDrop(object sender, DragEventArgs e)
        {
            // Unlock updates
            ImageList_DragLeave(mTreeView.Handle);

            // Get drop node
            TreeListViewItem dropNode = mTreeView.GetItemAt(mTreeView.PointToClient(new Point(e.X, e.Y)));

            // If drop node isn't equal to drag node, add drag node as child of drop node
            if (mDragNode != dropNode)
            {
                // Remove drag node from parent
                if (mDragNode.Parent == null)
                {
                    mTreeView.Items.Remove(mDragNode);
                }
                else
                {
                    mDragNode.Parent.Items.Remove(mDragNode);
                }

                // Add drag node to drop node
                dropNode.Items.Add(mDragNode);
                dropNode.Expand();

                try
                {
                    Cursor.Current = Cursors.WaitCursor;
                    Tasks.UpdateParentTask((int)mDragNode.Tag, (int)dropNode.Tag);
                }
                catch (ApplicationException aex)
                {
                    Cursor.Current = Cursors.Default;
                    MessageBox.Show(aex.Message, ParentForm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                finally
                {
                    Cursor.Current = Cursors.Default;
                }

                // Set drag node to null
                mDragNode = null;

                // Disable scroll timer
                mTimer.Enabled = false;
            }
            mTreeView.Refresh(); 
        }

        private void _TreeViewDragEnter(object sender, DragEventArgs e)
        {
            ImageList_DragEnter(mTreeView.Handle, e.X - mTreeView.Left,
                                e.Y - mTreeView.Top);

            // Enable timer for scrolling dragged item
            mTimer.Enabled = true;
        }

        private void _TreeViewDragLeave(object sender, EventArgs e)
        {
            ImageList_DragLeave(mTreeView.Handle);
            mTreeView.Refresh();
            // Disable timer for scrolling dragged item
            mTimer.Enabled = false;
        }

        private void _TreeViewGiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            if (e.Effect == DragDropEffects.Move)
            {
                // Show pointer cursor while dragging
                e.UseDefaultCursors = false;
                mTreeView.Cursor = Cursors.Default;
            }
            else e.UseDefaultCursors = true;
        }

        private void _TimerTick(object sender, EventArgs e)
        {
            // get node at mouse position
            Point pt = mTreeView.PointToClient(MousePosition);
            TreeListViewItem node = mTreeView.GetItemAt(pt);
            if (node == null) return;

            // if mouse is near to the top, scroll up
            if (pt.Y < 30)
            {
                // set actual node to the upper one
                if (node.PrevVisibleItem != null)
                {
                    node = node.PrevVisibleItem;

                    // hide drag image
                    ImageList_DragShowNolock(false);
                    // scroll and refresh
                    node.EnsureVisible();
                    mTreeView.Refresh();
                    // show drag image
                    ImageList_DragShowNolock(true);
                }
            }
            // if mouse is near to the bottom, scroll down
            else if (pt.Y > mTreeView.Size.Height - 30)
            {
                if (node.NextVisibleItem != null)
                {
                    node = node.NextVisibleItem;

                    ImageList_DragShowNolock(false);
                    node.EnsureVisible();
                    mTreeView.Refresh();
                    ImageList_DragShowNolock(true);
                }
            }
        }

        #endregion

        private void _MnuAddClick(object sender, EventArgs e)
        {
            AddNewTask();
        }

        private void _MnuRenameClick(object sender, EventArgs e)
        {
            EditSelectedTaskDescription();
        }

		private void _MnuDeleteClick(object sender, EventArgs e)
		{
			DeleteSelectedTask();
		}

		public void ShowPropertiesSelectedTask()
		{
            if(mTreeView.SelectedItems.Count==0)
                return;
			if(Tasks.RootTask.Id == (int) mTreeView.SelectedItems[0].Tag)
                return;
			var pf = new TaskPropertiesForm((int) mTreeView.SelectedItems[0].Tag);
			pf.ShowDialog(this);
		}

		private void _MnuPropertiesClick(object sender, EventArgs e)
		{
			ShowPropertiesSelectedTask();
		}

		private void _TreeViewDoubleClick(object sender, EventArgs e)
		{
			OnDoubleClick(e);
        }


        #region Framework Events

        private void Tasks_TasksRowChanged(object sender, TaskChangeEventArgs e)
        {
        	if (e.Action == DataRowAction.Add)
            {
                TreeListViewItem nodeParent = FindTaskNode(e.Task.ParentId);
                TreeListViewItem nodeChild = CreateNode(e.Task);
                nodeParent.Items.Add(nodeChild);
                return;
            }
        	if (e.Action == DataRowAction.Change)
        	{
        		TreeListViewItem node = FindTaskNode(e.Task.Id);
        		node.Text = e.Task.Description;
        		node.ImageIndex = e.Task.IconId;
        		string priority = e.Task.Priority == 0 ? String.Empty : e.Task.Priority.ToString();
        		node.SubItems[mPriorityColumnHeader.Index].Text = priority;
        	}
        }

		private void Tasks_TasksRowDeleting( object sender, TaskChangeEventArgs e )
        {
            TreeListViewItem node = FindTaskNode(e.Task.Id);
            if (node != null && node.ListView != null)
                node.Remove();
        }
        #endregion



    }
}