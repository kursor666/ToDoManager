using System;
using System.Collections.Generic;
using System.Windows;
using Caliburn.Micro;
using Ninject;
using ToDoManager.Model.Entities;
using ToDoManager.Model.Models;
using ToDoManager.Model.Models.Interfaces;
using ToDoManager.Model.Repository;
using ToDoManager.Model.Repository.Interfaces;
using ToDoManager.View.ViewModels;

namespace ToDoManager.View
{
    public class Bootstrapper : BootstrapperBase
    {
        private IKernel _kernel;

        public Bootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();
        }

        protected override void Configure()
        {
            _kernel = new StandardKernel();
            _kernel.Bind<IWindowManager>().To<WindowManager>().InSingletonScope();
            _kernel.Bind<IEventAggregator>().To<EventAggregator>().InSingletonScope();
            _kernel.Bind<ToDoManagerContext>().ToSelf();
            _kernel.Bind<IDbRepository<TaskEntity>>().To<DbRepository<TaskEntity>>();
            _kernel.Bind<IDbRepository<TaskGroupEntity>>().To<DbRepository<TaskGroupEntity>>();
            _kernel.Bind<ITaskModel>().To<TaskModel>();
            _kernel.Bind<ITaskGroupModel>().To<TaskGroupModel>();
            _kernel.Bind<EditGroupViewModel>().ToSelf();
            _kernel.Bind<EditTaskViewModel>().ToSelf();
        }

        protected override object GetInstance(Type service, string key)
        {
            return string.IsNullOrEmpty(key) ? _kernel.Get(service) : _kernel.Get(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _kernel.GetAll(service);
        }
        
        protected override void BuildUp(object instance)
        {
            _kernel.Inject(instance);
        }

        protected override void OnExit(object sender, EventArgs e)
        {
            _kernel.Dispose();
        }
    }
}