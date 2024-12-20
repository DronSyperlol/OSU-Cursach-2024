using Database;

namespace Core.Interfaces
{
    internal interface IWorker
    {
        public Task Handle(ApplicationContext context);
    }
}
