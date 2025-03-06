using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Framework.View;
using Framework.Model;

namespace Framework.ViewModel
{
    class DialogVM : BaseVM
    {
        public void CreateParameters(List<string> labels)
        {
            Height = (labels.Count + 3) * 40;

            foreach (var label in labels)
            {
                Parameters.Add(new DialogParameter()
                {
                    ParamText = label,
                    Foreground = Theme.TextForeground,
                    Height = 20,
                });
            }
        }

        public List<double> GetValues()
        {
            var values = new List<double>();

            foreach (var parameter in Parameters)
            {
                string text = parameter.InputText;
                if (text == null || text.Trim().Length == 0 || IsNumeric(text) == false)
                    values.Add(0);
                else
                    values.Add(double.Parse(text));
            }

            return values;
        }

        private bool IsNumeric(string text)
        {
            return double.TryParse(text, out _);
        }

        #region Properties and commands
        public double Height { get; set; }

        public IThemeMode Theme { get; set; } =
            LimeGreenTheme.Instance;

        public ObservableCollection<DialogParameter> Parameters { get; } =
            new ObservableCollection<DialogParameter>();

        private ICommand _closeCommand;
        public ICommand CloseCommand
        {
            get
            {
                if (_closeCommand == null)
                    _closeCommand = new RelayCommand(p =>
                    {
                        var window = Application.Current.Windows.OfType<DialogWindow>().SingleOrDefault(w => w.IsActive);
                        window?.Close();
                    });

                return _closeCommand;
            }
        }
        #endregion
    }
}