using OTS.Data.DataAccess;
using OTS.Data.Models;
namespace OTS.Data.Repository;

public class LoginRepository: ILoginRepository
{
    private readonly ISqlDataAccess _db;

    public LoginRepository(ISqlDataAccess db)
    {
        _db = db;
    }

    public async Task<CorpUserModel> UserLogin(CorpUserModel login)
    { 
        IEnumerable<CorpUserModel> result=await _db.GetData<CorpUserModel, dynamic>("sp_login", new { login.username, login.password,login.usertype });
       return result.FirstOrDefault();
    }

    public async Task<VendorModel> VendorLogin(VendorModel login)
    {
        IEnumerable<VendorModel> result = await _db.GetData<VendorModel, dynamic>("sp_vendor_login", new { login.username, login.password });
        return result.FirstOrDefault();
    }

    public async Task<ClientModel> ClientLogin(ClientModel login)
    {
        IEnumerable<ClientModel> result = await _db.GetData<ClientModel, dynamic>("SP_client_Login", new { login.username, login.password });
        return result.FirstOrDefault();
    }
}
