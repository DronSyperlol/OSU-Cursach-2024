using Database;

namespace Core.Interfaces
{
    public interface IWorker
    {
        public Task Handle(ApplicationContext context);
    }
}
