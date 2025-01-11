namespace Config.Entities
{
    public class PathCollection(string foreignUrl)
    {
        public string Static { get => foreignUrl + "static/"; }
    }
}
