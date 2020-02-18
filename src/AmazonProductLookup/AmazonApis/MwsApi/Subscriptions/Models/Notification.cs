namespace AmazonProductLookup.AmazonApis.MwsApi.Subscriptions.Models
{
	/// <remarks/>
	[System.SerializableAttribute]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	[System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
	public class Notification
	{
		private NotificationNotificationMetaData notificationMetaDataField;

		private NotificationNotificationPayload notificationPayloadField;

		/// <remarks/>
		public NotificationNotificationMetaData NotificationMetaData
		{
			get
			{
				return notificationMetaDataField;
			}

			set
			{
				notificationMetaDataField = value;
			}
		}

		/// <remarks/>
		public NotificationNotificationPayload NotificationPayload
		{
			get
			{
				return notificationPayloadField;
			}

			set
			{
				notificationPayloadField = value;
			}
		}
	}

	/// <remarks/>
	[System.SerializableAttribute]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	public class NotificationNotificationMetaData
	{
		private string notificationTypeField;

		private decimal payloadVersionField;

		private string uniqueIdField;

		private System.DateTime publishTimeField;

		private string sellerIdField;

		private string marketplaceIdField;

		/// <remarks/>
		public string NotificationType
		{
			get
			{
				return notificationTypeField;
			}

			set
			{
				notificationTypeField = value;
			}
		}

		/// <remarks/>
		public decimal PayloadVersion
		{
			get
			{
				return payloadVersionField;
			}

			set
			{
				payloadVersionField = value;
			}
		}

		/// <remarks/>
		public string UniqueId
		{
			get
			{
				return uniqueIdField;
			}

			set
			{
				uniqueIdField = value;
			}
		}

		/// <remarks/>
		public System.DateTime PublishTime
		{
			get
			{
				return publishTimeField;
			}

			set
			{
				publishTimeField = value;
			}
		}

		/// <remarks/>
		public string SellerId
		{
			get
			{
				return sellerIdField;
			}

			set
			{
				sellerIdField = value;
			}
		}

		/// <remarks/>
		public string MarketplaceId
		{
			get
			{
				return marketplaceIdField;
			}

			set
			{
				marketplaceIdField = value;
			}
		}
	}

	/// <remarks/>
	[System.SerializableAttribute]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	public partial class NotificationNotificationPayload
	{
		private NotificationNotificationPayloadAnyOfferChangedNotification anyOfferChangedNotificationField;

		/// <remarks/>
		public NotificationNotificationPayloadAnyOfferChangedNotification AnyOfferChangedNotification
		{
			get
			{
				return anyOfferChangedNotificationField;
			}

			set
			{
				anyOfferChangedNotificationField = value;
			}
		}
	}

	/// <remarks/>
	[System.SerializableAttribute]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	public class NotificationNotificationPayloadAnyOfferChangedNotification
	{
		private NotificationNotificationPayloadAnyOfferChangedNotificationOfferChangeTrigger offerChangeTriggerField;

		private NotificationNotificationPayloadAnyOfferChangedNotificationSummary summaryField;

		private NotificationNotificationPayloadAnyOfferChangedNotificationOffer[] offersField;

		/// <remarks/>
		public NotificationNotificationPayloadAnyOfferChangedNotificationOfferChangeTrigger OfferChangeTrigger
		{
			get
			{
				return offerChangeTriggerField;
			}

			set
			{
				offerChangeTriggerField = value;
			}
		}

		/// <remarks/>
		public NotificationNotificationPayloadAnyOfferChangedNotificationSummary Summary
		{
			get
			{
				return summaryField;
			}

			set
			{
				summaryField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlArrayItemAttribute("Offer", IsNullable = false)]
		public NotificationNotificationPayloadAnyOfferChangedNotificationOffer[] Offers
		{
			get
			{
				return offersField;
			}

			set
			{
				offersField = value;
			}
		}
	}

	/// <remarks/>
	[System.SerializableAttribute]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	public class NotificationNotificationPayloadAnyOfferChangedNotificationOfferChangeTrigger
	{
		private string marketplaceIdField;

		private string aSINField;

		private string itemConditionField;

		private System.DateTime timeOfOfferChangeField;

		/// <remarks/>
		public string MarketplaceId
		{
			get
			{
				return marketplaceIdField;
			}

			set
			{
				marketplaceIdField = value;
			}
		}

		/// <remarks/>
		public string ASIN
		{
			get
			{
				return aSINField;
			}

			set
			{
				aSINField = value;
			}
		}

		/// <remarks/>
		public string ItemCondition
		{
			get
			{
				return itemConditionField;
			}

			set
			{
				itemConditionField = value;
			}
		}

		/// <remarks/>
		public System.DateTime TimeOfOfferChange
		{
			get
			{
				return timeOfOfferChangeField;
			}

			set
			{
				timeOfOfferChangeField = value;
			}
		}
	}

	/// <remarks/>
	[System.SerializableAttribute]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	public partial class NotificationNotificationPayloadAnyOfferChangedNotificationSummary
	{
		private NotificationNotificationPayloadAnyOfferChangedNotificationSummaryOfferCount[] numberOfOffersField;

		private NotificationNotificationPayloadAnyOfferChangedNotificationSummaryLowestPrice[] lowestPricesField;

		private NotificationNotificationPayloadAnyOfferChangedNotificationSummaryBuyBoxPrices buyBoxPricesField;

		private NotificationNotificationPayloadAnyOfferChangedNotificationSummaryListPrice listPriceField;

		private NotificationNotificationPayloadAnyOfferChangedNotificationSummarySalesRank[] salesRankingsField;

		private NotificationNotificationPayloadAnyOfferChangedNotificationSummaryOfferCount1[] buyBoxEligibleOffersField;

		/// <remarks/>
		[System.Xml.Serialization.XmlArrayItemAttribute("OfferCount", IsNullable = false)]
		public NotificationNotificationPayloadAnyOfferChangedNotificationSummaryOfferCount[] NumberOfOffers
		{
			get
			{
				return numberOfOffersField;
			}

			set
			{
				numberOfOffersField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlArrayItemAttribute("LowestPrice", IsNullable = false)]
		public NotificationNotificationPayloadAnyOfferChangedNotificationSummaryLowestPrice[] LowestPrices
		{
			get
			{
				return lowestPricesField;
			}

			set
			{
				lowestPricesField = value;
			}
		}

		/// <remarks/>
		public NotificationNotificationPayloadAnyOfferChangedNotificationSummaryBuyBoxPrices BuyBoxPrices
		{
			get
			{
				return buyBoxPricesField;
			}

			set
			{
				buyBoxPricesField = value;
			}
		}

		/// <remarks/>
		public NotificationNotificationPayloadAnyOfferChangedNotificationSummaryListPrice ListPrice
		{
			get
			{
				return listPriceField;
			}

			set
			{
				listPriceField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlArrayItemAttribute("SalesRank", IsNullable = false)]
		public NotificationNotificationPayloadAnyOfferChangedNotificationSummarySalesRank[] SalesRankings
		{
			get
			{
				return salesRankingsField;
			}

			set
			{
				salesRankingsField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlArrayItemAttribute("OfferCount", IsNullable = false)]
		public NotificationNotificationPayloadAnyOfferChangedNotificationSummaryOfferCount1[] BuyBoxEligibleOffers
		{
			get
			{
				return buyBoxEligibleOffersField;
			}

			set
			{
				buyBoxEligibleOffersField = value;
			}
		}
	}

	/// <remarks/>
	[System.SerializableAttribute]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	public partial class NotificationNotificationPayloadAnyOfferChangedNotificationSummaryOfferCount
	{
		private string conditionField;

		private string fulfillmentChannelField;

		private int valueField;

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute]
		public string condition
		{
			get
			{
				return conditionField;
			}

			set
			{
				conditionField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute]
		public string fulfillmentChannel
		{
			get
			{
				return fulfillmentChannelField;
			}

			set
			{
				fulfillmentChannelField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlTextAttribute]
		public int Value
		{
			get
			{
				return valueField;
			}

			set
			{
				valueField = value;
			}
		}
	}

	/// <remarks/>
	[System.SerializableAttribute]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	public partial class NotificationNotificationPayloadAnyOfferChangedNotificationSummaryLowestPrice
	{
		private NotificationNotificationPayloadAnyOfferChangedNotificationSummaryLowestPriceLandedPrice landedPriceField;

		private NotificationNotificationPayloadAnyOfferChangedNotificationSummaryLowestPriceListingPrice listingPriceField;

		private NotificationNotificationPayloadAnyOfferChangedNotificationSummaryLowestPriceShipping shippingField;

		private string conditionField;

		private string fulfillmentChannelField;

		/// <remarks/>
		public NotificationNotificationPayloadAnyOfferChangedNotificationSummaryLowestPriceLandedPrice LandedPrice
		{
			get
			{
				return landedPriceField;
			}

			set
			{
				landedPriceField = value;
			}
		}

		/// <remarks/>
		public NotificationNotificationPayloadAnyOfferChangedNotificationSummaryLowestPriceListingPrice ListingPrice
		{
			get
			{
				return listingPriceField;
			}

			set
			{
				listingPriceField = value;
			}
		}

		/// <remarks/>
		public NotificationNotificationPayloadAnyOfferChangedNotificationSummaryLowestPriceShipping Shipping
		{
			get
			{
				return shippingField;
			}

			set
			{
				shippingField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute]
		public string condition
		{
			get
			{
				return conditionField;
			}

			set
			{
				conditionField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute]
		public string fulfillmentChannel
		{
			get
			{
				return fulfillmentChannelField;
			}

			set
			{
				fulfillmentChannelField = value;
			}
		}
	}

	/// <remarks/>
	[System.SerializableAttribute]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	public class NotificationNotificationPayloadAnyOfferChangedNotificationSummaryLowestPriceLandedPrice
	{
		private decimal amountField;

		private string currencyCodeField;

		/// <remarks/>
		public decimal Amount
		{
			get
			{
				return amountField;
			}

			set
			{
				amountField = value;
			}
		}

		/// <remarks/>
		public string CurrencyCode
		{
			get
			{
				return currencyCodeField;
			}

			set
			{
				currencyCodeField = value;
			}
		}
	}

	/// <remarks/>
	[System.SerializableAttribute]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	public class NotificationNotificationPayloadAnyOfferChangedNotificationSummaryLowestPriceListingPrice
	{
		private decimal amountField;

		private string currencyCodeField;

		/// <remarks/>
		public decimal Amount
		{
			get
			{
				return amountField;
			}

			set
			{
				amountField = value;
			}
		}

		/// <remarks/>
		public string CurrencyCode
		{
			get
			{
				return currencyCodeField;
			}

			set
			{
				currencyCodeField = value;
			}
		}
	}

	/// <remarks/>
	[System.SerializableAttribute]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	public class NotificationNotificationPayloadAnyOfferChangedNotificationSummaryLowestPriceShipping
	{
		private decimal amountField;

		private string currencyCodeField;

		/// <remarks/>
		public decimal Amount
		{
			get
			{
				return amountField;
			}

			set
			{
				amountField = value;
			}
		}

		/// <remarks/>
		public string CurrencyCode
		{
			get
			{
				return currencyCodeField;
			}

			set
			{
				currencyCodeField = value;
			}
		}
	}

	/// <remarks/>
	[System.SerializableAttribute]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	public class NotificationNotificationPayloadAnyOfferChangedNotificationSummaryBuyBoxPrices
	{
		private NotificationNotificationPayloadAnyOfferChangedNotificationSummaryBuyBoxPricesBuyBoxPrice buyBoxPriceField;

		/// <remarks/>
		public NotificationNotificationPayloadAnyOfferChangedNotificationSummaryBuyBoxPricesBuyBoxPrice BuyBoxPrice
		{
			get
			{
				return buyBoxPriceField;
			}

			set
			{
				buyBoxPriceField = value;
			}
		}
	}

	/// <remarks/>
	[System.SerializableAttribute]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	public class NotificationNotificationPayloadAnyOfferChangedNotificationSummaryBuyBoxPricesBuyBoxPrice
	{
		private NotificationNotificationPayloadAnyOfferChangedNotificationSummaryBuyBoxPricesBuyBoxPriceLandedPrice landedPriceField;

		private NotificationNotificationPayloadAnyOfferChangedNotificationSummaryBuyBoxPricesBuyBoxPriceListingPrice listingPriceField;

		private NotificationNotificationPayloadAnyOfferChangedNotificationSummaryBuyBoxPricesBuyBoxPriceShipping shippingField;

		private string conditionField;

		/// <remarks/>
		public NotificationNotificationPayloadAnyOfferChangedNotificationSummaryBuyBoxPricesBuyBoxPriceLandedPrice LandedPrice
		{
			get
			{
				return landedPriceField;
			}

			set
			{
				landedPriceField = value;
			}
		}

		/// <remarks/>
		public NotificationNotificationPayloadAnyOfferChangedNotificationSummaryBuyBoxPricesBuyBoxPriceListingPrice ListingPrice
		{
			get
			{
				return listingPriceField;
			}

			set
			{
				listingPriceField = value;
			}
		}

		/// <remarks/>
		public NotificationNotificationPayloadAnyOfferChangedNotificationSummaryBuyBoxPricesBuyBoxPriceShipping Shipping
		{
			get
			{
				return shippingField;
			}

			set
			{
				shippingField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute]
		public string condition
		{
			get
			{
				return conditionField;
			}

			set
			{
				conditionField = value;
			}
		}
	}

	/// <remarks/>
	[System.SerializableAttribute]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	public class NotificationNotificationPayloadAnyOfferChangedNotificationSummaryBuyBoxPricesBuyBoxPriceLandedPrice
	{
		private decimal amountField;

		private string currencyCodeField;

		/// <remarks/>
		public decimal Amount
		{
			get
			{
				return amountField;
			}

			set
			{
				amountField = value;
			}
		}

		/// <remarks/>
		public string CurrencyCode
		{
			get
			{
				return currencyCodeField;
			}

			set
			{
				currencyCodeField = value;
			}
		}
	}

	/// <remarks/>
	[System.SerializableAttribute]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	public class NotificationNotificationPayloadAnyOfferChangedNotificationSummaryBuyBoxPricesBuyBoxPriceListingPrice
	{
		private decimal amountField;

		private string currencyCodeField;

		/// <remarks/>
		public decimal Amount
		{
			get
			{
				return amountField;
			}

			set
			{
				amountField = value;
			}
		}

		/// <remarks/>
		public string CurrencyCode
		{
			get
			{
				return currencyCodeField;
			}

			set
			{
				currencyCodeField = value;
			}
		}
	}

	/// <remarks/>
	[System.SerializableAttribute]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	public class NotificationNotificationPayloadAnyOfferChangedNotificationSummaryBuyBoxPricesBuyBoxPriceShipping
	{
		private decimal amountField;

		private string currencyCodeField;

		/// <remarks/>
		public decimal Amount
		{
			get
			{
				return amountField;
			}

			set
			{
				amountField = value;
			}
		}

		/// <remarks/>
		public string CurrencyCode
		{
			get
			{
				return currencyCodeField;
			}

			set
			{
				currencyCodeField = value;
			}
		}
	}

	/// <remarks/>
	[System.SerializableAttribute]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	public class NotificationNotificationPayloadAnyOfferChangedNotificationSummaryListPrice
	{
		private decimal amountField;

		private string currencyCodeField;

		/// <remarks/>
		public decimal Amount
		{
			get
			{
				return amountField;
			}

			set
			{
				amountField = value;
			}
		}

		/// <remarks/>
		public string CurrencyCode
		{
			get
			{
				return currencyCodeField;
			}

			set
			{
				currencyCodeField = value;
			}
		}
	}

	/// <remarks/>
	[System.SerializableAttribute]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	public class NotificationNotificationPayloadAnyOfferChangedNotificationSummarySalesRank
	{
		private string productCategoryIdField;

		private uint rankField;

		/// <remarks/>
		public string ProductCategoryId
		{
			get
			{
				return productCategoryIdField;
			}

			set
			{
				productCategoryIdField = value;
			}
		}

		/// <remarks/>
		public uint Rank
		{
			get
			{
				return rankField;
			}

			set
			{
				rankField = value;
			}
		}
	}

	/// <remarks/>
	[System.SerializableAttribute]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	public class NotificationNotificationPayloadAnyOfferChangedNotificationSummaryOfferCount1
	{
		private string conditionField;

		private string fulfillmentChannelField;

		private int valueField;

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute]
		public string condition
		{
			get
			{
				return conditionField;
			}

			set
			{
				conditionField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute]
		public string fulfillmentChannel
		{
			get
			{
				return fulfillmentChannelField;
			}

			set
			{
				fulfillmentChannelField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlTextAttribute]
		public int Value
		{
			get
			{
				return valueField;
			}

			set
			{
				valueField = value;
			}
		}
	}

	/// <remarks/>
	[System.SerializableAttribute]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	public class NotificationNotificationPayloadAnyOfferChangedNotificationOffer
	{
		private string sellerIdField;

		private string subConditionField;

		private NotificationNotificationPayloadAnyOfferChangedNotificationOfferSellerFeedbackRating sellerFeedbackRatingField;

		private NotificationNotificationPayloadAnyOfferChangedNotificationOfferShippingTime shippingTimeField;

		private NotificationNotificationPayloadAnyOfferChangedNotificationOfferListingPrice listingPriceField;

		private NotificationNotificationPayloadAnyOfferChangedNotificationOfferShipping shippingField;

		private NotificationNotificationPayloadAnyOfferChangedNotificationOfferShipsFrom shipsFromField;

		private bool isFulfilledByAmazonField;

		private bool isBuyBoxWinnerField;

		private bool isExpeditedShippingAvailableField;

		private bool isFeaturedMerchantField;

		private bool shipsDomesticallyField;

		private bool shipsInternationallyField;

		/// <remarks/>
		public string SellerId
		{
			get
			{
				return sellerIdField;
			}

			set
			{
				sellerIdField = value;
			}
		}

		/// <remarks/>
		public string SubCondition
		{
			get
			{
				return subConditionField;
			}

			set
			{
				subConditionField = value;
			}
		}

		/// <remarks/>
		public NotificationNotificationPayloadAnyOfferChangedNotificationOfferSellerFeedbackRating SellerFeedbackRating
		{
			get
			{
				return sellerFeedbackRatingField;
			}

			set
			{
				sellerFeedbackRatingField = value;
			}
		}

		/// <remarks/>
		public NotificationNotificationPayloadAnyOfferChangedNotificationOfferShippingTime ShippingTime
		{
			get
			{
				return shippingTimeField;
			}

			set
			{
				shippingTimeField = value;
			}
		}

		/// <remarks/>
		public NotificationNotificationPayloadAnyOfferChangedNotificationOfferListingPrice ListingPrice
		{
			get
			{
				return listingPriceField;
			}

			set
			{
				listingPriceField = value;
			}
		}

		/// <remarks/>
		public NotificationNotificationPayloadAnyOfferChangedNotificationOfferShipping Shipping
		{
			get
			{
				return shippingField;
			}

			set
			{
				shippingField = value;
			}
		}

		/// <remarks/>
		public NotificationNotificationPayloadAnyOfferChangedNotificationOfferShipsFrom ShipsFrom
		{
			get
			{
				return shipsFromField;
			}

			set
			{
				shipsFromField = value;
			}
		}

		/// <remarks/>
		public bool IsFulfilledByAmazon
		{
			get
			{
				return isFulfilledByAmazonField;
			}

			set
			{
				isFulfilledByAmazonField = value;
			}
		}

		public bool IsBuyBoxWinner
		{
			get
			{
				return isBuyBoxWinnerField;
			}

			set
			{
				isBuyBoxWinnerField = value;
			}
		}

		/// <remarks/>
		public bool IsExpeditedShippingAvailable
		{
			get
			{
				return isExpeditedShippingAvailableField;
			}

			set
			{
				isExpeditedShippingAvailableField = value;
			}
		}

		/// <remarks/>
		public bool IsFeaturedMerchant
		{
			get
			{
				return isFeaturedMerchantField;
			}

			set
			{
				isFeaturedMerchantField = value;
			}
		}

		/// <remarks/>
		public bool ShipsDomestically
		{
			get
			{
				return shipsDomesticallyField;
			}

			set
			{
				shipsDomesticallyField = value;
			}
		}

		/// <remarks/>
		public bool ShipsInternationally
		{
			get
			{
				return shipsInternationallyField;
			}

			set
			{
				shipsInternationallyField = value;
			}
		}
	}

	/// <remarks/>
	[System.SerializableAttribute]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	public class NotificationNotificationPayloadAnyOfferChangedNotificationOfferSellerFeedbackRating
	{
		private int sellerPositiveFeedbackRatingField;

		private bool sellerPositiveFeedbackRatingFieldSpecified;

		private int feedbackCountField;

		/// <remarks/>
		public int SellerPositiveFeedbackRating
		{
			get
			{
				return sellerPositiveFeedbackRatingField;
			}

			set
			{
				sellerPositiveFeedbackRatingField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlIgnoreAttribute]
		public bool SellerPositiveFeedbackRatingSpecified
		{
			get
			{
				return sellerPositiveFeedbackRatingFieldSpecified;
			}

			set
			{
				sellerPositiveFeedbackRatingFieldSpecified = value;
			}
		}

		/// <remarks/>
		public int FeedbackCount
		{
			get
			{
				return feedbackCountField;
			}

			set
			{
				feedbackCountField = value;
			}
		}
	}

	/// <remarks/>
	[System.SerializableAttribute]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	public class NotificationNotificationPayloadAnyOfferChangedNotificationOfferShippingTime
	{
		private int minimumHoursField;

		private int maximumHoursField;

		private string availabilityTypeField;

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute]
		public int minimumHours
		{
			get
			{
				return minimumHoursField;
			}

			set
			{
				minimumHoursField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute]
		public int maximumHours
		{
			get
			{
				return maximumHoursField;
			}

			set
			{
				maximumHoursField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute]
		public string availabilityType
		{
			get
			{
				return availabilityTypeField;
			}

			set
			{
				availabilityTypeField = value;
			}
		}
	}

	/// <remarks/>
	[System.SerializableAttribute]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	public class NotificationNotificationPayloadAnyOfferChangedNotificationOfferListingPrice
	{
		private decimal amountField;

		private string currencyCodeField;

		/// <remarks/>
		public decimal Amount
		{
			get
			{
				return amountField;
			}

			set
			{
				amountField = value;
			}
		}

		/// <remarks/>
		public string CurrencyCode
		{
			get
			{
				return currencyCodeField;
			}

			set
			{
				currencyCodeField = value;
			}
		}
	}

	/// <remarks/>
	[System.SerializableAttribute]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	public class NotificationNotificationPayloadAnyOfferChangedNotificationOfferShipping
	{
		private decimal amountField;

		private string currencyCodeField;

		/// <remarks/>
		public decimal Amount
		{
			get
			{
				return amountField;
			}

			set
			{
				amountField = value;
			}
		}

		/// <remarks/>
		public string CurrencyCode
		{
			get
			{
				return currencyCodeField;
			}

			set
			{
				currencyCodeField = value;
			}
		}
	}

	/// <remarks/>
	[System.SerializableAttribute]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	public class NotificationNotificationPayloadAnyOfferChangedNotificationOfferShipsFrom
	{
		private string countryField;

		private string stateField;

		/// <remarks/>
		public string Country
		{
			get
			{
				return countryField;
			}

			set
			{
				countryField = value;
			}
		}

		/// <remarks/>
		public string State
		{
			get
			{
				return stateField;
			}

			set
			{
				stateField = value;
			}
		}
	}
}
