using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Windows.Threading;
using Caliburn.Micro;
using ToDoManager.Model.Models;
using ToDoManager.View.EventHandlers;
using ToDoManager.View.Utils;

namespace ToDoManager.View.ViewModels
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
    public class ManageViewModel : PropertyChangedBase, IHandle<AutoSaveInterval>
    {
        private readonly SettingsModel _settingsModel;
        private readonly IEventAggregator _eventAggregator;
        private readonly DispatcherTimer _timer;

        public ManageViewModel(SettingsModel settingsModel, IEventAggregator eventAggregator)
        {
            _settingsModel = settingsModel;
            _eventAggregator = eventAggregator;
            _timer = new DispatcherTimer();
            _timer.Tick += TimerTick;
            _eventAggregator.Subscribe(this);
        }

        private void TimerTick(object sender, EventArgs e)
        {
            Save();
        }

        public bool CanSave => _settingsModel.AutoSaveTimer == 0;

        public void Save()
        {
            _eventAggregator.Publish(EventTypes.Save, action => Task.Factory.StartNew(action).ContinueWith(Reload()));
            _eventAggregator.Publish(EventTypes.Reload, action => Task.Factory.StartNew(action));
        }

        public bool CanCancel => _settingsModel.AutoSaveTimer == 0;

        public void Cancel()
        {
            _eventAggregator.Publish(EventTypes.Cancel, action => Task.Factory.StartNew(action));
            _eventAggregator.Publish(EventTypes.Reload, action => Task.Factory.StartNew(action));
        }

        private Action<Task> Reload()
        {
            return task => _eventAggregator.Publish(EventTypes.Reload, action => Task.Factory.StartNew(action));
        }

        public void Handle(AutoSaveInterval message)
        {
            _timer.Stop();
            Refresh();
            if ((double)message == 0) return;
            _timer.Interval = TimeSpan.FromSeconds((double)message);
            _timer.Start();
        }
    }
}