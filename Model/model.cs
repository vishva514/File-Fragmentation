using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileFragmentationApp.Model
{
    public class FragmentationModel
    {
        public string InputFilePath { get; set; } = "input.txt";
        public string OutputFilePath { get; set; } = "output.txt";
        public string FragmentFolder { get; set; } = "Fragments";
        public int FragmentSize { get; set; }
        public List<string> FragmentFiles { get; set; } = new List<string>();
        public string InputText { get; set; }
        public int TotalFragments { get; set; }

        // ✅ Create Input File
        public void SaveInputToFile(string text)
        {
            InputText = text;
            File.WriteAllText(InputFilePath, InputText);
        }

        // ✅ Fragment the input text into files
        public void FragmentText()
        {
            if (Directory.Exists(FragmentFolder))
                Directory.Delete(FragmentFolder, true);
            Directory.CreateDirectory(FragmentFolder);

            string content = InputText ?? string.Empty;
            FragmentFiles.Clear();

            if (content.Length == 0)
                return;

            TotalFragments = (content.Length + FragmentSize - 1) / FragmentSize;
            int padLength = Math.Max(1, TotalFragments.ToString().Length);

            for (int i = 0; i < TotalFragments; i++)
            {
                int start = i * FragmentSize;
                int len = Math.Min(FragmentSize, content.Length - start);
                string frag = content.Substring(start, len);
                string fileName = Path.Combine(FragmentFolder, (i + 1).ToString("D" + padLength) + ".txt");
                File.WriteAllText(fileName, frag);
                FragmentFiles.Add(fileName);
            }
        }

        // ✅ Verify a specific fragment file
        public string GetFragmentContent(string fileName)
        {
            if (File.Exists(fileName))
                return File.ReadAllText(fileName);
            return null;
        }

        // ✅ Defragment files back to one file
        public void DefragmentFiles()
        {
            if (!Directory.Exists(FragmentFolder)) return;

            var files = Directory.GetFiles(FragmentFolder, "*.txt");
            Array.Sort(files);

            var sb = new StringBuilder();
            foreach (var f in files)
                sb.Append(File.ReadAllText(f));

            File.WriteAllText(OutputFilePath, sb.ToString());
        }

        // ✅ Compare input and output files
        public bool CompareInputAndOutput()
        {
            if (!File.Exists(InputFilePath) || !File.Exists(OutputFilePath))
                return false;

            string input = File.ReadAllText(InputFilePath);
            string output = File.ReadAllText(OutputFilePath);
            return input.Equals(output, StringComparison.Ordinal);
        }

        // ✅ Cleanup all files
        public void Cleanup()
        {
            if (Directory.Exists(FragmentFolder)) Directory.Delete(FragmentFolder, true);
            if (File.Exists(InputFilePath)) File.Delete(InputFilePath);
            if (File.Exists(OutputFilePath)) File.Delete(OutputFilePath);
        }
    }
}
