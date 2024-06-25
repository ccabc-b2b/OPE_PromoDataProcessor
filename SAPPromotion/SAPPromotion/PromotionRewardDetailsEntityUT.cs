namespace SAPPromotion
    {
    public class PromotionRewardDetailsEntityUT
    {
        public string PromotionID { get; set; }
        public string RequirementId_RWD { get; set; }
        public string PromoRewardID { get; set; }
        public string MaterialNumber { get; set; }
        public string MaterialGroupID { get; set; }
        public string RequirementQty_RWD { get; set; }
        public string RequirementValue_RWD { get; set; }
        public string RewardQty { get; set; }
        public string RewardValue { get; set; }
        public string RewardPercentage { get; set; }

        public PromotionRewardDetailsEntityUT(SAPPromotionRewardDetailsEntity promotionRewardDetailsEntity) 
        {
            this.PromotionID=promotionRewardDetailsEntity.PromotionID;
            this.RequirementId_RWD =promotionRewardDetailsEntity.RequirementId_RWD;
            this.PromoRewardID=promotionRewardDetailsEntity.PromoRewardID;
            this.MaterialNumber =promotionRewardDetailsEntity.MaterialNumber;
            this.MaterialGroupID =promotionRewardDetailsEntity.MaterialGroupID;
            this.RequirementQty_RWD = promotionRewardDetailsEntity.RequirementQty_RWD;
            this.RequirementValue_RWD = promotionRewardDetailsEntity.RequirementValue_RWD;
            this.RewardQty= promotionRewardDetailsEntity.RewardQty; ;
            this.RewardValue= promotionRewardDetailsEntity.RewardValue;
            this.RewardPercentage= promotionRewardDetailsEntity.RewardPercentage;
        }
    }
}
