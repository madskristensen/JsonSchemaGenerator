namespace JsonSchemaGenerator
{
    public class General : BaseOptionModel<General>, IRatingConfig
    {
        public int RatingRequests { get; set; }
    }
}
