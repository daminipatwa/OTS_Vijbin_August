using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTS.Data.Models
{
    public class combomodel
    {
        public List<VendorModel> VendorModels { get; set; }
        public List<CorpUserModel> CorpUserModels { get; set; }
        public List<CostCenter> costCenters { get; set; }
        public CostCenter costCenterModel { get; set; }
        public List<UserTypeModel> UserTypeModels { get; set; }
        public List<DepartmentModel> departmentModels { get; set; }

        public DepartmentModel departmentModel { get; set; }
        public List<BranchModel> branchModels { get; set; }
        public BranchModel branchModel { get; set; }
        public CorpUserModel CorpUser_Models { get; set; }
        public List<ClientModel> Client_Model { get; set; }
        public List<ProductModel> product_Model { get; set; }
        public ProductModel productModel { get; set; }
        public List<CartModel> cart_Model { get; set; }
        public List<OrderModel> OrderModel { get; set; }
        public List<order_list_model> order_list_model { get; set; }
    }
}
