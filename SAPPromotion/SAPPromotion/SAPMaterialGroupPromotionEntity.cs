using System.Collections.Generic;

namespace SAPPromotion
    {
    public class SAPMaterialGroupPromotionEntity
    {
        public string PromotionID { get; set; }
        public string MaterialGroupingDescription { get; set; }
        public string MaterialGrpInactiveIndicator { get; set; }
        public List<SAPMaterialPromotionDetailsEntity> MAGRIN { get; set; }

    }
}
