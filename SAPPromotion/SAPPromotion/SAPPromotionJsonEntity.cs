using System.Collections.Generic;

namespace SAPPromotion
    {
    public class SAPPromotionJsonEntity
    {
        public List<SAPPromotionMasterDetailsEntity> PRODHDR { get; set; }
        public List<SAPMaterialGroupPromotionEntity> MAGRHD { get; set; }
        public List<SAPCustomerGroupPromotionEntity> CUGRHD { get; set; }
    }
}
