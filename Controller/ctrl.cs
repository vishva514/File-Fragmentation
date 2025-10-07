using System;
using System.Text;
using System.IO;
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
            model.Cleanup();
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
                    model.Cleanup();
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

            model.SaveInputToFile(sb.ToString());
            view.Show($"Created '{model.InputFilePath}' ({model.InputText.Length} chars).\n");
        }

        private void Fragment()
        {
            view.Show("=== Step 2: Fragmentation ===");
            string input = view.Prompt("Enter fragment size: ");
            if (!int.TryParse(input, out int size) || size <= 0)
                throw new ArgumentException("Fragment size must be positive.");

            model.FragmentSize = size;
            model.FragmentText();

            if (model.FragmentFiles.Count == 0)
            {
                view.Show("Input text empty.");
                return;
            }

            view.Show($"Created {model.FragmentFiles.Count} fragment(s):");
            view.ShowFiles(model.FragmentFiles);
        }

        private void VerifyFile()
        {
            while (true) {
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

                string content = model.GetFragmentContent(filePath);
                if (content != null)
                {
                    view.Show($"File '{filePath}' exists:\n{content}");
                    return;
                }
                else
                {
                    view.ShowError("File not found.");
                }
                    
            }
        }

        private void Defragment()
        {
            view.Show("=== Step 4: Defragmentation ===");
            model.DefragmentFiles();
            view.Show($"Output saved to '{model.OutputFilePath}'.\n");
        }

        private void CompareFiles()
        {
            view.Show("=== Step 5: Compare Files ===");
            bool identical = model.CompareInputAndOutput();
            if (identical)
                view.Show("SUCCESS: Files are identical.");
            else
                view.ShowError("FAILURE: Files differ.");
        }
    }
}

