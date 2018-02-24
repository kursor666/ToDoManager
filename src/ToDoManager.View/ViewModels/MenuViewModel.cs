using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Media;
using Caliburn.Micro;
using ToDoManager.Model.Entities;
using ToDoManager.Model.Models;
using ToDoManager.View.EventHandlers;
using ToDoManager.View.Utils;

namespace ToDoManager.View.ViewModels
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
    public class MenuViewModel : PropertyChangedBase
    {
        private readonly SettingsModel _settingsModel;
        private readonly IEventAggregator _eventAggregator;
        private readonly IWindowManager _windowManager;

        public MenuViewModel(SettingsModel settingsModel, IEventAggregator eventAggregator,
            IWindowManager windowManager)
        {
            _settingsModel = settingsModel;
            _eventAggregator = eventAggregator;
            _windowManager = windowManager;
        }

        public bool DisableAutoSave
        {
            get => _settingsModel.AutoSaveTimer == (double) AutoSaveInterval.Disable;
            set
            {
                if (value)
                    SetInterval(AutoSaveInterval.Disable);
                NotifyOfPropertyChange(() => DisableAutoSave);
            }
        }

        public bool TenSecAutoSave
        {
            get => _settingsModel.AutoSaveTimer == (double) AutoSaveInterval.TenSecods;
            set
            {
                if (value)
                    SetInterval(AutoSaveInterval.TenSecods);
                NotifyOfPropertyChange(() => TenSecAutoSave);
            }
        }

        public bool ThirtySecAutoSave
        {
            get => _settingsModel.AutoSaveTimer == (double) AutoSaveInterval.ThirtySecods;
            set
            {
                if (value)
                    SetInterval(AutoSaveInterval.ThirtySecods);
                NotifyOfPropertyChange(() => ThirtySecAutoSave);
            }
        }

        public bool SixtySecAutoSave
        {
            get => _settingsModel.AutoSaveTimer == (double) AutoSaveInterval.SixtySeconds;
            set
            {
                if (value)
                    SetInterval(AutoSaveInterval.SixtySeconds);
                NotifyOfPropertyChange(() => SixtySecAutoSave);
            }
        }

        public void NewTask() => _eventAggregator.PublishOnUIThread(new EditEntityEvent<TaskEntity>(null));

        public void NewGroup() =>
            _eventAggregator.PublishOnUIThread(new EditEntityEvent<TaskGroupEntity>(null));

        public void Exit() => Environment.Exit(0);

        public void SetWhite() => SetColor(Color.FromRgb(255, 255, 255));

        public void SetLightGreen() => SetColor(Color.FromRgb(124, 249, 127));

        public void SetLightOrange() => SetColor(Color.FromRgb(255, 205, 130));

        public void SetLightBlue() => SetColor(Color.FromRgb(178, 207, 255));

        public void AboutWindowShow()
        {
            _windowManager.ShowWindow(new AboutViewModel());
        }

        private void SetColor(Color color)
        {
            _settingsModel.BackgroundColor = new SolidColorBrush(color);
            _eventAggregator.PublishOnUIThread(new SelectedBackgroungColorEvent(_settingsModel.BackgroundColor));
        }

        private void SetInterval(AutoSaveInterval interval)
        {
            _settingsModel.AutoSaveTimer = (double) interval;
            _eventAggregator.PublishOnUIThread((AutoSaveInterval) _settingsModel.AutoSaveTimer);
        }
    }
}