namespace TestDatabase.Dtos.ResponseDtos
{
  public class MessageToClientDto
  {
    public int MessageID { get; set; }
    public string Content { get; set; }
    public DateTime TimeStamp { get; set; }

    public SenderDto Sender { get; set; }
  }
}
