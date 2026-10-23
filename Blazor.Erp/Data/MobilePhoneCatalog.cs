namespace BlazorErp.Data;

public static class MobilePhoneCatalog
{
    public const string CategoryName = "Мобильные телефоны";

    public static IReadOnlyList<MobilePhoneSeed> Phones { get; } =
    [
        new("Motorola XOOM™ with Wi-Fi", "Motorola", "The Next, Next Generation — tablet powered by Android 3.0 (Honeycomb).", 34990, "motorola-xoom-with-wi-fi.0.jpg"),
        new("MOTOROLA XOOM™", "Motorola", "The Next, Next Generation — world's first tablet powered by Android 3.0.", 32990, "motorola-xoom.0.jpg"),
        new("MOTOROLA ATRIX™ 4G", "Motorola", "MOTOROLA ATRIX 4G — the world's most powerful smartphone.", 29990, "motorola-atrix-4g.0.jpg"),
        new("Dell Streak 7", "Dell", "Share photos, videos and movies together. Small enough to carry, big enough to gather around.", 24990, "dell-streak-7.0.jpg"),
        new("Samsung Gem™", "Samsung", "Touch display smartphone with more apps, more features and an affordable price.", 12990, "samsung-gem.0.jpg"),
        new("Dell Venue", "Dell", "Your personal express lane to everything.", 18990, "dell-venue.0.jpg"),
        new("Nexus S", "Google", "First phone to run Gingerbread (Android 2.3) — pure Google experience.", 27990, "nexus-s.0.jpg"),
        new("LG Axis", "LG", "Android powered, Google Maps Navigation, 5 customizable home screens.", 14990, "lg-axis.0.jpg"),
        new("Samsung Galaxy Tab™", "Samsung", "Ultra-mobile entertainment with 7\" display and Adobe Flash Player.", 31990, "samsung-galaxy-tab.0.jpg"),
        new("Samsung Showcase™ a Galaxy S™ phone", "Samsung", "Cinema quality 4\" touch display — rich picture brilliance, even outdoors.", 22990, "samsung-showcase-a-galaxy-s-phone.0.jpg"),
        new("DROID™ 2 Global by Motorola", "Motorola", "The first smartphone with a 1.2 GHz processor and global capabilities.", 25990, "droid-2-global-by-motorola.0.jpg"),
        new("DROID™ Pro by Motorola", "Motorola", "The next generation of DOES.", 26990, "droid-pro-by-motorola.0.jpg"),
        new("MOTOROLA BRAVO™ with MOTOBLUR™", "Motorola", "An experience to cheer about.", 16990, "motorola-bravo-with-motoblur.0.jpg"),
        new("Motorola DEFY™ with MOTOBLUR™", "Motorola", "Ready for everything life throws your way.", 17990, "motorola-defy-with-motoblur.0.jpg"),
        new("T-Mobile myTouch 4G", "T-Mobile", "Premium smartphone with blazing fast 4G speeds and video chat.", 21990, "t-mobile-mytouch-4g.0.jpg"),
        new("Samsung Mesmerize™ a Galaxy S™ phone", "Samsung", "Cinema quality experience with innovative 4\" touch display.", 23990, "samsung-mesmerize-a-galaxy-s-phone.0.jpg"),
        new("SANYO ZIO", "Sanyo", "Android smartphone with ultra-sleek styling and strong performance.", 11990, "sanyo-zio.0.jpg"),
        new("Samsung Transform™", "Samsung", "Customize your Android phone with Sprint ID Service Packs.", 15990, "samsung-transform.0.jpg"),
        new("T-Mobile G2", "T-Mobile", "First smartphone built for 4G speeds on T-Mobile's network.", 20990, "t-mobile-g2.0.jpg"),
        new("Motorola CHARM™ with MOTOBLUR™", "Motorola", "Fits easily in your pocket. Includes MOTOBLUR service.", 9990, "motorola-charm-with-motoblur.0.jpg")
    ];

    public record MobilePhoneSeed(string Name, string Brand, string Description, decimal Price, string ImageFile)
    {
        public string ImageUrl => $"/images/phones/{ImageFile}";
    }
}
