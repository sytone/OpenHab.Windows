using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;

namespace OpenHab.UI.ViewModels
{
    public class SelectionWidgetViewModel : WidgetViewModelBase
    {
        private WidgetMappingViewModel _selectedValue;
        private bool _ignoreValueChange;
        private readonly ObservableCollection<WidgetMappingViewModel> _mappings
            = new ObservableCollection<WidgetMappingViewModel>();
        private ICommand _toggleCommand;

        public WidgetMappingViewModel SelectedValue
        {
            get { return _selectedValue; }
            set { SetProperty(ref _selectedValue, value); }
        }

        public IEnumerable<WidgetMappingViewModel> Mappings
        {
            get { return _mappings; }
        }

        public bool IgnoreValueChange
        {
            get { return _ignoreValueChange; }
            private set { SetProperty(ref _ignoreValueChange, value); }
        }

        protected override void OnModelUpdated()
        {
            base.OnModelUpdated();
            _mappings.Clear();

            if (Widget.Mappings != null)
            {
                foreach (var mapping in Widget.Mappings)
                {
                    bool isChecked = false;
                    if (Widget.Item != null && mapping.Command != null)
                        isChecked = string.Equals(Widget.Item.State, mapping.Command, StringComparison.OrdinalIgnoreCase);

                    _mappings.Add(new WidgetMappingViewModel(mapping)
                    {
                        Command = ToggleCommand,
                        IsChecked = isChecked
                    });
                }
            }

            SelectedValue = _mappings.First(m => m.IsChecked == true);
        }


        public ICommand ToggleCommand
        {
            get
            {
                return (_toggleCommand) ??
                       (_toggleCommand = new DelegateCommand<WidgetMappingViewModel>(TogleCommandExecuted));
            }
        }

        public void TogleCommandExecuted(WidgetMappingViewModel widgetMapping)
        {
            if (widgetMapping == null) return;
            SendItemCommand(widgetMapping.Mapping.Command);
        }

    }
}