using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
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

        public MenuViewModel(SettingsModel settingsModel, IEventAggregator eventAggregator)
        {
            _settingsModel = settingsModel;
            _eventAggregator = eventAggregator;
        }

        public void NewTask() => _eventAggregator.PublishOnUIThread(new EditEntityEvent<TaskEntity>(new TaskEntity()));

        public void NewGroup() =>
            _eventAggregator.PublishOnUIThread(new EditEntityEvent<TaskGroupEntity>(new TaskGroupEntity()));

        public void Exit() => Environment.Exit(0);

        public bool DisableAutoSave
        {
            get => _settingsModel.AutoSaveTimer == (double) AutoSaveInterval.Disable;
            set
            {
                if (value)
                {
                    _settingsModel.AutoSaveTimer = (double) AutoSaveInterval.Disable;
                    _eventAggregator.PublishOnUIThread((AutoSaveInterval) _settingsModel.AutoSaveTimer);
                }

                NotifyOfPropertyChange(() => DisableAutoSave);
            }
        }

        public bool TenSecAutoSave
        {
            get => _settingsModel.AutoSaveTimer == (double) AutoSaveInterval.TenSecods;
            set
            {
                if (value)
                {
                    _settingsModel.AutoSaveTimer = (double) AutoSaveInterval.TenSecods;
                    _eventAggregator.PublishOnUIThread((AutoSaveInterval) _settingsModel.AutoSaveTimer);
                }

                NotifyOfPropertyChange(() => TenSecAutoSave);
            }
        }

        public bool ThirtySecAutoSave
        {
            get => _settingsModel.AutoSaveTimer == (double) AutoSaveInterval.ThirtySecods;
            set
            {
                if (value)
                {
                    _settingsModel.AutoSaveTimer = (double) AutoSaveInterval.ThirtySecods;
                    _eventAggregator.PublishOnUIThread((AutoSaveInterval) _settingsModel.AutoSaveTimer);
                }

                NotifyOfPropertyChange(() => ThirtySecAutoSave);
            }
        }

        public bool SixtySecAutoSave
        {
            get => _settingsModel.AutoSaveTimer == (double) AutoSaveInterval.SixtySeconds;
            set
            {
                if (value)
                {
                    _settingsModel.AutoSaveTimer = (double) AutoSaveInterval.SixtySeconds;
                    _eventAggregator.PublishOnUIThread((AutoSaveInterval) _settingsModel.AutoSaveTimer);
                }

                NotifyOfPropertyChange(() => SixtySecAutoSave);
            }
        }

        public void SetWhite() => SetColor(Color.FromRgb(255, 255, 255));

        public void SetLightGreen() => SetColor(Color.FromRgb(124, 249, 127));

        public void SetLightOrange() => SetColor(Color.FromRgb(255, 205, 130));

        public void SetLightBlue() => SetColor(Color.FromRgb(178, 207, 255));

        private void SetColor(Color color)
        {
            _settingsModel.BackgroundColor = new SolidColorBrush(color);
            _eventAggregator.PublishOnUIThread(new SelectedBackgroungColorEvent(_settingsModel.BackgroundColor));
        }
    }
}