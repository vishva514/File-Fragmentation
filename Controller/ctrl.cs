using System;
using System.IO;
using System.Text;
using FileFragmentationApp.Model;
using FileFragmentationApp.View;

namespace FileFragmentationApp.Controller
{
    public class FragmentController
    {
        private FragmentationModel model;
        private ConsoleView view;

        public FragmentController(FragmentationModel m, ConsoleView v)
        {
            model = m;
            view = v;
        }

        public void Run()
        {
            bool repeat = true;
            while (repeat)
            {
                try
                {
                    CreateInputFile();
                    Fragment();
                    VerifyFile();
                    Defragment();
                    CompareFiles();
                }
                catch (Exception ex)
                {
                    view.ShowError("Unhandled error: " + ex.Message);
                }

                var ans = view.Prompt("\nDelete all created files and run again? (Y/N): ").Trim().ToUpper();
                if (ans == "Y" || ans == "YES")
                {
                    Cleanup();
                    Console.Clear();
                }
                else
                {
                    view.Show("Exiting. Files are in '" + model.FragmentFolder + "' and output: '" + model.OutputFilePath + "'.");
                    repeat = false;
                }
            }
        }

        private void CreateInputFile()
        {
            view.Show("=== Step 1: Create input.txt ===");
            view.Show("Enter the paragraph you want to store. Enter '.' alone on a line to finish:");

            var sb = new StringBuilder();
            while (true)
            {
                string line = Console.ReadLine();
                if (line == null) break;
                if (line.Trim() == ".") break;
                sb.AppendLine(line);
            }

            model.InputText = sb.ToString();
            File.WriteAllText(model.InputFilePath, model.InputText);
            view.Show($"Created '{model.InputFilePath}' ({model.InputText.Length} chars).\n");
        }

        private void Fragment()
        {
            view.Show("=== Step 2: Fragmentation ===");
            string input = view.Prompt("Enter fragment size: ");
            if (!int.TryParse(input, out int size) || size <= 0)
                throw new ArgumentException("Fragment size must be positive.");

            model.FragmentSize = size;

            if (Directory.Exists(model.FragmentFolder))
                Directory.Delete(model.FragmentFolder, true);
            Directory.CreateDirectory(model.FragmentFolder);

            string content = model.InputText ?? string.Empty;
            model.FragmentFiles.Clear();

            if (content.Length == 0)
            {
                view.Show("Input text empty.");
                return;
            }

            model.TotalFragments = (content.Length + size - 1) / size;
            int padLength = Math.Max(1, model.TotalFragments.ToString().Length);

            for (int i = 0; i < model.TotalFragments; i++)
            {
                int start = i * size;
                int len = Math.Min(size, content.Length - start);
                string frag = content.Substring(start, len);
                string fileName = Path.Combine(model.FragmentFolder, (i + 1).ToString("D" + padLength) + ".txt");
                File.WriteAllText(fileName, frag);
                model.FragmentFiles.Add(fileName);
            }

            view.Show($"Created {model.FragmentFiles.Count} fragment(s):");
            view.ShowFiles(model.FragmentFiles);
        }

        private void VerifyFile()
        {
            view.Show("=== Step 3: Verify File ===");
            if (model.TotalFragments == 0) { view.Show("No fragments."); return; }

            int padLength = Math.Max(1, model.TotalFragments.ToString().Length);
            string input = view.Prompt("Enter file name or number: ").Trim();
            if (string.IsNullOrEmpty(input)) return;

            string filePath = input.EndsWith(".txt")
                ? Path.Combine(model.FragmentFolder, input)
                : Path.Combine(model.FragmentFolder,
                    int.TryParse(input, out int idx)
                        ? idx.ToString("D" + padLength) + ".txt"
                        : input + ".txt");

            if (File.Exists(filePath))
                view.Show($"File '{filePath}' exists:\n{File.ReadAllText(filePath)}");
            else
                view.ShowError("File not found.");
        }

        private void Defragment()
        {
            view.Show("=== Step 4: Defragmentation ===");
            if (!Directory.Exists(model.FragmentFolder)) { view.ShowError("No fragments folder."); return; }

            var files = Directory.GetFiles(model.FragmentFolder, "*.txt");
            Array.Sort(files);

            var sb = new StringBuilder();
            foreach (var f in files)
                sb.Append(File.ReadAllText(f));

            File.WriteAllText(model.OutputFilePath, sb.ToString());
            view.Show($"Output saved to '{model.OutputFilePath}'.\n");
        }

        private void CompareFiles()
        {
            view.Show("=== Step 5: Compare Files ===");
            if (!File.Exists(model.InputFilePath) || !File.Exists(model.OutputFilePath))
            {
                view.ShowError("Missing files.");
                return;
            }

            string input = File.ReadAllText(model.InputFilePath);
            string output = File.ReadAllText(model.OutputFilePath);

            if (input.Equals(output, StringComparison.Ordinal))
                view.Show("SUCCESS: Files are identical.");
            else
                view.ShowError("FAILURE: Files differ.");
        }

        private void Cleanup()
        {
            try
            {
                if (Directory.Exists(model.FragmentFolder)) Directory.Delete(model.FragmentFolder, true);
                if (File.Exists(model.InputFilePath)) File.Delete(model.InputFilePath);
                if (File.Exists(model.OutputFilePath)) File.Delete(model.OutputFilePath);
                view.Show("All files deleted.");
            }
            catch (Exception ex) { view.ShowError(ex.Message); }
        }
    }
}
