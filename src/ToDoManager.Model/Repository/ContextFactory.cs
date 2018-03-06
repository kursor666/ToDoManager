using System.Diagnostics.CodeAnalysis;

namespace ToDoManager.Model.Repository
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class ContextFactory
    {
        private ToDoManagerContext _context;

        public ToDoManagerContext Context => _context??(_context = new ToDoManagerContext());
    }
}