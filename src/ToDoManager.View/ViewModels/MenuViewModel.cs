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

        public void NewTask() => _eventAggregator.Publish(new EditEntityEvent<TaskEntity>(
            new TaskEntity()), action => Task.Factory.StartNew(action));

        public void NewGroup() => _eventAggregator.Publish(new EditEntityEvent<TaskGroupEntity>(
            new TaskGroupEntity()), action => Task.Factory.StartNew(action));

        public void Exit() => Environment.Exit(0);

        public bool DisableAutoSave
        {
            get => _settingsModel.AutoSaveTimer == (double) AutoSaveInterval.Disable;
            set
            {
                if (value)
                {
                    _settingsModel.AutoSaveTimer = (double) AutoSaveInterval.Disable;
                    _eventAggregator.Publish((AutoSaveInterval)_settingsModel.AutoSaveTimer, action => Task.Factory.StartNew(action));
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
                    _eventAggregator.Publish((AutoSaveInterval)_settingsModel.AutoSaveTimer, action => Task.Factory.StartNew(action));
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
                    _eventAggregator.Publish((AutoSaveInterval)_settingsModel.AutoSaveTimer, action => Task.Factory.StartNew(action));
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
                    _eventAggregator.Publish((AutoSaveInterval)_settingsModel.AutoSaveTimer, action => Task.Factory.StartNew(action));
                }
                NotifyOfPropertyChange(() => SixtySecAutoSave);
            }
        }

        public void SetWhite()
        {
            _settingsModel.BackgroundColor = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            _eventAggregator.Publish(new SelectedBackgroungColorEvent(_settingsModel.BackgroundColor),
                action => Task.Factory.StartNew(action));
        }

        public void SetLightGreen()
        {
            _settingsModel.BackgroundColor = new SolidColorBrush(Color.FromRgb(124, 249, 127));
            _eventAggregator.Publish(new SelectedBackgroungColorEvent(_settingsModel.BackgroundColor),
                action => Task.Factory.StartNew(action));
        }

        public void SetLightOrange()
        {
            _settingsModel.BackgroundColor = new SolidColorBrush(Color.FromRgb(255, 205, 130));
            _eventAggregator.Publish(new SelectedBackgroungColorEvent(_settingsModel.BackgroundColor),
                action => Task.Factory.StartNew(action));
        }

        public void SetLightBlue()
        {
            _settingsModel.BackgroundColor = new SolidColorBrush(Color.FromRgb(178, 207, 255));
            _eventAggregator.Publish(new SelectedBackgroungColorEvent(_settingsModel.BackgroundColor),
                action => Task.Factory.StartNew(action));
        }
    }
}