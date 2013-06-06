using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;

namespace UpdateToolGui
{
    using Extensions;

    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        CancellationTokenSource cts = new CancellationTokenSource();

        public void ThreadSafe(Action action)
        {
            if (this.Disposing || this.IsDisposed)
                return;
            this.Invoke(action);
        }

        public T ThreadSafe<T>(Func<T> action)
        {
            if (this.Disposing || this.IsDisposed)
                return default(T);
            return (T)this.Invoke(action);
        }

        protected void Progress(int value)
        {
            if (this.Disposing || this.IsDisposed)
                return;
            ThreadSafe(() => { prg_bar.Value = value; });
        }

        protected void Progress(int value, int max)
        {
            if (this.Disposing || this.IsDisposed)
                return;
            ThreadSafe(() => { prg_bar.Maximum = max; prg_bar.Value = value; });
        }

        protected void Progress(int value, int min, int max)
        {
            if (this.Disposing || this.IsDisposed)
                return;
            ThreadSafe(() => { prg_bar.Minimum = min; prg_bar.Maximum = max; prg_bar.Value = value; });
        }

        protected void Progress(string text = "")
        {
            if (this.Disposing || this.IsDisposed)
                return;
            ThreadSafe(() => { txt_status.Text = text; });
        }

        protected void Progress(string text, params object[] args)
        {
            if (this.Disposing || this.IsDisposed)
                return;
            ThreadSafe(() => { txt_status.Text = string.Format(text, args); });
        }
        
#if DEBUG
        protected void Log(string text, params object[] args)
        {
            Console.WriteLine("[{0}] {1}", DateTime.Now.ToLongTimeString(), string.Format(text, args));
        }
#endif

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            // Enable if checkbox is checked
            this.txt_updatefolder.Enabled
                = this.button3.Enabled
                = this.chk_checkdeleted.Enabled
                = this.chk_createchanged.Enabled
                = ((CheckBox)sender).Checked;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Restore directory
            if (!string.IsNullOrEmpty(txt_sourcefolder.Text) && Directory.Exists(txt_sourcefolder.Text))
                folderBrowserDialog1.SelectedPath = txt_sourcefolder.Text;

            // Prompt and apply to textbox if ok
            if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                txt_sourcefolder.Text = folderBrowserDialog1.SelectedPath;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Restore file
            if (!string.IsNullOrEmpty(txt_olddbpath.Text))
                openFileDialog1.FileName = txt_olddbpath.Text;

            // Prompt and apply to textbox if ok
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                txt_olddbpath.Text = openFileDialog1.FileName;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Restore directory
            if (!string.IsNullOrEmpty(txt_updatefolder.Text) && Directory.Exists(txt_updatefolder.Text))
                folderBrowserDialog3.SelectedPath = txt_updatefolder.Text;

            // Prompt and apply to textbox if ok
            if (folderBrowserDialog3.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                txt_updatefolder.Text = folderBrowserDialog3.SelectedPath;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                // Disable user input controls
                panel1.Enabled = false;
                panel1.Refresh();

                // Set progress to 0
                Progress(0, 1);

                // Validate input
#if DEBUG
                Log("Checking input...");
#endif
                if (string.IsNullOrEmpty(txt_sourcefolder.Text) || !Directory.Exists(txt_sourcefolder.Text))
                    throw new Exception("You need to enter a valid source folder path from where to analyze the files.");
                if (string.IsNullOrEmpty(txt_newdbpath.Text))
                    throw new Exception("You need to enter a valid target database path where to save the database.");
                if (string.Equals(txt_olddbpath.Text, txt_newdbpath.Text, StringComparison.OrdinalIgnoreCase))
                    throw new Exception("You need to choose different files for old and new database.");
                if (checkBox1.Checked)
                    if (string.IsNullOrEmpty(txt_updatefolder.Text))
                        throw new Exception("You need to enter a valid target folder path where to save the updated files.");

                // Start the job
#if DEBUG
                Log("Starting job...");
#endif
                backgroundWorker1.RunWorkerAsync();
            }
            catch (Exception err)
            {
                Debug.WriteLine(err);
#if DEBUG
                Log(string.Format("Error: {0}", err.Message));
#endif
                MessageBox.Show(err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                panel1.Enabled = true;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // Restore file
            if (!string.IsNullOrEmpty(txt_newdbpath.Text))
                saveFileDialog1.FileName = txt_newdbpath.Text;

            // Prompt and apply to textbox if ok
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                txt_newdbpath.Text = saveFileDialog1.FileName;
        }

        private string GetSize(double bytes, int decimals = 2)
        {
            int i = 0;
            while (bytes >= 1024)
            {
                bytes /= 1024;
                i++;
            }
            return string.Format("{0} {1}", Math.Round(bytes, decimals), new[] { "B", "KiB", "MiB", "GiB", "TiB" }[Math.Min(4, i)]);
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
#if DEBUG
            Log("Job now running asynchronous.");
#endif
            cts = new CancellationTokenSource();

            var db = new Database(); // the database
            var old_db = new Database(); // the database read from file
            var files = new List<FileInfo>(); // files that will be copied

            // user input
            string in_src = Path.GetFullPath(ThreadSafe(() => this.txt_sourcefolder.Text));
            string in_old_db = ThreadSafe(() => this.txt_olddbpath.Text);
            if (!string.IsNullOrEmpty(in_old_db))
                in_old_db = Path.GetFullPath(in_old_db);
            string in_db = Path.GetFullPath(ThreadSafe(() => this.txt_newdbpath.Text));
            bool in_copy = ThreadSafe(() => this.checkBox1.Checked);
            string in_copytarget = ThreadSafe(() => this.txt_updatefolder.Text);
            bool in_checkdeleted = ThreadSafe(() => this.chk_checkdeleted.Checked);
            bool in_createchanged = ThreadSafe(() => this.chk_createchanged.Checked);
            if (in_copy)
                in_copytarget = Path.GetFullPath(in_copytarget);
            bool in_compress = ThreadSafe(() => this.chk_compress.Checked);

            // read old database if existing
            if (!string.IsNullOrEmpty(in_old_db) && File.Exists(in_old_db))
            {
                Progress("Reading old database...");
#if DEBUG
                Log("Reading old database data...");
#endif

                using (var dbRStream = File.OpenText(in_old_db))
                {
                    Stream dbStream;

                    // Get header (if existent)
                    char[] buffer = new char[8];
                    if (dbRStream.BaseStream.Length >= 8)
                    {
                        dbRStream.Read(buffer, 0, 8);
                    }

                    if (new string(buffer) == "DEFLATEC") // deflate compression
                    {
#if DEBUG
                        Log("Database seems to be deflate-compressed (assumed by header), trying deflate decompression...");
#endif
                        dbRStream.BaseStream.Seek(dbRStream.CurrentEncoding.GetByteCount("DEFLATEC"), SeekOrigin.Begin);
                        dbStream = new System.IO.Compression.DeflateStream(dbRStream.BaseStream, System.IO.Compression.CompressionMode.Decompress);
                    }
                    else // no compression at all
                    {
#if DEBUG
                        Log("Database doesn't seem to be compressed.");
#endif
                        dbStream = dbRStream.BaseStream;
                        dbStream.Seek(0, SeekOrigin.Begin);
                    }

                    // Deserialize
                    old_db = Database.Deserialize(dbStream);
                }

#if DEBUG
                Log("Done. Database contains {0} entries.", db.Files.Count);
#endif
            }

            // Cache file list
            Progress("Preparing, this might take a few seconds...");
#if DEBUG
            Log("Searching files (this might take a bit)...");
#endif
            var in_src_di = new DirectoryInfo(in_src);
            var in_src_files = in_src_di.EnumerateFiles("*", SearchOption.AllDirectories).OrderByDescending(f => f.Length);
            var in_src_files_count = in_src_files.Count();
            var in_src_files_size = in_src_files.Sum(f => f.Length);
#if DEBUG
            Log("Found {0} files with a size of {1}.", in_src_files_count, GetSize(in_src_files_size));
#endif

            // File analysis
            // Generate hashes and save other file properties here, etc.
#if DEBUG
            Log("Now starting file analysis...");
#endif
            Stopwatch sw = new Stopwatch(); // for ETA calculation
            sw.Start();
            long remaining_size = in_src_files_size;
            object remaining_size_lock = new object();
            var remaining_files = in_src_files_count;
            object remaining_files_lock = new object();
            long sprogmax = in_src_files_size;
            int power = 0;
            while (sprogmax > int.MaxValue)
            {
                sprogmax /= 10;
                power++;
            }
            Progress(0, (int)sprogmax);

            var taskschedulers = new[] {
                new LimitedConcurrencyLevelTaskScheduler(1), // Level 0 for largest files
                new LimitedConcurrencyLevelTaskScheduler(1), // Level 1
                new LimitedConcurrencyLevelTaskScheduler(2), // Level 2
                new LimitedConcurrencyLevelTaskScheduler(4), // Level 3
                new LimitedConcurrencyLevelTaskScheduler(16), // Level 4
                new LimitedConcurrencyLevelTaskScheduler(64) // Level 5 for smallest files
            };
            var taskfactories = new List<TaskFactory>();
            foreach(var ts in taskschedulers)
                taskfactories.Add(
                    new TaskFactory(
                        cts.Token,
                        TaskCreationOptions.None,
                        TaskContinuationOptions.None,
                        ts
                    )
                );
            var tasks = new List<Task>();

            // queue tasks for all files
            foreach (FileInfo fileinfo in in_src_files)
            {
                int tasklevel =
                    0 // Level 0 for largest files (>= 512 MB)
                    + (fileinfo.Length < 512 * 1024 * 1024 ? 1 : 0) // Level 1 (< 512 MB)
                    + (fileinfo.Length < 128 * 1024 * 1024 ? 1 : 0) // Level 2 (< 128 MB)
                    + (fileinfo.Length < 10 * 1024 * 1024 ? 1 : 0) // Level 3 (< 10 MB)
                    + (fileinfo.Length < 1 * 1024 * 1024 ? 1 : 0) // Level 4 (< 1 MB)
                    + (fileinfo.Length < 100 * 1024 ? 1 : 0) // Level 5 (< 100 kB)
                    ;

                TaskFactory taskfactory = taskfactories[tasklevel];

                var task = taskfactory.StartNew(new Action<object>((fio) =>
                {
                    var fi = (FileInfo)fio;
                    DatabaseFileInfo db_fi = new DatabaseFileInfo();
                    db_fi.Name = fi.Name;
                    db_fi.Folder = fi.Directory.GetRelativePathFrom(in_src);
                    var relative_filepath = fi.GetRelativePathFrom(in_src);
#if DEBUG
                    Log("Now running: {0} ({1})", relative_filepath, GetSize(fi.Length));
#endif

                    // Check for old database entries
                    DatabaseFileInfo db_old_fi = null;
                    if (old_db.Files.Any(
                        d => d.Name == db_fi.Name && d.Folder == db_fi.Folder
                            ))
                    {
                        db_old_fi = old_db.Files.Single(d => d.Name == db_fi.Name && d.Folder == db_fi.Folder);
                    }

                    // File properties
                    db_fi.ModificationTime = fi.LastWriteTimeUtc;
                    db_fi.Size = fi.Length;

                    // Hash generation
                    var hashgen = MD5CryptoServiceProvider.Create();
                    hashgen.Initialize();
                    using (var fs = new FileStream(fi.FullName, FileMode.Open, FileAccess.Read, FileShare.Read, 4096 * 128))
                    {
                        byte[] buffer = new byte[4096];
                        while (fs.Position < fs.Length)
                        {
                            cts.Token.ThrowIfCancellationRequested();
                            int recv = fs.Read(buffer, 0, buffer.Length);
                            hashgen.TransformBlock(buffer, 0, recv, null, 0);
                            //lock (remaining_size_lock)
                            remaining_size -= recv;
                        }
                    }
                    hashgen.TransformFinalBlock(new byte[0], 0, 0);
                    db_fi.Hash = hashgen.Hash;

                    // Check if updated
                    if (in_copy)
                    {
                        bool isUpdated = db_old_fi == null;
                        if (!isUpdated && in_copy)
                            if (db_fi.Size != db_old_fi.Size || db_fi.ModificationTime != db_old_fi.ModificationTime)
                                isUpdated = true;
                        if (isUpdated)
                        {
#if DEBUG
                            Log("Adding to copy queue: {0}", relative_filepath);
#endif
                            lock (files)
                            {
                                files.Add(fi);
                            }
                        }
                    }

                    // Add to database
                    lock (db)
                    {
                        db.Files.Add(db_fi);
                    }

                    // Subtract from remaining files count
                    lock (remaining_files_lock)
                    {
                        remaining_files--;
                    }

#if DEBUG
                    Log("Now finished: {0}", relative_filepath);
#endif
                }), fileinfo);
                task.ContinueWith((tasky) =>
                {
                    if (tasky.Exception != null)
                    {
                        cts.Cancel();

                        MessageBox.Show(tasky.Exception.ToString());
                        Debug.WriteLine(tasky);

                        throw tasky.Exception;
                    }
                }, TaskContinuationOptions.OnlyOnFaulted);
                tasks.Add(task);
            }

            // status display
            long remaining_old = 0;
            for (int level = taskschedulers.Count() - 1; level >= 0 && !cts.IsCancellationRequested; level--)
            {
                var taskscheduler = taskschedulers[level];
#if DEBUG
                Log("Entering task level {0} with {1} parallel threads.", level, taskscheduler.MaximumConcurrencyLevel);
#endif

                while (tasks.Any(task => !task.IsCanceled && !task.IsCompleted && !task.IsFaulted))
                {
                    long remaining;
                    lock (remaining_size_lock)
                    {
                        remaining = remaining_size;
                    }
                    long remaining_files_current;
                    lock (remaining_files_lock)
                    {
                        remaining_files_current = remaining_files;
                    }
                    var byterate = (in_src_files_size - remaining) / sw.Elapsed.TotalSeconds;
                    var realbyterate = 4 * (remaining_old - remaining);
                    if (sw.Elapsed.TotalSeconds > 1)
                        Progress("[{2} left] Analysis running, {0} in {3} files left ({1}/s)...", GetSize(remaining), GetSize(realbyterate), TimeSpan.FromSeconds(remaining / byterate).ToPrettyFormat(), remaining_files_current);
                    else
                        Progress("Analysis running...");
                    Progress((int)((in_src_files_size - remaining) / Math.Pow(10, power)));
                    remaining_old = remaining;
                    Thread.Sleep(250);
                }
                
#if DEBUG
                Log("Leaving task level {0}.", level);
#endif
            }
            
            // cancelled?
            if (cts.IsCancellationRequested)
            {
#if DEBUG
                Log("Job cancelled (managed).");
#endif
                cts.Token.ThrowIfCancellationRequested();
            }
#if DEBUG
            else
                Log("Analysis finished, took {0}.", sw.Elapsed.ToString());
#endif

            // copy files to update folder if needed
            if (in_copy)
            {
#if DEBUG
                Log("Update folder creation enabled, now processing copy queue...");
                Log("Copy queue contains {0} files.", files.Count);
#endif
                Progress("Copying updates...");
                Progress(0, files.Count);

                var i = 0;

                foreach (FileInfo fi in files)
                {
                    var file = fi.GetRelativePathFrom(in_src);
                    var targetfile = new FileInfo(Path.Combine(in_copytarget, file));
                    var sourcefile = fi;

                    // Check if directory exists, if not then create
                    if (!targetfile.Directory.Exists)
                    {
#if DEBUG
                        Log("Creating directory: {0}", targetfile.GetRelativePathFrom(in_copytarget));
#endif
                        targetfile.Directory.Create();
                    }

                    // Finally copy the file
#if DEBUG
                    Log("Copying file: {0}", file);
#endif
                    Progress(++i);
                    sourcefile.CopyTo(targetfile.FullName, true);
                }


                if (in_createchanged && files.Any())
                {
#if DEBUG
                    Log("Changed files detected and changed files list creation enabled, writing changed_files.txt...");
#endif
                    using (var fs = File.Open(Path.Combine(new DirectoryInfo(in_copytarget).FullName, "changed_files.txt"), FileMode.Append))
                    {
                        fs.Seek(0, SeekOrigin.End);

                        using (var tw = new StreamWriter(fs))
                        {
                            tw.WriteLine();
                            tw.WriteLine("##################################################################################");
                            tw.WriteLine("# These are all files which have been detected as changed or new during file");
                            tw.WriteLine("# comparison between these databases:");
                            tw.WriteLine("#");
                            tw.WriteLine("#     Old database: {0}", in_old_db);
                            tw.WriteLine("#     New database: {0}", in_db);
                            tw.WriteLine("#");
                            tw.WriteLine("# Date: {0}", DateTime.Now.ToString());
                            tw.WriteLine("##################################################################################");
                            tw.WriteLine();

                            files.Sort((fi1, fi2) => { return fi1.FullName.CompareTo(fi2.FullName); });
                            foreach (var fi in files)
                                tw.WriteLine(fi.GetRelativePathFrom(in_src));
                        }
                    }
                }

                // check for deleted files if needed
                if (in_checkdeleted)
                {
#if DEBUG
                    Log("Deleted files check enabled, checking for deleted files...");
#endif

                    i = 0;
                    Progress(0, old_db.Files.Count);

                    var deletedFiles = new List<string>();

                    foreach (var dfi in old_db.Files)
                    {
                        var file = Path.Combine(dfi.Folder, dfi.Name);
                        bool exists = File.Exists(Path.Combine(new DirectoryInfo(in_src).FullName + Path.DirectorySeparatorChar, file));

                        if (!exists)
                        {
#if DEBUG
                            Log("Adding to deleted files list: {0}", file);
#endif
                            deletedFiles.Add(file);
                        }
                    }

                    if (deletedFiles.Any() && in_checkdeleted)
                    {
#if DEBUG
                        Log("Deleted files detected and deleted files list creation enabled, writing deleted_files.txt...");
#endif

                        using (var fs = File.Open(Path.Combine(new DirectoryInfo(in_copytarget).FullName, "deleted_files.txt"), FileMode.Append))
                        {
                            fs.Seek(0, SeekOrigin.End);

                            using (var tw = new StreamWriter(fs))
                            {
                                tw.WriteLine();
                                tw.WriteLine("##################################################################################");
                                tw.WriteLine("# These are all files which have been detected as deleted during file comparison");
                                tw.WriteLine("# between these databases:");
                                tw.WriteLine("#");
                                tw.WriteLine("#     Old database: {0}", in_old_db);
                                tw.WriteLine("#     New database: {0}", in_db);
                                tw.WriteLine("#");
                                tw.WriteLine("# Date: {0}", DateTime.Now.ToString());
                                tw.WriteLine("##################################################################################");
                                tw.WriteLine();

                                deletedFiles.Sort();
                                foreach (var file in deletedFiles)
                                    tw.WriteLine(file);
                            }
                        }
                    }
                }

            }

            Progress(1, 1);
#if DEBUG
            Log("Saving database...");
#endif
            File.Delete(in_db);

            using (var fs = File.CreateText(in_db))
            {
                if (in_compress)
                {
                    fs.Write("DEFLATEC"); // header to identify deflate compression
                    fs.Flush();
                    using (var fsc = new System.IO.Compression.DeflateStream(fs.BaseStream, System.IO.Compression.CompressionMode.Compress, true))
                        db.Serialize(fsc);
                }
                else
                    db.Serialize(fs);
            }

            Progress(0);
#if DEBUG
            Log("Job finishing...");
#endif
            return; // lol.
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            panel1.Enabled = true;
            cts.Cancel();

            if (e.Error != null)
            {
                if (e.Error is OperationCanceledException)
                {
                    Progress("Cancelled.");
#if DEBUG
                    Log("Job cancelled.");
#endif
                }
                else
                {
#if DEBUG
                    Log("Error occurred in background job: {0}.", e.Error.ToString());
#endif
                    Debug.WriteLine(e.Error.ToString());
                    Debug.WriteLine(e.Error.InnerException);
#if DEBUG
                    Log("Job failed.");
#endif
                    Progress("Error, see log.");
                }
                Progress(0, 1);
            }
            else
            {
#if DEBUG
                Log("Job finished.");
#endif
                Progress("Finished.");
                Progress(0, 1);
            }
        }

        private void panel1_EnabledChanged(object sender, EventArgs e)
        {
            btn_cancel.Visible = btn_cancel.Enabled = !(
                btn_run.Visible = btn_run.Enabled
                = ((Control)sender).Enabled
            );
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            cts.Cancel();
            btn_cancel.Enabled = false;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            btn_cancel.PerformClick();
        }

    }
}
