using OTS.Data.Models;

namespace OTS.Data.Repository
{
    public interface ILoginRepository
    {
        Task<CorpUserModel> UserLogin(CorpUserModel login);
        Task<VendorModel> VendorLogin(VendorModel login);
        Task<ClientModel> ClientLogin(ClientModel login);
    }
}