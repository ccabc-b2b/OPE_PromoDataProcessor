namespace SAPPromotion
    {
    public class PromotionRequirementsDetailsEntityUT
    {
        public string PromotionID { get; set; }
        public string RequirementId { get; set; }
        public string MaterialGroupID { get; set; }
        public string MaterialNumber { get; set; }
        public string ProductSegmentID { get; set; }
        public string RequirementQty { get; set; }
        public string RequirementValue { get; set; }

        public PromotionRequirementsDetailsEntityUT(SAPPromotionRequirementsDetailsEntity promotionRequirementsDetailsEntity)
        {
            this.PromotionID = promotionRequirementsDetailsEntity.PromotionID;  
            this.RequirementId = promotionRequirementsDetailsEntity.RequirementId;
            this.MaterialGroupID = promotionRequirementsDetailsEntity.MaterialGroupID;
            this.MaterialNumber = promotionRequirementsDetailsEntity.MaterialNumber;    
            this.ProductSegmentID= promotionRequirementsDetailsEntity.ProductSegmentID;
            this.RequirementQty = promotionRequirementsDetailsEntity.RequirementQty;
            this.RequirementValue = promotionRequirementsDetailsEntity.RequirementValue;
        }

    }
}
