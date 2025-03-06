using System.Windows;
using System.Collections.Generic;

using Framework.ViewModel;

namespace Framework.View
{
    public partial class DialogWindow : Window
    {
        private readonly DialogVM _dialogVM;

        public DialogWindow(MainVM mainVM, List<string> labels)
        {
            InitializeComponent();

            _dialogVM = new DialogVM();

            _dialogVM.Theme = mainVM.Theme;
            _dialogVM.CreateParameters(labels);

            DataContext = _dialogVM;
        }

        public List<double> GetValues()
        {
            return _dialogVM.GetValues();
        }
    }
}