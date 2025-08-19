namespace OTS.Data.Repository
{
    public interface ISendMailRepositery
    {
         void Send_Mail(string emailid, string subject, string msg);
    }
}