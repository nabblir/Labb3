using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb3.ViewModels
    {
    class ConfigurationViewModel : BaseViewModel
        {
        private readonly MainWindowViewModel? mainWindowViewModel;

        public ConfigurationViewModel(MainWindowViewModel? mainWindowViewModel)
            {
            this.mainWindowViewModel = mainWindowViewModel;
            }
        }
    }