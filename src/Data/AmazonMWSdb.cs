using Data.EFModels;
using System.Data.Entity;

namespace Data
{
	public class AmazonMWSdb : DbContext
	{
		public AmazonMWSdb()
			: base("name=DefaultConnection")
		{
		}

		public virtual DbSet<EFModels.AllOrdersData> AllOrdersDatas { get; set; }
		public virtual DbSet<EFModels.FeePreviewData> FeePreviewDatas { get; set; }
		public virtual DbSet<InventoryData> InventoryDatas { get; set; }
		public virtual DbSet<EFModels.ListingOffersLog> ListingOffersLogs { get; set; }
		public virtual DbSet<LoginAttempt> LoginAttempts { get; set; }
		public virtual DbSet<ManufacturerDiscount> ManufacturerDiscounts { get; set; }
		public virtual DbSet<EFModels.Manufacturer> Manufacturers { get; set; }
		public virtual DbSet<EFModels.PriceHistory> PriceHistories { get; set; }
		public virtual DbSet<ProductAmazonIdentifiersAudit> ProductAmazonIdentifiersAudits { get; set; }
		public virtual DbSet<EFModels.Product> Products { get; set; }
		public virtual DbSet<RepricingInformation> RepricingInformations { get; set; }
		public virtual DbSet<EFModels.UnsuppressedInventoryData> UnsuppressedInventoryDatas { get; set; }
		public virtual DbSet<SellerType> SellerTypes { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Entity<EFModels.AllOrdersData>()
				.Property(e => e.product_name)
				.IsUnicode(false);

			modelBuilder.Entity<EFModels.AllOrdersData>()
				.Property(e => e.item_price)
				.HasPrecision(19, 4);

			modelBuilder.Entity<EFModels.AllOrdersData>()
				.Property(e => e.item_tax)
				.HasPrecision(19, 4);

			modelBuilder.Entity<EFModels.AllOrdersData>()
				.Property(e => e.shipping_price)
				.HasPrecision(19, 4);

			modelBuilder.Entity<EFModels.AllOrdersData>()
				.Property(e => e.shipping_tax)
				.HasPrecision(19, 4);

			modelBuilder.Entity<EFModels.AllOrdersData>()
				.Property(e => e.gift_wrap_price)
				.HasPrecision(19, 4);

			modelBuilder.Entity<EFModels.AllOrdersData>()
				.Property(e => e.gift_wrap_tax)
				.HasPrecision(19, 4);

			modelBuilder.Entity<EFModels.AllOrdersData>()
				.Property(e => e.item_promotion_discount)
				.HasPrecision(19, 4);

			modelBuilder.Entity<EFModels.AllOrdersData>()
				.Property(e => e.ship_promotion_discount)
				.HasPrecision(19, 4);

			modelBuilder.Entity<EFModels.ListingOffersLog>()
				.Property(e => e.LowestFbaPrice)
				.HasPrecision(19, 4);

			modelBuilder.Entity<EFModels.ListingOffersLog>()
				.Property(e => e.LowestNonFbaPrice)
				.HasPrecision(19, 4);

			modelBuilder.Entity<EFModels.ListingOffersLog>()
				.Property(e => e.OurPrice)
				.HasPrecision(19, 4);

			modelBuilder.Entity<EFModels.ListingOffersLog>()
				.Property(e => e.BuyBoxPrice)
				.HasPrecision(19, 4);

			modelBuilder.Entity<ManufacturerDiscount>()
				.Property(e => e.DiscountPercentage)
				.HasPrecision(9, 6);

			modelBuilder.Entity<EFModels.PriceHistory>()
				.Property(e => e.NewPrice)
				.HasPrecision(10, 2);

			modelBuilder.Entity<EFModels.PriceHistory>()
				.Property(e => e.BreakEvenPrice)
				.HasPrecision(10, 2);

			modelBuilder.Entity<EFModels.PriceHistory>()
				.Property(e => e.MyOfferPriceInNotification)
				.HasPrecision(10, 2);

			modelBuilder.Entity<EFModels.PriceHistory>()
				.Property(e => e.AmazonsOfferPriceInNotification)
				.HasPrecision(10, 2);

			modelBuilder.Entity<EFModels.PriceHistory>()
				.Property(e => e.LowestFbaOfferPriceInNotification)
				.HasPrecision(10, 2);

			modelBuilder.Entity<EFModels.PriceHistory>()
				.Property(e => e.LowestNonFbaOfferPriceInNotification)
				.HasPrecision(10, 2);

			modelBuilder.Entity<EFModels.PriceHistory>()
				.Property(e => e.MyPriceFromProductsApi)
				.HasPrecision(10, 2);

			modelBuilder.Entity<EFModels.Product>()
				.Property(e => e.Cost)
				.HasPrecision(19, 4);

			modelBuilder.Entity<EFModels.Product>()
				.Property(e => e.PromotionCost)
				.HasPrecision(19, 4);

			modelBuilder.Entity<EFModels.Product>()
				.Property(e => e.OverrideCost)
				.HasPrecision(19, 4);

			modelBuilder.Entity<EFModels.Product>()
				.Property(e => e.MAPPrice)
				.HasPrecision(19, 4);

			modelBuilder.Entity<EFModels.Product>()
				.Property(e => e.MinPrice)
				.HasPrecision(19, 4);

			modelBuilder.Entity<EFModels.Product>()
				.Property(e => e.MaxPrice)
				.HasPrecision(19, 4);

			modelBuilder.Entity<EFModels.Product>()
				.Property(e => e.BreakevenPrice)
				.HasPrecision(19, 4);

			modelBuilder.Entity<EFModels.Product>()
				.Property(e => e.Length)
				.HasPrecision(9, 6);

			modelBuilder.Entity<EFModels.Product>()
				.Property(e => e.Width)
				.HasPrecision(9, 6);

			modelBuilder.Entity<EFModels.Product>()
				.Property(e => e.Height)
				.HasPrecision(9, 6);

			modelBuilder.Entity<EFModels.Product>()
				.Property(e => e.Weight)
				.HasPrecision(9, 6);

			modelBuilder.Entity<RepricingInformation>()
				.Property(e => e.MinimumPrice)
				.HasPrecision(9, 6);
		}
	}
}
