﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ToDoManager.Model.Entities;

namespace ToDoManager.Model.Models.Interfaces
{
    public interface ITaskGroupModel : IBaseModel<TaskGroupEntity>
    {
        IEnumerable<TaskEntity> GetTasksFromGroup(TaskGroupEntity groupEntity);
    }
}