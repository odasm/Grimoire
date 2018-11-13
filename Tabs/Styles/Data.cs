﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataCore;
using DataCore.Structures;

namespace Grimoire.Tabs.Styles
{
    public partial class Data : UserControl
    {
        #region Properties

        private string key;
        private Core core;
        private readonly Logs.Manager lManager;
        private readonly Tabs.Manager tManager;
        private readonly Utilities.Grid gridUtils;
        public List<IndexEntry> FilteredIndex = new List<IndexEntry>();
        public bool Filtered { get { return FilteredIndex.Count > 0; } }
        public int FilterCount { get { return FilteredIndex.Count; } }

        public DataCore.Core Core
        {
            get
            {
                if (core == null) { throw new Exception("DataCore is null!"); }
                return core;
            }
        }

        private bool grid_cs_enabled
        {
            get
            {
                if (grid.SelectedRows.Count > 1)
                    return !grid_cs.Items[0].Enabled && grid_cs.Items[1].Enabled && grid_cs.Items[2].Enabled;
                else
                    return grid_cs.Items[0].Enabled && grid_cs.Items[1].Enabled && grid_cs.Items[2].Enabled;
            }
            set
            {
                grid_cs.Enabled = value;
            }
        }

        private bool extensions_cs_enabled
        {
            get { return extensions_cs.Enabled; }
            set { extensions_cs.Enabled = value; }
        }

        private bool search_enabled
        {
            get { return searchInput.Enabled; }
            set { searchInput.Enabled = value; }
        }

        private bool tab_disabled
        {
            set
            {
                bool flipVal = (value) ? false : true;
                grid_cs_enabled = flipVal;
                extensions_cs_enabled = flipVal;
                search_enabled = flipVal;
            }
        }

        #endregion

        #region Constructors

        public Data()
        {
            InitializeComponent();
            initializeCore();
            lManager = Logs.Manager.Instance;
            tManager = Tabs.Manager.Instance;
            gridUtils = new Utilities.Grid();
        }

        public Data(string key)
        {
            InitializeComponent();
            this.key = key;
            initializeCore();
            lManager = Logs.Manager.Instance;
            tManager = Tabs.Manager.Instance;
            gridUtils = new Utilities.Grid();
        }

        #endregion

        #region Events

        private void Core_MessageOccured(object sender, MessageArgs e)
        {
            Invoke(new MethodInvoker(delegate { ts_status.Text = e.Message; }));
            lManager.Enter(Logs.Sender.DATA, Logs.Level.NOTICE, e.Message);
        }

        private void Core_CurrentMaxDetermined(object sender, CurrentMaxArgs e)
        {
            Invoke(new MethodInvoker(delegate { ts_progress.Maximum = (int)e.Maximum; }));
        }

        private void Core_CurrentProgressChanged(object sender, CurrentChangedArgs e)
        {
            Invoke(new MethodInvoker(delegate { ts_progress.Value = (int)e.Value; }));
        }

        private void Core_CurrentProgressReset(object sender, CurrentResetArgs e)
        {
            Invoke(new MethodInvoker(delegate
            {
                ts_progress.Maximum = 100;
                ts_progress.Value = 0;
            }));
        }

        private async void ts_file_new_Click(object sender, EventArgs e)
        {
            string dumpDirectory = Grimoire.Utilities.Paths.FolderPath;
            string buildDirectory = Grimoire.Utilities.OPT.GetString("build.directory");

            tab_disabled = true;

            await Task.Run(() => { core.BuildDataFiles(dumpDirectory, buildDirectory); });

            // TODO: Core reset

            ts_status.Text = string.Empty;

            tab_disabled = false;
        }

        private async void ts_file_load_Click(object sender, EventArgs e)
        {
            string filePath = Grimoire.Utilities.Paths.FilePath;
            if (Grimoire.Utilities.Paths.FileResult != DialogResult.OK)
                return;

            tab_disabled = true;

            try
            {
                await Task.Run(() => { core.Load(filePath); });
            }
            catch (Exception ex)
            {
                lManager.Enter(Logs.Sender.DATA, Logs.Level.ERROR, ex, "Exception occured while attempting to load file at: {0}", filePath);
            }
            finally
            {
                ts_file_load.Enabled = false;
                ts_file_new.Enabled = false;

                grid.RowCount = core.RowCount + 1;
                grid.VirtualMode = true;
                grid.CellValueNeeded += gridUtils.Grid_CellValueNeeded;
                grid.CellValuePushed += gridUtils.Grid_CellPushed;

                await Task.Run(() => { populate_selection_info(tManager.DataCore.Index[0]); });

                lManager.Enter(Logs.Sender.DATA, Logs.Level.NOTICE,
                    "{0} entries loaded from data.000 to tab: {1} from path:\n\t- {2}",
                    core.RowCount,
                    tManager.Text,
                    filePath);

                extStatus.Text = "Analyzing index...";

                extensions.Nodes.Add("all", "all");
                extensions.Nodes["all"].Nodes.Add(string.Format("Count: {0}", tManager.DataCore.RowCount));

                await Task.Run(() => {
                    foreach (ExtensionInfo extInfo in Core.ExtensionList)
                    {
                        this.Invoke(new MethodInvoker(delegate
                        {
                            extensions.Nodes.Add(extInfo.Type, extInfo.Type);
                            extensions.Nodes[extInfo.Type].Nodes.Add(string.Format("Count: {0}", extInfo.Count));
                            extensions.Nodes[extInfo.Type].Nodes.Add("Size: ");
                        }));
                    }
                });

                tab_disabled = false;
                extStatus.ResetText();
            }
        }

        private void extensions_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            long extSize = tManager.DataCore.GetExtensionSize(e.Node.Text);
            string formattedSize = Grimoire.Utilities.StringExt.FormatToSize(extSize);
            if (e.Node.Text != "all")
                extensions.Nodes[e.Node.Text].Nodes[1].Text += formattedSize;
        }

        private void extensions_AfterSelect(object sender, TreeViewEventArgs e)
        {
            string ext = e.Node.Text;

            if (ext.Length > 3) // Will catch accidental info clicks (Count, Size)
                return;

            if (ext != "all")
            {
                FilteredIndex = tManager.DataCore.GetEntriesByExtension(ext, SortType.Name);
                grid.Rows.Clear();
                grid.RowCount = FilteredIndex.Count + 1;
            }
            else if (ext == "all" && FilteredIndex.Count > 0)
            {
                FilteredIndex.Clear();
                grid.Rows.Clear();
                grid.RowCount = tManager.DataCore.RowCount + 1;
            }
        }

        private void grid_SelectionChanged(object sender, EventArgs e)
        {
            int rowCount = grid.SelectedRows.Count;

            if (rowCount == 1)
            {
                if (grid.SelectedRows[0].Cells[0].Value != null)
                {
                    IndexEntry entry = tManager.DataCore.GetEntry(grid.SelectedRows[0].Cells[0].Value.ToString());
                    populate_selection_info(entry);
                    grid_cs.Items[1].Enabled = true;
                }
            }
            else
            {
                populate_selection_info();
                grid_cs.Items[1].Enabled = false;
            }

            set_grid_cs_export_text();
        }

        private async void grid_cs_export_Click(object sender, EventArgs e)
        {
            string buildDirectory = Grimoire.Utilities.OPT.GetString("build.directory");
            string buildPath;

            for (int i = 0; i < grid.SelectedRows.Count; i++)
            {
                buildPath = string.Format(@"{0}\{1}", buildDirectory, grid.SelectedRows[i].Cells[0].Value.ToString());
                IndexEntry entry = core.GetEntry(grid.SelectedRows[i].Cells[0].Value.ToString());

                ts_status.Text = string.Format("Exporting: {0}...", entry.Name);

                lManager.Enter(Logs.Sender.DATA, Logs.Level.NOTICE, "Exporting: {0} to path:\n\t- {1}\n\t- Size: {2}", entry.Name, buildPath, entry.Length);

                try
                {
                    await Task.Run(() => { core.ExportFileEntry(buildPath, entry); }); // TODO: Build path should be buildDirectory
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Export Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    lManager.Enter(Logs.Sender.DATA, Logs.Level.ERROR, ex);
                }

                ts_status.Text = string.Empty;
            }
        }

        private async void extensions_cs_export_Click(object sender, EventArgs e)
        {
            string buildDirectory = Grimoire.Utilities.OPT.GetString("build.directory");

            string ext = extensions.SelectedNode.Text;
            if (ext.Length == 3)
            {
                List<IndexEntry> entries = core.GetEntriesByExtension(ext);

                ts_status.Text = string.Format("Exporting: {0}...", ext);

                tab_disabled = true;

                try
                {
                    await Task.Run(() =>
                    {
                        if (ext == "all")
                        {
                            core.ExportAllEntries(buildDirectory);
                        }
                        else
                        {
                            buildDirectory += string.Format(@"\{0}\", ext);

                            if (!Directory.Exists(buildDirectory))
                                Directory.CreateDirectory(buildDirectory);

                            core.ExportExtEntries(buildDirectory, ext);
                        }
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Extension Export Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    lManager.Enter(Logs.Sender.DATA, Logs.Level.ERROR, ex);
                }
                finally
                {
                    lManager.Enter(Logs.Sender.DATA, Logs.Level.NOTICE, "Exported {0} Rows from Tab: {1}", entries.Count, tManager.Text);
                }

                ts_status.Text = string.Empty;

                tab_disabled = false;
            }
        }

        private void grid_cs_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            set_grid_cs_export_text();
        }

        private void grid_cs_compare_Click(object sender, EventArgs e)
        {
            string compareFile = Grimoire.Utilities.Paths.FilePath;
            string filename = grid.SelectedRows[0].Cells[0].Value.ToString();
            string externalHash = null;
            string internalHash = null;

            if (Grimoire.Utilities.Paths.FileResult != DialogResult.OK)
                return;
            try
            {
                externalHash = DataCore.Functions.Hash.GetSHA512Hash(compareFile);
                byte[] buffer = core.GetFileBytes(filename);
                internalHash = DataCore.Functions.Hash.GetSHA512Hash(buffer, buffer.Length);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Compare Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lManager.Enter(Logs.Sender.DATA, Logs.Level.ERROR, ex);
            }
            finally
            {
                string result = (externalHash == internalHash) ? "MATCH" : "MISMATCHED";
                string msg = string.Format("Compared files: {0}", result);
                MessageBox.Show(msg, "Comparison Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
                lManager.Enter(Logs.Sender.DATA, Logs.Level.NOTICE, "File Comparison:\n\tFilename: {0}\n\tResult: {1}", filename, result);
            }
        }

        private async void grid_cs_insert_Click(object sender, EventArgs e)
        {
            string filepath = Grimoire.Utilities.Paths.FilePath;
            if (Grimoire.Utilities.Paths.FileResult == DialogResult.OK)
            {
                ts_status.Text = string.Format("Importing: {0}...", Path.GetFileName(filepath));

                tab_disabled = true;

                try
                {
                    await Task.Run(() =>
                    {
                        core.ImportFileEntry(filepath);
                        core.Save(Grimoire.Utilities.OPT.GetString("build.directory"));
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Import Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    lManager.Enter(Logs.Sender.DATA, Logs.Level.ERROR, ex);
                }
                finally
                {
                    tab_disabled = false;
                    ts_status.Text = string.Empty;
                }
            }
        }

        private void searchInput_TextChanged(object sender, EventArgs e)
        {
            if (grid.Rows.Count > 0)
            {
                if (searchInput.Text.Length > 3)
                {
                    FilteredIndex = core.GetEntriesByPartialName(searchInput.Text);
                    grid.Rows.Clear();
                    grid.RowCount = FilteredIndex.Count + 1;
                }
                else
                {
                    FilteredIndex.Clear();
                    grid.Rows.Clear();
                    grid.RowCount = tManager.DataCore.RowCount + 1;
                }
            }
        }

        private void grid_DoubleClick(object sender, EventArgs e)
        {
            if (grid.SelectedRows.Count == 1)
                grid_cs_export_Click(null, EventArgs.Empty);
        }

        #endregion

        #region Methods (Public)

        public void Clear()
        {
            grid.Rows.Clear();
            extensions.Nodes.Clear();
            set_grid_cs_export_text();
            ts_file_load.Enabled = true;
            ts_file_new.Enabled = true;
        }

        #endregion

        #region Methods (private)

        private void initializeCore()
        {
            bool backup = Grimoire.Utilities.OPT.GetBool("data.backup");
            int codepage = Grimoire.Utilities.OPT.GetInt("data.encoding");
            Encoding encoding = Encoding.GetEncoding(codepage);
            core = new Core(backup, encoding);
            core.CurrentMaxDetermined += Core_CurrentMaxDetermined;
            core.CurrentProgressChanged += Core_CurrentProgressChanged;
            core.CurrentProgressReset += Core_CurrentProgressReset;
            core.MessageOccured += Core_MessageOccured;
        }

        private void populate_selection_info()
        {
            dataId.ResetText();
            offset.ResetText();
            size.ResetText();
            encrypted.ResetText();
            extension.ResetText();
        }

        private void populate_selection_info(IndexEntry entry)
        {
            string ext = Path.GetExtension(entry.Name).Remove(0, 1);
            Invoke(new MethodInvoker(delegate {
                dataId.Text = entry.DataID.ToString();
                offset.Text = entry.Offset.ToString();
                size.Text = Grimoire.Utilities.StringExt.FormatToSize(entry.Length);
                encrypted.Text = tManager.DataCore.ExtensionEncrypted(ext).ToString();
                extension.Text = ext;
            }));

        }

        private void set_grid_cs_export_text()
        {
            int rowCount = grid.SelectedRows.Count;

            if (grid_cs_enabled && rowCount > 1)
            {
                grid_cs.Items[1].Text = (rowCount == 1) ? "Export" : string.Format("Export ({0})", rowCount);
            }
        }

        #endregion
    }
}
