﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Windows;
using Caliburn.Micro;
using Ninject;
using ToDoManager.Model.Entities;
using ToDoManager.Model.Models;
using ToDoManager.Model.Models.Interfaces;
using ToDoManager.Model.Repository;
using ToDoManager.Model.Repository.Interfaces;
using ToDoManager.View.EventHandlers;
using ToDoManager.View.Utils;
using ToDoManager.View.ViewModels;

namespace ToDoManager.View
{
    public class Bootstrapper : BootstrapperBase
    {
        private IKernel _kernel;

        public Bootstrapper() => Initialize();

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            dynamic settings = new ExpandoObject();
            settings.Title = "Менеджер задач";
            DisplayRootViewFor<ShellViewModel>(settings);
            _kernel.Get<IEventAggregator>().PublishOnBackgroundThread(new LoadStartEvent());
        }

        protected override void Configure()
        {
            _kernel = new StandardKernel();
            _kernel.Bind<IWindowManager>().To<WindowManager>().InSingletonScope();
            _kernel.Bind<IEventAggregator>().To<EventAggregator>().InSingletonScope();
            _kernel.Bind<ToDoManagerContext>().ToSelf().InSingletonScope();
            _kernel.Bind<ContextFactory>().ToSelf().InSingletonScope();
            _kernel.Bind<IDbRepository<TaskEntity>>().To<DbRepository<TaskEntity>>().InSingletonScope();
            _kernel.Bind<IDbRepository<TaskGroupEntity>>().To<DbRepository<TaskGroupEntity>>().InSingletonScope();
            _kernel.Bind<ISettingsRepository>().To<SettingsRepository>().InSingletonScope();
            _kernel.Bind<ITaskModel, IBaseModel>().To<TaskModel>();
            _kernel.Bind<ITaskGroupModel, IBaseModel>().To<TaskGroupModel>();
            _kernel.Bind<EditGroupViewModel>().ToSelf();
            _kernel.Bind<EditTaskViewModel>().ToSelf();
            _kernel.Bind<EntityToVmConverter>().ToSelf();
            _kernel.Bind<SettingsModel>().ToSelf().InSingletonScope();
        }

        protected override object GetInstance(Type service, string key) =>
            service != null ? string.IsNullOrEmpty(key) ? _kernel.Get(service) : _kernel.Get(service, key) : null;

        protected override IEnumerable<object> GetAllInstances(Type service) => _kernel.GetAll(service);

        protected override void BuildUp(object instance) => _kernel.Inject(instance);

        protected override void OnExit(object sender, EventArgs e) => _kernel.Dispose();
    }
}