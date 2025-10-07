using FileFragmentationApp.Controller;
using FileFragmentationApp.Model;
using FileFragmentationApp.View;

namespace FileFragmentationApp
{
    public static class Program
    {
        public static void Main()
        {
            var model = new FragmentationModel();
            var view = new ConsoleView();
            var controller = new FragmentController(model, view);
            controller.Run();
        }
    }
}
