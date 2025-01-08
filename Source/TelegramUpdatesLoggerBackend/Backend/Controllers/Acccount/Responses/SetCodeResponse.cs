namespace Backend.Controllers.Acccount.Responses
{
    public class SetCodeResponse : Tools.HttpDataBase
    {
        public required long ownerId { get; set; }   
        public required string phone {  get; set; }
        public required string status { get; set; }
    }
}
