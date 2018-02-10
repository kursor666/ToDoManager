using System;
using System.Diagnostics.CodeAnalysis;
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

        private void TimerTick(object sender, EventArgs e) => Save();

        public bool CanSave => _settingsModel.AutoSaveTimer == 0;

        public void Save()
        {
            _eventAggregator.PublishOnUIThread(new SaveEvent());
            //Reload();
        }

        public bool CanCancel => _settingsModel.AutoSaveTimer == 0;

        public void Cancel()
        {
            _eventAggregator.PublishOnUIThread(new CancelEvent());
            Reload();
        }

        private void Reload() => _eventAggregator.PublishOnUIThread(new ReloadEvent());

        public void Handle(AutoSaveInterval message)
        {
            _timer.Stop();
            if ((double) message != 0)
            {
                _timer.Interval = TimeSpan.FromSeconds((double) message);
                _timer.Start();
            }
            Refresh();
        }
    }
}