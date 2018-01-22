using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Windows.Media;
using Caliburn.Micro;
using ToDoManager.Model.Entities;
using ToDoManager.Model.Models;
using ToDoManager.View.EventHandlers;

namespace ToDoManager.View.ViewModels
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class MenuViewModel : PropertyChangedBase
    {
        private readonly SettingsModel _settingsModel;
        private readonly IEventAggregator _eventAggregator;

        public MenuViewModel(SettingsModel settingsModel, IEventAggregator eventAggregator)
        {
            _settingsModel = settingsModel;
            _eventAggregator = eventAggregator;
            _eventAggregator.Publish(new SelectedBackgroungColorEvent(_settingsModel.BackgroundColor),
                action => Task.Factory.StartNew(action));
        }

        public void NewTask() => _eventAggregator.Publish(new EditEntityEvent<TaskEntity>(
            new TaskEntity()), action => Task.Factory.StartNew(action));

        public void NewGroup() => _eventAggregator.Publish(new EditEntityEvent<TaskGroupEntity>(
            new TaskGroupEntity()), action => Task.Factory.StartNew(action));

        public void Exit() => Environment.Exit(0);

        public bool AutoSaveEnabled
        {
            get => _settingsModel.AutoSaveEnabled;
            set
            {
                _settingsModel.AutoSaveEnabled = value;
                NotifyOfPropertyChange(() => AutoSaveEnabled);
            }
        }

        public void SetLightGreen()
        {
            _settingsModel.BackgroundColor = new SolidColorBrush(Color.FromRgb(124, 249, 127));
            _eventAggregator.Publish(
                new SelectedBackgroungColorEvent(_settingsModel.BackgroundColor),
                action => Task.Factory.StartNew(action));
        }

        public void SetLightOrange()
        {
            _settingsModel.BackgroundColor = new SolidColorBrush(Color.FromRgb(255, 205, 130));
            _eventAggregator.Publish(
                new SelectedBackgroungColorEvent(_settingsModel.BackgroundColor),
                action => Task.Factory.StartNew(action));
        }

        public void SetLightBlue()
        {
            _settingsModel.BackgroundColor = new SolidColorBrush(Color.FromRgb(178, 207, 255));
            _eventAggregator.Publish(
                new SelectedBackgroungColorEvent(_settingsModel.BackgroundColor),
                action => Task.Factory.StartNew(action));
        }
    }
}