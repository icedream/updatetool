using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using UpdateToolGui.Extensions;
using UpdateToolGui.Properties;

namespace UpdateToolGui
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private CancellationTokenSource _cts = new CancellationTokenSource();

        public void ThreadSafe(Action action)
        {
            if (Disposing || IsDisposed)
                return;
            Invoke(action);
        }

        public T ThreadSafe<T>(Func<T> action)
        {
            if (Disposing || IsDisposed)
                return default(T);
            return (T) Invoke(action);
        }

        protected void Progress(int value)
        {
            if (Disposing || IsDisposed)
                return;
            ThreadSafe(() => { prg_bar.Value = value; });
        }

        protected void Progress(int value, int max)
        {
            if (Disposing || IsDisposed)
                return;
            ThreadSafe(() =>
            {
                prg_bar.Maximum = max;
                prg_bar.Value = value;
            });
        }

        protected void Progress(int value, int min, int max)
        {
            if (Disposing || IsDisposed)
                return;
            ThreadSafe(() =>
            {
                prg_bar.Minimum = min;
                prg_bar.Maximum = max;
                prg_bar.Value = value;
            });
        }

        protected void Progress(string text = "")
        {
            if (Disposing || IsDisposed)
                return;
            ThreadSafe(() => { txt_status.Text = text; });
        }

        protected void Progress(string text, params object[] args)
        {
            if (Disposing || IsDisposed)
                return;
            ThreadSafe(() => { txt_status.Text = string.Format(text, args); });
        }

#if DEBUG
        protected void Log(string text, params object[] args)
        {
            Console.WriteLine(@"[{0}] {1}", DateTime.Now.ToLongTimeString(), string.Format(text, args));
        }
#endif

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            // Enable if checkbox is checked
            txt_updatefolder.Enabled
                = button3.Enabled
                    = chk_checkdeleted.Enabled
                        = chk_createchanged.Enabled
                            = ((CheckBox) sender).Checked;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Restore directory
            if (!string.IsNullOrEmpty(txt_sourcefolder.Text) && Directory.Exists(txt_sourcefolder.Text))
                folderBrowserDialog1.SelectedPath = txt_sourcefolder.Text;

            // Prompt and apply to textbox if ok
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                txt_sourcefolder.Text = folderBrowserDialog1.SelectedPath;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Restore file
            if (!string.IsNullOrEmpty(txt_olddbpath.Text))
                openFileDialog1.FileName = txt_olddbpath.Text;

            // Prompt and apply to textbox if ok
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                txt_olddbpath.Text = openFileDialog1.FileName;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Restore directory
            if (!string.IsNullOrEmpty(txt_updatefolder.Text) && Directory.Exists(txt_updatefolder.Text))
                folderBrowserDialog3.SelectedPath = txt_updatefolder.Text;

            // Prompt and apply to textbox if ok
            if (folderBrowserDialog3.ShowDialog() == DialogResult.OK)
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
                        throw new Exception(
                            "You need to enter a valid target folder path where to save the updated files.");

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
                MessageBox.Show(err.Message, Resources.MainForm_button4_Click_Error, MessageBoxButtons.OK,
                    MessageBoxIcon.Stop);
                panel1.Enabled = true;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // Restore file
            if (!string.IsNullOrEmpty(txt_newdbpath.Text))
                saveFileDialog1.FileName = txt_newdbpath.Text;

            // Prompt and apply to textbox if ok
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                txt_newdbpath.Text = saveFileDialog1.FileName;
        }

        private static string _getSize(double bytes, int decimals = 2)
        {
            var i = 0;
            while (bytes >= 1024)
            {
                bytes /= 1024;
                i++;
            }
            return string.Format("{0} {1}", Math.Round(bytes, decimals),
                new[] {"B", "KiB", "MiB", "GiB", "TiB"}[Math.Min(4, i)]);
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
#if DEBUG
            Log("Job now running asynchronous.");
#endif
            _cts = new CancellationTokenSource();

            var db = new Database(); // the database
            var oldDb = new Database(); // the database read from file
            var files = new List<FileInfo>(); // files that will be copied

            // user input
            var inSrc = Path.GetFullPath(ThreadSafe(() => txt_sourcefolder.Text));
            var inOldDb = ThreadSafe(() => txt_olddbpath.Text);
            if (!string.IsNullOrEmpty(inOldDb))
                inOldDb = Path.GetFullPath(inOldDb);
            var inDb = Path.GetFullPath(ThreadSafe(() => txt_newdbpath.Text));
            var inCopy = ThreadSafe(() => checkBox1.Checked);
            var inCopytarget = ThreadSafe(() => txt_updatefolder.Text);
            var inCheckdeleted = ThreadSafe(() => chk_checkdeleted.Checked);
            var inCreatechanged = ThreadSafe(() => chk_createchanged.Checked);
            if (inCopy)
                inCopytarget = Path.GetFullPath(inCopytarget);
            var inCompress = ThreadSafe(() => chk_compress.Checked);

            // read old database if existing
            if (!string.IsNullOrEmpty(inOldDb) && File.Exists(inOldDb))
            {
                Progress("Reading old database...");
#if DEBUG
                Log("Reading old database data...");
#endif

                using (var dbRStream = File.OpenText(inOldDb))
                {
                    Stream dbStream;

                    // Get header (if existent)
                    var buffer = new char[8];
                    if (dbRStream.BaseStream.Length >= 8)
                    {
                        dbRStream.Read(buffer, 0, 8);
                    }

                    if (new string(buffer) == "DEFLATEC") // deflate compression
                    {
#if DEBUG
                        Log(
                            "Database seems to be deflate-compressed (assumed by header), trying deflate decompression...");
#endif
                        dbRStream.BaseStream.Seek(dbRStream.CurrentEncoding.GetByteCount("DEFLATEC"), SeekOrigin.Begin);
                        dbStream = new DeflateStream(dbRStream.BaseStream, CompressionMode.Decompress);
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
                    oldDb = Database.Deserialize(dbStream);
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
            var inSrcDi = new DirectoryInfo(inSrc);
            var inSrcFiles =
                inSrcDi.EnumerateFiles("*", SearchOption.AllDirectories).OrderByDescending(f => f.Length);
            var inSrcFilesCount = inSrcFiles.Count();
            var inSrcFilesSize = inSrcFiles.Sum(f => f.Length);
#if DEBUG
            Log("Found {0} files with a size of {1}.", inSrcFilesCount, _getSize(inSrcFilesSize));
#endif

            // File analysis
            // Generate hashes and save other file properties here, etc.
#if DEBUG
            Log("Now starting file analysis...");
#endif
            var sw = new Stopwatch(); // for ETA calculation
            sw.Start();
            var remainingSize = inSrcFilesSize;
            var remainingSizeLock = new object();
            var remainingFiles = inSrcFilesCount;
            var remainingFilesLock = new object();
            var sprogmax = inSrcFilesSize;
            var power = 0;
            while (sprogmax > int.MaxValue)
            {
                sprogmax /= 10;
                power++;
            }
            Progress(0, (int) sprogmax);

            var taskschedulers = new[]
            {
                new LimitedConcurrencyLevelTaskScheduler(1), // Level 0 for largest files
                new LimitedConcurrencyLevelTaskScheduler(1), // Level 1
                new LimitedConcurrencyLevelTaskScheduler(2), // Level 2
                new LimitedConcurrencyLevelTaskScheduler(4), // Level 3
                new LimitedConcurrencyLevelTaskScheduler(16), // Level 4
                new LimitedConcurrencyLevelTaskScheduler(64) // Level 5 for smallest files
            };
            var taskfactories =
                taskschedulers.Select(
                    ts => new TaskFactory(_cts.Token, TaskCreationOptions.None, TaskContinuationOptions.None, ts))
                    .ToList();
            var tasks = new List<Task>();

            // queue tasks for all files
            foreach (var task in from fileinfo in inSrcFiles
                let tasklevel = 0 // Level 0 for largest files (>= 512 MB)
                                + (fileinfo.Length < 512*1024*1024 ? 1 : 0) // Level 1 (< 512 MB)
                                + (fileinfo.Length < 128*1024*1024 ? 1 : 0) // Level 2 (< 128 MB)
                                + (fileinfo.Length < 10*1024*1024 ? 1 : 0) // Level 3 (< 10 MB)
                                + (fileinfo.Length < 1*1024*1024 ? 1 : 0) // Level 4 (< 1 MB)
                                + (fileinfo.Length < 100*1024 ? 1 : 0)
                let taskfactory = taskfactories[tasklevel]
                select taskfactory.StartNew((fio =>
                {
                    var fi = (FileInfo) fio;
                    var dbFi = new DatabaseFileInfo {Name = fi.Name, Folder = fi.Directory.GetRelativePathFrom(inSrc)};
                    var relativeFilepath = fi.GetRelativePathFrom(inSrc);
#if DEBUG
                    Log("Now running: {0} ({1})", relativeFilepath, _getSize(fi.Length));
#endif

                    // Check for old database entries
                    DatabaseFileInfo dbOldFi = null;
                    if (oldDb.Files.Any(
                        d => d.Name == dbFi.Name && d.Folder == dbFi.Folder
                        ))
                    {
                        dbOldFi = oldDb.Files.Single(d => d.Name == dbFi.Name && d.Folder == dbFi.Folder);
                    }

                    // File properties
                    dbFi.ModificationTime = fi.LastWriteTimeUtc;
                    dbFi.Size = fi.Length;

                    // Hash generation
                    MD5 hashgen = MD5.Create();
                    hashgen.Initialize();
                    using (
                        var fs = new FileStream(fi.FullName, FileMode.Open, FileAccess.Read, FileShare.Read, 4096*128))
                    {
                        var buffer = new byte[4096];
                        while (fs.Position < fs.Length)
                        {
                            _cts.Token.ThrowIfCancellationRequested();
                            int recv = fs.Read(buffer, 0, buffer.Length);
                            hashgen.TransformBlock(buffer, 0, recv, null, 0);
                            //lock (remaining_size_lock)
                            remainingSize -= recv;
                        }
                    }
                    hashgen.TransformFinalBlock(new byte[0], 0, 0);
                    dbFi.Hash = hashgen.Hash;

                    // Check if updated
                    if (inCopy)
                    {
                        var isUpdated = dbOldFi == null;
                        if (!isUpdated)
                        {
                            if (dbFi.Size != dbOldFi.Size || dbFi.ModificationTime != dbOldFi.ModificationTime)
                                isUpdated = true; // TODO: Why is this not used???
                        }
                        else
                        {
#if DEBUG
                            Log("Adding to copy queue: {0}", relativeFilepath);
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
                        db.Files.Add(dbFi);
                    }

                    // Subtract from remaining files count
                    lock (remainingFilesLock)
                    {
                        remainingFiles--;
                    }

#if DEBUG
                    Log("Now finished: {0}", relativeFilepath);
#endif
                }), fileinfo))
            {
                task.ContinueWith(tasky =>
                {
                    if (tasky.Exception == null) return;
                    _cts.Cancel();

                    MessageBox.Show(tasky.Exception.ToString());
                    Debug.WriteLine(tasky);

                    throw tasky.Exception;
                }, TaskContinuationOptions.OnlyOnFaulted);
                tasks.Add(task);
            }

            // status display
            long remainingOld = 0;
            for (int level = taskschedulers.Count() - 1; level >= 0 && !_cts.IsCancellationRequested; level--)
            {
                LimitedConcurrencyLevelTaskScheduler taskscheduler = taskschedulers[level];
#if DEBUG
                Log("Entering task level {0} with {1} parallel threads.", level, taskscheduler.MaximumConcurrencyLevel);
#endif

                while (tasks.Any(task => !task.IsCanceled && !task.IsCompleted && !task.IsFaulted))
                {
                    long remaining;
                    lock (remainingSizeLock)
                    {
                        remaining = remainingSize;
                    }
                    long remainingFilesCurrent;
                    lock (remainingFilesLock)
                    {
                        remainingFilesCurrent = remainingFiles;
                    }
                    var byterate = (inSrcFilesSize - remaining)/sw.Elapsed.TotalSeconds;
                    var realbyterate = 4*(remainingOld - remaining);
                    if (sw.Elapsed.TotalSeconds > 1 && byterate > 0)
                        Progress("[{2} left] Analysis running, {0} in {3} files left ({1}/s)...", _getSize(remaining),
                            _getSize(realbyterate), TimeSpan.FromSeconds(remaining/byterate).ToPrettyFormat(),
                            remainingFilesCurrent);
                    else
                        Progress("Analysis running...");
                    Progress((int) ((inSrcFilesSize - remaining)/Math.Pow(10, power)));
                    remainingOld = remaining;
                    Thread.Sleep(250);
                }

#if DEBUG
                Log("Leaving task level {0}.", level);
#endif
            }

            // cancelled?
            if (_cts.IsCancellationRequested)
            {
#if DEBUG
                Log("Job cancelled (managed).");
#endif
                _cts.Token.ThrowIfCancellationRequested();
            }
#if DEBUG
            else
                Log("Analysis finished, took {0}.", sw.Elapsed.ToString());
#endif

            // copy files to update folder if needed
            if (inCopy)
            {
#if DEBUG
                Log("Update folder creation enabled, now processing copy queue...");
                Log("Copy queue contains {0} files.", files.Count);
#endif
                Progress("Copying updates...");
                Progress(0, files.Count);

                var i = 0;

                foreach (var fi in files)
                {
                    var file = fi.GetRelativePathFrom(inSrc);
                    var targetfile = new FileInfo(Path.Combine(inCopytarget, file));
                    var sourcefile = fi;

                    // Check if directory exists, if not then create
                    Debug.Assert(targetfile.Directory != null, "targetfile.Directory != null");
                    if (!targetfile.Directory.Exists)
                    {
#if DEBUG
                        Log("Creating directory: {0}", targetfile.GetRelativePathFrom(inCopytarget));
#endif
                        Debug.Assert(targetfile.Directory != null, "targetfile.Directory != null");
                        targetfile.Directory.Create();
                    }

                    // Finally copy the file
#if DEBUG
                    Log("Copying file: {0}", file);
#endif
                    Progress(++i);
                    sourcefile.CopyTo(targetfile.FullName, true);
                }


                if (inCreatechanged && files.Any())
                {
#if DEBUG
                    Log("Changed files detected and changed files list creation enabled, writing changed_files.txt...");
#endif
                    using (
                        var fs =
                            File.Open(Path.Combine(new DirectoryInfo(inCopytarget).FullName, "changed_files.txt"),
                                FileMode.Append))
                    {
                        fs.Seek(0, SeekOrigin.End);

                        using (var tw = new StreamWriter(fs))
                        {
                            tw.WriteLine();
                            tw.WriteLine(
                                "##################################################################################");
                            tw.WriteLine("# These are all files which have been detected as changed or new during file");
                            tw.WriteLine("# comparison between these databases:");
                            tw.WriteLine("#");
                            tw.WriteLine("#     Old database: {0}", inOldDb);
                            tw.WriteLine("#     New database: {0}", inDb);
                            tw.WriteLine("#");
                            tw.WriteLine("# Date: {0}", DateTime.Now);
                            tw.WriteLine(
                                "##################################################################################");
                            tw.WriteLine();

                            files.Sort((fi1, fi2) => string.Compare(fi1.FullName, fi2.FullName, StringComparison.Ordinal));
                            foreach (var fi in files)
                                tw.WriteLine(fi.GetRelativePathFrom(inSrc));
                        }
                    }
                }

                // check for deleted files if needed
                if (inCheckdeleted)
                {
#if DEBUG
                    Log("Deleted files check enabled, checking for deleted files...");
#endif

                    Progress(0, oldDb.Files.Count);

                    var deletedFiles = new List<string>();

                    foreach (var file in oldDb.Files
                        .Select(dfi => Path.Combine(dfi.Folder, dfi.Name))
                        .Where(file => !File.Exists(Path.Combine(new DirectoryInfo(inSrc).FullName + Path.DirectorySeparatorChar, file))))
                    {
#if DEBUG
                        Log("Adding to deleted files list: {0}", file);
#endif
                        deletedFiles.Add(file);
                    }

                    if (deletedFiles.Any())
                    {
#if DEBUG
                        Log(
                            "Deleted files detected and deleted files list creation enabled, writing deleted_files.txt...");
#endif

                        using (
                            FileStream fs =
                                File.Open(Path.Combine(new DirectoryInfo(inCopytarget).FullName, "deleted_files.txt"),
                                    FileMode.Append))
                        {
                            fs.Seek(0, SeekOrigin.End);

                            using (var tw = new StreamWriter(fs))
                            {
                                tw.WriteLine();
                                tw.WriteLine(
                                    "##################################################################################");
                                tw.WriteLine(
                                    "# These are all files which have been detected as deleted during file comparison");
                                tw.WriteLine("# between these databases:");
                                tw.WriteLine("#");
                                tw.WriteLine("#     Old database: {0}", inOldDb);
                                tw.WriteLine("#     New database: {0}", inDb);
                                tw.WriteLine("#");
                                tw.WriteLine("# Date: {0}", DateTime.Now);
                                tw.WriteLine(
                                    "##################################################################################");
                                tw.WriteLine();

                                deletedFiles.Sort();
                                foreach (string file in deletedFiles)
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
            File.Delete(inDb);

            using (StreamWriter fs = File.CreateText(inDb))
            {
                if (inCompress)
                {
                    fs.Write("DEFLATEC"); // header to identify deflate compression
                    fs.Flush();
                    using (var fsc = new DeflateStream(fs.BaseStream, CompressionMode.Compress, true))
                        db.Serialize(fsc);
                }
                else
                    db.Serialize(fs);
            }

            Progress(0);
#if DEBUG
            Log("Job finishing...");
#endif
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            panel1.Enabled = true;
            _cts.Cancel();

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
                    = ((Control) sender).Enabled
                );
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            _cts.Cancel();
            btn_cancel.Enabled = false;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            btn_cancel.PerformClick();
        }
    }
}