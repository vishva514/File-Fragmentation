using System.Collections.Generic;


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
    }
}